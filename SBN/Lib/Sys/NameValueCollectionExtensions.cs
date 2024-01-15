using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace SBN.Lib.Sys
{
    public static class NameValueCollectionExtensions
    {
        //https://stackoverflow.com/questions/391023/make-namevaluecollection-accessible-to-linq-query
        public static IEnumerable<(string name, string value)> ToTuples(this NameValueCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            return
                from key in collection.Cast<string>()
                from value in collection.GetValues(key)
                select (key, value);
        }
    }
}
