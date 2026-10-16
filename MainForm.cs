using System.Diagnostics;
using System.Text;
using Microsoft.Web.WebView2.Core;
using Windows.Data.Pdf;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Streams;

namespace PdfOcr;

internal sealed partial class MainForm : Form
{
    private readonly string language;
    private readonly string? initialPath;
    private string? pdfPath;

    public MainForm()
        : this("es", null)
    {
    }

    public MainForm(string language, string? initialPath)
    {
        this.language = language;
        this.initialPath = initialPath;
        InitializeComponent();
    }

    private void ToggleOcrText(object? sender, EventArgs e)
    {
        SetOcrTextVisible(mainSplitContainer.Panel2Collapsed);
    }

    private void SetOcrTextVisible(bool visible)
    {
        mainSplitContainer.Panel2Collapsed = !visible;
        toggleOcrTextButton.Text = visible ? "Ocultar texto OCR" : "Mostrar texto OCR";
    }

    private async void InitializeWebView(object? sender, EventArgs e)
    {
        try
        {
            await web.EnsureCoreWebView2Async();
            web.AllowExternalDrop = true;
            web.CoreWebView2.NavigationStarting += WebNavigationStarting;
            web.CoreWebView2.NewWindowRequested += WebNewWindowRequested;
            if (initialPath is not null)
            {
                OpenPath(initialPath);
            }
        }
        catch (Exception ex)
        {
            openButton.Enabled = false;
            MessageBox.Show(
                $"No se pudo iniciar WebView2. Verificá que WebView2 Runtime esté instalado.\n\n{ex.Message}",
                "Error al iniciar",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void OpenPdf(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Filter = "Archivos PDF|*.pdf" };
        if (dialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        OpenPath(dialog.FileName);
    }

    private void OpenPath(string file)
    {
        if (!File.Exists(file) || !string.Equals(Path.GetExtension(file), ".pdf", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(
                "El archivo seleccionado no es un PDF válido.",
                "Archivo no válido",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        SetOpenedPdf(file);
        web.CoreWebView2.Navigate(new Uri(pdfPath!).AbsoluteUri);
    }

    private void SetOpenedPdf(string file)
    {
        pdfPath = Path.GetFullPath(file);
        ocrButton.Enabled = true;
        text.Clear();
        SetOcrTextVisible(false);
        status.Text = string.Empty;
    }

    private void WebNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (Uri.TryCreate(e.Uri, UriKind.Absolute, out var uri)
            && uri.IsFile
            && string.Equals(Path.GetExtension(uri.LocalPath), ".pdf", StringComparison.OrdinalIgnoreCase)
            && File.Exists(uri.LocalPath))
        {
            SetOpenedPdf(uri.LocalPath);
        }
    }

    private void WebNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        if (!TryGetLocalPdf(e.Uri, out var file))
        {
            return;
        }

        e.Handled = true;
        BeginInvoke(() => OpenPath(file));
    }

    private static bool TryGetLocalPdf(string uriText, out string file)
    {
        if (Uri.TryCreate(uriText, UriKind.Absolute, out var uri)
            && uri.IsFile
            && string.Equals(Path.GetExtension(uri.LocalPath), ".pdf", StringComparison.OrdinalIgnoreCase)
            && File.Exists(uri.LocalPath))
        {
            file = uri.LocalPath;
            return true;
        }

        file = string.Empty;
        return false;
    }

    private async void RunOcr(object? sender, EventArgs e)
    {
        if (pdfPath is null)
        {
            return;
        }

        ocrButton.Enabled = false;
        try
        {
            var engine = OcrEngine.TryCreateFromLanguage(new Language(language))
                         ?? OcrEngine.TryCreateFromUserProfileLanguages();
            if (engine is null)
            {
                MessageBox.Show(
                    $"No hay un paquete de idioma OCR para '{language}'. " +
                    "Instalalo en Configuración > Hora e idioma > Idioma.");
                return;
            }

            SetOcrTextVisible(true);

            var file = await StorageFile.GetFileFromPathAsync(pdfPath);
            var pdf = await PdfDocument.LoadFromFileAsync(file);
            var resultText = new StringBuilder();

            for (uint pageIndex = 0; pageIndex < pdf.PageCount; pageIndex++)
            {
                status.Text = $"OCR página {pageIndex + 1}/{pdf.PageCount}...";
                using var page = pdf.GetPage(pageIndex);

                var scale = Math.Min(
                    3.0,
                    OcrEngine.MaxImageDimension / Math.Max(page.Size.Width, page.Size.Height));
                using var stream = new InMemoryRandomAccessStream();
                await page.RenderToStreamAsync(stream, new PdfPageRenderOptions
                {
                    DestinationWidth = (uint)(page.Size.Width * scale),
                    DestinationHeight = (uint)(page.Size.Height * scale)
                });

                var decoder = await BitmapDecoder.CreateAsync(stream);
                using var bitmap = await decoder.GetSoftwareBitmapAsync();
                var result = await engine.RecognizeAsync(bitmap);

                resultText.AppendLine($"--- Página {pageIndex + 1} ---");
                foreach (var line in result.Lines)
                {
                    resultText.AppendLine(line.Text);
                }

                resultText.AppendLine();
                text.Text = resultText.ToString();
            }

            status.Text = "Listo";
        }
        catch (Exception ex)
        {
            status.Text = "Error";
            MessageBox.Show(ex.Message, "Error de OCR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            ocrButton.Enabled = true;
        }
    }

    private void toolStripButton1_Click(object? sender, EventArgs e)
    {       
        Process.Start(new ProcessStartInfo("ms-settings:defaultapps") { UseShellExecute = true });
    }
}
