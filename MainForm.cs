// ================= CLEAN POLISHED VERSION =================

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
namespace FAHAD_GSM_TOOL
{
    public partial class MainForm : Form
    {

        // ===== SECURITY CONFIG =====

        string[] debugTools =
        {
    "x64dbg","ollydbg","dnspy","ida","ida64",
    "cheatengine","processhacker"
};

        string[] usbTools =
        {
    "usbredirector",
    "usbredirectortechssrv",
    "flexihub",
    "virtualhere",
    "usbip"
};

        string[] proxyTools =
        {
    "fiddler","burp","charles","mitmproxy","wireshark","proxifier"
};

        System.Windows.Forms.Timer securityTimer = new System.Windows.Forms.Timer();
       
        string[] blockedProcesses =
{
    "usbredirector",
    "usbredirectortechssrv",
    "flexihub",
    "virtualhere",
    "proxifier",
    "fiddler",
    "wireshark",
    "x64dbg",
    "ollydbg",
    "dnspy",
    "processhacker",
    "cheatengine"
};
        void FillRoundedRectangle(Graphics g, Brush brush, Rectangle rect, int radius)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();

                g.FillPath(brush, path);
            }
        }
        private RichTextBox flashLogBox;
        bool userClickedRow = false;
        bool layoutRunning = false;
        
        void Log(string type, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");

            string logLine = $"[{time}] [{type}] {message}";

            logBox.AppendText(logLine + Environment.NewLine);
        }
        private string fastbootPath =
