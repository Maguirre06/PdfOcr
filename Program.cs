namespace PdfOcr;

internal static class Program
{
    private const string OcrLanguage = "es";

    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm(OcrLanguage));
    }
}
