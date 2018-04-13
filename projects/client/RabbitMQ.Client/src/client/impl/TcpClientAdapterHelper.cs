using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace RabbitMQ.Client
{
    public static class TcpClientAdapterHelper
    {
#if NET35
        public static IPAddress GetMatchingHost(ICollection<IPAddress> addresses, AddressFamily addressFamily)
#else
        public static IPAddress GetMatchingHost(IReadOnlyCollection<IPAddress> addresses, AddressFamily addressFamily)
#endif
        {
            var ep = addresses.FirstOrDefault(a => a.AddressFamily == addressFamily);
            if (ep == null && addresses.Count == 1 && addressFamily == AddressFamily.Unspecified)
            {
                return addresses.Single();
            }
            return ep;
        }
    }
}