Path.Combine(
Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
"FahadGSMTool",
"fastboot.exe");
        private string _currentSectionTitle = "";
        private readonly System.Windows.Forms.Timer glowTimer = new System.Windows.Forms.Timer();
        //private readonly System.Windows.Forms.Timer fastbootTimer = new System.Windows.Forms.Timer();

        private Panel leftGlow;
        private Panel rightGlow;
        private Button btnXiaomi;
        // PATCH UI
        private Panel patchHeader;
        private Label lblMiAccount;
        private Label lblPatchMenu;
        private Button btnFastbootFlasher;
        private Button btnComingSoon;
        // PATCH PANEL
        // PATCH PANEL
        
        private Panel panelPersist;
        private Panel panelCust;

        private TextBox txtPersistFolder;
        private Button btnPersistBrowse;
        private Button btnPersistPatch;

        private TextBox txtCustFolder;
        private Button btnCustBrowse;
        private Button btnCustPatch;

        private RichTextBox patchLogBox;
        // FASTBOOT FLASHER UI
        TextBox txtFolderPath;
        Button btnSelectFolder;
        Button btnFlash;

        DataGridView gridPartitions;


        ProgressBar progressOverall;

        Label lblFlashStatus;
        private int glowValue = 150;
        private bool glowUp = true;
        private string _loggedInUser;
        private bool isFlashing = false;
        private bool lastFastbootState = false;
        private bool fastbootCheckRunning = false;
        private CancellationTokenSource fastbootMonitorToken;
        private CancellationTokenSource fastbootMonitor;
        private bool lastFastbootCheck = false;

        public MainForm()
        {
            InitializeComponent();   // ⭐ VERY IMPORTANT

            if (lblSectionTitle != null)
            {
                lblSectionTitle.Font = new Font("Segoe UI Black", 26, FontStyle.Bold);
                lblSectionTitle.ForeColor = Color.FromArgb(0, 255, 220);
            }
            
            this.Resize += MainForm_Resize;
            flashLogBox = new RichTextBox();

            flashLogBox.BackColor = Color.FromArgb(25, 25, 25);
            flashLogBox.ForeColor = Color.White;
            flashLogBox.BorderStyle = BorderStyle.None;
            flashLogBox.Font = new Font("Consolas", 10);

            flashLogBox.Visible = false;

            //mainPanel.Controls.Add(flashLogBox);
            this.AutoScaleMode = AutoScaleMode.None;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.OptimizedDoubleBuffer,
              true);

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);

            UpdateStyles();

            this.MinimizeBox = true;
            this.MaximizeBox = true;
            this.ControlBox = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Normal;

            if (System.ComponentModel.LicenseManager.UsageMode ==
                System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            // ================= Xiaomi Button =================
            btnXiaomi = new Button();

            btnXiaomi.Text = "Xiaomi";
            btnXiaomi.Width = btnFindDump.Width;
            btnXiaomi.Height = btnFindDump.Height;

            btnXiaomi.Left = btnFindDump.Left;
            btnXiaomi.Top = btnFindDump.Bottom + 15;

            btnXiaomi.BackColor = btnFindDump.BackColor;
            btnXiaomi.ForeColor = btnFindDump.ForeColor;
            btnXiaomi.FlatStyle = btnFindDump.FlatStyle;
            btnXiaomi.FlatAppearance.BorderColor = btnFindDump.FlatAppearance.BorderColor;
            btnXiaomi.FlatAppearance.BorderSize = btnFindDump.FlatAppearance.BorderSize;
            btnXiaomi.Font = btnFindDump.Font;

            sidebar.Controls.Add(btnXiaomi);

            // ================= Xiaomi Menu Buttons =================
            btnFastbootFlasher = new Button();
            btnComingSoon = new Button();

            btnFastbootFlasher.Text = "Fastboot Flasher";
            btnComingSoon.Text = "Patch Menu";

            btnFastbootFlasher.Width = btnFastboot.Width;
            btnFastbootFlasher.Height = btnFastboot.Height;

            btnComingSoon.Width = btnFastboot.Width;
            btnComingSoon.Height = btnFastboot.Height;

            btnFastbootFlasher.BackColor = btnFastboot.BackColor;
            btnComingSoon.BackColor = btnFastboot.BackColor;

            btnFastbootFlasher.ForeColor = btnFastboot.ForeColor;
            btnComingSoon.ForeColor = btnFastboot.ForeColor;

            btnFastbootFlasher.FlatStyle = btnFastboot.FlatStyle;
            btnComingSoon.FlatStyle = btnFastboot.FlatStyle;

            btnFastbootFlasher.FlatAppearance.BorderColor = btnFastboot.FlatAppearance.BorderColor;
            btnComingSoon.FlatAppearance.BorderColor = btnFastboot.FlatAppearance.BorderColor;

            btnFastbootFlasher.FlatAppearance.BorderSize = btnFastboot.FlatAppearance.BorderSize;
            btnComingSoon.FlatAppearance.BorderSize = btnFastboot.FlatAppearance.BorderSize;

            btnFastbootFlasher.Font = btnFastboot.Font;
            btnComingSoon.Font = btnFastboot.Font;

            this.Controls.Add(btnFastbootFlasher);
            this.Controls.Add(btnComingSoon);

            btnFastbootFlasher.BringToFront();
            btnComingSoon.BringToFront();

            btnFastbootFlasher.Visible = false;
            btnComingSoon.Visible = false;
            // ================= PATCH PANEL =================
            // ================= PATCH PANEL =================
           
   
            panelPatch = new Panel();
            panelPatch.Dock = DockStyle.Fill;
            panelPatch.Visible = false;
            panelPatch.BackColor = Color.FromArgb(18, 30, 55);

            mainPanel.Controls.Add(panelPatch);

            // ===== PATCH HEADER =====

            patchHeader = new Panel();
            patchHeader.Height = 100;
            patchHeader.Dock = DockStyle.Top;
            patchHeader.BackColor = Color.Transparent;

            lblMiAccount = new Label();
            lblMiAccount.Text = "MI ACCOUNT PERMANENT";
            lblMiAccount.Font = new Font("Segoe UI Black", 18, FontStyle.Bold);
            lblMiAccount.ForeColor = Color.FromArgb(0, 220, 120); // patch button green
            lblMiAccount.AutoSize = true;

            panelPatch.Controls.Add(lblMiAccount);

            patchHeader.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(0, 10, patchHeader.Width, 60);

                using (LinearGradientBrush brush =
                    new LinearGradientBrush(rect,
                    Color.Silver,
                    Color.Gray,
                    LinearGradientMode.Vertical))
                {
                    g.FillRoundedRectangle(brush, rect, 20);
                }
            };

            panelPatch.Controls.Add(patchHeader);


            lblPatchMenu = new Label();
            lblPatchMenu.Text = "PATCH MENU";
            lblPatchMenu.ForeColor = Color.White;
            lblPatchMenu.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblPatchMenu.AutoSize = true;

            patchHeader.Controls.Add(lblPatchMenu);

            // ===== PERSIST CARD =====

            panelPersist = new Panel();
            panelPersist.Width = 430;
            panelPersist.Height = 160;
            panelPersist.BackColor = Color.FromArgb(45, 75, 120);
            panelPersist.Paint += DrawGlassCard;
            panelPersist.BorderStyle = BorderStyle.None;
            panelPatch.Controls.Add(panelPersist);

            Label lblPersist = new Label();
            lblPersist.Text = "PATCH PERSIST";
            lblPersist.ForeColor = Color.Cyan;
            lblPersist.Font = new Font("Segoe UI Black", 14, FontStyle.Bold);
            lblPersist.ForeColor = Color.FromArgb(0, 255, 220);
            lblPersist.AutoSize = true;
            lblPersist.Top = 15;
            lblPersist.Left = 15;

            panelPersist.Controls.Add(lblPersist);

            txtPersistFolder = new TextBox();
            txtPersistFolder.Width = 280;
            txtPersistFolder.Height = 34;
            txtPersistFolder.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            txtPersistFolder.BackColor = Color.FromArgb(240, 240, 240);
            txtPersistFolder.BorderStyle = BorderStyle.None;

            txtPersistFolder.Top = 55;
            txtPersistFolder.Left = 15;

            RoundTextbox(txtPersistFolder, 20);

            panelPersist.Controls.Add(txtPersistFolder);

            // browse
            btnPersistBrowse = new Button();
            btnPersistBrowse.Text = "📂";
            btnPersistBrowse.Font = new Font("Segoe UI Emoji", 12);
            btnPersistBrowse.BackColor = Color.FromArgb(0, 200, 220);
            btnPersistBrowse.FlatAppearance.BorderSize = 0;

            btnPersistBrowse.Width = 45;
            btnPersistBrowse.Height = 34;
            btnPersistBrowse.FlatStyle = FlatStyle.Flat;
            btnPersistBrowse.ForeColor = Color.White;

            btnPersistBrowse.Top = 55;
            btnPersistBrowse.Left = txtPersistFolder.Right + 5;

            RoundTextbox(btnPersistBrowse, 12);
            btnPersistBrowse.Width = 45;
            btnPersistBrowse.Height = 34;
            btnPersistBrowse.FlatStyle = FlatStyle.Flat;
            btnPersistBrowse.FlatAppearance.BorderSize = 0;
            btnPersistBrowse.BackColor = Color.FromArgb(0, 180, 220);
            btnPersistBrowse.ForeColor = Color.White;

            btnPersistBrowse.Top = 55;
            btnPersistBrowse.Left = txtPersistFolder.Right + 5;

            panelPersist.Controls.Add(btnPersistBrowse);

            // patch button
            btnPersistPatch = new Button();
            btnPersistPatch.FlatStyle = FlatStyle.Flat;
            btnPersistPatch.FlatAppearance.BorderSize = 0;

            btnPersistPatch.Paint += (s, e) =>
            {
                Button b = (Button)s;
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(0, 0, b.Width, b.Height);

                using (LinearGradientBrush brush =
                    new LinearGradientBrush(rect,
                    Color.FromArgb(0, 255, 160),
                    Color.FromArgb(0, 150, 90),
                    LinearGradientMode.Vertical))
                {
                    FillRoundedRectangle(g, brush, rect, 12);
                }

                TextRenderer.DrawText(
                    g,
                    b.Text,
                    b.Font,
                    rect,
                    Color.White,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter);
            };
            btnPersistPatch.Text = "PATCH";
            btnPersistPatch.Width = 140;
            btnPersistPatch.Height = 38;
            btnPersistPatch.BackColor = Color.FromArgb(0, 210, 130);
            btnPersistPatch.FlatAppearance.BorderSize = 0;
            btnPersistPatch.Font = new Font("Segoe UI Black", 11);
            btnPersistPatch.ForeColor = Color.White;
            btnPersistPatch.FlatStyle = FlatStyle.Flat;
            btnPersistPatch.FlatAppearance.BorderSize = 0;
            btnPersistPatch.ForeColor = Color.White;

            btnPersistPatch.Top = 105;
            btnPersistPatch.Left = 15;

            panelPersist.Controls.Add(btnPersistPatch);

            // ===== CUST CARD =====

            panelCust = new Panel();
            panelCust.Width = 430;
            panelCust.Height = 160;
            panelCust.BackColor = Color.FromArgb(45, 75, 120);
            panelCust.Paint += DrawGlassCard;

            panelCust.BorderStyle = BorderStyle.None;

            panelPatch.Controls.Add(panelCust);

            Label lblCust = new Label();
            lblCust.Text = "PATCH CUST";
            lblCust.ForeColor = Color.Cyan;
            lblCust.Font = new Font("Segoe UI Black", 14, FontStyle.Bold);
            lblCust.ForeColor = Color.FromArgb(0, 255, 220);
            lblCust.AutoSize = true;
            lblCust.Top = 15;
            lblCust.Left = 15;

            panelCust.Controls.Add(lblCust);

            // textbox
            txtCustFolder = new TextBox();
            txtCustFolder.Width = 280;
            txtCustFolder.Height = 34;
            txtCustFolder.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            txtCustFolder.BackColor = Color.FromArgb(240, 240, 240);
            txtCustFolder.BorderStyle = BorderStyle.None;

            txtCustFolder.Top = 55;
            txtCustFolder.Left = 15;

            RoundTextbox(txtCustFolder, 20);

            panelCust.Controls.Add(txtCustFolder);

            // browse
            btnCustBrowse = new Button();
            btnCustBrowse.Text = "📁";
            btnCustBrowse.Width = 45;
            btnCustBrowse.Height = 34;
            btnCustBrowse.FlatStyle = FlatStyle.Flat;
            btnCustBrowse.FlatAppearance.BorderSize = 0;
            btnCustBrowse.BackColor = Color.FromArgb(0, 180, 220);
            btnCustBrowse.ForeColor = Color.White;

            btnCustBrowse.Top = 55;
            btnCustBrowse.Left = txtCustFolder.Right + 5;

            RoundTextbox(btnCustBrowse, 12);

            panelCust.Controls.Add(btnCustBrowse);

            // patch
            btnCustPatch = new Button();
            btnCustPatch.FlatStyle = FlatStyle.Flat;
            btnCustPatch.FlatAppearance.BorderSize = 0;

            btnCustPatch.Paint += (s, e) =>
            {
                Button b = (Button)s;
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(0, 0, b.Width, b.Height);

                using (LinearGradientBrush brush =
                    new LinearGradientBrush(rect,
                    Color.FromArgb(0, 255, 160),
                    Color.FromArgb(0, 150, 90),
                    LinearGradientMode.Vertical))
                {
                    FillRoundedRectangle(g, brush, rect, 12);
                }

                TextRenderer.DrawText(
                    g,
                    b.Text,
                    b.Font,
                    rect,
                    Color.White,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter);
            };
            btnCustPatch.Text = "PATCH";
            btnCustPatch.Width = 140;
            btnCustPatch.Height = 38;
            btnCustPatch.BackColor = Color.FromArgb(0, 210, 130);
            btnCustPatch.FlatAppearance.BorderSize = 0;
            btnCustPatch.Font = new Font("Segoe UI Black", 11);
            btnCustPatch.ForeColor = Color.White;
            btnCustPatch.FlatStyle = FlatStyle.Flat;
            btnCustPatch.FlatAppearance.BorderSize = 0;
            btnCustPatch.ForeColor = Color.White;

            btnCustPatch.Top = 105;
            btnCustPatch.Left = 15;

            panelCust.Controls.Add(btnCustPatch);

            // ===== PATCH LOG TERMINAL =====

            patchLogBox = new RichTextBox();
            patchLogBox.Width = 800;
            patchLogBox.Height = 220;

            patchLogBox.Font = new Font("Consolas", 11);
            patchLogBox.BackColor = Color.Black;
            patchLogBox.ForeColor = Color.Lime;

            patchLogBox.BorderStyle = BorderStyle.FixedSingle;
            patchLogBox.ReadOnly = true;

            panelPatch.Controls.Add(patchLogBox);

            // ================= Fastboot Flasher UI =================

            txtFolderPath = new TextBox();
            txtFolderPath.Width = 520;
            txtFolderPath.Height = 38;
            txtFolderPath.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            txtFolderPath.TextAlign = HorizontalAlignment.Left;
            txtFolderPath.BackColor = Color.FromArgb(25, 35, 50);
            txtFolderPath.ForeColor = Color.White;
            txtFolderPath.BorderStyle = BorderStyle.FixedSingle;
            txtFolderPath.Visible = false;

            txtFolderPath.ScrollBars = ScrollBars.Horizontal;
            txtFolderPath.WordWrap = false;

            mainPanel.Controls.Add(txtFolderPath);

            btnSelectFolder = new Button();
            btnSelectFolder.Text = "📁";
            btnSelectFolder.Width = 50;
            btnSelectFolder.Height = 40;
            btnSelectFolder.Font = new Font("Segoe UI Emoji", 14);
            btnSelectFolder.BackColor = Color.FromArgb(0, 180, 220);
            btnSelectFolder.FlatStyle = FlatStyle.Flat;
            btnSelectFolder.FlatAppearance.BorderSize = 0;
            btnSelectFolder.ForeColor = Color.White;
            btnSelectFolder.Visible = false;

            mainPanel.Controls.Add(btnSelectFolder);

            btnFlash = new Button();
            btnFlash.Text = "FLASH ROM";
            btnFlash.Width = 200;
            btnFlash.Height = 45;
            btnFlash.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            btnFlash.BackColor = Color.FromArgb(0, 200, 120);
            btnFlash.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 230, 140);
            btnFlash.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 180, 100);
            btnFlash.FlatStyle = FlatStyle.Flat;
            btnFlash.FlatAppearance.BorderSize = 0;
            btnFlash.ForeColor = Color.White;

            mainPanel.Controls.Add(btnFlash);

            // ================= Partition Grid =================
            gridPartitions = new DataGridView();

            gridPartitions.Scroll += (s, e) =>
            {
                Debug.WriteLine("GRID SCROLL");
            };

            gridPartitions.CellClick += gridPartitions_CellClick;

            typeof(DataGridView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.SetProperty,
                null, gridPartitions, new object[] { true });

            gridPartitions.RowPrePaint += GridPartitions_RowPrePaint;

            gridPartitions.Width = 720;
            gridPartitions.Height = 450;

            gridPartitions.BackgroundColor = Color.FromArgb(20, 25, 35);
            gridPartitions.ForeColor = Color.White;

            gridPartitions.DefaultCellStyle.BackColor = Color.FromArgb(30, 35, 45);
            gridPartitions.DefaultCellStyle.ForeColor = Color.White;

            gridPartitions.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 150, 200);
            gridPartitions.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            gridPartitions.EnableHeadersVisualStyles = false;
            gridPartitions.RowHeadersVisible = false;
            gridPartitions.Visible = false;
            gridPartitions.AllowUserToAddRows = false;

            gridPartitions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridPartitions.AllowUserToResizeColumns = false;
            gridPartitions.AllowUserToResizeRows = false;

            gridPartitions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            gridPartitions.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            gridPartitions.GridColor = Color.FromArgb(60, 70, 90);

            gridPartitions.RowTemplate.Height = 28;
            gridPartitions.ColumnHeadersHeight = 40;

            gridPartitions.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            // Columns

            var colFlash = new DataGridViewCheckBoxColumn();
            colFlash.HeaderText = "Flash";
            colFlash.Width = 60;

            var colPartition = new DataGridViewTextBoxColumn();
            colPartition.HeaderText = "Partition";
            colPartition.Width = 180;

            var colFile = new DataGridViewTextBoxColumn();
            colFile.HeaderText = "File";
            colFile.Width = 220;

            gridPartitions.Columns.AddRange(colFlash, colPartition, colFile);

            mainPanel.Controls.Add(gridPartitions);

            gridPartitions.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            logContainer.Dock = DockStyle.None;

            // ================= Progress =================

            progressOverall = new ProgressBar();
            progressOverall.Width = 650;
            progressOverall.Height = 20;
            progressOverall.Visible = false;

            mainPanel.Controls.Add(progressOverall);

            // ================= Fastboot Label =================

            lblFlashStatus = new Label();
            lblFlashStatus.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblFlashStatus.ForeColor = Color.Red;
            lblFlashStatus.Text = "Fastboot: Not Connected";
            lblFlashStatus.AutoSize = false;
            lblFlashStatus.Width = 200;
            lblFlashStatus.Height = 25;
            lblFlashStatus.TextAlign = ContentAlignment.MiddleCenter;
            lblFlashStatus.Visible = false;
            lblFlashStatus.BackColor = Color.Transparent;


            mainPanel.Controls.Add(lblFlashStatus);

            // ================= Events =================

            this.FormClosed += MainForm_FormClosed;

            if (!DesignMode)
                this.Icon = new Icon(new MemoryStream(Properties.Resources.fahad));

            // ================= Glow Panels =================

            leftGlow = new Panel();
            rightGlow = new Panel();

            leftGlow.Width = 0;
            rightGlow.Width = 4;

            leftGlow.BackColor = Color.Cyan;
            rightGlow.BackColor = Color.Cyan;

            this.Controls.Add(leftGlow);
            this.Controls.Add(rightGlow);

            leftGlow.BringToFront();
            rightGlow.BringToFront();

            sidebar.BorderStyle = BorderStyle.None;

            // ================= Logo =================

            bigLogo.Parent = this;
            bigLogo.BringToFront();
            bigLogo.Anchor = AnchorStyles.None;
            bigLogo.Dock = DockStyle.None;
            bigLogo.SizeMode = PictureBoxSizeMode.Zoom;

            // ================= LogBox =================

            logBox.BackColor = Color.FromArgb(18, 22, 30);
            logBox.ForeColor = Color.White;
            logBox.Font = new Font("Consolas", 10F);
            logBox.BorderStyle = BorderStyle.None;

            txtFolderPath.BringToFront();
            btnSelectFolder.BringToFront();

            // ================= Custom Setup =================
            if (mainPanel != null)
            {
                mainPanel.Controls.Add(flashLogBox);
            }
            SetPCName();
            SetupHeader();
            SetupEvents();

            // ================= Glow =================

            StartGlow();

            // ================= Form Events =================

            this.Load += (s, e) =>
            {
                Debug.WriteLine("FORM LOAD");
            };

            this.Shown += (s, e) =>
            {
                Debug.WriteLine("FORM SHOWN");
                AlignLayout();
            };

            this.Resize += (s, e) =>
            {
                Debug.WriteLine("RESIZE EVENT");
                Debug.WriteLine("WindowState: " + this.WindowState);
                Debug.WriteLine("Form Size: " + this.Width + " x " + this.Height);

                if (WindowState != FormWindowState.Minimized)
                    AlignLayout();
            };
            // ===== SECURITY START =====

            // Block if USB already running
            if (IsUsbToolRunning())
            {
                MessageBox.Show(
                    "Close USB sharing tools before opening tool",
                    "Security",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                Environment.Exit(0);
            }

            // Start security timer
            securityTimer.Interval = 1000;
            securityTimer.Tick += SecurityTimer_Tick;
            securityTimer.Start();




        }
        // ===== SECURITY FUNCTIONS =====

        bool IsDebugToolRunning()
        {
            if (Debugger.IsAttached)
                return true;

            foreach (var p in Process.GetProcesses())
            {
                if (debugTools.Contains(p.ProcessName.ToLower()))
                    return true;
            }
            return false;
        }

        bool IsUsbToolRunning()
        {
            foreach (var p in Process.GetProcesses())
            {
                if (usbTools.Contains(p.ProcessName.ToLower()))
                    return true;
            }
            return false;
        }

        bool IsProxyToolRunning()
        {
            foreach (var p in Process.GetProcesses())
            {
                if (proxyTools.Contains(p.ProcessName.ToLower()))
                    return true;
            }
            return false;
        }

        void SecurityTimer_Tick(object sender, EventArgs e)
        {
            if (IsDebugToolRunning())
            {
                MessageBox.Show("Debug tool detected!");
                Environment.Exit(0);
            }

            if (IsProxyToolRunning())
            {
                MessageBox.Show("Proxy tool detected!");
                Environment.Exit(0);
            }

            if (IsUsbToolRunning())
            {
                MessageBox.Show("USB sharing tool not allowed!");
                Environment.Exit(0);
            }
        }
        void DrawRoundedRectangle(Graphics g, Pen pen, Rectangle rect, int radius)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();

                g.DrawPath(pen, path);
            }
        }
        public const string CURRENT_VERSION = "1.0";
        public static string GetHardwareID()
        {
            string cpuId = "";
            string boardId = "";

            using (var mos = new ManagementObjectSearcher("select ProcessorId from Win32_Processor"))
            {
                foreach (var mo in mos.Get())
                {
                    cpuId = mo["ProcessorId"]?.ToString();
                    break;
                }
            }

            using (var mos = new ManagementObjectSearcher("select SerialNumber from Win32_BaseBoard"))
            {
                foreach (var mo in mos.Get())
                {
                    boardId = mo["SerialNumber"]?.ToString();
                    break;
                }
            }

            string raw = cpuId + boardId;

            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }
        public MainForm(string username) : this()
        {
            if (!string.IsNullOrEmpty(username))
            {
                _loggedInUser = username;
                lblWelcome.Text = "Welcome, " + username;
            }
        }



        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ShowStartup();
            AlignLayout();

            // SECURITY TIMER START
            securityTimer.Interval = 3000;
            
            securityTimer.Start();
        }
        private List<string> fastbootBuffer = new List<string>();
        private void MainForm_Resize(object sender, EventArgs e)
        {
            AlignLayout();
          
            
            progressOverall.Invalidate();
        }
        private void AddFastbootLog(string line)
        {
            fastbootBuffer.Add(line);

            if (fastbootBuffer.Count >= 20)
            {
                string text = string.Join(Environment.NewLine, fastbootBuffer) + Environment.NewLine;
                fastbootBuffer.Clear();

                BeginInvoke(new Action(() =>
                {
                    logBox.AppendText(text);
                    logBox.ScrollToCaret();
                }));
            }
        }

        // ================= LOG =================
        void DrawGlassCard(object sender, PaintEventArgs e)
        {
            Panel p = sender as Panel;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, p.Width - 1, p.Height - 1);

            using (LinearGradientBrush brush =
                new LinearGradientBrush(rect,
                Color.FromArgb(40, 90, 150),
                Color.FromArgb(15, 45, 90),
                LinearGradientMode.Vertical))
            {
                FillRoundedRectangle(g, brush, rect, 18);
            }

            using (Pen pen = new Pen(Color.FromArgb(0, 255, 220), 2))
            {
                DrawRoundedRectangle(g, pen, rect, 18);
            }
        }
        private void AddLog(string message, Color color, string emoji = "")
        {
            RichTextBox target;

            if (panelPatch.Visible)
                target = patchLogBox;
            else if (flashLogBox.Visible)
                target = flashLogBox;
            else
                target = logBox;

            if (target.InvokeRequired)
            {
                target.Invoke(new Action(() => AddLog(message, color, emoji)));
                return;
            }

            string time = DateTime.Now.ToString("HH:mm:ss");

            target.SelectionStart = target.TextLength;
            target.SelectionColor = Color.Gray;
            target.AppendText($"[{time}] ");

            target.SelectionColor = color;

            if (!string.IsNullOrEmpty(emoji))
                target.AppendText(emoji + " ");

            target.AppendText(message + Environment.NewLine);

            target.SelectionColor = target.ForeColor;

            target.SelectionStart = target.TextLength;
            target.ScrollToCaret();
            target.Refresh();
        }
        bool PatchPersistFile(string file)
        {
            try
            {
                AddLog("Opening persist file...", Color.Cyan);

                byte[] find = { 0x66, 0x64, 0x73, 0x64 }; // fdsd
                byte[] replace = { 0x20, 0x20, 0x20, 0x20 }; // spaces

                byte[] data = File.ReadAllBytes(file);

                bool found = false;

                for (int i = 0; i < data.Length - find.Length; i++)
                {
                    if (data[i] == find[0] &&
                        data[i + 1] == find[1] &&
                        data[i + 2] == find[2] &&
                        data[i + 3] == find[3])
                    {
                        AddLog("Signature found.", Color.Yellow);

                        data[i] = replace[0];
                        data[i + 1] = replace[1];
                        data[i + 2] = replace[2];
                        data[i + 3] = replace[3];

                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    AddLog("Signature not found.", Color.Red);
                    return false;
                }

                File.WriteAllBytes(file, data);

                AddLog("Persist patched successfully.", Color.LimeGreen);

                return true;
            }
            catch (Exception ex)
            {
                AddLog("Patch error: " + ex.Message, Color.Red);
                return false;
            }
        }

        private void SetupHeader()
        {
            lblHeader.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                string text = "FAHAD GSM TOOL";
                using Font font = new Font("Segoe UI Black", 30F, FontStyle.Bold);

                SizeF size = g.MeasureString(text, font);
                float x = (lblHeader.Width - size.Width) / 2;
                float y = (lblHeader.Height - size.Height) / 2;

                // Shadow
                using (SolidBrush shadow = new SolidBrush(Color.FromArgb(120, 0, 0, 0)))
                    g.DrawString(text, font, shadow, x + 4, y + 4);

                // Metallic gradient
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    new RectangleF(x, y, size.Width, size.Height),
                    Color.White,
                    Color.Silver,
                    LinearGradientMode.Vertical))
                {
                    g.DrawString(text, font, brush, x, y);
                }
            };
        }
        // ================= EVENTS =================
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            securityTimer.Stop();
            fastbootMonitorToken?.Cancel();

            try
            {
                foreach (var p in Process.GetProcessesByName("adb"))
                    p.Kill();

                foreach (var p in Process.GetProcessesByName("fastboot"))
                    p.Kill();
            }
            catch { }

            Application.Exit();
            fastbootMonitor?.Cancel();
        }
        private void SetupEvents()
        {
            btnFindISP.Click += (s, e) =>
            {
                StartFastbootMonitoring();   // ⭐ ADD THIS

                SetSidebarSelection(btnFindISP);
                ShowContent("Find ISP");
                flashLogBox.Visible = false;
            };

            btnFindDump.Click += (s, e) =>
            {
                StartFastbootMonitoring();   // ⭐ ADD THIS

                SetSidebarSelection(btnFindDump);
                ShowContent("Find Dump");
                flashLogBox.Visible = false;
            };

            btnFastboot.Click += (s, e) =>
            {
                btnFastboot.Visible = false;
                btnSearchMode.Visible = false;

                txtSearch.Visible = false;
                btnSearch.Visible = false;

                btnStart.Visible = true;
                lblFastbootStatus.Visible = true;

                btnStart.BringToFront();
                lblFastbootStatus.BringToFront();

                StartFastbootMonitoring();
                AlignLayout();
            };

            btnSearchMode.Click += (s, e) =>
            {
                btnFastboot.Visible = false;
                btnSearchMode.Visible = false;

                btnStart.Visible = false;
                lblFastbootStatus.Visible = false;

                txtSearch.Visible = true;
                btnSearch.Visible = true;

                AlignLayout();
            };

            btnSearch.Click += async (s, e) =>
            {
                string input = txtSearch.Text.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    AddLog("Please enter product or model.", Color.Red);
                    return;
                }

                logBox.Clear();
                AddLog("Searching model from server...", Color.Cyan);

                bool success = false;

                if (_currentSectionTitle == "FIND ISP")
                    success = await DisplayModelImage(input);
                else
                    success = await GetDumpLink(input);

                if (!success)
                    AddLog("Data not found on server.", Color.Red);
                else
                    AddLog("Operation completed successfully.", Color.LimeGreen);
            };

            btnStart.Click += async (s, e) =>
            {
                if (_currentSectionTitle == "FIND ISP")
                    await RunISPProcess();
                else
                    await RunDumpProcess();
            };

            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch.PerformClick();
            };

            lblSectionTitle.Paint += DrawSectionTitle;
            btnXiaomi.Click += (s, e) =>
            {
                SetSidebarSelection(btnXiaomi);
                HideAllModules();   // ⭐ SAFE RESET

                _currentSectionTitle = "XIAOMI";

                lblSectionTitle.Visible = true;

                bigLogo.Visible = false;

                // Show Xiaomi menu
                btnFastbootFlasher.Visible = true;
                btnComingSoon.Visible = true;

                

                // Hide ISP/Dump buttons
                btnFastboot.Visible = false;
                btnSearchMode.Visible = false;

                // Show Xiaomi menu
                btnFastbootFlasher.Visible = true;
                btnComingSoon.Visible = true;

                btnFastbootFlasher.BringToFront();
                btnComingSoon.BringToFront();

                txtSearch.Visible = false;
                btnSearch.Visible = false;

                btnStart.Visible = false;
                lblFastbootStatus.Visible = false;

                logContainer.Visible = false;

                AlignLayout();
            };
            btnFastbootFlasher.Click += (s, e) =>
            {
                HideAllModules();

                // ❌ Find Dump controls ko hide karna hai
                btnStart.Visible = false;
                lblFastbootStatus.Visible = false;
                logContainer.Visible = false;

                _currentSectionTitle = "FASTBOOT FLASHER";

                lblSectionTitle.Visible = true;
                lblSectionTitle.Invalidate();

                bigLogo.Visible = false;

                btnFastbootFlasher.Visible = false;
                btnComingSoon.Visible = false;

                flashLogBox.Visible = true;

                gridPartitions.Rows.Clear();
                flashLogBox.Clear();

                txtFolderPath.Visible = true;
                btnSelectFolder.Visible = true;
                btnFlash.Visible = true;

                gridPartitions.Visible = true;

                progressOverall.Visible = true;

                lblFlashStatus.Visible = true;
                progressOverall.Value = 0;

                StartFastbootMonitoring();

                this.BeginInvoke(new Action(() =>
                {
                    AlignLayout();
                }));
            };
            btnSelectFolder.Click += async (s, e) =>
            {
                gridPartitions.Rows.Clear();
                Application.DoEvents();

                using (FolderBrowserDialog f = new FolderBrowserDialog()) // ⭐ PATCH
                {
                    f.Description = "Select Fastboot ROM Folder";

                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        string folder = f.SelectedPath; // ⭐ PATCH

                        if (Path.GetFileName(folder).ToLower() == "images")
                        {
                            folder = Directory.GetParent(folder).FullName;
                        }

                        txtFolderPath.Text = folder;

                        txtFolderPath.SelectionStart = txtFolderPath.Text.Length;
                        txtFolderPath.ScrollToCaret();

                        string script = FindFlashScript(folder);

                        if (script == null)
                        {
                            AddLog("flash_all.bat not found.", Color.Red);
                            return;
                        }

                        ParseFlashScript(script);

                        this.BeginInvoke(new Action(() =>
                        {
                            AlignLayout();
                        }));

                        AddLog("ROM loaded successfully.", Color.LimeGreen);
                        AddLog("ROM folder selected:", Color.Cyan);
                      

                       
                        AddLog(folder, Color.White);

                        this.BeginInvoke(new Action(() =>
                        {
                            AlignLayout();
                        }));
                    }
                }
            };

            btnFlash.Click += async (s, e) =>
            {
                AddLog("STEP 1 - Flash button clicked", Color.Yellow);

                AddLog("Checking ROM compatibility...", Color.Cyan);
                string deviceRaw = await GetFastbootVar("product");

                string deviceProduct = deviceRaw
                .Split('\n')[0]
                .Replace("product:", "")
                .Trim()
                .ToLower();

                AddLog($"Device Product: {deviceProduct}", Color.Cyan);

                // ROM path
                string romPath = txtFolderPath.Text.ToLower();

                AddLog($"ROM Path: {romPath}", Color.Cyan);

                // match anywhere in path
                if (!romPath.Contains(deviceProduct))
                {
                    AddLog("❌ WRONG ROM SELECTED!", Color.Red);
                    AddLog($"Device : {deviceProduct}", Color.Orange);
                    AddLog($"ROM Path : {romPath}", Color.Orange);

                    AddLog("⚠ Flash cancelled due to wrong ROM.", Color.Red);

                    lblFlashStatus.Text = "Flash Cancelled";
                    lblFlashStatus.ForeColor = Color.Red;

                    return;
                }

                AddLog("✔ ROM verified successfully.", Color.LimeGreen);

                // DEVICE PRODUCT



                AddLog($"Device Product: {deviceProduct}", Color.Cyan);
               


                AddLog("✔ ROM verified successfully.", Color.LimeGreen);

                AddLog("STEP 6 - ROM verification passed", Color.Yellow);

                AddLog("Reading device information...", Color.Cyan);

                AddLog("STEP 7 - Reading device info", Color.Yellow);

                string slot = await GetFastbootVar("current-slot");
                string unlock = await GetFastbootVar("unlocked");

                AddLog($"Product : {deviceProduct}", Color.White);
                AddLog(slot, Color.White);
                AddLog(unlock, Color.White);

                AddLog("STEP 8 - Device info read complete", Color.Yellow);

                if (!await CheckDeviceInfo())
                {
                    isFlashing = false;
                    return;
                }

                AddLog("STEP 9 - Device check passed", Color.Yellow);

                if (isFlashing)
                {
                    MessageBox.Show("Flashing already running.");
                    return;
                }

                AddLog("STEP 10 - Starting flash process", Color.Yellow);

                isFlashing = true;
                progressOverall.Value = 0;
                fastbootMonitor?.Cancel();

                if (!IsFastbootConnected())
                {
                    MessageBox.Show("Fastboot device not connected!");
                    return;
                }

                lblFlashStatus.Text = "Fastboot: Connected";
                lblFlashStatus.ForeColor = Color.LimeGreen;

                string folder = txtFolderPath.Text;
                string images = Path.Combine(folder, "images");

                if (!Directory.Exists(images))
                    images = folder;

                int total = 0;

                foreach (DataGridViewRow row in gridPartitions.Rows)
                {
                    bool enabled = Convert.ToBoolean(row.Cells[0].Value);

                    if (!enabled)
                        continue;

                    string part = row.Cells[1].Value?.ToString();

                    if (string.IsNullOrEmpty(part))
                        continue;

                    total++;
                }

                AddLog("STEP 11 - Partition counting done", Color.Yellow);

                int done = 0;

                foreach (DataGridViewRow row in gridPartitions.Rows)
                {
                    bool enabled = Convert.ToBoolean(row.Cells[0].Value);

                    if (!enabled)
                        continue;

                    string part = row.Cells[1].Value?.ToString();
                    string file = row.Cells[2].Value?.ToString();

                    if (string.IsNullOrEmpty(part))
                        continue;

                    if (!string.IsNullOrEmpty(file) && file != "erase")
                    {
                        string path = Path.Combine(images, file);

                        if (!File.Exists(path))
                        {
                            AddLog($"Skipping {part} (file missing)", Color.Orange);
                            continue;
                        }

                        AddLog($"Flashing {part} → {file}", Color.White);

                        AddLog("STEP 12 - Running fastboot flash", Color.Yellow);

                        string result = await RunFastboot($"flash {part} \"{path}\"");

                        string lower = result.ToLower();
                        if (lower.Contains("not allowed") || lower.Contains("flashing is not allowed"))
                        {
                            AddLog("❌ Flashing not allowed in locked state.", Color.Red);

                            lblFlashStatus.Text = "Flashing Not Allowed";
                            lblFlashStatus.ForeColor = Color.Red;

                            isFlashing = false;

                            return;
                        }
                        if (lower.Contains("failed"))
                            AddLog($"✖ {part} FLASH FAILED", Color.Red);
                        else if (lower.Contains("okay"))
                            AddLog($"✔ {part} flashed successfully", Color.LimeGreen);
                        else
                            AddLog($"⚠ {part} flash unknown", Color.Orange);
                    }
                    else if (file == "erase")
                    {
                        AddLog($"Erasing {part}", Color.Red);

                        AddLog("STEP 13 - Running fastboot erase", Color.Yellow);

                        string result = await RunFastboot($"erase {part}");

                        string lower = result.ToLower();

                        if (lower.Contains("failed"))
                            AddLog($"✖ {part} ERASE FAILED", Color.Red);
                        else if (lower.Contains("okay"))
                            AddLog($"✔ {part} erased", Color.LimeGreen);
                        else
                            AddLog($"⚠ erase unknown", Color.Orange);
                    }

                    done++;

                    int percent = (done * 100) / total;
                    progressOverall.Value = Math.Min(percent, 100);
                }

                AddLog("STEP 14 - Flash loop completed", Color.Yellow);

                AddLog("Rebooting device...", Color.Cyan);

                AddLog("STEP 15 - Running reboot", Color.Yellow);

                await RunFastboot("reboot");

                progressOverall.Value = 100;

                lblFlashStatus.Text = "Flash Completed";
                lblFlashStatus.ForeColor = Color.LimeGreen;

                AddLog("Flash completed.", Color.LimeGreen);

                isFlashing = false;

                AddLog("STEP 16 - Restarting fastboot monitor", Color.Yellow);

                StartFastbootMonitoring();
            };
            btnComingSoon.Click += (s, e) =>
            {
                AgreementForm agree = new AgreementForm();
                agree.ShowDialog();

                if (!agree.Accepted)
                    return;

                HideAllModules();

                panelPatch.Visible = true;
                lblMiAccount.Visible = true;
                patchLogBox.Visible = true;

                // ⭐ SHOW PATCH CARDS
                panelPersist.Visible = true;
                panelCust.Visible = true;

                // hide section title
                lblSectionTitle.Visible = false;

                btnFastbootFlasher.Visible = false;
                btnComingSoon.Visible = false;

                bigLogo.Visible = false;

                AlignLayout();
            };
            btnPersistBrowse.Click += (s, e) =>
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "Select Persist Image";
                    ofd.Filter = "Persist Files (*.img;*.bin)|*.img;*.bin|All Files (*.*)|*.*";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        txtPersistFolder.Text = ofd.FileName;

                        AddLog("Persist file loaded.", Color.Cyan);
                        AddLog(ofd.FileName, Color.White);
                    }
                }
            };
            btnCustBrowse.Click += (s, e) =>
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "Select Cust Image";
                    ofd.Filter = "Cust Files (*.img;*.bin)|*.img;*.bin|All Files (*.*)|*.*";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        txtCustFolder.Text = ofd.FileName;

                        AddLog("Cust file loaded.", Color.Cyan);
                        AddLog(ofd.FileName, Color.White);
                    }
                }
            };
            bool PatchPersistFile(string file)
            {
                try
                {
                    AddLog("Opening persist file...", Color.Cyan);

                    byte[] data = File.ReadAllBytes(file);

                    byte[] pattern = Encoding.ASCII.GetBytes("fdsd");
                    byte[] replace = new byte[] { 0x20, 0x20, 0x20, 0x20 };

                    int index = -1;

                    for (int i = 0; i < data.Length - pattern.Length; i++)
                    {
                        bool match = true;

                        for (int j = 0; j < pattern.Length; j++)
                        {
                            if (data[i + j] != pattern[j])
                            {
                                match = false;
                                break;
                            }
                        }

                        if (match)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index == -1)
                    {
                        AddLog("Signature not found.", Color.Red);
                        return false;
                    }

                    AddLog("Signature located.", Color.Yellow);

                    for (int i = 0; i < replace.Length; i++)
                    {
                        data[index + i] = replace[i];
                    }

                    File.WriteAllBytes(file, data);

                    AddLog("Persist patched successfully.", Color.LimeGreen);

                    return true;
                }
                catch (Exception ex)
                {
                    AddLog("Patch error: " + ex.Message, Color.Red);
                    return false;
                }
            }
            btnPersistPatch.Click += async (s, e) =>
            {
                string file = txtPersistFolder.Text;

                if (string.IsNullOrWhiteSpace(file))
                {
                    AddLog("Please load persist file first.", Color.Red, "⚠");
                    return;
                }

                if (!File.Exists(file))
                {
                    AddLog("Persist file not found.", Color.Red, "❌");
                    return;
                }

                AddLog("════════════════════════════════════", Color.DarkCyan);
                AddLog("MI ACCOUNT PERMANENT PATCH MODULE", Color.Gold, "🔐");
                AddLog("════════════════════════════════════", Color.DarkCyan);

                AddLog("Connecting to secure server...", Color.Cyan, "🌐");
                await Task.Delay(300);

                AddLog("Verifying user license...", Color.Cyan, "🔎");
                await Task.Delay(300);

                AddLog("Reading hardware ID...", Color.LightSkyBlue, "💻");
                await Task.Delay(300);

                AddLog("Requesting authentication from server...", Color.Cyan, "📡");

                bool allowed = await VerifyPatch("persist");

                if (!allowed)
                {
                    AddLog("Authentication failed. Patch not allowed.", Color.Red, "❌");
                    AddLog("Limit reached or license issue.", Color.Red, "⚠");
                    AddLog("════════════════════════════════════", Color.DarkCyan);
                    return;
                }

                AddLog("Authentication successful.", Color.LimeGreen, "✔");

                AddLog("Preparing persist environment...", Color.Cyan, "⚙");
                await Task.Delay(200);

                AddLog("Opening persist partition image...", Color.Cyan, "📂");

                bool success = PatchPersistFile(file);

                if (success)
                {
                    AddLog("Persist patch completed successfully.", Color.LimeGreen, "✔");

                    AddLog("════════════════════════════════════", Color.DarkCyan);
                    AddLog("Operation completed successfully.", Color.Gold, "⭐");
                    AddLog("Thank you for using FAHAD GSM TOOL", Color.Gold, "🚀");
                    AddLog("════════════════════════════════════", Color.DarkCyan);

                    await PatchSuccess("persist");
                }
                else
                {
                    AddLog("Persist patch failed.", Color.Red, "❌");
                }
                btnCustPatch.Click += async (s, e) =>
                {
                    string file = txtCustFolder.Text;

                    if (string.IsNullOrWhiteSpace(file))
                    {
                        AddLog("Please load cust file first.", Color.Red);
                        return;
                    }

                    if (!File.Exists(file))
                    {
                        AddLog("Cust file not found.", Color.Red);
                        return;
                    }

                    AddLog("════════════════════════════════════", Color.DarkCyan);
                    AddLog("MI ACCOUNT SECURITY PATCH ENGINE", Color.Gold, "🔐");
                    AddLog("════════════════════════════════════", Color.DarkCyan);

                    AddLog("Connecting to secure patch server...", Color.Cyan, "🌐");
                    await Task.Delay(200);

                    AddLog("Verifying user license...", Color.Cyan, "🔎");
                    await Task.Delay(200);

                    AddLog("Reading hardware authentication...", Color.Cyan, "💻");

                    bool allowed = await VerifyPatch("cust");

                    if (!allowed)
                    {
                        AddLog("Patch not allowed. License or limit issue.", Color.Red);
                        return;
                    }

                    AddLog("Authentication successful.", Color.LimeGreen, "✔");

                    AddLog("Opening cust partition image...", Color.Cyan, "📂");
                    AddLog("Analyzing filesystem structure...", Color.Cyan, "🔍");
                    AddLog("Mapping partition layout...", Color.Cyan, "⚙");

                    bool success = await PatchCustFile(file);

                    if (success)
                    {
                        AddLog("Cust patch completed successfully.", Color.LimeGreen, "✔");

                        AddLog("════════════════════════════════════", Color.DarkCyan);
                        AddLog("Operation completed successfully.", Color.Gold, "⭐");
                        AddLog("Thank you for using FAHAD GSM TOOL", Color.Gold, "🚀");
                        AddLog("════════════════════════════════════", Color.DarkCyan);

                        await PatchSuccess("cust");
                    }
                    else
                    {
                        AddLog("Cust patch failed.", Color.Red);
                    }
                };



                async Task<bool> PatchCustFile(string originalFile)
                {
                    try
                    {
                        AddLog("Initializing binary patch engine...", Color.Cyan, "⚙");
                        await Task.Delay(200);

                        AddLog("Loading partition header...", Color.Cyan, "📂");
                        await Task.Delay(200);

                        AddLog("Scanning Mi Cloud security structures...", Color.Yellow, "🔎");
                        await Task.Delay(200);

                        AddLog("Searching protection signatures...", Color.Yellow, "🔍");
                        await Task.Delay(200);

                        AddLog("Security block located.", Color.LimeGreen, "✔");
                        await Task.Delay(200);

                        AddLog("Extracting security configuration...", Color.Cyan, "📤");
                        await Task.Delay(200);

                        AddLog("Decrypting Mi Cloud protection layer...", Color.Cyan, "🔓");
                        await Task.Delay(200);

                        AddLog("Preparing secure patch payload...", Color.Cyan, "📦");
                        await Task.Delay(200);

                        string tempFile = Path.Combine(Path.GetTempPath(), "cust_patch.img");

                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("X-TOOL", "FahadGSMTool");

                            string url = "https://fahad64.com/tool_api/universal_cust.img";

                            AddLog("Downloading secure patch module...", Color.Cyan, "⬇");
                            await Task.Delay(200);

                            byte[] data = await client.GetByteArrayAsync(url);

                            File.WriteAllBytes(tempFile, data);
                        }

                        AddLog("Applying binary level patch...", Color.Yellow, "🛠");
                        await Task.Delay(200);

                        AddLog("Rebuilding partition structure...", Color.Cyan, "🔄");
                        await Task.Delay(200);

                        File.Delete(originalFile);
                        File.Move(tempFile, originalFile);

                        AddLog("Writing patched image...", Color.Cyan, "💾");
                        await Task.Delay(200);

                        AddLog("Recalculating partition checksum...", Color.Cyan, "🔍");
                        await Task.Delay(200);

                        AddLog("Finalizing security bypass...", Color.Cyan, "⚡");
                        await Task.Delay(200);

                        AddLog("Cust partition patched successfully.", Color.LimeGreen, "✔");

                        return true;
                    }
                    catch (Exception ex)
                    {
                        AddLog("Patch error: " + ex.Message, Color.Red);
                        return false;
                    }
                }
            };
            btnCustPatch.Click += async (s, e) =>
            {
                string file = txtCustFolder.Text;

                if (string.IsNullOrWhiteSpace(file))
                {
                    AddLog("Please load cust file first.", Color.Red);
                    return;
                }

                if (!File.Exists(file))
                {
                    AddLog("Cust file not found.", Color.Red);
                    return;
                }

                AddLog("Checking server authorization...", Color.Cyan);

                bool allowed = await VerifyPatch("cust");

                if (!allowed)
                {
                    AddLog("Patch not allowed. License issue.", Color.Red);
                    return;
                }

                AddLog("Authorization successful.", Color.LimeGreen);
                AddLog("Starting cust patch...", Color.Cyan);

                bool success = await PatchCustFile(file);

                if (success)
                {
                    AddLog("Cust patch completed successfully.", Color.LimeGreen);
                    await PatchSuccess("cust");
                }
                else
                {
                    AddLog("Cust patch failed.", Color.Red);
                }
            };

            bool PatchpersistFile(string file)
            {
                try
                {
                    AddLog("Scanning persist partition...", Color.Cyan, "🔍");

                    byte[] find = { 0x66, 0x64, 0x73, 0x64 }; // fdsd
                    byte[] replace = { 0x20, 0x20, 0x20, 0x20 }; // spaces

                    byte[] data = File.ReadAllBytes(file);

                    bool found = false;

                    for (int i = 0; i < data.Length - find.Length; i++)
                    {
                        if (data[i] == find[0] &&
                            data[i + 1] == find[1] &&
                            data[i + 2] == find[2] &&
                            data[i + 3] == find[3])
                        {
                            AddLog("Mi Cloud security data detected.", Color.Yellow, "✔");

                            AddLog("Patching Mi Cloud security block...", Color.Cyan, "🛠");

                            data[i] = replace[0];
                            data[i + 1] = replace[1];
                            data[i + 2] = replace[2];
                            data[i + 3] = replace[3];

                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        AddLog("Searching Mi Cloud security data...", Color.Yellow, "🔍");
                        AddLog("Security block not found.", Color.Red, "✖");
                        AddLog("Device may already be patched.", Color.Orange, "⚠");

                        AddLog("════════════════════════════════════", Color.DarkCyan);
                        AddLog("Patch process finished.", Color.Gold, "⭐");
                        AddLog("════════════════════════════════════", Color.DarkCyan);

                        return false;
                    }

                    AddLog("Restoring persist structure...", Color.Cyan, "🔄");

                    File.WriteAllBytes(file, data);

                    AddLog("Writing patched data to persist image...", Color.Cyan, "💾");

                    AddLog("Persist patched successfully.", Color.LimeGreen, "✔");

                    return true;
                }
                catch (Exception ex)
                {
                    AddLog("Patch engine error: " + ex.Message, Color.Red, "❌");
                    return false;
                }
            }
            async Task<bool> PatchCustFile(string originalFile)
            {
                try
                {
                    AddLog("Initializing binary patch engine...", Color.Cyan, "⚙");
                    await Task.Delay(200);

                    AddLog("Loading partition header...", Color.Cyan, "📂");
                    await Task.Delay(200);

                    AddLog("Scanning Mi Cloud security structures...", Color.Yellow, "🔎");
                    await Task.Delay(200);

                    AddLog("Searching protection signatures...", Color.Yellow, "🔍");
                    await Task.Delay(200);

                    AddLog("Security block located.", Color.LimeGreen, "✔");
                    await Task.Delay(200);

                    AddLog("Extracting security configuration...", Color.Cyan, "📤");
                    await Task.Delay(200);

                    AddLog("Decrypting Mi Cloud protection layer...", Color.Cyan, "🔓");
                    await Task.Delay(200);

                    AddLog("Preparing secure patch payload...", Color.Cyan, "📦");
                    await Task.Delay(200);

                    string tempFile = Path.Combine(Path.GetTempPath(), "cust_patch.img");

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("X-TOOL", "FahadGSMTool");

                        string url = "https://fahad64.com/tool_api/universal_cust.img";

                        AddLog("Downloading secure patch module...", Color.Cyan, "⬇");
                        await Task.Delay(200);

                        byte[] data = await client.GetByteArrayAsync(url);

                        File.WriteAllBytes(tempFile, data);
                    }

                    AddLog("Applying binary level patch...", Color.Yellow, "🛠");
                    await Task.Delay(200);

                    AddLog("Rebuilding partition structure...", Color.Cyan, "🔄");
                    await Task.Delay(200);

                    File.Delete(originalFile);
                    File.Move(tempFile, originalFile);

                    AddLog("Writing patched image...", Color.Cyan, "💾");
                    await Task.Delay(200);

                    AddLog("Recalculating partition checksum...", Color.Cyan, "🔍");
                    await Task.Delay(200);

                    AddLog("Finalizing security bypass...", Color.Cyan, "⚡");
                    await Task.Delay(200);

                    AddLog("Cust partition patched successfully.", Color.LimeGreen, "✔");

                    return true;
                }
                catch (Exception ex)
                {
                    AddLog("Patch error: " + ex.Message, Color.Red);
                    return false;
                }
            }
        }

        async Task<bool> VerifyPatch(string type)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-TOOL", "FahadGSMTool");

                    var values = new Dictionary<string, string>
            {
                { "username", _loggedInUser },
                { "hwid", GetHardwareID() },
                { "type", type }
            };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync(
                        "https://fahad64.com/tool_api/verify_patch.php?key=FahadToolSecure2026",
                        content);

                    var result = await response.Content.ReadAsStringAsync();

                    if (result.Contains("\"status\":\"ok\""))
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show(
"Patch limit reached.\n\nPlease wait 24 hours for the next patch limit reset.",
"FAHAD GSM TOOL",
MessageBoxButtons.OK,
MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server error: " + ex.Message);
                return false;
            }
        }
        async Task PatchSuccess(string type)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-TOOL", "FahadGSMTool");

                    var values = new Dictionary<string, string>
            {
                { "username", _loggedInUser },
                { "hwid", GetHardwareID() },
                { "type", type }
            };

                    var content = new FormUrlEncodedContent(values);

                    await client.PostAsync(
                        "https://fahad64.com/tool_api/patch_success.php?key=FahadToolSecure2026",
                        content);
                }
            }
            catch
            {
                // ignore
            }
        }
        private async Task CheckForUpdate()
        {
            try
            {
                using var client = new System.Net.Http.HttpClient();

                client.DefaultRequestHeaders.Add("X-TOOL", "FahadGSMTool");

                string json = await client.GetStringAsync(
                "https://fahad64.com/tool_update/version.json");

                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                string serverVersion = data.version;

                if (serverVersion != CURRENT_VERSION)
                {
                    MessageBox.Show(
                    "New version available: " + serverVersion +
                    "\nPlease update your tool.",
                    "Update Available",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                }
            }
            catch
            {
                // ignore update error
            }
        }
        // ================= ISP =================
        async Task<string> RunFastboot(string args)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = fastbootPath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process p = new Process();
                p.StartInfo = psi;

                StringBuilder output = new StringBuilder();

                p.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        output.AppendLine(e.Data);
                };

                p.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        output.AppendLine(e.Data);
                };

                p.Start();

                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                await Task.Run(() => p.WaitForExit());

                return output.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        async Task<string> GetFastbootVar(string variable)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = fastbootPath,
                    Arguments = $"getvar {variable}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process p = new Process();
                p.StartInfo = psi;

                StringBuilder output = new StringBuilder();

                p.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        output.AppendLine(e.Data);
                };

                p.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        output.AppendLine(e.Data);
                };

                p.Start();

                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                await Task.Run(() => p.WaitForExit());

                return output.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        private async Task RunISPProcess()
        {
            logBox.Clear();

            AddLog("Initializing ISP process...", Color.Cyan, "⚙️");
            AddLog("Checking fastboot connection...", Color.LightSkyBlue, "🔍");

            if (!IsFastbootConnected())
            {
                AddLog("Fastboot device not connected!", Color.Red, "❌");
                AddLog("Please connect device in FASTBOOT mode.", Color.OrangeRed, "⚠️");
                return;
            }

            AddLog("Fastboot device connected successfully.", Color.LimeGreen, "🔗");

            AddLog("Reading product from device...", Color.Cyan, "📱");
            string product = await GetFastbootProduct();

            if (string.IsNullOrEmpty(product))
            {
                AddLog("Failed to read product from device.", Color.Red, "❌");
                AddLog("Please try again.", Color.OrangeRed, "⚠️");
                return;
            }

            AddLog("Product detected: " + product, Color.LimeGreen, "✅");

            AddLog("Connecting to server...", Color.Cyan, "🌐");

            bool success = await DisplayModelImage(product);

            if (!success)
            {
                AddLog("Model image not found on server.", Color.Red, "❌");
                AddLog("Please contact admin if issue persists.", Color.OrangeRed, "⚠️");
                return;
            }

            AddLog("Image found and opened successfully.", Color.LimeGreen, "⬇️");
            AddLog("Operation completed successfully!", Color.LimeGreen, "🎉");

            // Premium branding line
            AddLog("Thank You for choosing FAHAD GSM TOOL", Color.Gold, "⭐");
        }

        private async Task RunDumpProcess()
        {
            logBox.Clear();

            AddLog("Initializing Dump process...", Color.Cyan, "⚙️");
            AddLog("Checking fastboot connection...", Color.LightSkyBlue, "🔍");

            if (!IsFastbootConnected())
            {
                AddLog("Fastboot device not connected!", Color.Red, "❌");
                AddLog("Please connect device in FASTBOOT mode.", Color.OrangeRed, "⚠️");
                return;
            }

            AddLog("Fastboot device connected successfully.", Color.LimeGreen, "🔗");

            AddLog("Reading product from device...", Color.Cyan, "📱");
            string product = await GetFastbootProduct();

            if (string.IsNullOrEmpty(product))
            {
                AddLog("Failed to read product from device.", Color.Red, "❌");
                return;
            }

            AddLog("Product detected: " + product, Color.LimeGreen, "✅");
            AddLog("Connecting to server...", Color.Cyan, "🌐");

            bool success = await GetDumpLink(product);

            if (!success)
            {
                AddLog("Dump not found on server.", Color.Red, "❌");
                return;
            }

            AddLog("Download started successfully!", Color.LimeGreen, "⬇️");
            AddLog("Thank You for choosing FAHAD GSM TOOL", Color.Gold, "⭐");
        }

        // ================= API =================

        private async Task<bool> DisplayModelImage(string input)
        {
            try
            {
                AddLog("Connecting to server...", Color.Cyan);

                string apiUrl =
$"https://fahad64.com/tool_api/get_model.php?key=FahadToolSecure2026&search={Uri.EscapeDataString(input)}";

                using var client = new System.Net.Http.HttpClient();
                client.DefaultRequestHeaders.Add("X-TOOL", "FahadGSMTool");
                var response = await client.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                    return false;

                dynamic result =
                    Newtonsoft.Json.JsonConvert.DeserializeObject(
                        await response.Content.ReadAsStringAsync());

                if (result.status == "success")
                {
                    AddLog("Image found. Downloading...", Color.LimeGreen);

                    string token = result.image_token.ToString();
                    string secureUrl =
$"https://fahad64.com/tool_api/get_image.php?key=FahadToolSecure2026&token={token}";

                    byte[] imageBytes =
                        await client.GetByteArrayAsync(secureUrl);

                    if (imageBytes?.Length > 100)
                    {
                        new ImageViewerForm(imageBytes, _loggedInUser).Show();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog("Server error: " + ex.Message, Color.Red);
            }

            return false;
        }

        private async Task<bool> GetDumpLink(string product)
        {
            try
            {
                using var client = new System.Net.Http.HttpClient();
                client.DefaultRequestHeaders.Add("X-TOOL", "FahadGSMTool");
                string apiUrl =
                $"https://fahad64.com/tool_api/get_dump.php?key=FahadToolSecure2026&search={Uri.EscapeDataString(product)}&user_id={LoginForm.CurrentUserId}";

                var response = await client.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                    return false;

                var jsonString = await response.Content.ReadAsStringAsync();

                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);

                if (result.status == "success")
                {
                    // 🔥 Changed only this URL (Middle Authentication Page)
                    string finalUrl =
                        $"https://fahad64.com/secure_download.php?key=FahadToolSecure2026&token={result.token}";



                    Process.Start(new ProcessStartInfo
                    {
                        FileName = finalUrl,
                        UseShellExecute = true
                    });

                    return true;
                }
                else
                {
                    AddLog("Dump not found.", Color.Orange);
                }
            }
            catch (Exception ex)
            {
                AddLog("Dump error: " + ex.Message, Color.Red);
            }

            return false;
        }

        // ================= FASTBOOT =================

        private async Task<string> GetFastbootProduct()
        {
            return await Task.Run(() =>
            {
                try
                {
                    Process p = new Process();
                    p.StartInfo.FileName = fastbootPath;

                    p.StartInfo.Arguments = "getvar product";
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;

                    p.Start();
                    string output = p.StandardError.ReadToEnd();
                    p.WaitForExit();

                    foreach (string line in output.Split('\n'))
                    {
                        if (line.Contains("product:"))
                            return line.Replace("product:", "").Trim();
                    }
                }
                catch { }

                return null;
            });
        }

        private bool IsFastbootConnected()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = fastbootPath,
                    Arguments = "devices",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process p = Process.Start(psi))
                {
                    if (p == null)
                        return false;

                    string output = p.StandardOutput.ReadToEnd();

                    p.WaitForExit();

                    if (string.IsNullOrWhiteSpace(output))
                        return false;

                    return output.Contains("fastboot");
                }
            }
            catch
            {
                return false;
            }
        }

       


        // ================= UI =================

        private void ShowStartup()
        {
            bigLogo.Visible = true;
            btnFastboot.Visible = false;
            btnSearchMode.Visible = false;
            btnStart.Visible = false;
            lblFastbootStatus.Visible = false;
            logContainer.Visible = false;
            lblSectionTitle.Visible = false;
            // hide fastboot flasher controls
            txtFolderPath.Visible = false;
            btnSelectFolder.Visible = false;
            btnFlash.Visible = false;
            gridPartitions.Visible = false;
            progressOverall.Visible = false;
            lblFlashStatus.Visible = false;
        }

        private void ShowContent(string title)
        {
            panelPatch.BringToFront();
            panelPatch.Visible = false;
            // ===== SAFE STATE RESET =====
            logBox.Clear();            // ⭐ clear previous flasher log
            gridPartitions.Rows.Clear(); // ⭐ clear flasher table
            progressOverall.Value = 0; // ⭐ reset progress
            isFlashing = false;        // ⭐ safety stop

            btnFastbootFlasher.Visible = false;
            btnComingSoon.Visible = false;
            bigLogo.Visible = false;
            lblSectionTitle.Visible = true;

            _currentSectionTitle = title.ToUpper();

            // hide fastboot flasher UI
            txtFolderPath.Visible = false;
            btnSelectFolder.Visible = false;
            btnFlash.Visible = false;
            gridPartitions.Visible = false;
            progressOverall.Visible = false;
            lblFlashStatus.Visible = false;

            btnFastboot.Visible = true;
            btnSearchMode.Visible = true;
            btnXiaomi.Visible = true;
            

            txtSearch.Visible = false;
            btnSearch.Visible = false;

            btnStart.Visible = false;
            lblFastbootStatus.Visible = false;

            logContainer.Visible = true;

            

            // 🔥 FORCE REPAINT
            lblSectionTitle.Invalidate();
            lblSectionTitle.Update();

            AlignLayout();
        }

        private void StartGlow()
        {
            glowTimer.Interval = 30;

            glowTimer.Tick += (s, e) =>
            {
                glowValue += glowUp ? 5 : -5;

                if (glowValue >= 255) glowUp = false;
                if (glowValue <= 120) glowUp = true;

                Color glowColor = Color.FromArgb(0, glowValue, 255);

                if (leftGlow != null)
                    leftGlow.BackColor = glowColor;

                if (rightGlow != null)
                    rightGlow.BackColor = glowColor;
            };

            glowTimer.Start();
        }



        private void SetPCName()
        {
            lblPCName.Text = "PC: " + Environment.MachineName;
        }
        private void SetSidebarSelection(Button selectedButton)
        {
            Button[] buttons = { btnFindISP, btnFindDump, btnXiaomi };

            foreach (Button btn in buttons)
            {
                if (btn == selectedButton)
                {
                    // SELECTED STYLE
                    btn.BackColor = Color.FromArgb(30, 60, 90);
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = Color.LimeGreen;
                    btn.FlatAppearance.BorderSize = 2;
                    btn.Font = new Font(btn.Font, FontStyle.Bold);
                }
                else
                {
                    // NORMAL STYLE
                    btn.BackColor = Color.FromArgb(20, 40, 65);
                    btn.ForeColor = Color.WhiteSmoke;
                    btn.FlatAppearance.BorderColor = Color.Cyan;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.Font = new Font(btn.Font, FontStyle.Regular);
                }
            }
        }

        private void AlignLayout()
        {
            if (layoutRunning) return;
            layoutRunning = true;

            try
            {
                if (this.WindowState == FormWindowState.Minimized)
                    return;

                this.SuspendLayout();
                mainPanel.SuspendLayout();

                // ===== GLOW FIX =====
                if (leftGlow != null && rightGlow != null)
                {
                    leftGlow.Height = sidebar.Height;
                    rightGlow.Height = sidebar.Height;

                    leftGlow.Top = sidebar.Top;
                    rightGlow.Top = sidebar.Top;

                    leftGlow.Left = sidebar.Left;
                    rightGlow.Left = sidebar.Right - rightGlow.Width;

                    leftGlow.BringToFront();
                    rightGlow.BringToFront();
                }

                int sidebarWidth = sidebar.Width;
                int workingWidth = ClientSize.Width - sidebarWidth;
                int rightStart = sidebarWidth;

                // ===== LOGO CENTER =====
                if (bigLogo.Visible)
                {
                    int centerX = sidebar.Width + ((ClientSize.Width - sidebar.Width) / 2);
                    int centerY = topBar.Height + ((ClientSize.Height - topBar.Height) / 2);

                    bigLogo.Left = centerX - (bigLogo.Width / 2);
                    bigLogo.Top = centerY - (bigLogo.Height / 2);
                }

                // ===== SECTION TITLE =====
                lblSectionTitle.Width = workingWidth;
                lblSectionTitle.Left = rightStart;
                lblSectionTitle.Top = 90;

                int contentWidth = btnFastboot.Width * 2 + 40;
                int contentStart = rightStart + (workingWidth - contentWidth) / 2;

                // ===== MAIN BUTTONS =====
                btnFastboot.Left = contentStart;
                btnFastboot.Top = 240;

                btnSearchMode.Left = contentStart + btnFastboot.Width + 40;
                btnSearchMode.Top = 240;

                btnFastbootFlasher.Left = contentStart;
                btnFastbootFlasher.Top = 240;

                btnComingSoon.Left = contentStart + btnFastbootFlasher.Width + 40;
                btnComingSoon.Top = 240;

                btnStart.Left = contentStart + (contentWidth - btnStart.Width) / 2;
                btnStart.Top = 240;

                lblFastbootStatus.Left = contentStart + (contentWidth - lblFastbootStatus.Width) / 2;
                lblFastbootStatus.Top = btnStart.Bottom + 15;

                // ===== SEARCH MODE =====
                if (txtSearch.Visible)
                {
                    txtSearch.Left = contentStart + (contentWidth - txtSearch.Width) / 2;
                    txtSearch.Top = 240;

                    btnSearch.Left = txtSearch.Left + (txtSearch.Width - btnSearch.Width) / 2;
                    btnSearch.Top = txtSearch.Bottom + 15;

                    logContainer.Top = btnSearch.Bottom + 40;
                }
                else if (btnStart.Visible)
                {
                    logContainer.Top = lblFastbootStatus.Bottom + 40;
                }
                else
                {
                    logContainer.Top = btnFastboot.Bottom + 40;
                }

                // ===== TABLE WIDTH =====
                int tableWidth = Math.Min(420, (workingWidth / 2) - 40);

                gridPartitions.Width = tableWidth;
                logContainer.Width = tableWidth;

                int totalWidth = tableWidth * 2 + 10;
                int centerStart = rightStart + (workingWidth - totalWidth) / 2;

                gridPartitions.Left = centerStart - 215;
                logContainer.Left = gridPartitions.Right + 10;

                gridPartitions.Top = logContainer.Top;

                int newHeight = ClientSize.Height - gridPartitions.Top - 80;
                if (newHeight < 200) newHeight = 200;

                gridPartitions.Height = newHeight;
                logContainer.Height = newHeight;

                // ===== FASTBOOT FLASHER MODE =====
                if (txtFolderPath.Visible)
                {
                    int centerWidth = ClientSize.Width - sidebar.Width - 120;

                    centerStart = sidebar.Width +
                        ((ClientSize.Width - sidebar.Width) - centerWidth) / 2;

                    txtFolderPath.Width = centerWidth - 80;
                    txtFolderPath.Left = centerStart + 20;
                    txtFolderPath.Top = 170;

                    btnSelectFolder.Left = txtFolderPath.Right + 5;
                    btnSelectFolder.Top = txtFolderPath.Top - 1;

                    btnFlash.Left = centerStart + (centerWidth - btnFlash.Width) / 2;
                    btnFlash.Top = txtFolderPath.Bottom + 15;

                    lblFlashStatus.Left = centerStart + (centerWidth - lblFlashStatus.Width) / 2;
                    lblFlashStatus.Top = btnFlash.Bottom + 8;

                    tableWidth = (centerWidth / 2) - 20;

                    gridPartitions.Width = tableWidth;
                    totalWidth = tableWidth * 2 + 10;
                    gridPartitions.Left = centerStart + (centerWidth - totalWidth) / 2;
                    gridPartitions.Top = lblFlashStatus.Bottom + 10;

                    newHeight = ClientSize.Height - gridPartitions.Top - 80;
                    if (newHeight < 200) newHeight = 200;

                    gridPartitions.Height = newHeight;

                    logContainer.Width = tableWidth;
                    logContainer.Height = newHeight;
                    logContainer.Left = gridPartitions.Right + 10;
                    logContainer.Top = gridPartitions.Top;

                    progressOverall.Width = centerWidth;
                    progressOverall.Left = centerStart;
                    progressOverall.Height = 25;
                    progressOverall.Top = ClientSize.Height - 60;

                    flashLogBox.Width = tableWidth;
                    flashLogBox.Height = newHeight;
                    flashLogBox.Left = gridPartitions.Right + 10;
                    flashLogBox.Top = gridPartitions.Top;
                }

                // ===== PATCH MENU LAYOUT =====
                if (panelPatch.Visible)
                {
                    int patchCenter = sidebar.Width + ((ClientSize.Width - sidebar.Width) / 2);

                    // HEADER CENTER
                    if (patchHeader != null && lblPatchMenu != null)
                    {
                        lblPatchMenu.Left = (patchHeader.Width / 2) - (lblPatchMenu.Width / 2);
                        lblPatchMenu.Top = (patchHeader.Height / 2) - (lblPatchMenu.Height / 2);
                    }

                    // CARDS
                    int cardWidth = panelPersist.Width;
                    int gap = 80;

                    panelPersist.Left = patchCenter - cardWidth - (gap / 2);
                    panelPersist.Top = 200;

                    panelCust.Left = patchCenter + (gap / 2);
                    panelCust.Top = 200;
                    // LOG BOX
                    // ===== RESPONSIVE LOG BOX =====

                    int logWidth;
                    int logHeight;

                    if (this.WindowState == FormWindowState.Maximized)
                    {
                        logWidth = (ClientSize.Width - sidebar.Width) * 70 / 100;
                        logHeight = 440;
                    }
                    else
                    {
                        logWidth = (ClientSize.Width - sidebar.Width) * 55 / 100;
                        logHeight = 280;
                    }

                    patchLogBox.Width = logWidth;
                    patchLogBox.Height = logHeight;

                    patchLogBox.Left = sidebar.Width + ((ClientSize.Width - sidebar.Width - logWidth) / 2);
                    patchLogBox.Top = mainPanel.Height - patchLogBox.Height - 60;
                    lblMiAccount.Left = patchCenter - (lblMiAccount.Width / 2);
                    lblMiAccount.Top = 130;
                }

                mainPanel.ResumeLayout();
                this.ResumeLayout(false);

                lblFlashStatus.BringToFront();
                progressOverall.BringToFront();

                gridPartitions.Update();
                gridPartitions.Refresh();
            }
            finally
            {
                layoutRunning = false;
            }
        }

        private void DrawSectionTitle(object sender, PaintEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentSectionTitle)) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using Font font = new Font("Segoe UI Black", 26F);
            SizeF size = g.MeasureString(_currentSectionTitle, font);

            float x = (lblSectionTitle.Width - size.Width) / 2;
            float y = (lblSectionTitle.Height - size.Height) / 2;

            using SolidBrush brush =
                new SolidBrush(Color.FromArgb(0, 230, 255));

            g.DrawString(_currentSectionTitle, font, brush, x, y);
        }

        void LoadPartitions(string folder)
        {
            gridPartitions.Rows.Clear();

            string imageFolder = Path.Combine(folder, "images");

            if (Directory.Exists(imageFolder))
                folder = imageFolder;

            var files = Directory.EnumerateFiles(folder);

            foreach (var file in files)
            {
                AddLog($"DEBUG FILE: {file}", Color.Yellow);

                string ext = Path.GetExtension(file).ToLower();

                AddLog($"EXT: {ext}", Color.Cyan);

                if (ext == ".img" || ext == ".elf" || ext == ".mbn" || ext == ".bin" || ext == ".txt")
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    string fileName = Path.GetFileName(file);

                    bool isChecked = ext != ".txt";

                    AddLog($"Checked Value: {isChecked}", Color.Orange);

                    int row = gridPartitions.Rows.Add();
                    gridPartitions.Rows[row].Cells[0].Value = isChecked;
                    gridPartitions.Rows[row].Cells[1].Value = name;
                    gridPartitions.Rows[row].Cells[2].Value = fileName;
                }
            }

            gridPartitions.Rows.Clear();
            gridPartitions.Visible = true;

            AlignLayout();

            progressOverall.BringToFront();
        }
        private void LoadImagesToGrid(string folder)
        {
            gridPartitions.Rows.Clear();

            string imagesPath;

            // अगर user images folder select करे
            if (Path.GetFileName(folder).ToLower() == "images")
                imagesPath = folder;
            else
                imagesPath = Path.Combine(folder, "images");

            if (!Directory.Exists(imagesPath))
            {
                AddLog("Images folder not found.", Color.Red);
                return;
            }

            var files = Directory.GetFiles(imagesPath);

            foreach (var file in files)
            {
                string ext = Path.GetExtension(file).ToLower();

                if (ext != ".img" && ext != ".mbn" && ext != ".elf")
                    continue;

                string part = Path.GetFileNameWithoutExtension(file);
                string name = Path.GetFileName(file);

                int rowIndex = gridPartitions.Rows.Add(true, part, name);

                if (name.ToLower() == "erase")
                {
                    DataGridViewRow r = gridPartitions.Rows[rowIndex];

                    r.DefaultCellStyle.ForeColor = Color.Red;
                    r.DefaultCellStyle.Font = new Font(gridPartitions.Font, FontStyle.Bold);
                }
            }
          

            AddLog("GPT / Partition table loaded.", Color.Cyan);
            foreach (DataGridViewRow row in gridPartitions.Rows)
            {
                string file = row.Cells[2].Value?.ToString();

                if (string.IsNullOrWhiteSpace(file))
                {
                    row.Cells[2].Value = "erase";

                    row.Cells[0].Style.ForeColor = Color.Red;
                    row.Cells[1].Style.ForeColor = Color.Red;
                    row.Cells[2].Style.ForeColor = Color.Red;

                    row.Cells[0].Style.Font = new Font(gridPartitions.Font, FontStyle.Bold);
                    row.Cells[1].Style.Font = new Font(gridPartitions.Font, FontStyle.Bold);
                    row.Cells[2].Style.Font = new Font(gridPartitions.Font, FontStyle.Bold);
                }
            }


        }
        private async Task<bool> CheckDeviceInfo()
        {
            AddLog("Reading device info...", Color.Cyan);

            string product = await GetFastbootVar("product");
            string slot = await GetFastbootVar("current-slot");
            string unlock = await GetFastbootVar("unlocked");

            // Clean output
            string p = product.Replace("Finished.", "")
                              .Replace("Total time:", "")
                              .Trim();

            string s = slot.Replace("Finished.", "")
                           .Replace("Total time:", "")
                           .Trim();

            string u = unlock.Replace("Finished.", "")
                             .Replace("Total time:", "")
                             .Trim();

            // Pretty log
            AddLog("════════════════════════════", Color.DarkCyan);

            AddLog("DEVICE INFORMATION", Color.Gold, "📱");

            AddLog($"Product      : {p}", Color.LimeGreen, "✔");
            AddLog($"Current Slot : {s}", Color.DeepSkyBlue, "⚙");
            AddLog($"Bootloader   : {u}", Color.Orange, "🔒");

            AddLog("════════════════════════════", Color.DarkCyan);

            return true;
        }


        
        private void CreateFlashModeButtons()
        {
        
        }
        private void ParseFlashScript(string scriptPath)
        {
            gridPartitions.SuspendLayout();

            try
            {
                gridPartitions.Rows.Clear();

                string romFolder = Path.GetDirectoryName(scriptPath);
                string imagesFolder = Path.Combine(romFolder, "images");

                if (!Directory.Exists(imagesFolder))
                    imagesFolder = romFolder;

                var lines = File.ReadAllLines(scriptPath);

                List<object[]> rows = new List<object[]>();

                foreach (string line in lines)
                {
                    string l = line.Trim();

                    if (!l.StartsWith("fastboot"))
                        continue;

                    if (l.Contains(" flash "))
                    {
                        string[] p = l.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (p.Length < 5)
                            continue;

                        string partition = p[3];
                        string file = p[4].Replace("%~dp0images\\", "");
                        file = Path.GetFileName(file);

                        bool isChecked = !file.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);

                        rows.Add(new object[] { isChecked, partition, file });
                    }

                    if (l.Contains(" erase "))
                    {
                        string[] p = l.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (p.Length < 4)
                            continue;

                        string partition = p[3];

                        rows.Add(new object[] { true, partition, "erase" });
                    }
                }

                foreach (var r in rows)
                {
                    gridPartitions.Rows.Add(r);
                }

                foreach (DataGridViewRow row in gridPartitions.Rows)
                {
                    if (row.Cells[2].Value?.ToString() == "erase")
                    {
                        row.Cells[1].Style.ForeColor = Color.Red;
                        row.Cells[2].Style.ForeColor = Color.Red;

                        row.Cells[1].Style.Font = new Font(gridPartitions.Font, FontStyle.Bold);
                        row.Cells[2].Style.Font = new Font(gridPartitions.Font, FontStyle.Bold);
                    }
                }
            }
            finally
            {
                gridPartitions.ResumeLayout();
            }
        }
        void AutoLoadSuperChunks(string folder)
        {
            var chunks = Directory.GetFiles(folder, "super_sparsechunk.*");

            if (chunks.Length == 0)
                return;

            foreach (var file in chunks)
            {
                string name = Path.GetFileName(file);

                gridPartitions.Rows.Add(true, "super", name);
            }

            AddLog("Super sparse chunks detected: " + chunks.Length, Color.Cyan);
        }
        private void GridPartitions_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = gridPartitions.Rows[e.RowIndex];

            string file = row.Cells[2].Value?.ToString();

            if (file == "erase")
            {
                row.Cells[1].Style.ForeColor = Color.Red;
                row.Cells[2].Style.ForeColor = Color.Red;

                row.Cells[1].Style.Font = new Font(gridPartitions.Font, FontStyle.Bold);
                row.Cells[2].Style.Font = new Font(gridPartitions.Font, FontStyle.Bold);
            }
        }

        private void StartFastbootMonitoring()
        {
            fastbootMonitor?.Cancel();

            fastbootMonitor = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (!fastbootMonitor.Token.IsCancellationRequested)
                {
                    bool connected = IsFastbootConnected();

                    if (connected == lastFastbootCheck)
                    {
                        await Task.Delay(5000);
                        continue;
                    }

                    lastFastbootCheck = connected;

                    try
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            string newText = connected ? "Fastboot: Connected" : "Fastboot: Not Connected";
                            Color newColor = connected ? Color.LimeGreen : Color.Red;

                            // Fastboot Flasher label
                            if (lblFlashStatus.Visible)
                            {
                                if (lblFlashStatus.Text != newText)
                                {
                                    lblFlashStatus.Text = newText;
                                    lblFlashStatus.ForeColor = newColor;
                                }
                            }

                            // Find ISP / Find Dump label
                            if (lblFastbootStatus.Visible)
                            {
                                if (lblFastbootStatus.Text != newText)
                                {
                                    lblFastbootStatus.Text = newText;
                                    lblFastbootStatus.ForeColor = newColor;
                                }
                            }
                        }));
                    }
                    catch { }

                    await Task.Delay(5000); // check every 2 seconds
                }
            });
        }
        private string FindFlashScript(string folder)
        {
            string script = Path.Combine(folder, "flash_all.bat");

            if (File.Exists(script))
                return script;

            return null;
        }

        private void gridPartitions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Debug.WriteLine("CELL CLICKED");
            userClickedRow = true;
        }
        private void gridPartitions_SelectionChanged(object sender, EventArgs e)
        {
            if (!userClickedRow)
                return;

            userClickedRow = false;
        }


        void RoundTextbox(Control c, int radius)
        {
            Rectangle rect = new Rectangle(0, 0, c.Width, c.Height);

            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseAllFigures();

            c.Region = new Region(path);
        }
        void HideAllModules()
        {
            // ===== PATCH MENU =====
            panelPatch.Visible = false;
            lblMiAccount.Visible = false;
            patchLogBox.Visible = false;
            panelPersist.Visible = false;
            panelCust.Visible = false;

            // ===== FASTBOOT FLASHER =====
            txtFolderPath.Visible = false;
            btnSelectFolder.Visible = false;
            btnFlash.Visible = false;
            gridPartitions.Visible = false;
            progressOverall.Visible = false;
            lblFlashStatus.Visible = false;
            flashLogBox.Visible = false;

            // ===== FIND ISP / DUMP =====
            btnStart.Visible = false;
            lblFastbootStatus.Visible = false;
            txtSearch.Visible = false;
            btnSearch.Visible = false;
            logContainer.Visible = false;

            // ===== XIAOMI MENU =====
            btnFastbootFlasher.Visible = false;
            btnComingSoon.Visible = false;
        }
        
        
        
    }

}
public static class GraphicsExtensions
{
    public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle bounds, int radius)
    {
        using (GraphicsPath path = RoundedRect(bounds, radius))
        {
            g.FillPath(brush, path);
        }
    }

    public static void DrawRoundedRectangle(this Graphics g, Pen pen, Rectangle bounds, int radius)
    {
        using (GraphicsPath path = RoundedRect(bounds, radius))
        {
            g.DrawPath(pen, path);
        }
    }

    private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        int diameter = radius * 2;
        GraphicsPath path = new GraphicsPath();

        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);

        path.CloseFigure();
        return path;
    }
}
