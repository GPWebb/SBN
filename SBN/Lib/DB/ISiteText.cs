using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SBN.Lib.DB
{
    public interface ISiteText
    {
        Task<IDictionary<string, string>> Read(Guid sessionToken, IEnumerable<string> siteTextEntries);
    }
}
