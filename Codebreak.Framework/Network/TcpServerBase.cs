﻿using Codebreak.Framework.Generic;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Codebreak.Framework.Network
{
    public abstract class TcpServerBase<TServer, TClient> : TaskProcessor<TServer>, IServer<TClient>
        where TServer : TcpServerBase<TServer, TClient>, new()
        where TClient : TcpClientBase<TClient>, new()
    {
        private Socket _socket;
        private ObjectPool<SocketAsyncEventArgs> _sendPool;
        private ObjectPool<SocketAsyncEventArgs> _recvPool;
        private BufferManager _bufferManager;
        private ConcurrentStack<int> _freeId;
        private ConcurrentDictionary<int, TClient> _clients;
        public const int MAX_CLIENT = 10000;

        public string Host
        {
            get;
            private set;
        }

        public int Port
        {
            get;
            private set;
        }

        public int BackLog
        {
            get;
            private set;
        }

        public IEnumerable<TClient> Clients
        {
            get
            {
                return _clients.Values;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="backLog"></param>
        public TcpServerBase(int maxClient = MAX_CLIENT)
            : base(typeof(TServer).Name)
        {
            _bufferManager = new BufferManager(1024, 20000);
            _sendPool = new ObjectPool<SocketAsyncEventArgs>(CreateSendSaea, 10000);
            _recvPool = new ObjectPool<SocketAsyncEventArgs>(CreateRecvSaea, 10000);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clients = new ConcurrentDictionary<int, TClient>();
            _freeId = new ConcurrentStack<int>();
            for (int i = maxClient; i > 0; i--)
                _freeId.Push(i);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private SocketAsyncEventArgs CreateSendSaea()
        {
            var saea = new SocketAsyncEventArgs();
            saea.Completed += IOCompleted;
            return saea;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private PoolableSocketAsyncEventArgs CreateRecvSaea()
        {
            var saea = new PoolableSocketAsyncEventArgs(_bufferManager);
            saea.Completed += IOCompleted;
            return saea;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start(string host, int port, int backLog = 100)
        {
            Host = host;
            Port = port;
            BackLog = backLog;

            _socket.NoDelay = true;
            _socket.Bind(new IPEndPoint(IPAddress.Parse(Host), Port));
            _socket.Listen(BackLog);

            for(int i = 0; i < BackLog; i++)
            {
                StartAccept(null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="arg"></param>
        private void AsyncSafe(Func<SocketAsyncEventArgs, bool> func, SocketAsyncEventArgs arg)
        {
            if (!func(arg))
                IOCompleted(this, arg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="saea"></param>
        private void IOCompleted(object sender, SocketAsyncEventArgs saea)
        {
            switch (saea.LastOperation)
            {
                case SocketAsyncOperation.Accept: ProcessAccepted(saea); break;
                case SocketAsyncOperation.Receive: ProcessReceived(saea); break;
                case SocketAsyncOperation.Send: ProcessSent(saea); break;
                case SocketAsyncOperation.Disconnect: ProcessDisconnected(saea); break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saea"></param>
        private void ProcessDisconnected(SocketAsyncEventArgs saea)
        {
            Disconnect((TClient)saea.UserToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saea"></param>
        private void StartAccept(SocketAsyncEventArgs saea)
        {
            if(saea == null)
            {
                saea = new SocketAsyncEventArgs();
                saea.Completed += IOCompleted;
            }
            else
            {
                saea.AcceptSocket = null;
            }

            AsyncSafe(_socket.AcceptAsync, saea);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saea"></param>
        private void StartReceive(SocketAsyncEventArgs saea, TClient client)
        {
            if (saea == null)
            {
                saea = _recvPool.Pop();
                saea.UserToken = client;
                saea.AcceptSocket = client.Socket;
            }

            AsyncSafe(client.Socket.ReceiveAsync, saea);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private bool AddClient(TClient client)
        {
            int clientId = -1;
            if (!_freeId.TryPop(out clientId))
                return false;
            client.Id = clientId;
            return _clients.AddOrUpdate(clientId, client, (id, cl) => client) == client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saea"></param>
        private void ProcessAccepted(SocketAsyncEventArgs saea)
        {
            // get connected socket
            var socket = saea.AcceptSocket;

            StartAccept(saea);

            // create new client
            var client = new TClient();
            client.Socket = socket;
            client.Ip = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
            client.Server = this;

            // add the client
            if (AddClient(client))
            {
                // start receiving data
                StartReceive(null, client);

                // raise client connection
                OnClientConnected(client);
            }
            else
            {
                // server busy
                Disconnect(client);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saea"></param>
        private void ProcessReceived(SocketAsyncEventArgs saea)
        {
            var client = (TClient)saea.UserToken;

            // client disconnected
            if(saea.BytesTransferred == 0)
            {
                saea.Completed -= IOCompleted;
                Disconnect(client);
                return;
            }
            
            // raise event
            OnDataReceived(client, saea.Buffer, saea.Offset, saea.BytesTransferred);
            
            StartReceive(saea, client);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saea"></param>
        private void ProcessSent(SocketAsyncEventArgs saea)
        {
            saea.SetBuffer(null, 0, 0);
            _sendPool.Push(saea);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        public void Disconnect(TClient client)
        {
            if (client == null)
                return;
            
            var socket = client.Socket;
            
            socket.Shutdown(SocketShutdown.Both);

            if (socket.Connected) 
                socket.Disconnect(false);

            if (client.Id != -1)
            {
                _clients.TryRemove(client.Id, out client);

                if (client != null)
                {
                    _freeId.Push(client.Id);

                    OnClientDisconnected(client);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        public void Send(TClient client, byte[] data)
        {
            if (client == null)
                return;
            
            var saea = _sendPool.Pop();
            saea.SetBuffer(data, 0, data.Length);
            AsyncSafe(client.Socket.SendAsync, saea);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SendToAll(byte[] data)
        {
            foreach (var client in _clients.Values)
                Send(client, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        protected abstract void OnClientConnected(TClient client);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        protected abstract void OnClientDisconnected(TClient client);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        protected abstract void OnDataReceived(TClient client, byte[] buffer, int offset, int count);
    }
}