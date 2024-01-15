using System;
using System.Xml.Linq;

namespace SBN.Lib.Action.Data
{
    public interface IActionDataCache
    {
        void Set(string cacheKey, XElement apiData, DateTimeOffset expiryDateTime);
        bool TryGet(string cacheKey, out XElement apiData);
    }
}