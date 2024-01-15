using System.Collections.Generic;
    
namespace SBN
{
    public class EnvironmentConfig
    {
        public bool IndividualUnauthenticatedSessions { get; set; }

        public int SessionCookieExpiryMinutes { get; set; }

        public bool PageReturnData { get; set; }

        public IEnumerable<Environment> Environments { get; set; }

        public class Environment
        {
            public string Name { get; set; }
            public IEnumerable<string> Hosts { get; set; }
            public string DataSource { get; set; }
            public string InitialCatalog { get; set; }
            public string UserID { get; set; }
            public string Password { get; set; }
        }
    }
}
