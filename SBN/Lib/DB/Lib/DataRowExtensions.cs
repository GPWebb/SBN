using System;
using System.Data;
using System.Xml.Linq;

namespace SBN.Lib.DB.Lib
{
    public static class DataRowExtensions
    {
        public static XElement XmlField(this DataRow dataRow, string field)
        {
            string fieldData = null;

            try
            {
                fieldData = dataRow[field].ToString();

                if (string.IsNullOrWhiteSpace(fieldData)) return null;

                return XElement.Parse(fieldData);
            }
            catch (Exception e)
            {
                throw new Exception($"Cannot read field '{field}': {e.Message}{Environment.NewLine}{fieldData}", e);
            }
        }

        public static XElement XmlField(this DataRow dataRow, int fieldPosition)
        {
            string fieldData = null;

            try
            {
                fieldData = dataRow[fieldPosition].ToString();

                if (string.IsNullOrWhiteSpace(fieldData)) return null;

                return XElement.Parse(fieldData);
            }
            catch (Exception e)
            {
                throw new Exception($"Cannot read field '{fieldPosition}': {e.Message}{Environment.NewLine}{fieldData}", e);
            }
        }
    }
}
