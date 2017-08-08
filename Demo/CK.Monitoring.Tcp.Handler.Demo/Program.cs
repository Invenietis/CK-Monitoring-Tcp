using CK.Core;
using CK.Monitoring.Handlers;
using System;
using System.IO;
using System.Reflection;

namespace CK.Monitoring.Tcp.Handler.Demo
{
    class Program
    {
        static void Main( string[] args )
        {
            SetupActivityMonitor();
            ActivityMonitor m = new ActivityMonitor();
            bool doContinue = true;
            while( doContinue )
            {
                Console.WriteLine( $"Press q to quit, m to send a line, s to send a CriticalError" );
                var k = Console.ReadKey( true );
                switch( k.Key )
                {
                    case ConsoleKey.Q:
                        m.Info( "Goodbye" );
                        doContinue = false;
                        break;
                    case ConsoleKey.M:
                        m.Info( $"Hello world - {DateTime.Now.ToString( "R" )} - {Guid.NewGuid()}" );
                        break;
                    case ConsoleKey.S:
                        new SystemActivityMonitor( false, "Test" ).Error( $"CriticalError - {DateTime.Now.ToString( "R" )}" );
                        break;
                    default:
                        m.Warn( $"Unknown key {k.Key}" );
                        break;

                }
            }

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
            grandOutputConfig.AddHandler( new TcpHandlerConfiguration()
            {
                Host = "localhost",
                Port = 33712,
                IsSecure = false,
                AppName = typeof( Program ).GetTypeInfo().Assembly.GetName().Name,
                PresentEnvironmentVariables = true,
                PresentMonitoringAssemblyInformation = true,
                HandleSystemActivityMonitorErrors = true,
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
