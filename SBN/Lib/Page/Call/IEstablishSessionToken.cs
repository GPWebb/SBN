using System.Threading.Tasks;
using System;

namespace SBN.Lib.Page.Call
{
    public interface IEstablishSessionToken
    {
        Task<Guid> Establish();
    }
}