using System.Text;
using Microsoft.Web.WebView2.WinForms;
using Windows.Data.Pdf;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Streams;

namespace PdfOcr;

internal sealed class MainForm : Form
{
    private readonly string language;
    private readonly WebView2 web = new() { Dock = DockStyle.Fill };
    private readonly TextBox text = new()
    {
        Dock = DockStyle.Fill,
        Multiline = true,
        ScrollBars = ScrollBars.Both,
        WordWrap = false,
        Font = new Font("Consolas", 10)
    };
    private readonly ToolStripButton openButton = new("Abrir PDF");
    private readonly ToolStripButton ocrButton = new("OCR") { Enabled = false };
    private readonly ToolStripLabel status = new(string.Empty);
    private string? pdfPath;

    public MainForm(string language)
    {
        this.language = language;
        Text = "PDF + OCR";
        Width = 1300;
        Height = 800;

        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 800
        };
        split.Panel1.Controls.Add(web);
        split.Panel2.Controls.Add(text);

        var toolbar = new ToolStrip();
        toolbar.Items.AddRange([openButton, ocrButton, status]);

        Controls.Add(split);
        Controls.Add(toolbar);

        openButton.Click += OpenPdf;
        ocrButton.Click += RunOcr;
        Load += InitializeWebView;
    }

    private async void InitializeWebView(object? sender, EventArgs e)
    {
        try
        {
            await web.EnsureCoreWebView2Async();
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

        pdfPath = dialog.FileName;
        web.Source = new Uri(pdfPath);
        ocrButton.Enabled = true;
        text.Clear();
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
}
