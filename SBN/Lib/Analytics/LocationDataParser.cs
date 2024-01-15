using System;
using System.Xml.Linq;
using Microsoft.SqlServer.Types;
using SBN.Lib.Xml.XPath;

namespace SBN.Lib.Analytics
{
    public class LocationDataParser : ILocationDataParser
    {
        public LocationDataParser(IXPathFacade xpathFacade)
        {
            _xpathFacade = xpathFacade;
        }

        internal const int sqlGeographyGrid = 4326; //https://en.wikipedia.org/wiki/World_Geodetic_System, EPSG:4326, 
        private readonly IXPathFacade _xpathFacade;

        public string Data(XElement data, string item)
        {
            try
            {
                return _xpathFacade.SelectElement(data, $"//geoplugin_{item}").Value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public double DataDbl(XElement data, string item) => double.Parse(_xpathFacade.SelectElement(data, $"//geoplugin_{item}").Value);

        public LocationData Parse(string locationData)
        {
            var locationDataXml = XElement.Parse(locationData);

            var latLong = SqlGeography.Point(DataDbl(locationDataXml, "latitude"), DataDbl(locationDataXml, "longitude"), sqlGeographyGrid);


            return new LocationData
            {
                LatLong = latLong,
                City = Data(locationDataXml, "city"),
                Region = Data(locationDataXml, "region"),
                RegionCode = Data(locationDataXml, "regionCode"),
                RegionName = Data(locationDataXml, "regionName"),
                CountryCode = Data(locationDataXml, "countryCode"),
                CountryName = Data(locationDataXml, "countryName"),
                ContinentCode = Data(locationDataXml, "continentCode"),
                ContinentName = Data(locationDataXml, "continentName"),
                Timezone = Data(locationDataXml, "timezone"),
            };

            /*
             <geoPlugin>
                <geoplugin_request>217.155.107.146</geoplugin_request>
                <geoplugin_status>200</geoplugin_status>
                <geoplugin_delay>2ms</geoplugin_delay>
                <geoplugin_credit>Some of the returned data includes GeoLite data created by MaxMind, available from <a href='http://www.maxmind.com'>http://www.maxmind.com</a>.</geoplugin_credit>
                <geoplugin_city>Hampton</geoplugin_city>
                <geoplugin_region>England</geoplugin_region>
                <geoplugin_regionCode>RIC</geoplugin_regionCode>
                <geoplugin_regionName>Richmond upon Thames</geoplugin_regionName>
                <geoplugin_areaCode></geoplugin_areaCode>
                <geoplugin_dmaCode></geoplugin_dmaCode>
                <geoplugin_countryCode>GB</geoplugin_countryCode>
                <geoplugin_countryName>United Kingdom</geoplugin_countryName>
                <geoplugin_inEU>0</geoplugin_inEU>
                <geoplugin_euVATrate></geoplugin_euVATrate>
                <geoplugin_continentCode>EU</geoplugin_continentCode>
                <geoplugin_continentName>Europe</geoplugin_continentName>
                <geoplugin_latitude>51.4128</geoplugin_latitude>
                <geoplugin_longitude>-0.3772</geoplugin_longitude>
                <geoplugin_locationAccuracyRadius>50</geoplugin_locationAccuracyRadius>
                <geoplugin_timezone>Europe/London</geoplugin_timezone>
                <geoplugin_currencyCode>GBP</geoplugin_currencyCode>
                <geoplugin_currencySymbol>&#163;</geoplugin_currencySymbol>
                <geoplugin_currencySymbol_UTF8>£</geoplugin_currencySymbol_UTF8>
                <geoplugin_currencyConverter>0.7346</geoplugin_currencyConverter>
            </geoPlugin>
            */
        }
    }
}
