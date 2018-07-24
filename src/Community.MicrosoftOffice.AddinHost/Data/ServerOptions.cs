// © Alexander Kozlenko. Licensed under the MIT License.

namespace Community.MicrosoftOffice.AddinHost.Data
{
    /// <summary>Provides the server options.</summary>
    internal sealed class ServerOptions
    {
        /// <summary>Initializes a new instance of the <see cref="ServerOptions" /> class.</summary>
        public ServerOptions()
        {
        }

        /// <summary>Gets or sets the logging file path.</summary>
        public string LoggingFilePath
        {
            get;
            set;
        }
    }
}