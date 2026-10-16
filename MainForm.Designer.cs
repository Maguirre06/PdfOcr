#nullable disable

using Microsoft.Web.WebView2.WinForms;

namespace PdfOcr;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private ToolStrip toolbar = null!;
    private ToolStripButton openButton = null!;
    private ToolStripButton ocrButton = null!;
    private ToolStripButton toggleOcrTextButton = null!;
    private ToolStripLabel status = null!;
    private SplitContainer mainSplitContainer = null!;
    private WebView2 web = null!;
    private TextBox text = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        toolbar = new ToolStrip();
        openButton = new ToolStripButton();
        ocrButton = new ToolStripButton();
        toggleOcrTextButton = new ToolStripButton();
        status = new ToolStripLabel();
        toolStripButton1 = new ToolStripButton();
        mainSplitContainer = new SplitContainer();
        web = new WebView2();
        text = new TextBox();
        toolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
        mainSplitContainer.Panel1.SuspendLayout();
        mainSplitContainer.Panel2.SuspendLayout();
        mainSplitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)web).BeginInit();
        SuspendLayout();
        // 
        // toolbar
        // 
        toolbar.Items.AddRange(new ToolStripItem[] { openButton, ocrButton, toggleOcrTextButton, status, toolStripButton1 });
        toolbar.Location = new Point(0, 0);
        toolbar.Name = "toolbar";
        toolbar.Size = new Size(1300, 25);
        toolbar.TabIndex = 0;
        // 
        // openButton
        // 
        openButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        openButton.Name = "openButton";
        openButton.Size = new Size(61, 22);
        openButton.Text = "Abrir PDF";
        openButton.Click += OpenPdf;
        // 
        // ocrButton
        // 
        ocrButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        ocrButton.Enabled = false;
        ocrButton.Name = "ocrButton";
        ocrButton.Size = new Size(35, 22);
        ocrButton.Text = "OCR";
        ocrButton.Click += RunOcr;
        // 
        // toggleOcrTextButton
        // 
        toggleOcrTextButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        toggleOcrTextButton.Name = "toggleOcrTextButton";
        toggleOcrTextButton.Size = new Size(108, 22);
        toggleOcrTextButton.Text = "Mostrar texto OCR";
        toggleOcrTextButton.ToolTipText = "Mostrar u ocultar el texto obtenido por OCR";
        toggleOcrTextButton.Click += ToggleOcrText;
        // 
        // status
        // 
        status.Name = "status";
        status.Size = new Size(0, 22);
        // 
        // toolStripButton1
        // 
        toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
        toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
        toolStripButton1.ImageTransparentColor = Color.Magenta;
        toolStripButton1.Name = "toolStripButton1";
        toolStripButton1.Size = new Size(86, 22);
        toolStripButton1.Text = "Predeterminar";
        toolStripButton1.Click += toolStripButton1_Click;
        // 
        // mainSplitContainer
        // 
        mainSplitContainer.Dock = DockStyle.Fill;
        mainSplitContainer.Location = new Point(0, 25);
        mainSplitContainer.Name = "mainSplitContainer";
        // 
        // mainSplitContainer.Panel1
        // 
        mainSplitContainer.Panel1.Controls.Add(web);
        // 
        // mainSplitContainer.Panel2
        // 
        mainSplitContainer.Panel2.Controls.Add(text);
        mainSplitContainer.Panel2Collapsed = true;
        mainSplitContainer.Size = new Size(1300, 775);
        mainSplitContainer.SplitterDistance = 800;
        mainSplitContainer.TabIndex = 1;
        // 
        // web
        // 
        web.AllowExternalDrop = true;
        web.BackColor = SystemColors.Window;
        web.CreationProperties = null;
        web.DefaultBackgroundColor = Color.White;
        web.Dock = DockStyle.Fill;
        web.Location = new Point(0, 0);
        web.Name = "web";
        web.Size = new Size(1300, 775);
        web.TabIndex = 0;
        web.ZoomFactor = 1D;
        // 
        // text
        // 
        text.Dock = DockStyle.Fill;
        text.Font = new Font("Consolas", 10F);
        text.Location = new Point(0, 0);
        text.Multiline = true;
        text.Name = "text";
        text.ScrollBars = ScrollBars.Both;
        text.Size = new Size(96, 100);
        text.TabIndex = 0;
        text.WordWrap = false;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1300, 800);
        Controls.Add(mainSplitContainer);
        Controls.Add(toolbar);
        Icon = (Icon)resources.GetObject("$this.Icon");
        Name = "MainForm";
        Text = "PDF + OCR";
        Load += InitializeWebView;
        toolbar.ResumeLayout(false);
        toolbar.PerformLayout();
        mainSplitContainer.Panel1.ResumeLayout(false);
        mainSplitContainer.Panel2.ResumeLayout(false);
        mainSplitContainer.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
        mainSplitContainer.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)web).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private ToolStripButton toolStripButton1 = null!;
}
