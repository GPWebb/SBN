using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SBN.Lib.Login;

namespace SBN.Lib.DB
{
    public interface ILogins
    {
        Task<Guid> GetStartSessionToken(HttpContext httpContext);
        Task<LoginResponse> Login(Guid sessionToken, string username, string password);
        void Logout(Guid sessionToken);
        void EnsureUnauthenticatedSessionLive(Guid sessionToken);
    }
}