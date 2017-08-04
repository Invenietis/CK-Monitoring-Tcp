using System;
using CK.ControlChannel.Abstractions;

namespace CK.Monitoring.Tcp.Server
{
    internal class TcpAuthHandler : IAuthorizationHandler
    {
        public bool OnAuthorizeSession( IServerClientSession s )
        {
            return true;
        }
    }
}