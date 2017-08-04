using CK.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Reflection;

namespace CK.Monitoring.Tcp.Handler
{
    public class TcpHandlerConfiguration : IHandlerConfiguration
    {
        /// <summary>
        /// Hostname of the CK.Monitoring.Tcp server
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port of the CK.Monitoring.Tcp server
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// True if the connection with the CK.Monitoring.Tcp server
        /// is secured with SSL/TLS; otherwise false.
        /// </summary>
        public bool IsSecure { get; set; }

        /// <summary>
        /// The callback used by <see cref="SslStream"/> to select a client certificate.
        /// If null: no client certificate will be proposed.
        /// This property is only used if <see cref="IsSecure"/> is true.
        /// </summary>
        public LocalCertificateSelectionCallback LocalCertificateSelectionCallback { get; set; }

        /// <summary>
        /// The callback used by <see cref="SslStream"/> to validate the certificate
        /// presented by the CK.Monitoring.Tcp server.
        /// If null: the certificate will be validated with the system's default validation.
        /// This property is only used if <see cref="IsSecure"/> is true.
        /// </summary>
        public RemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; set; }

        /// <summary>
        /// The minimum delay before trying to open a connection with the CK.Monitoring.Tcp server
        /// after a connection failed, in milliseconds.
        /// </summary>
        public int ConnectionRetryDelayMs { get; set; } = 10 * 1000;

        /// <summary>
        /// The name of the application, as presented to the CK.Monitoring.Tcp server.
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// The name of the client, as presented to the CK.Monitoring.Tcp server.
        /// Defaults to the host name of the local computer.
        /// </summary>
        public string ClientName { get; set; } = Dns.GetHostName();

        /// <summary>
        /// Additional authentication data, as presented to the CK.Monitoring.Tcp server.
        /// </summary>
        public Dictionary<string, string> AdditionalAuthenticationData { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// True if the authentication data presented to the CK.Monitoring.Tcp server
        /// should include assembly information about this assembly and CK.Monitoring's assembly.
        /// </summary>
        public bool PresentMonitoringAssemblyInformation { get; set; }

        /// <summary>
        /// True if the authentication data presented to the CK.Monitoring.Tcp server
        /// should include all environment variables available to the running process.
        /// </summary>
        public bool PresentEnvironmentVariables { get; set; }

        /// <summary>
        /// True if low-level <see cref="SystemActivityMonitor"/> errors should be sent.
        /// </summary>
        public bool HandleSystemActivityMonitorErrors { get; set; }

        public IHandlerConfiguration Clone()
        {
            return new TcpHandlerConfiguration()
            {
                Host = Host,
                Port = Port,
                IsSecure = IsSecure,
                LocalCertificateSelectionCallback = LocalCertificateSelectionCallback,
                RemoteCertificateValidationCallback = RemoteCertificateValidationCallback,
                ConnectionRetryDelayMs = ConnectionRetryDelayMs,
                AppName = AppName,
                ClientName = ClientName,
                AdditionalAuthenticationData = AdditionalAuthenticationData,
                PresentMonitoringAssemblyInformation = PresentMonitoringAssemblyInformation,
                PresentEnvironmentVariables = PresentEnvironmentVariables,
                HandleSystemActivityMonitorErrors = HandleSystemActivityMonitorErrors,
            };
        }

        internal IReadOnlyDictionary<string, string> BuildAuthData()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if( AdditionalAuthenticationData != null )
            {
                foreach( var kvp in AdditionalAuthenticationData )
                {
                    dict.Add( kvp.Key, kvp.Value );
                }
            }

            dict["AppName"] = AppName;
            dict["ClientName"] = ClientName;
            dict["LogEntryVersion"] = LogReader.CurrentStreamVersion.ToString();

            if( PresentMonitoringAssemblyInformation )
            {
                AddAssemblyInformation( dict, typeof( TcpHandlerConfiguration ) );
                AddAssemblyInformation( dict, typeof( GrandOutput ) );
                AddAssemblyInformation( dict, typeof( LogLevel ) );
            }

            if( PresentEnvironmentVariables )
            {
                foreach( DictionaryEntry e in Environment.GetEnvironmentVariables() )
                {
                    dict[$"ENV:{e.Key}"] = e.Value.ToString();
                }
            }

            return dict;
        }

        private static void AddAssemblyInformation( Dictionary<string, string> dict, Type t )
        {
            AssemblyName an = t.GetTypeInfo().Assembly.GetName();
            dict[$"ASSEMBLY:{an.Name}"] = an.FullName;
        }

        public void AddAssemblyInformationFromType( Type t )
        {
            if( AdditionalAuthenticationData == null )
            {
                AdditionalAuthenticationData = new Dictionary<string, string>();
            }
            AddAssemblyInformation( AdditionalAuthenticationData, t );
        }
    }
}
