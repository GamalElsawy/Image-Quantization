using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        public List<RGBPixel> ExtractDistColors(RGBPixel [,] ImageMatrix)
        {

            List<RGBPixel> listOfDistColor = new List<RGBPixel>();
            byte[,,] isExected = new byte[256, 256, 256];
            for (int i = 0; i < ImageOperations.GetHeight(ImageMatrix); i++)
            {
                for (int j = 0; j < ImageOperations.GetWidth(ImageMatrix); j++)
                {

                    RGBPixel point;
                    point.red = ImageMatrix[i, j].red;
                    point.green = ImageMatrix[i, j].green;
                    point.blue = ImageMatrix[i, j].blue;

                    if (isExected[point.red, point.green, point.blue] == 0)
                    {
                        listOfDistColor.Add(point);
                        isExected[point.red, point.green, point.blue] = 1;
                    }
                }
            }
            return listOfDistColor;
        }

        public double prim(int sizeOfDC,List<RGBPixel> LODC)
        {
            double res = 0.0;
            KeyValuePair<double, KeyValuePair<RGBPixel, RGBPixel>>[] edgeList =
                new KeyValuePair<double, KeyValuePair<RGBPixel, RGBPixel>>[sizeOfDC - 1];

            PriorityQueue<KeyValuePair<double, int>> pq =
                new PriorityQueue<KeyValuePair<double, int>>((int)1e8);

            double[] dist = new double[sizeOfDC];
            bool[] vis = new bool[sizeOfDC];
            int[] perant = new int[sizeOfDC];
            for (int i = 0; i < sizeOfDC; i++)
                dist[i] = System.Double.MaxValue;
            dist[0] = 0.0;
            pq.Enqueue(new KeyValuePair<double, int>(0.0, 0),0.0);
            while(pq.Size() > 0)
            {
                double d = pq.Get().Key;
                int u = pq.Get().Value;
                pq.Dequeue();
                if (vis[u]) continue;
                res += d;
                vis[u] = true;
                for(int i = 0;i < sizeOfDC; i++)
                {
                    if (vis[i]) continue;
                    long a = LODC[i].red - LODC[u].red,
                        b = LODC[i].green - LODC[u].green,
                        c = LODC[i].blue - LODC[u].blue;

                    double edgeVal = Math.Sqrt(a * a + b * b + c * c);

                    if(edgeVal < dist[i])
                    {
                        dist[i] = edgeVal;
                        perant[i] = u;
                        pq.Enqueue(new KeyValuePair<double, int>(edgeVal,i),-edgeVal);
                    }
                }


            }

            for(int i = 1,j = 0;i < sizeOfDC; i++,j++)
            {
                edgeList[j] = new KeyValuePair<double, KeyValuePair<RGBPixel, RGBPixel>>(dist[i], new KeyValuePair<RGBPixel, RGBPixel>(LODC[perant[i]], LODC[i]));
            }
            return res;
        } 
        RGBPixel[,] ImageMatrix;
        private void btnOpen_Click(object sender, EventArgs e)
        {
            
            
            PriorityQueue<KeyValuePair<double, KeyValuePair<RGBPixel, RGBPixel>>> P =
                new PriorityQueue<KeyValuePair<double, KeyValuePair<RGBPixel, RGBPixel>>>((int)1e8);


            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);

                
                List<RGBPixel> listOfDistColor = ExtractDistColors(ImageMatrix); 
                double ans = prim(listOfDistColor.Count,listOfDistColor);
                
                MessageBox.Show(ans.ToString() + " && " + listOfDistColor.Count.ToString());
            }
            while (P.Size() > 0)
            {
                MessageBox.Show(P.Dequeue().Key.ToString());
            }
           
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value ;
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }
        
    }
}