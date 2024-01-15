using System;
using Microsoft.AspNetCore.Http;

namespace SBN.Lib.Sys
{
    public static class SessionExtensions
    {
        //HACK Shouldn't be necessary but I suddenly started getting a random bug setting session values.
        //They're an optimisation anyway so leave this for now and do the more important jobs but fix one day.
        public static bool TrySetString(this ISession session, string key, string value)
        {
            try
            {
                session.SetString(key, value);
                return true;
            }
            catch (Exception ex)
            {
                _ = ex;

                return false;
            }
        }
    }
}
