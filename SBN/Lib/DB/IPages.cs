using System;
using System.Data;
using System.Threading.Tasks;

namespace SBN.Lib.DB
{
    public interface IPages
    {
        Task<DataSet> Get(Guid sessionToken, string pathAndQuery, string referrer);
    }
}