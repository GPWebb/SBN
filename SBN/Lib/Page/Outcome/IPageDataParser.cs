using Microsoft.AspNetCore.Http;
using System;
using System.Data;

namespace SBN.Lib.Page.Outcome
{
    public interface IPageDataParser
    {
        PageData Parse(DataSet pageDataSet, HttpContext context, Guid sessionToken);
    }
}
