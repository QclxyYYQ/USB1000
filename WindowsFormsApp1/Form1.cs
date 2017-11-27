using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using WebSocket4Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        bool[] state = new bool[16];
        short selState;
        public Form1()
        {
            InitializeComponent();
            for (int i = 1; i <= 16; i++)
            {
                checkedListBox1.Items.Add(i, false);
            }
            txRate.Text = SampleRate.ToString();
        }

        private int CountChannel()
        {
            int count = 0;
            foreach (int i in checkedListBox1.Items)
            {
                if (checkedListBox1.GetItemChecked(i - 1))
                {
                    count++;
                    state[i - 1] = true;
                }
            }
            return count;
        }
        float[] data = new float[500000];

        bool running = false;
        /// <summary>
        /// 每通道采样率=设置采样率SampleRate/使用通道数，单位 S/s，最高 500kS/s 模拟输入采样率（开启多通道时最高 200kS/s）
        /// </summary>
        int SampleRate = 5000;
        /// <summary>
        /// 使用的通道数
        /// </summary>
        int ChansCount = 1;

        int NumToRead;

        ConcurrentQueue<float[,]> dataQueue = new ConcurrentQueue<float[,]>();

        Thread recordThread;

        FileStream fs;
        StreamWriter sw;

        double totalTime = 0;
        /// <summary>
        /// 消费者线程，用于从队列里读取数据写入Excel文件
        /// </summary>
        void record()
        {
            try
            {
                //以当前时间为文件名创建文件
                fs = new FileStream(DateTime.Now.ToFileTime() + ".csv", FileMode.Create);
                sw = new StreamWriter(fs, Encoding.ASCII);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            double stepTime = (1.0 / SampleRate) * 1000;
            while (running || dataQueue.Count != 0)
            {
                if (dataQueue.TryDequeue(out float[,] t))
                {
                    for (int k = 0; k < NumToRead; k++)
                    {
                        //写入时间
                        sw.Write(totalTime);
                        sw.Write(",");
                        for (int i = 0; i < ChansCount; i++)
                        {
                            sw.Write(t[i, k]);
                            if (k != ChansCount - 1)
                                sw.Write(",");
                        }
                        //最后用换行隔开每一次采样数据
                        sw.Write('\n');
                        totalTime += stepTime;
                    }

                }
                Thread.Yield();
            }
            sw.Flush();
            sw.Close();
            fs.Close();
            Debug.WriteLine("记录完成");
        }
        /// <summary>
        /// 将16个通道选择转为2个字节存储
        /// </summary>
        /// <returns></returns>
        private short GetChannel()
        {
            short t = 0x00;
            for (int i = 0; i < 16; i++)
            {
                if (state[i])
                    t |= (short)(1 << i);
            }
            selState = t;
            return t;
        }
        private void InitSeries()
        {
            chart1.Series.Clear();
            //chart1.ChartAreas[0].Area3DStyle.Enable3D = true;
            chart1.ChartAreas[0].Area3DStyle.Enable3D = false;

            for (int i = 0; i < 16; i++)
            {
                if (state[i])
                {
                    var s = chart1.Series.Add("通道" + (i + 1));
                    s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    s.BorderWidth = 2;
                }

            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            ChansCount = CountChannel();
            if (ChansCount == 0)
            {
                MessageBox.Show("请先选择通道");
                return;
            }
            InitSeries();
            GetChannel();
            checkedListBox1.Enabled = false;
            int temp = 0;
            if (USB1000.FindUSBDAQ() == 0)
            {
                MessageBox.Show("找不到采集卡硬件，请确保已经正确连接电脑并且驱动安装正常。");
                return;
            }
            try
            {
                temp = USB1000.OpenDevice(0);
                temp = USB1000.ResetDevice(0);
                temp = USB1000.SetUSB1AiRange(0, (int)USB1000.AiRange.V5);
                temp = USB1000.SetSampleRate(0, SampleRate);
                temp = USB1000.SetChanMode(0, (int)USB1000.ChannelMode.NRSE);

                temp = USB1000.SetChanSel(0, selState);


                temp = USB1000.SetSoftTrig(0, 1);

                if (temp == 0)
                {
                    NumToRead = SampleRate / ChansCount / 10;//每通道0.1秒读多少个数据（采样率单位是点/秒）
                    totalTime = 0;
                    Debug.WriteLine("num to read: " + NumToRead);
                    Debug.WriteLine("开始采集");
                    running = true;

                    recordThread = new Thread(record);
                    recordThread.Start();

                    temp = USB1000.StartRead(0);

                    timer1.Interval = 80;//ms   Timer设置比0.1s略小，以保障数据无滞后不溢出
                    timer1.Start();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stop();
        }
        private void stop()
        {
            if (running)
            {
                timer1.Stop();
                running = false;
                checkedListBox1.Enabled = true;
                USB1000.StopRead(0);                   // 关闭计算机从采集卡读数线程
                USB1000.SetSoftTrig(0, 0);             // 关闭软件触发信号
                USB1000.ClearBufs(0);                  // 清空数据缓存空间
                USB1000.CloseDevice(0);                // 关闭设备
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (running)
            {
                int AiFifoFree = USB1000.GetAiChans(0, NumToRead, selState, data, 1000);
                lb_fifo.Text = AiFifoFree.ToString();
                if (AiFifoFree < 0)
                {
                    Debug.WriteLine("ErrorCode:" + AiFifoFree);
                }
                else
                {

                    float[,] f = new float[ChansCount, NumToRead];
                    for (int k = 0; k < ChansCount; k++)
                    {
                        //重新采样，取一部分点进行绘图，如果把所有点都用来绘图会导致刷新速度变慢。
                        chart1.Series[k].Points.AddY(data[NumToRead * k]);//只读取每个通道的第一个值
                        //label3.Text = data[NumToRead * k].ToString();

                        //实际上所有数据点都毫无丢失地存储了
                        for (int i = 0; i < NumToRead; i++)
                        {
                            f[k, i] = data[NumToRead * k + i];
                        }
                    }
                    dataQueue.Enqueue(f);

                }

                lb_queue.Text = dataQueue.Count.ToString();

                //当点数过多时，清空图表重新绘制
                if (chart1.Series[0].Points.Count > 500)
                    for (int i = 0; i < chart1.Series.Count; i++)
                        chart1.Series[i].Points.Clear();

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chart1.Series.Count; i++)
                chart1.Series[i].Points.Clear();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            stop();
        }

        WebSocket websocket;

        private void button4_Click(object sender, EventArgs e)
        {

            websocket = new WebSocket("ws://localhost:10066/websocket");
            websocket.Opened += Websocket_Opened;
            websocket.Error += Websocket_Error;
            websocket.Closed += Websocket_Closed;
            websocket.MessageReceived += Websocket_MessageReceived;
            websocket.Open();

        }

        private void Websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Debug.WriteLine("收到:" + e.Message);

        }

        private void Websocket_Closed(object sender, EventArgs e)
        {
            Debug.WriteLine("连接关闭");

        }

        private void Websocket_Opened(object sender, EventArgs e)
        {
            Debug.WriteLine("连接成功");
        }

        private void Websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Debug.WriteLine("出错:" + e.Exception.Message);

        }


    }
}
