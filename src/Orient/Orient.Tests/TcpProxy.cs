using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orient.Tests
{
    public class TcpProxy : IDisposable
    {
        private Socket MainSocket;

        private List<Socket> Sockets = new List<Socket>();

        public int Forwarding { get; private set; }

        private int toPort;

        public TcpProxy()
        {
            MainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public TcpProxy(int fromPort, int toPort = 2424)
        {
            Forwarding = fromPort;
            this.toPort = toPort;
            Start();
        }

        public void Start()
        {       
            MainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            MainSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), Forwarding));
            MainSocket.Listen(10);

            Task.Run(() =>
                {
                    try
                    {
                        while (true)
                        {
                            var source = MainSocket.Accept();
                            Sockets.Add(source);
                            var destination = new TcpProxy();
                            Sockets.Add(destination.MainSocket);
                            var state = new State(source, destination.MainSocket);
                            destination.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), toPort), source);
                            source.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);
                        }
                    }
                    catch
                    {

                    }
                });
        }

        public void End()
        {
            foreach (var socket in Sockets)
                if (socket != null)
                    socket.Dispose();
            if (MainSocket != null)
                MainSocket.Dispose();
        }

        private void Connect(EndPoint remoteEndpoint, Socket destination)
        {
            var state = new State(MainSocket, destination);
            MainSocket.Connect(remoteEndpoint);
            MainSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnDataReceive, state);
        }

        private static void OnDataReceive(IAsyncResult result)
        {
            var state = (State)result.AsyncState;
            try
            {
                var bytesRead = state.SourceSocket.EndReceive(result);
                if (bytesRead > 0)
                {
                    state.DestinationSocket.Send(state.Buffer, bytesRead, SocketFlags.None);
                    state.SourceSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);
                }
            }
            catch
            {
                state.DestinationSocket.Close();
                state.SourceSocket.Close();
            }
        }

        private class State
        {
            public Socket SourceSocket { get; private set; }

            public Socket DestinationSocket { get; private set; }

            public byte[] Buffer { get; private set; }

            public State(Socket source, Socket destination)
            {
                SourceSocket = source;
                DestinationSocket = destination;
                Buffer = new byte[8192];
            }
        }

        public void Dispose()
        {
            End();
        }
    }
}

