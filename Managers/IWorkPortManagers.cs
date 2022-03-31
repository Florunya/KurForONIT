using System;
using System.Collections.Generic;
using Kur.Models;

namespace Kur.Managers
{
    public interface IWorkPortManagers
    {
        public List<EndPoint> GetActiveTcpListeners();
        public List<PortInfo> GetActiveTcpConnections();
        public List<EndPoint> GetActiveUdpListeners();
    }
}
