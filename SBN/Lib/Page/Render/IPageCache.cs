using System;

namespace SBN.Lib.Page.Render
{
    public interface IPageCache
    {
        void Set(string pathAndQuery, string pageTemplateString, DateTime cacheExpiry);

        string Get(string pathAndQuery);
    }
}