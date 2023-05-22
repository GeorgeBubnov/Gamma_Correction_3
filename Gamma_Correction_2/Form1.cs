using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gamma_Library;

namespace Gamma_Correction_2
{
    public partial class Form1 : Form
    {
        SortedList<string, object> images;
        string[] files;
        Label[] lab;
        int curImgNum;
        public object locker = new object();

        public Form1()
        {
            InitializeComponent();
            lab = new Label[] { label1, label2, label3, label4, label5, label6, label7, label8, label9, label10 };
        }

        private void btnOFD_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "Изображкние (*.png;*.jpg)|*.png;*.jpg",
                Title = "Выберите изображения"
            };
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            else
            {
                for (int i = 0; i < 10; i++)
                    lab[i].Visible = false;
                files = ofd.FileNames;
                images = new SortedList<string, object>();
                if (files.Length > 10)
                {
                    MessageBox.Show("Количество файлов не должно быть больше 10", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                for (int i = 0; i < files.Length; i++)
                {
                    lab[i].Visible = true;
                    lab[i].Text = Path.GetFileName(files[i]);
                    SortedList<string, object> par = new SortedList<string, object>();
                    par.Add("Image", new Bitmap(files[i]));
                    par.Add("Number", i);
                    Algorithm gamma = new Algorithm();
                    gamma.init(par);
                    images.Add($"{i}", gamma);
                }
                curImgNum = -1;
            }
        }

        public void Progress(int number, double percentage, int width, int height)
        {
            lock (locker)
            {
                lab[number].Text = $"{Path.GetFileName(files[number])} " +
                    $"({width}x{height}) Обработано {percentage:#.#}%";
            }
        }

        public void DoWork(int index, ProgressDelegate progress)
        {
            using (BackgroundWorker worker = new BackgroundWorker())
            {
                worker.DoWork += (obj, ea) => ((Algorithm)images[$"{index}"]).startHandle(progress);
                worker.RunWorkerAsync();
            }
        }

        private void btnCorrect_Click(object sender, EventArgs e)
        {
            if (files != null && files?.Length <= 10)
            {
                Algorithm.GammaNum = (float)numGamma.Value;
                ProgressDelegate progressBar = Progress;
                CheckForIllegalCrossThreadCalls = false;
                for (int i = 0; i < files.Length; i++)
                    DoWork(i, progressBar);
            }
            else
                MessageBox.Show("Ошибка. Не выраны изображения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        public void ShowImage()
        {
            labelCurNum.Text = $"Исходное изображение {Path.GetFileName(files[curImgNum])}";
            pictureBox1.Image = ((Algorithm)images[$"{curImgNum}"]).Source;
            pictureBox2.Image = ((Algorithm)images[$"{curImgNum}"]).Result;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (files != null)
            {
                if (curImgNum == files.Length - 1)
                    curImgNum = 0;
                else
                    curImgNum++;
                ShowImage();
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (files != null)
            {
                if (curImgNum == 0)
                    curImgNum = files.Length - 1;
                else
                    curImgNum--;
                if (curImgNum == -2)
                    curImgNum = 0;
                ShowImage();
            }
        }

        private void btnСhart_Click(object sender, EventArgs e)
        {
            if (files != null)
                new Thread(() =>
                {
                    MessageBox.Show("Построени графика может занять некоторое время.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Form2 graf = new Form2(files[0]);
                    if (graf.ShowDialog() == DialogResult.Cancel) return;
                }).Start();
            else
                MessageBox.Show("Ошибка. Не выраны изображения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
