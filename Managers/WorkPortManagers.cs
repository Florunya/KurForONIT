using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Kur.Models;

namespace Kur.Managers
{
    public class WorkPortManagers : IWorkPortManagers
    {
        private IPGlobalProperties _properties;
        public WorkPortManagers()
        {
            _properties = IPGlobalProperties.GetIPGlobalProperties();
        }

        public List<PortInfo> GetActiveTcpConnections()
        {
            var connections = _properties.GetActiveTcpConnections();
            return connections.Select(p =>
            {
                return new PortInfo(
                    portNumber: p.LocalEndPoint.Port,
                    local: $"{p.LocalEndPoint.Address}:{p.LocalEndPoint.Port}",
                    remote: $"{p.RemoteEndPoint.Address}:{p.RemoteEndPoint.Port}",
                    state: p.State.ToString());
            }).ToList();
        }

        public List<EndPoint> GetActiveTcpListeners()
        {
            var tcpListeners = _properties.GetActiveTcpListeners();
            return tcpListeners.Select(p => {
                return new EndPoint(
                    port: p.Port.ToString(),
                    address: p.Address.ToString(),
                    addressFamily: p.AddressFamily.ToString());
            }).ToList();
        }

        public List<EndPoint> GetActiveUdpListeners()
        {
            var udpListeners = _properties.GetActiveUdpListeners();
            return udpListeners.Select(p =>
            {
                return new EndPoint(
                    port: p.Port.ToString(),
                    address: p.Address.ToString(),
                    addressFamily: p.AddressFamily.ToString());
            }).ToList();
        }
    }
}
