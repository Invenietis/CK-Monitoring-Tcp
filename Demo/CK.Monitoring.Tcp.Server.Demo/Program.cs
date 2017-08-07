using CK.Core;
using CK.Monitoring.Handlers;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Util;
using System;
using System.IO;
using System.Reflection;
using CK.ControlChannel.Abstractions;
using Lucene.Net.Documents;
using static Lucene.Net.Documents.Field;

namespace CK.Monitoring.Tcp.Server.Demo
{
    class Program
    {
        static void Main( string[] args )
        {
            SetupActivityMonitor();
            Program p = new Program();
            p.Run();
            GrandOutput.Default.Dispose();
        }

        public Program()
        {

        }

        IndexWriter _indexWriter;

        public void Run()
        {
            ActivityMonitor m = new ActivityMonitor();

            // Setup Lucene
            var lucenePath = EnsureDirectory( Path.Combine( GetAssemblyDirectory(), "Lucene" ) );
            StandardAnalyzer analyzer = new StandardAnalyzer( LuceneVersion.LUCENE_48 );
            IndexWriterConfig config = new IndexWriterConfig( LuceneVersion.LUCENE_48, analyzer );

            using( Lucene.Net.Store.Directory index = Lucene.Net.Store.FSDirectory.Open( lucenePath ) )
            using( _indexWriter = new IndexWriter( index, config ) )
            using( var server = new TcpServer(
            "127.0.0.1",
            33712,
            new DemoHandler()
            ) )
            {
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

            }

        }

        void Server_OnGrandOutputEvent( object sender, LogEntryEventArgs e )
        {
            IActivityMonitor m = new ActivityMonitor();
            m.Info( e.Entry.Text );
            AddDocument( e.Entry, e.Client );
        }

        private void AddDocument( ILogEntry entry, IServerClientSession client )
        {
            Document doc = new Document
            {
                new StringField( "DocumentType", "ILogEntry", Store.NO ),
                new StringField( "LogType", entry.LogType.ToString(), Store.YES ),
                new StringField( "LogTime", entry.LogTime.ToIndexableString(), Store.YES ),
                new StringField( "LogLevel", entry.LogLevel.ToString(), Store.YES ),
                new StringField( "SessionId", client.SessionId, Store.YES ),
                new StringField( "AppName", client.ClientData["AppName"], Store.YES ),
                new StringField( "ClientName", client.ClientData["ClientName"], Store.YES )
            };

            if( entry.FileName != null ) { doc.Add( new StringField( "FileName", entry.FileName.ToString(), Store.YES ) ); }
            if( entry.LineNumber > 0 ) { doc.Add( new Int32Field( "LineNumber", entry.LineNumber, Store.YES ) ); }
            if( entry.Text != null ) { doc.Add( new TextField( "Text", entry.Text, Store.YES ) ); }
            if( entry.Tags != ActivityMonitor.Tags.Empty ) { doc.Add( new TextField( "Tags", entry.Tags.ToIndexableString(), Store.YES ) ); }
            if( entry.Exception != null ) { doc.Add( new TextField( "Exception", entry.Exception.ToIndexableString(), Store.YES ) ); }
            if( entry.Conclusions != null ) { doc.Add( new TextField( "Conclusions", entry.Conclusions.ToIndexableString(), Store.YES ) ); }

            _indexWriter.AddDocument( doc );
        }

        private void AddLowLevelErrorDocument( string lowLevelError, IServerClientSession client )
        {
            Document doc = new Document
            {
                new StringField( "DocumentType", "LowLevelError", Store.NO ),
                new StringField( "SessionId", client.SessionId, Store.YES ),
                new StringField( "AppName", client.ClientData["AppName"], Store.YES ),
                new StringField( "ClientName", client.ClientData["ClientName"], Store.YES ),
                new StringField( "LogTime", DateTools.DateToString(DateTime.UtcNow, DateTools.Resolution.MILLISECOND), Store.YES ),
                new TextField( "LowLevelError",lowLevelError, Store.YES ),
            };
            _indexWriter.AddDocument( doc );
        }

        void Server_OnLowLevelError( object sender, LowLevelErrorEventArgs e )
        {
            IActivityMonitor m = new ActivityMonitor();
            m.Info( e.LowLevelError );
        }

        static void SetupActivityMonitor()
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
            return EnsureDirectory( Path.Combine( GetAssemblyDirectory(), "Logs" ) );
        }

        static string GetAssemblyDirectory()
        {
            return Path.GetDirectoryName( typeof( Program ).GetTypeInfo().Assembly.Location );
        }

        static string EnsureDirectory( string path )
        {
            if( !Directory.Exists( path ) )
            {
                Directory.CreateDirectory( path );
            }
            return path;
        }
    }
}
