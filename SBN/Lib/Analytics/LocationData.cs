using Microsoft.SqlServer.Types;

namespace SBN.Lib.Analytics
{
    public class LocationData
    {
		public SqlGeography LatLong { get; set; }
		public string City { get; set; }
		public string Region { get; set; }
		public string RegionCode { get; set; }
		public string RegionName { get; set; }
		public string CountryCode { get; set; }
		public string CountryName { get; set; }
		public string ContinentCode { get; set; }
		public string ContinentName { get; set; }
		public string Timezone { get; set; }
	}
}