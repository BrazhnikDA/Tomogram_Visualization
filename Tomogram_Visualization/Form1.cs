using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tomogram_Visualization
{
    public partial class Form1 : Form
    {
        Bin bin = new Bin();
        View view = new View();
        bool IsLoad = false;
        int currentLayer = 0;
        GLgraphics glgraphics = new GLgraphics();
        public Form1()
        {
            InitializeComponent(); 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            glgraphics.Setup(glControl1.Width, glControl1.Height);
        }

        bool IsReDraw = false;
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (IsLoad)
            {
                if (radioButton1.Checked)
                {
                    view.DrawQuads(currentLayer);
                }
                else
                {
                    if (radioButton3.Checked)
                    {
                        view.DrawQuadsStrip(currentLayer);
                    }
                    else
                    {
                        if (IsReDraw)
                        {

                            view.generateTextureImage(currentLayer);
                            view.Load2DTexture();
                            IsReDraw = false;
                        }
                        view.DrawTexture();
                    }
                    glControl1.SwapBuffers();
                }
            }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                trackBar1.Maximum = Bin.Z - 1;
                IsLoad = true;
                glControl1.Invalidate();
            }
        }      

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            IsReDraw = true;
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            view.minimum = trackBar2.Value;
            IsReDraw = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            view.TF = trackBar3.Value;
            IsReDraw = true;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("Просмотр томограмм (FPS={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        private void glControl1_SizeChanged(object sender, EventArgs e)
        {
            glControl1.Width = this.Width;
            glControl1.Height = this.Height - 325;
            glControl1.BackColor = Color.White;
        }
    }
}
