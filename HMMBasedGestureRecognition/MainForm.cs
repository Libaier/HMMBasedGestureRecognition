using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;



namespace Recognizer.HMM
{
    public class MainForm : System.Windows.Forms.Form
    {
        #region Fields

        private RecognizerUtils _rec;
        private bool _recording;
        private bool _isDown;
        private ArrayList _points;
        private List<ArrayList> _pointsList;
        private ViewForm _viewFrm;

        private List<String> _directionalCodewordsList;
        
        private String _directionalCodewords;

        private HiddenMarkovClassifier hmmc;


        #endregion

        #region Form Elements

        private System.Windows.Forms.Label lblRecord;
        private System.Windows.Forms.MainMenu MainMenu;
        private System.Windows.Forms.MenuItem Exit;
        private System.Windows.Forms.MenuItem LoadGesture;
        private System.Windows.Forms.MenuItem ViewGesture;
        private System.Windows.Forms.MenuItem RecordGesture;
        private System.Windows.Forms.MenuItem GestureMenu;
        private System.Windows.Forms.MenuItem ClearGestures;
        private System.Windows.Forms.MenuItem HelpMenu;
        private System.Windows.Forms.MenuItem About;
        private MenuItem FileMenu;
        private MenuItem HMMMenu;
        private MenuItem LoadAndTrain;
        private MenuItem LoadFromHMMFile;
        private Label lblResult;
        private IContainer components;

        #endregion

        #region Start & Stop

        [STAThread]
        static void Main(string[] args)
        {
            Application.Run(new MainForm());
        }

