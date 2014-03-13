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
using System.Threading;

namespace Recognizer.HMM
{
    public class MainForm : System.Windows.Forms.Form
    {
        #region Fields

        private RecognizerUtils _rec;
        private bool _recording;
        private bool _isDown;
        private ArrayList _points;
        private Queue<int> _directionalCodewordsQueue;
        private List<ArrayList> _pointsList;
        private ViewForm _viewFrm;

        private List<String> _directionalCodewordsList;

        //private String _directionalCodewords;

        private String _info;

        private HiddenMarkovClassifier _hmmc;
        private HiddenMarkovModel[] _hmms;
        //Font 
        private Font _font;
        private int _maxCount = 200;

        private bool _motiveStart = false;

        //private Thread ComputingThread;

        // [NatNet] Our NatNet object
        private NatNetML.NatNetClientML _NatNet;
        // [NatNet] Our NatNet Frame of Data object
        private NatNetML.FrameOfMocapData _FrameOfData = new NatNetML.FrameOfMocapData();

        private float xOffSet = 0, yOffSet = 0;
        private int scale = 1;
        private int frameCnt = 0;
        private const int MinNoPoints = 5;

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
        private MenuItem SaveHMMFile;
        private MenuItem Optitrack;
        private MenuItem ConnectToMotive;
        private MenuItem DisconnectMotive;
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
            _directionalCodewordsQueue = new Queue<int>();
            _directionalCodewordsList = new List<string>();
            _pointsList = new List<ArrayList>();

            _font = new Font(FontFamily.GenericSansSerif, 8.25f);
            _viewFrm = null;
            lblResult.Text = String.Empty;
            this.KeyPreview = true;
            //ComputingThread = new Thread(new ThreadStart(this.HMMDecode));
            //ComputingThread.Start();
            // Spin for a while waiting for the started thread to become
            // alive:
            //while (!ComputingThread.IsAlive) ;

