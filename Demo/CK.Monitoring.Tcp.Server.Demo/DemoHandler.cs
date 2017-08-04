using System;
using CK.ControlChannel.Abstractions;
using CK.Core;

namespace CK.Monitoring.Tcp.Server.Demo
{
    internal class DemoHandler : IAuthorizationHandler
    {
        public bool OnAuthorizeSession( IServerClientSession s )
        {
            IActivityMonitor m = new ActivityMonitor();
            foreach( var kvp in s.ClientData )
            {
                m.Info( $"{kvp.Key}: {kvp.Value}" );
            }
            return true;
        }
    }
}