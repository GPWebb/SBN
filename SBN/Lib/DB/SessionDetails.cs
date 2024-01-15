using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SBN.Lib.DB
{
    public class SessionDetails
    {
        public DataRow ProfileData { get; set; }

        public IEnumerable<string> Permissions { get; set; }

        public SessionDetails()
        { }

        public SessionDetails(DataSet sessionData)
        {
            ProfileData = sessionData.Tables[0].Rows[0];

            Permissions = sessionData.Tables[1].Select().Select(x => x.Field<string>("PermissionName"));
        }
    }
}