            //// Put the Main thread to sleep for 1 millisecond to allow oThread
            //// to do some work:
            //Thread.Sleep(1);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                Disconnect();
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
            this.SaveHMMFile = new System.Windows.Forms.MenuItem();
            this.Optitrack = new System.Windows.Forms.MenuItem();
            this.ConnectToMotive = new System.Windows.Forms.MenuItem();
            this.DisconnectMotive = new System.Windows.Forms.MenuItem();
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
            this.Optitrack,
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
            this.LoadFromHMMFile,
            this.SaveHMMFile});
            this.HMMMenu.Text = "&HMM";
            // 
            // LoadAndTrain
            // 
            this.LoadAndTrain.Index = 0;
            this.LoadAndTrain.Text = "&Load .xml And Train...";
            this.LoadAndTrain.Click += new System.EventHandler(this.LoadAndTrain_Click);
            // 
            // LoadFromHMMFile
            // 
            this.LoadFromHMMFile.Index = 1;
            this.LoadFromHMMFile.Text = "Load From .&hmm File...";
            this.LoadFromHMMFile.Click += new System.EventHandler(this.LoadFromHMMFile_Click);
            // 
            // SaveHMMFile
            // 
            this.SaveHMMFile.Index = 2;
            this.SaveHMMFile.Text = "&Save to .hmm File";
            this.SaveHMMFile.Click += new System.EventHandler(this.SaveHMMFile_Click);
            // 
            // Optitrack
            // 
            this.Optitrack.Index = 3;
            this.Optitrack.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ConnectToMotive,
            this.DisconnectMotive});
            this.Optitrack.Text = "&Optitrack";
            // 
            // ConnectToMotive
            // 
            this.ConnectToMotive.Index = 0;
            this.ConnectToMotive.Text = "&Connect to Motive";
            this.ConnectToMotive.Click += new System.EventHandler(this.ConnectToMotive_Click);
            // 
            // DisconnectMotive
            // 
            this.DisconnectMotive.Index = 1;
            this.DisconnectMotive.Text = "&Disconnect";
            this.DisconnectMotive.Click += new System.EventHandler(this.DisconnectMotive_Click);
            // 
            // HelpMenu
            // 
            this.HelpMenu.Index = 4;
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
            this.KeyPreview = true;
            this.Menu = this.MainMenu;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Recognizer.HMM";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
            this.ResumeLayout(false);

        }
        private void MainForm_Load(object sender, EventArgs e)
        {

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

        #region HMM Menu
        private void LoadAndTrain_Click(object sender, EventArgs e)
        {

            List<int> outputLabels = new List<int>();
            List<int[]> inputSequences = new List<int[]>();


            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Gestures (*.xml)|*.xml";
            dlg.Title = "Load Gestures";
            dlg.RestoreDirectory = false;
            dlg.Multiselect = true;

            ITopology[] forwards = null;
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                lblResult.Text = "Training...";
                forwards = new Forward[dlg.FileNames.Length];
                for (int i = 0; i < dlg.FileNames.Length; i++)
                {

                    string name = dlg.FileNames[i];
                    List<int[]> inputSequencesTemp = _rec.LoadDirectionalCodewordsFile(name);
                    int sum = 0;
                    int avg = 1;
                    for (int j = 0; j < inputSequencesTemp.Count; j++)
                    {
                        inputSequences.Add(inputSequencesTemp[j]);
                        sum = sum + inputSequencesTemp[j].Length;
                        outputLabels.Add(i);
                    }
                    avg = sum / inputSequencesTemp.Count/10;
                    //if (avg > 3)
                    //{
                    //    forwards[i] = new Forward(avg, 3);
                    //}
                    //else
                    //{
                        forwards[i] = new Forward(avg, 2);
                    //}
                }
                ReloadViewForm();
                _hmmc = new HiddenMarkovClassifier(dlg.FileNames.Length, forwards, 16);
                //_hmmc = new HiddenMarkovClassifier(4, customs, 16);
                //hmmc.Models[0] = new HiddenMarkovModel();
                //hmmc.Models[0].Transitions = null;kovModel();
                // And create a algorithms to teach each of the inner models
                var teacher = new HiddenMarkovClassifierLearning(_hmmc,

                    // We can specify individual training options for each inner model:
                    modelIndex => new BaumWelchLearning(_hmmc.Models[modelIndex])
                    {
                        Tolerance = 0.001, // iterate until log-likelihood changes less than 0.001
                        Iterations = 0     // don't place an upper limit on the number of iterations
                    });
                teacher.Run((int[][])inputSequences.ToArray(), (int[])outputLabels.ToArray());

                _hmmc.Threshold = teacher.Threshold();
                //_hmmc.Sensitivity = 1;
                _hmms = _hmmc.Models;
                for (int i = 0; i < dlg.FileNames.Length; i++)
                {
                    _hmms[i].Tag = Gesture.ParseName(dlg.FileNames[i]);
                }
                lblResult.Text = "Success!!";
            }

            ////ITopology forward = new Forward(4,3);
            //ITopology[] forwards = new Forward[4];
            //forwards[0] = new Forward(5, 3);
            //forwards[1] = new Forward(5, 3);
            //forwards[2] = new Forward(5, 3);
            //forwards[3] = new Forward(5, 3);
            //ITopology[] customs = new Custom[4];
            //for (int i = 0; i < 4; i++)
            //{
            //    double[,] transitionMatrix;
            //    double[] initialState;
            //    forwards[i].Create(false, out transitionMatrix, out initialState);
            //    transitionMatrix[0, 0] = 0;
            //    transitionMatrix[(int)Math.Sqrt(transitionMatrix.Length) - 1, (int)Math.Sqrt(transitionMatrix.Length) - 1] = 0;
            //    customs[i] = new Custom(transitionMatrix, initialState);
            //}



        }

        private void SaveHMMFile_Click(object sender, EventArgs e)
        {
            if (_hmmc != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Gestures (*.hmm)|*.hmm";
                dlg.Title = "Save Gesture As";
                dlg.AddExtension = true;
                dlg.RestoreDirectory = false;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _hmmc.Save(dlg.FileName);
                    ReloadViewForm();
                }

                dlg.Dispose();


            }
        }

        private void LoadFromHMMFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Gestures (*.hmm)|*.hmm";
            dlg.Title = "Load Gestures";
            dlg.RestoreDirectory = false;
            dlg.Multiselect = true;

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _hmmc = HiddenMarkovClassifier.Load(dlg.FileName);
                _hmms = _hmmc.Models;
                if (_hmmc != null)
                {
                    lblResult.Text = "Success!!";
                }
                ReloadViewForm();
            }
        }
        #endregion

        #region Optitrack Menu
        private void ConnectToMotive_Click(object sender, EventArgs e)
        {
            //Optitrack opt = new Optitrack(_points);
            //_motiveStart = true;
            _points.Clear();
            _directionalCodewordsQueue.Clear();
            Invalidate();

            _motiveStart = ConnectMotive();
        }

        private void DisconnectMotive_Click(object sender, EventArgs e)
        {
            Disconnect();
           
            _points.Clear();
            _directionalCodewordsQueue.Clear();
            Invalidate();
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
            lock (_points)
            {
                if (_points.Count > 0)
                {
                    PointF p0 = (PointF)(PointR)_points[0]; // draw the first point bigger
                    e.Graphics.FillEllipse(_recording ? Brushes.Firebrick : Brushes.DarkBlue, p0.X - 5f, p0.Y - 5f, 10f, 10f);
                }
                e.Graphics.DrawString(_info, _font, Brushes.Black, new PointF(20, 40));

                foreach (PointR r in _points)
                {
                    PointF p = (PointF)r; // cast
                    e.Graphics.FillEllipse(_recording ? Brushes.Firebrick : Brushes.DarkBlue, p.X - 2f, p.Y - 2f, 4f, 4f);
                }
            }
        }
        #endregion

        #region Keyboard Events
        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Up)
            //{

            //    if (hmmc != null)
            //    {
            //        hmmc.Sensitivity++;
            //        info = "Sensitivity:" + hmmc.Sensitivity;
            //    }
            //}

            //if (e.KeyCode == Keys.Down)
            //{

            //    if (hmmc != null)
            //    {
            //        hmmc.Sensitivity--;
            //        info = "Sensitivity:" + hmmc.Sensitivity;
            //    }
            //}

        }
        #endregion

        #region Mouse Events

        private void MainForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                if (_pointsList.Count >= 1)
                {
                    _directionalCodewordsList.RemoveAt(_directionalCodewordsList.Count - 1);
                    _pointsList.RemoveAt(_pointsList.Count - 1);
                    _points.Clear();
                    if (_pointsList.Count - 1 >= 0)
                    {
                        foreach (PointR p in _pointsList[_pointsList.Count - 1])
                        {
                            _points.Add(new PointR(p.X, p.Y));
                        }
                    }
                    _info = "Sample count:" + _directionalCodewordsList.Count;
                    Invalidate();
                }
            }
            if (e.Button == MouseButtons.Left)
            {
                _isDown = true;
                _points.Clear();
                _directionalCodewordsQueue.Clear();
                if (!_motiveStart)
                {

                    _points.Add(new PointR(e.X, e.Y, Environment.TickCount));

                }
                Invalidate();
            }

        }

        private void MainForm_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {

                if (_isDown)
                {

                    if (_points.Count >= 2)
                    {
                        PointR p = (PointR)_points[_points.Count - 1];
                        //_directionalCodewords = "" + getDirectionalCodewords(e.X, e.Y, p.X, p.Y);
                        _directionalCodewordsQueue.Enqueue(getDirectionalCodewords(e.X, e.Y, p.X, p.Y));
                        //if (_directionalCodewordsQueue.Count > _maxCount)
                        //{
                        //    _directionalCodewordsQueue.Dequeue();
                        //}
                    }
                    if (!_motiveStart)
                    {

                        _points.Add(new PointR(e.X, e.Y, Environment.TickCount));

                    }

                    Recongnize();

                    Invalidate();

                }


            }
            //Invalidate(new Rectangle(e.X - 2, e.Y - 2, 4, 4));

        }

        private void MainForm_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {

                if (_isDown)
                {
                    _isDown = false;
                    if (_points.Count >= 5) // require 5 points for a valid gesture
                    {
                        if (_recording)
                        {
                            //_directionalCodewords = _directionalCodewords.Substring(1, _directionalCodewords.Length - 1);
                            _directionalCodewordsList.Add(_rec.encode(_directionalCodewordsQueue.ToArray()));

                            //ArrayList pointsTemp = new ArrayList();
                            //foreach (PointR r in _points)
                            //{
                            //    pointsTemp.Add(new PointR(r.X, r.Y));
                            //}
                            if (!_motiveStart)
                            {
                                _pointsList.Add(new ArrayList(_points));
                            }
                            //_info = "Sample count:" + _pointsList.Count;
                            _info = "Sample count:" + _directionalCodewordsList.Count;
                            Invalidate();
                        }
                    }

                }

            }
        }

        private int getDirectionalCodewords(double x, double y, double lastX, double lastY)
        {
            double angle = Math.Atan2((y - lastY),(x - lastX)); // keep it in radians
            if (angle < 0)
                angle += 2 * 3.1425926;
            return (int)(angle * (8 / 3.1425926)); // convert to <0, 16)
        }
        #endregion

        #region Optitrack
        private bool ConnectMotive()
        {
            if (_NatNet != null)
            {
                _NatNet.Uninitialize();
            }
            //timer = new HiResTimer();
            _NatNet = new NatNetML.NatNetClientML(0);

            // [NatNet] set a "Frame Ready" callback function (event handler) handler that will be
            // called by NatNet when NatNet receives a frame of data from the server application
            _NatNet.OnFrameReady += new NatNetML.FrameReadyEventHandler(m_NatNet_OnFrameReady);

            // [NatNet] connect to a NatNet server
            string strLocalIP = "127.0.0.1";
            string strServerIP = "127.0.0.1";

            int code = _NatNet.Initialize(strLocalIP, strServerIP);
            if (code == 0)
            {
                return true;
            }
            return false;


        }

        private void ConnectMotive(string strLocalIP, string strServerIP)
        {
            if (_NatNet != null)
            {
                _NatNet.Uninitialize();
            }
            //timer = new HiResTimer();
            _NatNet = new NatNetML.NatNetClientML(0);

            // [NatNet] set a "Frame Ready" callback function (event handler) handler that will be
            // called by NatNet when NatNet receives a frame of data from the server application
            _NatNet.OnFrameReady += new NatNetML.FrameReadyEventHandler(m_NatNet_OnFrameReady);

            //// [NatNet] connect to a NatNet server
            //string strLocalIP = "127.0.0.1";
            //string strServerIP = "127.0.0.1";

            int code = _NatNet.Initialize(strLocalIP, strServerIP);
        }

        // [NatNet] m_NatNet_OnFrameReady will be called when a frame of Mocap
        // data has is received from the server application.
        //
        // Note: This callback is on the network service thread, so it is
        // important  to return from this function quickly as possible 
        // to prevent incoming frames of data from buffering up on the
        // network socket.
        //
        // Note: "data" is a reference structure to the current frame of data.
        // NatNet re-uses this same instance for each incoming frame, so it should
        // not be kept (the values contained in "data" will become replaced after
        // this callback function has exited).

        private void m_NatNet_OnFrameReady(NatNetML.FrameOfMocapData data, NatNetML.NatNetClientML client)
        {
            //Int64 currTime = timer.Value;
            //if (lastTime != 0)
            //{
            //    // Get time elapsed in tenths of a millisecond.
            //    Int64 timeElapsedInTicks = currTime - lastTime;
            //    Int64 timeElapseInTenthsOfMilliseconds = (timeElapsedInTicks * 10000) / timer.Frequency;

            //}

            //m_FrameQueue.Clear();
            //m_FrameQueue.Enqueue(data);
            lock (_points)
            {

                int n = data.nOtherMarkers;
                //if (_isDown)
                //{
                while (n > 0)
                {
                    float x = 0;
                    float y = 0;

                    x = data.OtherMarkers[n - 1].x;
                    y = -data.OtherMarkers[n - 1].y;
                    if (0 == frameCnt)
                    {
                        scale = Math.Abs((int)(100 / y));
                        x = x * scale;
                        y = y * scale;
                        xOffSet = 1200 / 2 - x;
                        yOffSet = 720 / 2 - y;
                    }
                    else
                    {
                        //lblResult.Text = "x:" + x+"y:"+y;
                        x = x * scale + xOffSet;
                        y = y * scale + yOffSet;
                        //lock (_points)
                        //{

                        //    //pointNow = new TimePointF(x, y, TimeEx.NowMs);
                        //    //if (Math.Sqrt(Math.Pow((pointNow.X - x), 2) + Math.Pow((pointNow.Y - y), 2)) > 5)
                        //    //{
                        //    _points.Add(new TimePointF(x, y, TimeEx.NowMs));
                        //    //}
                        
                        if (_points.Count >= 2)
                        {
                            PointR p = (PointR)_points[_points.Count - 1];
                            //_directionalCodewords = "" + getDirectionalCodewords(e.X, e.Y, p.X, p.Y);
                            _directionalCodewordsQueue.Enqueue(getDirectionalCodewords(x, y, p.X, p.Y));
                            if (_directionalCodewordsQueue.Count > _maxCount)
                            {
                                _directionalCodewordsQueue.Dequeue();
                            }
                        }
                        _points.Add(new PointR(x, y));
                        Recongnize();
                        Invalidate();
                        //    Invalidate(new Rectangle((int)x - 2, (int)y - 2, 4, 4));
                        //}
                    }
                    frameCnt++;
                    n--;
                }
            }

            //    }
            //  }
            // lastTime = currTime;

        }

        private void Disconnect()
        {
            // [NatNet] disconnect

            if (_NatNet != null)
            {
                _NatNet.Uninitialize();
            }
        }

        private void Recongnize()
        {
            //info = info + "a";
            if (_hmmc != null && _directionalCodewordsQueue.Count > 1 && !_recording) // not recording, so testing
            {
                Queue<int> directionalCodewordsQueueTemp = _directionalCodewordsQueue;
                //while (directionalCodewordsQueueTemp.Count > 30)
                //{
                    //lblResult.Text = "Recognizing...";
                    _info = null;
                    _info = _info + _rec.encode(_directionalCodewordsQueue.ToArray()) + "\n";
                    //int[] observations = _rec.decode(_directionalCodewords);
                    int[] observations = directionalCodewordsQueueTemp.ToArray();
                    _info = _info + _hmmc.Compute(observations) + "\n";

                    string gestureName = (string)_hmms[0].Tag;
                    double probTemp = 0;
                    _hmmc[0].Decode(observations, out probTemp);
                    //double probTemp = hmms[0].Evaluate(observations);
                    foreach (HiddenMarkovModel hmm in _hmms)
                    {
                        //double prob = hmm.Evaluate(observations);
                        double prob = 0;
                        int[] viterbipath = hmm.Decode(observations, out prob);
                        if (prob > probTemp)
                        {
                            gestureName = (string)hmm.Tag;
                            probTemp = prob;
                        }
                        //info = info + hmm.Tag + "\t" + hmm.Evaluate(observations) + "\t";
                        _info = _info + hmm.Tag + "\t" + prob + "\t";
                        // = hmm.Decode(observations);
                        foreach (int state in viterbipath)
                        {
                            _info = _info + state + " ";
                        }
                        _info = _info + "\n";

                    }
                    double probTM = 0;
                    int[] viterbipathTM = _hmmc.Threshold.Decode(observations, out probTM);
                    _info = _info + "ThresholdModel\t" + probTM + "\t";
                    //hmmc.Threshold.Decode(observations);
                    foreach (int state in viterbipathTM)
                    {
                        _info = _info + state + " ";
                    }
                    _info = _info + "\n";

                    if (probTM > probTemp)
                    {
                        gestureName = "Threshold";
                        _info = _info + "\n\n" + gestureName;
                    }
                    else
                    {
                        _info = _info + "\n\n" + gestureName;
                        _directionalCodewordsQueue.Clear();
                       //break;
                    }
                    //for (int loop = 0; loop < 1; loop++)
                    //{
                    //    directionalCodewordsQueueTemp.Dequeue();
                    //}
                    Invalidate();
                //}
            }
        }
        #endregion
    }
}
