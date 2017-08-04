using CK.ControlChannel.Abstractions;
using System;

namespace CK.Monitoring.Tcp.Server
{
    public class LowLevelErrorEventArgs : EventArgs
    {
        public string LowLevelError { get; }
        public IServerClientSession Client { get; }

        public LowLevelErrorEventArgs( string lowLevelError, IServerClientSession client )
        {
            LowLevelError = lowLevelError;
            Client = client;
        }
    }
}