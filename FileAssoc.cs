using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace PdfOcr;

internal static class FileAssoc
{
    private const string AppName = "PDF + OCR";
    private const string ProgId = "PdfOcr.pdf";
    private const string AppKey = @"Software\PdfOcr";

    public static void Register()
    {
        var executable = Environment.ProcessPath!;
        var fileIcon = Path.Combine(AppContext.BaseDirectory, "Icopdf.ico");
        var iconResource = File.Exists(fileIcon) ? fileIcon : executable;
        var command = $"\"{executable}\" \"%1\"";

        using (var key = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{ProgId}"))
        {
            key.SetValue("", AppName);
            using var icon = key.CreateSubKey("DefaultIcon");
            icon.SetValue("", $"\"{iconResource}\",0");
            using var openCommand = key.CreateSubKey(@"shell\open\command");
            openCommand.SetValue("", command);
        }

        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\.pdf\OpenWithProgids"))
        {
            key.SetValue(ProgId, "", RegistryValueKind.None);
        }

        using (var key = Registry.CurrentUser.CreateSubKey(
                   $@"Software\Classes\Applications\{Path.GetFileName(executable)}"))
        {
            key.SetValue("FriendlyAppName", AppName);
            using var openCommand = key.CreateSubKey(@"shell\open\command");
            openCommand.SetValue("", command);
            using var supportedTypes = key.CreateSubKey("SupportedTypes");
            supportedTypes.SetValue(".pdf", "");
        }

        using (var key = Registry.CurrentUser.CreateSubKey($@"{AppKey}\Capabilities"))
        {
            key.SetValue("ApplicationName", AppName);
            key.SetValue("ApplicationDescription", "Visor de PDF con OCR");
            using var fileAssociations = key.CreateSubKey("FileAssociations");
            fileAssociations.SetValue(".pdf", ProgId);
        }

        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\RegisteredApplications"))
        {
            key.SetValue("PdfOcr", $@"{AppKey}\Capabilities");
        }

        SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
        Process.Start(new ProcessStartInfo("ms-settings:defaultapps?registeredAppUser=PdfOcr")
        {
            UseShellExecute = true
        });
    }

    public static void Unregister()
    {
        var executable = Environment.ProcessPath!;
        Registry.CurrentUser.DeleteSubKeyTree($@"Software\Classes\{ProgId}", false);
        Registry.CurrentUser.DeleteSubKeyTree(
            $@"Software\Classes\Applications\{Path.GetFileName(executable)}",
            false);
        Registry.CurrentUser.DeleteSubKeyTree(AppKey, false);

        using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\.pdf\OpenWithProgids", true))
        {
            key?.DeleteValue(ProgId, false);
        }

        using (var key = Registry.CurrentUser.OpenSubKey(@"Software\RegisteredApplications", true))
        {
            key?.DeleteValue("PdfOcr", false);
        }

        SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
    }

    [DllImport("shell32.dll")]
    private static extern void SHChangeNotify(int eventId, uint flags, IntPtr item1, IntPtr item2);
}