        public MainForm()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            InitializeComponent();
            _rec = new RecognizerUtils();
            _points = new ArrayList();
            _directionalCodewordsList = new List<string>();
            _pointsList = new List<ArrayList>();
            _viewFrm = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblRecord = new System.Windows.Forms.Label();
            this.MainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.FileMenu = new System.Windows.Forms.MenuItem();
            this.Exit = new System.Windows.Forms.MenuItem();
            this.GestureMenu = new System.Windows.Forms.MenuItem();
            this.RecordGesture = new System.Windows.Forms.MenuItem();
            this.LoadGesture = new System.Windows.Forms.MenuItem();
            this.ViewGesture = new System.Windows.Forms.MenuItem();
            this.ClearGestures = new System.Windows.Forms.MenuItem();
            this.HMMMenu = new System.Windows.Forms.MenuItem();
            this.LoadAndTrain = new System.Windows.Forms.MenuItem();
            this.LoadFromHMMFile = new System.Windows.Forms.MenuItem();
            this.HelpMenu = new System.Windows.Forms.MenuItem();
            this.About = new System.Windows.Forms.MenuItem();
            this.lblResult = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblRecord
            // 
            this.lblRecord.BackColor = System.Drawing.Color.Transparent;
            this.lblRecord.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRecord.Font = new System.Drawing.Font("Courier New", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecord.ForeColor = System.Drawing.Color.Firebrick;
            this.lblRecord.Location = new System.Drawing.Point(0, 0);
            this.lblRecord.Name = "lblRecord";
            this.lblRecord.Size = new System.Drawing.Size(1184, 26);
            this.lblRecord.TabIndex = 1;
            this.lblRecord.Text = "[Recording]";
            this.lblRecord.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblRecord.Visible = false;
            // 
            // MainMenu
            // 
            this.MainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.FileMenu,
            this.GestureMenu,
            this.HMMMenu,
            this.HelpMenu});
            // 
            // FileMenu
            // 
            this.FileMenu.Index = 0;
            this.FileMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.Exit});
            this.FileMenu.Text = "&File";
            // 
            // Exit
            // 
            this.Exit.Index = 0;
            this.Exit.Text = "E&xit";
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // GestureMenu
            // 
            this.GestureMenu.Index = 1;
            this.GestureMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.RecordGesture,
            this.LoadGesture,
            this.ViewGesture,
            this.ClearGestures});
            this.GestureMenu.Text = "&Gestures";
            this.GestureMenu.Popup += new System.EventHandler(this.GestureMenu_Popup);
            // 
            // RecordGesture
            // 
            this.RecordGesture.Index = 0;
            this.RecordGesture.Shortcut = System.Windows.Forms.Shortcut.F1;
            this.RecordGesture.Text = "&Record";
            this.RecordGesture.Click += new System.EventHandler(this.RecordGesture_Click);
            // 
            // LoadGesture
            // 
            this.LoadGesture.Index = 1;
            this.LoadGesture.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.LoadGesture.Text = "&Load...";
            this.LoadGesture.Click += new System.EventHandler(this.LoadGesture_Click);
            // 
            // ViewGesture
            // 
            this.ViewGesture.Index = 2;
            this.ViewGesture.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
            this.ViewGesture.Text = "&View";
            this.ViewGesture.Click += new System.EventHandler(this.ViewGesture_Click);
            // 
            // ClearGestures
            // 
            this.ClearGestures.Index = 3;
            this.ClearGestures.Text = "&Clear";
            this.ClearGestures.Click += new System.EventHandler(this.ClearGestures_Click);
            // 
            // HMMMenu
            // 
            this.HMMMenu.Index = 2;
            this.HMMMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.LoadAndTrain,
            this.LoadFromHMMFile});
            this.HMMMenu.Text = "&HMM";
            // 
            // LoadAndTrain
            // 
            this.LoadAndTrain.Index = 0;
            this.LoadAndTrain.Text = "&Load And Train...";
            this.LoadAndTrain.Click += new System.EventHandler(this.LoadAndTrain_Click);
            // 
            // LoadFromHMMFile
            // 
            this.LoadFromHMMFile.Index = 1;
            this.LoadFromHMMFile.Text = "Load From &HMM File...";
            // 
            // HelpMenu
            // 
            this.HelpMenu.Index = 3;
            this.HelpMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.About});
            this.HelpMenu.Text = "&Help";
            // 
            // About
            // 
            this.About.Index = 0;
            this.About.Text = "&About...";
            this.About.Click += new System.EventHandler(this.About_Click);
            // 
            // lblResult
            // 
            this.lblResult.BackColor = System.Drawing.Color.Transparent;
            this.lblResult.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblResult.Font = new System.Drawing.Font("Courier New", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult.ForeColor = System.Drawing.Color.Firebrick;
            this.lblResult.Location = new System.Drawing.Point(0, 26);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(1184, 26);
            this.lblResult.TabIndex = 2;
            this.lblResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1184, 640);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.lblRecord);
            this.Menu = this.MainMenu;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Recognizer.HMM";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
            this.ResumeLayout(false);

        }
        #endregion

        #region File Menu

        private void Exit_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        #endregion

        #region Gestures Menu

        private void GestureMenu_Popup(object sender, System.EventArgs e)
        {

            ViewGesture.Checked = (_viewFrm != null && !_viewFrm.IsDisposed);
            ClearGestures.Enabled = (_rec.NumGestures > 0);
        }

        private void LoadGesture_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Gestures (*.xml)|*.xml";
            dlg.Title = "Load Gestures";
            dlg.RestoreDirectory = false;
            dlg.Multiselect = true;

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                for (int i = 0; i < dlg.FileNames.Length; i++)
                {
                    string name = dlg.FileNames[i];
                    _rec.LoadGesture(name);
                }
                ReloadViewForm();
            }
        }

        private void ViewGesture_Click(object sender, System.EventArgs e)
        {
            if (_viewFrm != null && !_viewFrm.IsDisposed)
            {
                _viewFrm.Close();
                _viewFrm = null;
            }
            else
            {
                _viewFrm = new ViewForm(_rec.Gestures);
                _viewFrm.Owner = this;
                _viewFrm.Show();
            }
        }

        // helper fn
        private void ReloadViewForm()
        {
            if (_viewFrm != null && !_viewFrm.IsDisposed)
            {
                _viewFrm.Close();
                _viewFrm = new ViewForm(_rec.Gestures);
                _viewFrm.Owner = this;
                _viewFrm.Show();
            }
        }



        private void RecordGesture_Click(object sender, System.EventArgs e)
        {
            _points.Clear();
            Invalidate();
            _recording = !_recording; // recording will happen on mouse-up
            if (_recording)
            {
                RecordGesture.Text = "Stop Recording";

            }
            else
            {
                if (_directionalCodewordsList.Count > 0)
                {

                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.Filter = "Gestures (*.xml)|*.xml";
                    dlg.Title = "Save Gesture As";
                    dlg.AddExtension = true;
                    dlg.RestoreDirectory = false;

                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        _rec.SaveDirectionalCodewords(dlg.FileName, _directionalCodewordsList);
                        String filename = Gesture.ParseName(dlg.FileName);
                        for (int i = 0; i < _pointsList.Count; i++)
                        {
                            _rec.SaveGesture(filename + i + ".xml", _pointsList[i]);
                        }
                        ReloadViewForm();
                    }

                    dlg.Dispose();


                    _directionalCodewordsList.Clear();
                    _pointsList.Clear();
                    _points.Clear();
                    lblResult.Text = "0";
                    Invalidate();

                    RecordGesture.Text = "Record";
                }

            }
            lblRecord.Visible = _recording;
        }

        private void ClearGestures_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(this, "This will clear all loaded gestures. (It will not delete any XML files.)", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                _rec.ClearGestures();
                ReloadViewForm();
            }
        }


        #endregion

        #region About Menu

        private void About_Click(object sender, System.EventArgs e)
        {
            AboutForm frm = new AboutForm(_points);
            frm.ShowDialog(this);
        }

        #endregion

        #region Window Form Events

        private void MainForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (_points.Count > 0)
            {
                PointF p0 = (PointF)(PointR)_points[0]; // draw the first point bigger
                e.Graphics.FillEllipse(_recording ? Brushes.Firebrick : Brushes.DarkBlue, p0.X - 5f, p0.Y - 5f, 10f, 10f);
            }
            foreach (PointR r in _points)
            {
                PointF p = (PointF)r; // cast
                e.Graphics.FillEllipse(_recording ? Brushes.Firebrick : Brushes.DarkBlue, p.X - 2f, p.Y - 2f, 4f, 4f);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblResult.Text = String.Empty;
        }

        #endregion

        #region Mouse Events

        private void MainForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (_directionalCodewordsList.Count >= 1)
                {
                    _directionalCodewordsList.RemoveAt(_directionalCodewordsList.Count - 1);
                    _pointsList.RemoveAt(_pointsList.Count-1);
                    if (_pointsList.Count - 1 >= 0)
                    {
                        _points = _pointsList[_pointsList.Count - 1];
                    }
                    else
                    {
                        _points.Clear();
                    }
                    
                    lblResult.Text = _directionalCodewordsList.Count + "";
                    Invalidate();
                }
            }
            else
            {
                _isDown = true;
                _points.Clear();
                _directionalCodewords = null;
                _points.Add(new PointR(e.X, e.Y, Environment.TickCount));
                Invalidate();
            }
        }

        private void MainForm_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_isDown)
            {
                if (_points.Count >= 2)
                {
                    PointR p = (PointR)_points[_points.Count - 1];
                    _directionalCodewords = _directionalCodewords + "-" + getDirectionalCodewords(e.X, e.Y, p.X, p.Y);
                }
                _points.Add(new PointR(e.X, e.Y, Environment.TickCount));
                Invalidate(new Rectangle(e.X - 2, e.Y - 2, 4, 4));
            }
        }

        private void MainForm_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_isDown)
            {
                _isDown = false;

                if (_points.Count >= 5) // require 5 points for a valid gesture
                {
                    if (_recording)
                    {
                        _directionalCodewords = _directionalCodewords.Substring(1, _directionalCodewords.Length - 1);
                        _directionalCodewordsList.Add(_directionalCodewords);

                        _pointsList.Add(new ArrayList(_points));


                        lblResult.Text = _directionalCodewordsList.Count + "";
                       
                        Invalidate();
                    }
                    else if (hmmc != null) // not recording, so testing
                    {

                        //Application.DoEvents(); // forces label to display

                        _directionalCodewords = _directionalCodewords.Substring(1, _directionalCodewords.Length - 1);
                            int x = hmmc.Compute(_rec.decode(_directionalCodewords));
                            lblResult.Text = x+"";
                            Invalidate();
                    }
                }
            }
        }

        private int getDirectionalCodewords(double x, double y, double lastX, double lastY)
        {
            double angle = Math.Atan2(y - lastY, x - lastX); // keep it in radians
            if (angle < 0)
                angle += 2 * 3.1425926;
            return (int)(angle * (8 / 3.1425926)); // convert to <0, 16)
        }
        #endregion



        private void LoadAndTrain_Click(object sender, EventArgs e)
        {

            List<int> outputLabels = new List<int>(256);
            List<int[]> inputSequences = new List<int[]>(256);

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Gestures (*.xml)|*.xml";
            dlg.Title = "Load Gestures";
            dlg.RestoreDirectory = false;
            dlg.Multiselect = true;

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                for (int i = 0; i < dlg.FileNames.Length; i++)
                {
                    string name = dlg.FileNames[i];
                    List<int[]> inputSequencesTemp = _rec.LoadDirectionalCodewordsFile(name);

                    for (int j = 0; j < inputSequencesTemp.Count; j++)
                    {
                        inputSequences.Add(inputSequencesTemp[j]);
                        outputLabels.Add(i);
                    }
                }
                ReloadViewForm();
            }
            ITopology forward = new Forward(5, 3);
            hmmc = new HiddenMarkovClassifier(3, forward, 16);
            // And create a algorithms to teach each of the inner models
            var teacher = new HiddenMarkovClassifierLearning(hmmc,

                // We can specify individual training options for each inner model:
                modelIndex => new BaumWelchLearning(hmmc.Models[modelIndex])
                {
                    Tolerance = 0.001, // iterate until log-likelihood changes less than 0.001
                    Iterations = 0     // don't place an upper limit on the number of iterations
                });
            teacher.Run((int[][])inputSequences.ToArray(), (int[])outputLabels.ToArray());
        }
    }
}
