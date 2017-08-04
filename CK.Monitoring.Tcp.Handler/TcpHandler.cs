using System;
using CK.Core;
using CK.ControlChannel.Tcp;
using System.IO;
using System.Text;

namespace CK.Monitoring.Tcp.Handler
{
    public class TcpHandler : IGrandOutputHandler
    {
        private TcpHandlerConfiguration _config;
        private ControlChannelClient _client;

        public TcpHandler( TcpHandlerConfiguration config )
        {
            _config = config ?? throw new ArgumentNullException( nameof( config ) );
        }

        public bool Activate( IActivityMonitor m )
        {
            if( _client == null )
            {
                _client = new ControlChannelClient(
                    _config.Host,
                    _config.Port,
                    _config.BuildAuthData(),
                    _config.IsSecure,
                    _config.RemoteCertificateValidationCallback,
                    _config.LocalCertificateSelectionCallback,
                    _config.ConnectionRetryDelayMs
                    );

                if( _config.HandleSystemActivityMonitorErrors )
                {
                    SystemActivityMonitor.OnError += SystemActivityMonitor_OnError;
                }

                _client.OpenAsync( m ).GetAwaiter().GetResult();
            }
            return true;
        }

        private void SystemActivityMonitor_OnError( object sender, SystemActivityMonitor.LowLevelErrorEventArgs e )
        {
            _client.SendAsync( "SystemActivityMonitor.Error", GenerateLowLevelErrorPayload( e ) );
        }

        private static byte[] GenerateLowLevelErrorPayload( SystemActivityMonitor.LowLevelErrorEventArgs e )
        {
            using( MemoryStream ms = new MemoryStream() )
            using( CKBinaryWriter bw = new CKBinaryWriter( ms, Encoding.UTF8, true ) )
            {
                bw.Write( e.ErrorMessage );
                return ms.ToArray();
            }
        }

        public bool ApplyConfiguration( IActivityMonitor m, IHandlerConfiguration c )
        {
            return false;
        }

        public void Deactivate( IActivityMonitor m )
        {
            if( _client != null )
            {
                _client.Dispose();
                _client = null;
            }
        }

        public void Handle( GrandOutputEventInfo logEvent )
        {
            using( MemoryStream ms = new MemoryStream() )
            using( CKBinaryWriter bw = new CKBinaryWriter( ms, Encoding.UTF8, true ) )
            {
                logEvent.Entry.WriteLogEntry( bw );
                _client.SendAsync( "GrandOutputEventInfo", ms.ToArray() ).GetAwaiter().GetResult();
            }
        }

        public void OnTimer( TimeSpan timerSpan )
        {
        }
    }
}
