using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpringPendulum
{
    public partial class SpringPendulum : Form
    {
        public SpringPendulum()
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = springObj;
            cavasSize = splitContainer1.Panel1.ClientSize;
        }
        Size cavasSize = new Size();
        Bitmap cavasImage = null;
        SpringPendulumObject springObj = new SpringPendulumObject();
        void UpdateView(Graphics g)
        {
            if (!timer1.Enabled)
            {
                return;
            }
            
            
            if (springObj.ShowVetricalAngle)
            {
                g.FillRectangle(Brushes.Blue, 10, 10, 30, 20);
                g.DrawString("Vertical Angle", this.Font, Brushes.Blue, 50, 10);
                DrawBlockCurve(springObj.HistoryBuffer[0], Pens.Blue, 0, g);
            }
            if (springObj.ShowAngleSpeed)
            {
                g.FillRectangle(Brushes.Green, 10, 50, 30, 20);
                g.DrawString("Angle Speed", this.Font, Brushes.Green, 50, 50);
                DrawBlockCurve(springObj.HistoryBuffer[1], Pens.Green, 0, g);
            }
            if (springObj.ShowSpringLen)
            {
                g.FillRectangle(Brushes.Red, 10, 90, 30, 20);
                g.DrawString("Spring Len", this.Font, Brushes.Red, 50, 90);
                DrawBlockCurve(springObj.HistoryBuffer[2], Pens.Red, springObj.SpringLenght, g);
            }
            if (springObj.ShowSpringSpeed)
            {
                g.FillRectangle(Brushes.White, 10, 130, 30, 20);
                g.DrawString("Spring Speed", this.Font, Brushes.White, 50, 130);
                DrawBlockCurve(springObj.HistoryBuffer[3], Pens.White, 0, g);
            }
            
            int len = (int)springObj.SpringLenght;
            int x = (int)springObj.X + cavasSize.Width / 2;
            int y = (int)springObj.Y + cavasSize.Height / 2;
            DrawSpring(cavasSize.Width / 2, cavasSize.Height / 2, x, y, g, 15);
           // g.DrawRectangle(Pens.Red, x - 15, y - 15, 31, 31);
            g.FillEllipse(Brushes.Yellow, x - 15, y - 15, 31, 31);
            
        }

        private void DrawSpring(int x0, int y0, int x, int y, Graphics g, int s)
        {
            double dx0 = x - x0;
            double dy0 = y - y0;
            double ds = Math.Sqrt(dx0 * dx0 + dy0 * dy0);
            double dist = 25;
            while (dist * s < ds)
            {
                dist *= 1.05;
            }
            dist = Math.Sqrt(s * s * dist * dist - ds * ds) / s;
            dx0 /= ds;
            dy0 /= ds;
            double dx = -dy0;
            double dy = dx0;
            ds /= (s + 1);
            
            Point[] ps = new Point[2 + s];
            ps[0] = new Point(x0, y0);
            ps[s + 1] = new Point(x, y);
            double flag = -1;
            int x1 = 0;
            int y1 = 0;
            for (int i = 1; i < 1 + s;  i ++)
            {
                x1 = x0 + (int)((i * ds) * dx0 + dist * dx * flag);
                y1 = y0 + (int)((i * ds) * dy0 + dist * dy * flag);
                ps[i] = new Point(x1, y1);
                flag *= -1;
            }
            for (int i = 1; i < s + 2; i++)
            {
                g.DrawLine(Pens.Yellow, ps[i - 1], ps[i]);
            }

             //   g.DrawLine(Pens.Red, x0, y0, x, y);
        }
        private void DrawBlockCurve(double[] rec, Pen p,double bs, Graphics g)
        {
            if (rec == null)
            {
                return;
            }

            //find out max and min
            double minValue = double.MaxValue;
            double maxValue = double.MinValue;
            for (int i = 0; i < rec.Length; i++)
            {
                if (rec[i] < minValue)
                {
                    minValue = rec[i];
                }
                if (rec[i] > maxValue)
                {
                    maxValue = rec[i];
                }
            }
            maxValue -= minValue;
            int verSize = cavasSize.Height;
            
            //draw graphic
            try
            {
                int h = cavasSize.Height;
                double stepSize = 1;
                double pCur = 0;
                int x1 = 0;
                int x2 = 0;
                int y1 = 0;
                int y2 = 0;
                double offset = 0;
                x1 = 0;
                y1 = h - (int)((rec[0] - minValue) * verSize / maxValue);

                int  zeroh = h - (int)((bs - minValue) * verSize / maxValue);
                g.DrawLine(p, 0, zeroh, 20, zeroh);
                g.DrawLine(p, cavasSize.Width - 20, zeroh, 20, zeroh);
                g.DrawString(bs.ToString("0.00"), this.Font, new SolidBrush(p.Color), 0, zeroh - 13);
                g.DrawString(bs.ToString("0.00"), this.Font, new SolidBrush(p.Color), cavasSize.Width - 20, zeroh - 13);
                if (rec.Length < cavasSize.Width)
                {
                    stepSize = (double)cavasSize.Width / (double)rec.Length;
                    pCur = 0;
                    for (int i = 1; i < rec.Length; i++)
                    {
                        pCur += stepSize;
                        x2 = (int)pCur;
                        y2 = h - (int)((rec[i] - minValue) * verSize / maxValue);
                        g.DrawLine(p, x1, y1, x2, y2);
                        x1 = x2;
                        y1 = y2;
                    }
                }
                else
                {

                    List<int> keyPoints = new List<int>();
                    keyPoints.Add(0);
                    for (int i = 1; i < rec.Length - 1; i++)
                    {
                        if ((rec[i] - rec[i - 1]) * (rec[i + 1] - rec[i]) < 0)
                        {
                            keyPoints.Add(i);
                        }
                    }
                    keyPoints.Add(rec.Length - 1);
                    double[] blockSize = new double[keyPoints.Count - 1];
                    for (int i = 0; i < blockSize.Length; i++)
                    {
                        blockSize[i] = (double)((keyPoints[i + 1] - keyPoints[i]) * (double)cavasSize.Width) / (double)rec.Length;
                    }

                    for (int i = 1; i < keyPoints.Count; i++)
                    {
                        if (blockSize[i - 1] < 1.0)
                        {
                            g.DrawLine(p, (int)offset, (int)((rec[keyPoints[i - 1]] - minValue) * verSize / maxValue), (int)offset, (int)((rec[keyPoints[i]] - minValue) * Height / maxValue));
                        }
                        else
                        {
                            pCur = keyPoints[i - 1];
                            stepSize = (double)(keyPoints[i] - keyPoints[i - 1]) / blockSize[i - 1];
                            for (int k = (int)offset + 1; k < offset + blockSize[i - 1]; k++)
                            {
                                x2 = k;
                                pCur += stepSize;
                                y2 = (int)pCur;
                                if (y2 >= rec.Length)
                                {
                                    y2 = rec.Length - 1;
                                }
                                y2 = h - (int)((rec[y2] - minValue) * verSize / maxValue);
                                g.DrawLine(p, x1, y1, x2, y2);
                                x1 = x2;
                                y1 = y2;
                            }
                        }
                        offset += blockSize[i - 1];
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void DrawXY(double[] xs, double[] ys, Pen p, Graphics g)
        {

            double scaleX = 0;
            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double scaleY = 0;
            double minY = double.MaxValue;
            double maxY = double.MinValue;
            int n = xs.Length;
            if (ys.Length != n)
            {
                return;
            }
            for (int i = 0; i < n; i++)
            {
                if (xs[i] > maxX)
                {
                    maxX = xs[i];
                }
                if (xs[i] < minX)
                {
                    minX = xs[i];
                }
                if (ys[i] > maxY)
                {
                    maxY = ys[i];
                }
                if (ys[i] < minY)
                {
                    minY = ys[i];
                }
            }
            scaleX = maxX - minX;
            scaleY = maxY - minY;
            int x = 0;
            int y = 0;
            for (int i = 0; i < n; i++)
            {
                x = (int)Math.Round((xs[i] - minX) * cavasSize.Width / scaleX);
                y = (int)Math.Round((ys[i] - minY) * cavasSize.Height / scaleY);
                g.DrawRectangle(p, x, y, 2, 2);
            }
        }
        private void runButton_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
            }
            else
            {
                timer1.Start();
                springObj.Reset();
            }
            runButton.Text = timer1.Enabled ? "Stop" : "Start";
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            if (cavasImage == null)
            {
                cavasImage = new Bitmap(cavasSize.Width, cavasSize.Height);
            }
            Graphics g = Graphics.FromImage(cavasImage);
            g.FillRectangle(Brushes.Black, 0, 0, cavasImage.Width, cavasImage.Height);
            UpdateView(g);
            g.Dispose();
            e.Graphics.DrawImage(cavasImage,0,0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            springObj.Step(10);
            Graphics g = Graphics.FromImage(cavasImage);
            g.FillRectangle(Brushes.Black, 0, 0, cavasImage.Width, cavasImage.Height);
            UpdateView(g);
            g.Dispose();
            g = splitContainer1.Panel1.CreateGraphics();
            g.DrawImage(cavasImage,0,0);
            g.Dispose();
        }

        private void splitContainer1_Panel1_SizeChanged(object sender, EventArgs e)
        {
            cavasSize = splitContainer1.Panel1.ClientSize;
            cavasImage = new Bitmap(cavasSize.Width, cavasSize.Height);
        }
    }
}
