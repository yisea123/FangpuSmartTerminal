﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace fangpu_terminal
{
    class TerminalCommon
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        private const uint WM_SYSCOMMAND = 0x0112;
        private const int SC_MONITORPOWER = 0xf170;
        /*
        S7-200
        04-S 05-SM 06-AI 07-AQ 1E-C 81-I 82-Q 83-M 184-V 1F-T
        读写函数前三个参数为：存储区类型（如上），相应存储区开始地址，数据长度或数据
        CT为16位数据，其它的可以是8位到32位数据

        S7-1200
        81-I 82-Q 83-M 84-D
        读写函数前三个参数为：存储区类型（如上），相应存储区开始地址，数据长度或数据
        D区调用需要加为DB的那个块，如程序中DB3则为0x84+0x300
        S7S.Get_Cpu_State() 只适用于S7-1200
        */

        public static string S7200AreaS = "S";
        public static string S7200AreaSM = "SM";
        public static string S7200AreaAI = "AI";
        public static string S7200AreaAQ = "AQ";
        public static string S7200AreaC = "C";
        public static string S7200AreaI = "I";
        public static string S7200AreaQ = "Q";
        public static string S7200AreaM = "M";
        public static string S7200AreaV = "V";
        public static string S7200AreaT = "T";
        public static string S7200DataByte = "byte";
        public static string S7200DataBit = "bit";
        public static string S7200DataWord = "word";
        public static string S7200DataDword = "dword";

        public static Dictionary<string, string> warn_info = new Dictionary<string, string>();
        public static string[] warn_stop_info;
        public static string[] demould_info;

        //获取内网IP
        public static string GetInternalIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
        public static bool isgoodnumber(TextBox box, KeyPressEventArgs e)
        {
            try
            {
                int kc = (int)e.KeyChar;
                if ((kc < 48 || kc > 57) && kc != 8 && kc != 46)
                {
                    return true;
                }
                else if (kc == 46)
                {
                    if (box.Text.Length <= 0)
                    {
                        return true;
                    }
                    else
                    {
                        float f;
                        float oldf;
                        bool b1 = false, b2 = false;
                        b1 = float.TryParse(box.Text, out oldf);
                        b2 = float.TryParse(box.Text + e.KeyChar.ToString(), out f);
                        if (b2 == false)
                        {
                            if (b1 == true)
                                return true;
                            else
                                return false;
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return true;
            }

        }
        /// <summary>
        /// Restart the app
        /// </summary>
        /// <param name="terminal"></param>
        public static void AppRestart(FangpuTerminal terminal)
        {
            terminal.AbortAllThread();
            terminal.restartbutton = true;
            TerminalLogWriter.WriteInfoLog(typeof(TerminalCommon), "Application is about to restart...");
            Application.ExitThread();
            Application.Restart();
        }
        /// <summary>
        /// Reboot the system
        /// </summary>
        /// <param name="terminal"></param>
        public static void SystemReboot(FangpuTerminal terminal)
        {
            terminal.AbortAllThread();
            var startinfo = new ProcessStartInfo("shutdown.exe",
                "-r -t 00");
            TerminalLogWriter.WriteInfoLog(typeof(TerminalCommon), "System is about to reboot...");
            Process.Start(startinfo);
           
        }
        /// <summary>
        /// Shutdown the terminal
        /// </summary>
        /// <param name="terminal"></param>
        public static void SystemShutdown(FangpuTerminal terminal)
        {
            terminal.AbortAllThread();
            var startinfo = new ProcessStartInfo("shutdown.exe",
                "-s -t 00");
            TerminalLogWriter.WriteInfoLog(typeof(TerminalCommon), "Terminal is about to shutdown...");
            Process.Start(startinfo);
        }

        public static void ProcessStart(string filepath,string workingdir)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = filepath;
            psi.UseShellExecute = false;
            psi.WorkingDirectory = workingdir;
            psi.CreateNoWindow = true;
            Process.Start(psi);
        }
        public static void TurnOff()
        {
            SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, 2);
        }


    }
}
