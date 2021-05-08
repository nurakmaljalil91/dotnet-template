using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SocketServerLib
{
    public class SocketServerLib
    {
        private IPAddress _ipAddress;
        private int _port;
        private TcpListener _tcpListener;
        public bool IsRunning { get; set; }

        public async void Listen(IPAddress ipAddress = null, int port = 23000)
        {
            ipAddress ??= IPAddress.Any;

            if (port <= 0)
            {
                port = 23000;
            }

            _ipAddress = ipAddress;

            _port = port;

            Console.WriteLine($"IP Address: {_ipAddress} - Port: {_port}");

            _tcpListener = new TcpListener(_ipAddress, _port);

            try
            {
                _tcpListener.Start();

                IsRunning = true;

                while (IsRunning)
                {
                    var acceptTcp = await _tcpListener.AcceptTcpClientAsync(); // return tcp client

                    Console.WriteLine($"Client Connect Successfully {acceptTcp}");

                    ProcessTcpClient(acceptTcp);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private async void ProcessTcpClient(TcpClient client)
        {
            try
            {
                var stream = client.GetStream();

                var reader = new StreamReader(stream);

                var buffer = new char[64];

                while (IsRunning)
                {
                    Console.WriteLine("Ready to read");

                    var receive = await reader.ReadAsync(buffer, 0, buffer.Length);

                    Console.WriteLine($"Receive {receive}");

                    if (receive == 0)
                    {
                        Console.WriteLine("Socket disconnected");
                        break;
                    }

                    var receiveText = new string(buffer);

                    Console.WriteLine($"Receive decode {receiveText}");

                    Array.Clear(buffer, 0, buffer.Length);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
