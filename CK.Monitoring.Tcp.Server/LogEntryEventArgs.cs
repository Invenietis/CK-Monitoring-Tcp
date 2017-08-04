using CK.ControlChannel.Abstractions;
using System;

namespace CK.Monitoring.Tcp.Server
{
    public class LogEntryEventArgs : EventArgs
    {
        public ILogEntry Entry { get; }
        public IServerClientSession Client { get; }

        public LogEntryEventArgs( ILogEntry entry, IServerClientSession client )
        {
            Entry = entry;
            Client = client;
        }
    }
}