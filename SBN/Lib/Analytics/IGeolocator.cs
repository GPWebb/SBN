using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SBN.Lib.Analytics
{
    public interface IGeolocator
    {
        void Set(Guid sessionToken, HttpContext context);
    }
}
