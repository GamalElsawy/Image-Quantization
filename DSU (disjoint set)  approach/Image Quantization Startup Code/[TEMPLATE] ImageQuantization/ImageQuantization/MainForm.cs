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
    public struct RGBSum
    {
        public long red,green, blue;
    }
    public partial class MainForm : Form
    {

        
        List<KeyValuePair<double, KeyValuePair<int, int>>> edgeList;
        List<RGBPixel> listOfDistColor;
        int[] perant;
        //int[] Visted;
        int k = 2;
        //List<int>[] adj;
        int[] rep, sz;
        RGBSum[] sumOfCh;
        int comp;
        //Dictionary<RGBPixel, int> mp;
        int[,,] id;
        //PriorityQueue<KeyValuePair<int, int>> edgeSorted;
        public MainForm()
        {
            InitializeComponent();
        }

        public int findu(int u)
        {
            if (rep[u] == u) return u;
            rep[u] = findu(rep[u]);
            return rep[u];
        }

        public bool join(int u, int v)
        {
            u = findu(u);
            v = findu(v);
            if (sz[v] < sz[u])
            {
                int tmp = v;
                v = u;
                u = tmp;
            }
            rep[u] = v;
            sz[v] += sz[u];
            sumOfCh[v].red += sumOfCh[u].red;
            sumOfCh[v].green += sumOfCh[u].green;
            sumOfCh[v].blue += sumOfCh[u].blue;
            comp--;
            if (comp <= k)
                return true;
            return false;
        }
        void find_cal() {
            rep = new int[listOfDistColor.Count];
            sz = new int[listOfDistColor.Count];
            sumOfCh = new RGBSum[listOfDistColor.Count];
            comp = listOfDistColor.Count;
            for (int i = 0; i < rep.Length; i++)
            {
                rep[i] = i;
                sz[i] = 1;
                sumOfCh[i].red = listOfDistColor[i].red;
                sumOfCh[i].green = listOfDistColor[i].green;
                sumOfCh[i].blue = listOfDistColor[i].blue;
            }
            //edgeList.Sort((x, y) => x.Key.CompareTo(y.Key));

            if (comp <= k) return;
            bool[] vis = new bool[edgeList.Count];
            int idx = 0;
            while (comp > k)
            {
                double mn = Double.MaxValue;
                int p = 0, c = 0;
                for (int i = 0; i < edgeList.Count; i++)
                {
                   if (!vis[i] && edgeList[i].Key < mn)
                    {
                        mn = edgeList[i].Key;
                        p = edgeList[i].Value.Key;
                        c = edgeList[i].Value.Value;
                        idx = i;
                    }
                }
                vis[idx] = true;
                 if (rep[p] != rep[c])
                    {
                        if (join(p, c))
                            return;
                    }

            }
            //SolveDFS();
        }
        /*void SolveDFS()
        {
            int cnt = 1;
            //Visted = new int[listOfDistColor.Count];
            //cluster = new Dictionary<RGBPixel, int>();
            //NodePerCluster = new RGBPixel[k+1];

            for (int i = 0; i < listOfDistColor.Count; ++i)
            {
                if (Visted[i] == 0) {
                    RGBPixel cntr = new RGBPixel(); 
                    long red = 0, green = 0, blue = 0;
                    int temp=DFS(i,cnt,1, ref red, ref green, ref blue);
                    red /= temp;
                    green /= temp;
                    blue /= temp;
                    cntr.red = (byte)red;
                    cntr.green = (byte)green;
                    cntr.blue = (byte)blue;
                    NodePerCluster[cnt] = cntr;
                    ++cnt;
                }
            }
        }
        int DFS(int parent, int cntt,int dis, ref long red, ref long green, ref long blue)
        {
           
            Visted[parent] = 1;
            cluster.Add(listOfDistColor[parent], cntt);
            red += listOfDistColor[parent].red;
            green += listOfDistColor[parent].green;
            blue += listOfDistColor[parent].blue;
            for (int it=0;it<adj[parent].Count;++it)
            { KeyValuePair<int, int>pa=new KeyValuePair<int, int>(parent, adj[parent][it]);
                if (Visted[adj[parent][it]]==0 &&cutFromTo.ContainsKey(pa)==false)
                    dis += 1 + DFS(adj[parent][it], cntt,0, ref red, ref green, ref blue);
            }
            return dis;
        }*/
        public List<RGBPixel> ExtractDistColors(RGBPixel [,] ImageMatrix)
        {

            listOfDistColor = new List<RGBPixel>();
            id = new int[256, 256, 256];
            int idx = 0;
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
                        id[point.red, point.green, point.blue] = idx;
                        idx++;
                        isExected[point.red, point.green, point.blue] = 1;
                    }
                }
            }
            return listOfDistColor;
        }

        public double prim(int sizeOfDC,List<RGBPixel> LODC)
        {
            double res = 0.0;
           edgeList = new List<KeyValuePair<double, KeyValuePair<int, int>>>();
            //adj = new List<int>[sizeOfDC];
            PriorityQueue<KeyValuePair<double, int>> pq =
                new PriorityQueue<KeyValuePair<double, int>>((int)1e8);
            double[] dist = new double[sizeOfDC ];
            bool[] vis = new bool[sizeOfDC];
            
            perant = new int[sizeOfDC];
            for (int i = 0; i < sizeOfDC; i++)
            {
                dist[i] = System.Double.MaxValue;
                //adj[i] = new List<int>();
            }
            dist[0] = 0.0;
            perant[0] = 0;
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
                //edgeSorted.Enqueue(new KeyValuePair<int, int>(perant[i], i), -dist[i]);
                edgeList.Add(new KeyValuePair<double, KeyValuePair<int, int>>(dist[i], new KeyValuePair<int, int>(perant[i], i)));
                /*adj[perant[i]].Add(i);
                adj[i].Add(perant[i]);*/
            }
            return res;
        }
        void adjustMatrix(ref RGBPixel[,] ImageMatrix)
        {
            for (int i = 0; i < ImageOperations.GetHeight(ImageMatrix); i++)
            {
                for (int j = 0; j < ImageOperations.GetWidth(ImageMatrix); j++)
                {
                    RGBPixel point;
                    point.red = ImageMatrix[i, j].red;
                    point.green = ImageMatrix[i, j].green;
                    point.blue = ImageMatrix[i, j].blue;
                    int idx = findu(id[point.red, point.green, point.blue]);
                    RGBPixel newColor;
                    if (sz[idx] != 0)
                    {
                        newColor.red = (byte)(sumOfCh[idx].red / sz[idx]);
                        newColor.green = (byte)(sumOfCh[idx].green / sz[idx]);
                        newColor.blue = (byte)(sumOfCh[idx].blue / sz[idx]);
                        ImageMatrix[i, j] = newColor;
                    }
                    /*int x = 0;
                    x++;*/

                }
            }
        }
        RGBPixel[,] ImageMatrix;
        private void btnOpen_Click(object sender, EventArgs e)
        {
            
            
            //PriorityQueue<KeyValuePair<double, KeyValuePair<RGBPixel, RGBPixel>>> P =
                //new PriorityQueue<KeyValuePair<double, KeyValuePair<RGBPixel, RGBPixel>>>((int)1e8);


            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
                
                List<RGBPixel> listOfDistColor = ExtractDistColors(ImageMatrix);

            }
           /* while (P.Size() > 0)
            {
                MessageBox.Show(P.Dequeue().Key.ToString());
            }*/
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            double ans = prim(listOfDistColor.Count,listOfDistColor);
            find_cal();
            adjustMatrix(ref ImageMatrix);
            sw.Stop();
            long time = sw.ElapsedMilliseconds;
            //OpenedFilePath = openFileDialog1.FileName;
            //ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
            
            MessageBox.Show(ans.ToString() + " && " + listOfDistColor.Count.ToString());
            MessageBox.Show("Time elapsed: " + time + "ms ");
            /*int k=3;
            List<RGBPixel> centers = new List<RGBPixel>();
            Dictionary<int, int> dec = new Dictionary<int, int>();
            while (centers.Count < k)
            {
                Random rnd = new Random();
                int rd = rnd.Next(0, 1000000000)% listOfDistColor.Count;
                if (dec[rd] == 0)
                {
                    centers.Add(listOfDistColor[rd]);
                    dec[rd] = 1;
                }
            }
            dec.Clear();
            List<int> myList = new List<int>(listOfDistColor.Count);
            for(int ii = 0; ii < listOfDistColor.Count; ++ii)
            {
                myList[ii] = -1;
            }
            double Last_error = 0;
            while (true)
            {
                for(int ii = 0; ii < listOfDistColor.Count; ++ii)
                {
                    int cid = find_cal(listOfDistColor, listOfDistColor[ii]);
                    myList[ii]=cid;

                }

            }*/

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

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void nudMaskSize_ValueChanged(object sender, EventArgs e)
        {
            //k = Int32.Parse(nudMaskSize.Value.ToString());
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().ToString() == "") return;
            k = Int32.Parse(textBox1.Text.Trim().ToString());
        }
    }
}