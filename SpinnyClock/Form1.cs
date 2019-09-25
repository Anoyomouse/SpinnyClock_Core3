using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace SpinnyClock
{
    public partial class Form1 : Form
    {
        private List<TimeItem> lst;
        private List<Button> butRem;

        public Form1()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        }

        private void timTick_Tick(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lst = new List<TimeItem>();
            butRem = new List<Button>();

            FileInfo f = new FileInfo("data.xml");
            if (!f.Exists)
            {
                StreamWriter write = new StreamWriter("data.xml");
                write.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                write.WriteLine("<root>");
                write.WriteLine("	<time name=\"Anoyomouse\" offset=\"+2\" dst=\"False\" />");
                write.WriteLine("</root>");
                write.Flush();
                write.Close();
            }

            XPathDocument doc = new XPathDocument("data.xml");

            XPathNavigator nav = doc.CreateNavigator();
            nav.MoveToNext();

            //ParseTree(nav, ref lst);

			lst.Add(new TimeItem("My Time", +2, false));
        }

        Bitmap bmp;
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.ClientSize.Height != (30 + lst.Count * 20 + 100 + 20))
            {
                int offset = this.Height - this.ClientSize.Height;

                this.Height = (30 + lst.Count * 20 + 100 + 20) + offset;

                cmdExit.Top = this.Height - 10 - cmdExit.Height;

                cmdAdd.Top = cmdExit.Top - 10 - cmdAdd.Height;

                cmdExit.Left = this.Width - cmdExit.Width - 10;
                cmdAdd.Left = cmdExit.Left;
            }

            if (bmp == null || (bmp.Width != this.ClientSize.Width || bmp.Height != this.ClientSize.Height))
            {
                bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
                this.Region = DrawStuff.DoRegion(this.ClientSize.Width, this.ClientSize.Height);
            }

            Graphics g = Graphics.FromImage(bmp);
            Color bg = Color.FromArgb(255, 7, 47, 0);
            this.BackColor = bg;
            g.FillRectangle(new SolidBrush(bg), 0, 0, this.ClientSize.Width, this.ClientSize.Height);

            Pen lg, ng, dg;
            lg = new Pen(Color.FromArgb(255, 7, 70, 0));
            ng = new Pen(Color.FromArgb(255, 7, 50, 0));
            dg = new Pen(Color.FromArgb(255, 7, 30, 0));
            {
                Pen tmp;
                tmp = lg;
                lg = dg;
                dg = tmp;
            }
            DrawStuff.DrawHatchedBackground(g, this.ClientSize.Width, this.ClientSize.Height, 10, lg, ng, dg);

            Pen p = new Pen(Color.FromArgb(255, 14, 65, 0), 6);

            DrawStuff.DrawArcBorder(g, this.ClientSize.Width, this.ClientSize.Height, 3, p);

            p = new Pen(Color.Goldenrod, 2);

            DrawStuff.DrawArcBorder(g, this.ClientSize.Width, this.ClientSize.Height, 0, p);

            Font fnt = new System.Drawing.Font("MS Sans Serif", 8.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));

            DateTime now = DateTime.Now;
            int i = 0;
            foreach (TimeItem itm in lst)
            {
                DateTime newtime = now.ToUniversalTime().AddHours(itm.Offset);
                if (itm.DST)
                {
                    bool bDST = DSTTime.Get.Offset(now);
                    if (bDST)
                        newtime = newtime.AddHours(1);
                }
                DrawStuff.DrawTextShadowLeft(g, itm.Name, fnt, Brushes.Goldenrod, 20 + i * 20, 100, 0);
                var size = DrawStuff.DrawTextShadow(g, newtime.ToString("HH:mm:ss fffffff"),
                    fnt, Brushes.Goldenrod, 110, 20 + i * 20);

                if ((i + 1) == Button_num)
                    DrawStuff.DrawTextShadow(g, "Remove", fnt, Brushes.Gold, 110 + size.Width + 10, 20 + i * 20);
                else
                    DrawStuff.DrawTextShadow(g, "Remove", fnt, Brushes.LightGreen, 110 + size.Width + 10, 20 + i * 20);

                i++;
            }

            DrawSpinnyClock(g, now, 20, 30 + lst.Count * 20);

            e.Graphics.DrawImage(bmp, 0, 0);
        }

        private void DrawSpinnyClock(Graphics g, DateTime now, int left, int top)
        {
            float x, y;
            double theta;

            for (int c = 0; c < 360; c += (360 / 12))
            {
                theta = ((c + (360 - 90)) % 360) * (Math.PI / 180);

                x = (float)Math.Cos(theta) * (100 / 2);
                y = (float)Math.Sin(theta) * (100 / 2);
                g.DrawLine(Pens.Black, left + (100 / 2) - x, top + (100 / 2) - y, left + (100 / 2) + x, top + (100 / 2) + y);
            }

            theta = ((((now.Hour % 12) / 12.0f) * 360) + (((now.Minute % 60) / 60.0f) * (360 / 12)) + (360 - 90)) * (Math.PI / 180);
            x = (float)Math.Cos(theta) * (100 / 4);
            y = (float)Math.Sin(theta) * (100 / 4);
            g.DrawLine(Pens.Goldenrod, left + (100 / 2), top + (100 / 2), left + (100 / 2) + x, top + (100 / 2) + y);

            theta = ((((now.Minute % 60) / 60.0f) * 360) + (((now.Second % 60) / 60.0f) * (360 / 60)) + (360 - 90)) * (Math.PI / 180);
            x = (float)Math.Cos(theta) * (100 / 3);
            y = (float)Math.Sin(theta) * (100 / 3);
            g.DrawLine(Pens.Goldenrod, left + (100 / 2), top + (100 / 2), left + (100 / 2) + x, top + (100 / 2) + y);

            theta = (((((now.Second % 60) / 60.0f)) * 360) + ((now.Millisecond / 1000.0) * (360 / 60)) + (360 - 90)) * (Math.PI / 180);
            x = (float)Math.Cos(theta) * (100 / 2);
            y = (float)Math.Sin(theta) * (100 / 2);
            g.DrawLine(Pens.Red, left + (100 / 2), top + (100 / 2), left + (100 / 2) + x, top + (100 / 2) + y);

            theta = (((((now.Millisecond % 1000) / 1000.0f)) * 360) + (360 - 90)) * (Math.PI / 180);
            x = (float)Math.Cos(theta) * (100 / 2);
            y = (float)Math.Sin(theta) * (100 / 2);
            g.DrawLine(Pens.Red, left + (100 / 2), top + (100 / 2), left + (100 / 2) + x, top + (100 / 2) + y);

            g.DrawEllipse(Pens.LightGreen, left, top, 100, 100);

            g.DrawEllipse(Pens.LightGreen, left + (100 / 2) - (6 / 2), top + (100 / 2) - (6 / 2), 6, 6);
        }

        bool bDragging;
        Point pntStart;
        int Button_num;
        private void StartDrag(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            pntStart = new Point();
            if (e.Button == MouseButtons.Left)
            {
                Button_num = 0;
                if (e.X > 110 + 120 && e.X < 110 + 120 + 40 && e.Y > 20 && e.Y < (20 + lst.Count * 20))
                {
                    Button_num = ((e.Y - 20) / 20) + 1;
                }
                bDragging = true;
                pntStart.X = e.X;
                pntStart.Y = e.Y;
            }
        }

        private void DoDrag(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (bDragging && e.Button == MouseButtons.Left)
            {
                int dx = -pntStart.X + e.X;
                int dy = -pntStart.Y + e.Y;

                this.Left += dx;
                this.Top += dy;
            }
        }

        private void EndDrag(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Button_num != 0)
            {
                DialogResult res = MessageBox.Show("Are you sure you want to remove " + lst[Button_num - 1].Name + "?", "Spinny Clock", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (res == DialogResult.Yes)
                {
                    lst.RemoveAt(Button_num - 1);
                    //SaveXML();
                }

                Button_num = 0;
            }
            bDragging = false;
        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
