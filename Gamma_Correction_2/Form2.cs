using Gamma_Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gamma_Correction_2
{
    public partial class Form2 : Form
    {
        public Form2(string file)
        {
            InitializeComponent();
            ProgressDelegate progressBar = Progress;
            CheckForIllegalCrossThreadCalls = false;
            Algorithm.GammaNum = 0.1f;
            chart1.Series[0].Points.Clear();
            for (int i = 30; i <= 300; i += 10)
            {
                SortedList<string, object> par = new SortedList<string, object>();
                Bitmap image = new Bitmap(file);
                image.SetResolution(i, i);
                par.Add("Image", image);
                par.Add("Number", i);
                Algorithm gamma = new Algorithm();
                gamma.init(par);
                gamma.startHandle(progressBar);
                chart1.Series[0].Points.AddXY(i, gamma.stopwatch.ElapsedMilliseconds);
            }
        }
        public void Progress(int number, double percentage, int width, int height) { }
    }
}
