#if !NETFX_CORE
using System;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;

namespace RabbitMQ.Client
{
    /// <summary>
    /// Simple wrapper around TcpClient.
    /// </summary>
    public class TcpClientAdapter : ITcpClient
    {
        protected Socket sock;

        public TcpClientAdapter(Socket socket)
        {
            if (socket == null)
                throw new InvalidOperationException("socket must not be null");

            this.sock = socket;
        }

#if NET35
        public virtual Task ConnectAsync(string host, int port)
#else
        public virtual async Task ConnectAsync(string host, int port)
#endif
        {
            AssertSocket();
#if NET35
            var adds = Dns.GetHostAddresses(host);
#else
           var adds = await Dns.GetHostAddressesAsync(host).ConfigureAwait(false);
#endif
            var ep = TcpClientAdapterHelper.GetMatchingHost(adds, sock.AddressFamily);
            if (ep == default(IPAddress))
            {
                throw new ArgumentException("No ip address could be resolved for " + host);
            }
            #if CORECLR
            await sock.ConnectAsync(ep, port).ConfigureAwait(false);
            #else
            sock.Connect(ep, port);
            #endif
#if NET35
            return TaskEx.FromResult(0);
#endif
        }

        public virtual void Close()
        {
            this.Dispose();
        }

        public virtual void Dispose()
        {
            if (sock != null)
            {
#if NET35
                sock.Close();
#else
                sock.Dispose();
#endif
            }
            sock = null;
        }

        public virtual NetworkStream GetStream()
        {
            AssertSocket();
            return new NetworkStream(sock);
        }

        public virtual Socket Client
        {
            get
            {
                return sock;
            }
        }

        public virtual bool Connected
        {
            get
            {
                if(sock == null) return false;
                return sock.Connected;
            }
        }

        public virtual int ReceiveTimeout
        {
            get
            {
                AssertSocket();
                return sock.ReceiveTimeout;
            }
            set
            {
                AssertSocket();
                sock.ReceiveTimeout = value;
            }
        }

        private void AssertSocket()
        {
            if(sock == null)
            {
                throw new InvalidOperationException("Cannot perform operation as socket is null");
            }
        }
    }
}
#endif
