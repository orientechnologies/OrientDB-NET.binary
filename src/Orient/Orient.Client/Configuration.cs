namespace Orient.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Contains global configurations for the driver.
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// The recieve timeout
        /// </summary>
        private static int timeout;

        /// <summary>
        /// Initializes the <see cref="Configuration"/> class.
        /// </summary>
        static Configuration()
        {
            timeout = 60 * 1000;
            RetryCount = 0;
        }

        /// <summary>
        /// Gets or sets the receive timeout for the underlying TCP connection in milliseconds.
        /// </summary>
        /// <value>
        /// The timeout in msec.
        /// </value>
        public static int Timeout
        {
            get
            {
                return timeout;
            }

            set
            {
                timeout = value;
                OClient.DropAndEstablishConnectionsInAllPools();
            }
        }

        /// <summary>
        /// Gets or sets the retry count. If an operation fails with IOException it will be retried this many times.
        /// </summary>
        /// <value>
        /// The retry count for operations.
        /// </value>
        public static int RetryCount { get; set; }
    }
}
