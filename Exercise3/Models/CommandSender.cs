﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;

namespace Exercise3.Models
{
    public class CommandSender
    {
        private IPEndPoint ep;
        private TcpClient client;
        public volatile bool isSenderConnected = false;
        //public volatile bool abort = false; // for immidiate cancel


        // private constructor
        private CommandSender() { }
        
        public string Ip { get; set; }
        public int Port { get; set; }

        #region Singleton
        private static CommandSender m_Instance = null;
        public static CommandSender Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new CommandSender();
                }
                return m_Instance;
            }
        }
        #endregion
        // connect function
        public void connectToServer(string ip, int port)
        {
            //Thread thread = new Thread(() =>
            //{
            client = new TcpClient();
            ep = new IPEndPoint(IPAddress.Parse(ip), port);
            while (!client.Connected)
            {
                    try
                    {
                    client.Connect(ep);
                    }
                    catch
                    {
                    }

                }
                isSenderConnected = true;
                //abort = false;
                Debug.WriteLine("You are connected");
            //});
            //thread.Start();
            //thread.Join();
        }
        

        public string sendAndGetData(string data)
        {
            // Send data to server

            string strValue;
            if (!isSenderConnected)
            {
                return "not connected yet";
            }
            string toSend = data + "\r\n";
            NetworkStream stream = client.GetStream();
            Byte[] bytesSent = Encoding.ASCII.GetBytes(toSend);
            stream.Write(bytesSent, 0, bytesSent.Length);
            StreamReader streamReader = new StreamReader(stream);
            string readFrom = streamReader.ReadLine();
            string res = extractValue(readFrom);
            return res;

        }

        public string extractValue(string strValue)
        {
            string[] extractedValues = strValue.Split('\'');
            string res = extractedValues[1];
            return res;
        }

        // close client function
        public void close()
        {
            client.Close();
            //s.Close();
            isSenderConnected = false;
            Debug.WriteLine("close connection");
            //abort = true;
        }

    }
}