using System;

namespace VERSUS.Core
{
    public class VersusOptions
    {
        public string ConnectionString { get; set; }

        public TimeSpan CommandTimeout { get; set; }

        public string ErrorHandlingRoute { get; set; }
    }
}