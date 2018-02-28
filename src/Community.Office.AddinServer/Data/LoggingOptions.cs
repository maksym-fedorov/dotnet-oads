namespace Community.Office.AddinServer.Data
{
    /// <summary>Provides HTTP server logging options.</summary>
    internal sealed class LoggingOptions
    {
        /// <summary>Initializes a new instance of the <see cref="LoggingOptions" /> class.</summary>
        public LoggingOptions()
        {
        }

        /// <summary>Gets or sets logging file path.</summary>
        public string FilePath
        {
            get;
            set;
        }
    }
}