using System;
using System.Threading.Tasks;
using SBN.Lib.Analytics;

namespace SBN.Lib.DB
{
    public interface IAnalytics
    {
        Task SetSessionLocation(Guid sessionToken, LocationData locationData);
        Task RecordHit(Guid sessionToken, string context, string pathAndQuery, string referrer, DateTime hitDateTime);
    }
}