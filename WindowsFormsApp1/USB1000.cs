using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    /// <summary>
    /// USB1000采集卡库函数
    /// Written By 杨宇庆
    /// </summary>
    public static class USB1000
    {

        public const int NO_USBDAQ = -1;
        public const int DevIndex_Overflow = -2;
        public const int Bad_Firmware = -3;
        public const int USBDAQ_Closed = -4;
        public const int Transfer_Data_Fail = -5;
        public const int NO_Enough_Memory = -6;
        public const int Time_Out = -7;
        public const int Not_Reading = -8;
        public enum AiRange
        {
            /// <summary>
            /// -5V ~ +5V
            /// </summary>
            V5 = 5,
            /// <summary>
            /// 0V ~ 10V
            /// </summary>
            V10 = 10
        }
        public enum ChannelMode
        {
            DIFF = 0,
            NRSE = 1,
            RSE = 3
        }
        [DllImport("usb-1000.dll", EntryPoint = "FindUSBDAQ", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int FindUSBDAQ();
        [DllImport("usb-1000.dll", EntryPoint = "OpenDevice", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int OpenDevice(int DevIndex);
        [DllImport("usb-1000.dll", EntryPoint = "ResetDevice", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int ResetDevice(int DevIndex);
        [DllImport("usb-1000.dll", EntryPoint = "CloseDevice", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void CloseDevice(int DevIndex);
        [DllImport("usb-1000.dll", EntryPoint = "SetUSB1AiRange", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetUSB1AiRange(int DevIndex, float Range);
        [DllImport("usb-1000.dll", EntryPoint = "SetSampleRate", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetSampleRate(int DevIndex, int SampleRate);
        [DllImport("usb-1000.dll", EntryPoint = "SetChanSel", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetChanSel(int DevIndex, short ChSel);
        [DllImport("usb-1000.dll", EntryPoint = "SetChanMode", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetChanMode(int DevIndex, byte ChanMode);
        [DllImport("usb-1000.dll", EntryPoint = "SetSoftTrig", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetSoftTrig(int DevIndex, byte Trig);
        [DllImport("usb-1000.dll", EntryPoint = "SetDioOut", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetDioOut(int DevIndex, int DioOut);

        [DllImport("usb-1000.dll", EntryPoint = "TransDioIn", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int TransDioIn(int DevIndex, byte TransDioSwitch);

        [DllImport("usb-1000.dll", EntryPoint = "SetCounter", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetCounter(int DevIndex, byte CtrNum, byte CtrMode, byte CtrEdge);

        [DllImport("usb-1000.dll", EntryPoint = "StartCounter", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int StartCounter(int DevIndex, byte CtrNum, byte OnOff);

        [DllImport("usb-1000.dll", EntryPoint = "InitDA", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int InitDA(int DevIndex);
        [DllImport("usb-1000.dll", EntryPoint = "SetDA", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetDA(int DevIndex, byte DANum, float DAVolt);

        [DllImport("usb-1000.dll", EntryPoint = "SetWavePt", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWavePt(int DevIndex, byte DANum, float DAVolt);

        [DllImport("usb-1000.dll", EntryPoint = "ClrWavePt", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int ClrWavePt(int DevIndex, byte DANum);

        [DllImport("usb-1000.dll", EntryPoint = "SetWaveSampleRate", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWaveSampleRate(int DevIndex, int WaveSampleRate);


        [DllImport("usb-1000.dll", EntryPoint = "WaveOutput", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int WaveOutput(int DevIndex, byte DANum);

        [DllImport("usb-1000.dll", EntryPoint = "ClearBufs", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int ClearBufs(int DevIndex);

        [DllImport("usb-1000.dll", EntryPoint = "ClearCounter", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int ClearCounter(int DevIndex, byte CtrNum);

        [DllImport("usb-1000.dll", EntryPoint = "StartRead", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int StartRead(int DevIndex);
        [DllImport("usb-1000.dll", EntryPoint = "StopRead", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int StopRead(int DevIndex);

        [DllImport("usb-1000.dll", EntryPoint = "GetAiChans", CharSet = CharSet.Auto, CallingConvention = CallingConvention.ThisCall)]
        public static extern int GetAiChans(int DevIndex, int Num, short ChSel, float[] data, long TimeOut);
        [DllImport("usb-1000.dll", EntryPoint = "GetDioIn", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetDioIn(int DevIndex);
        [DllImport("usb-1000.dll", EntryPoint = "GetCounter", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetCounter(int DevIndex, byte CtrNum);
        [DllImport("usb-1000.dll", EntryPoint = "GetCtrTime", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern double GetCtrTime(int DevIndex, byte CtrNum);
        [DllImport("usb-1000.dll", EntryPoint = "GotoCalibrate", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GotoCalibrate(int DevIndex, int Code);
        [DllImport("usb-1000.dll", EntryPoint = "WriteCali", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteCali(int DevIndex, int Addr, char[] Data, int Code);



        /*
         
int _stdcall FindUSBDAQ();
int _stdcall OpenDevice(int DevIndex);
int _stdcall ResetDevice(int DevIndex);
void _stdcall CloseDevice(int DevIndex);
int _stdcall SetUSB1AiRange(int DevIndex, float Range);
int _stdcall SetSampleRate(int DevIndex, unsigned int SampleRate);
int _stdcall SetChanSel(int DevIndex, unsigned short ChSel);
int _stdcall SetChanMode(int DevIndex, unsigned char ChanMode);
int _stdcall SetSoftTrig(int DevIndex, unsigned char Trig);
int _stdcall SetDioOut(int DevIndex, unsigned int DioOut);
int _stdcall TransDioIn(int DevIndex, unsigned char TransDioSwitch);
int _stdcall SetCounter(int DevIndex, unsigned char CtrNum, unsigned char CtrMode, unsigned char CtrEdge);
int _stdcall StartCounter(int DevIndex, unsigned char CtrNum, unsigned char OnOff);
int _stdcall InitDA(int DevIndex);
int _stdcall SetDA(int DevIndex, unsigned char DANum, float DAVolt);
int _stdcall SetWavePt(int DevIndex, unsigned char DANum, float DAVolt);
int _stdcall ClrWavePt(int DevIndex, unsigned char DANum);
int _stdcall SetWaveSampleRate(int DevIndex, unsigned int WaveSampleRate);
int _stdcall WaveOutput(int DevIndex, unsigned char DANum);
int _stdcall ClearBufs(int DevIndex);
int _stdcall ClearCounter(int DevIndex, unsigned char CtrNum);
int _stdcall StartRead(int DevIndex);
int _stdcall StopRead(int DevIndex);
int _stdcall GetAiChans(int DevIndex, unsigned long Num, unsigned short ChSel, float *Ai, long TimeOut);
unsigned int _stdcall GetDioIn(int DevIndex);
unsigned int _stdcall GetCounter(int DevIndex, unsigned char CtrNum);
double _stdcall GetCtrTime(int DevIndex, unsigned char CtrNum);
int _stdcall GotoCalibrate(int DevIndex, int Code);
int _stdcall WriteCali(int DevIndex, int Addr, unsigned char Data[8], int Code);
         **/
    }
}
