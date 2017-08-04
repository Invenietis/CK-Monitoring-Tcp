using CK.Core;
using CK.Monitoring.Handlers;
using System;
using System.IO;
using System.Reflection;

namespace CK.Monitoring.Tcp.Server.Demo
{
    class Program
    {
        static void Main( string[] args )
        {
            SetupActivityMonitor();
            ActivityMonitor m = new ActivityMonitor();

            var server = new TcpServer(
                "127.0.0.1",
                33712,
                new DemoHandler()
                );
            server.Open();

            server.OnLowLevelError += Server_OnLowLevelError;
            server.OnGrandOutputEvent += Server_OnGrandOutputEvent;

            bool doContinue = true;
            while( doContinue )
            {
                Console.WriteLine( $"Press q to quit" );
                var k = Console.ReadKey( true );
                switch( k.Key )
                {
                    case ConsoleKey.Q:
                        m.Info( "Goodbye" );
                        doContinue = false;
                        break;
                    default:
                        m.Warn( $"Unknown key {k.Key}" );
                        break;

                }
            }

            server.Dispose();
        }

        private static void Server_OnGrandOutputEvent( object sender, LogEntryEventArgs e )
        {
            IActivityMonitor m = new ActivityMonitor();
            m.Info( e.Entry.Text );
        }

        private static void Server_OnLowLevelError( object sender, LowLevelErrorEventArgs e )
        {
            IActivityMonitor m = new ActivityMonitor();
            m.Info( e.LowLevelError );
        }

        private static void SetupActivityMonitor()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            ActivityMonitor.DefaultFilter = LogFilter.Debug;
            ActivityMonitor.AutoConfiguration += ( monitor ) =>
            {
                monitor.Output.RegisterClient( new ActivityMonitorConsoleClient() );
            };
            SystemActivityMonitor.RootLogPath = GetLogDirectory();
            GrandOutputConfiguration grandOutputConfig = new GrandOutputConfiguration();
            grandOutputConfig.AddHandler( new TextFileConfiguration()
            {
                MaxCountPerFile = 10000,
                Path = "Text",
            } );
            GrandOutput.EnsureActiveDefault( grandOutputConfig );
        }

        static string GetLogDirectory()
        {
            var dllPath = typeof( Program ).GetTypeInfo().Assembly.Location;
            var dllDir = Path.GetDirectoryName( dllPath );
            var logPath = Path.Combine( dllDir, "Logs" );
            if( !Directory.Exists( logPath ) )
            {
                Directory.CreateDirectory( logPath );
            }
            return logPath;
        }
    }
}