using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;

namespace Recognizer.HMM
{
	public class AboutForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button OK;
		private System.ComponentModel.Container components = null;

		private ArrayList _points;

		public AboutForm(ArrayList points)
		{
			InitializeComponent();
            _points = Utils.TranslateBBoxTo(points, new PointR(50, 120));
		}

		protected override void Dispose( bool disposing )
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(108, 245);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(90, 24);
            this.OK.TabIndex = 0;
            this.OK.Text = "OK";
            // 
            // AboutForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(304, 281);
            this.Controls.Add(this.OK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AboutForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About Recognizer";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AboutForm_Paint);
            this.Resize += new System.EventHandler(this.AboutForm_Resize);
            this.ResumeLayout(false);

		}
		#endregion

		private void AboutForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Font f = new Font(FontFamily.GenericSansSerif, 8.25f);
            string msg = "HMM Recognizer v" + Assembly.GetExecutingAssembly().GetName().Version + "\r\nCopyright (C) 2014-2015\r\n\r\nLibaier\r\n\r\nlibaier.net";
			e.Graphics.DrawString(msg, f, Brushes.Black, 10f, 10f);
			f.Dispose();

            if (_points.Count > 0)
            {
                PointF p0 = (PointF) (PointR) _points[0];
                e.Graphics.FillEllipse(Brushes.Firebrick, p0.X - 5f, p0.Y - 5f, 10f, 10f);
            }
			foreach (PointR r in _points)
			{
				PointF p = (PointF) r; // cast
				e.Graphics.FillEllipse(Brushes.Firebrick, p.X - 2f, p.Y - 2f, 4f, 4f);
			}
		}

        private void AboutForm_Resize(object sender, EventArgs e)
        {
            OK.Left = this.Width / 2 - OK.Width / 2 - 4;
            OK.Top = this.Height - 69;
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {

        }
	}
}
