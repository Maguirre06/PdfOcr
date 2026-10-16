using System.Diagnostics;

namespace PdfOcr;

internal static class Program
{
    private const string OcrLanguage = "es";

    [STAThread]
    private static void Main(string[] args)
    {
        if (!Debugger.IsAttached)
        {
            if (args.Contains("--register"))
            {
                FileAssoc.Register();
                return;
            }

            if (args.Contains("--unregister"))
            {
                FileAssoc.Unregister();
                return;
            }
        }

        ApplicationConfiguration.Initialize();
        var initialPath = args.FirstOrDefault(argument => !argument.StartsWith("--"));
        Application.Run(new MainForm(OcrLanguage, initialPath));
    }
}
