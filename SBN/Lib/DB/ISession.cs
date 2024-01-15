using System;
using System.Threading.Tasks;

namespace SBN.Lib.DB
{
    public interface ISession
    {
        Task<SessionDetails> SessionDetails(Guid sessionToken);
    }
}