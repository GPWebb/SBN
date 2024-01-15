using System;

namespace SBN.Lib.Login
{
    public class LoginResponse
    {
        public enum LoginResponseType
        {
            Success,
            SessionMustBeCreated,
            InvalidUsername,
            InvalidPassword,
            PasswordOutdated,
            PasswordMustBeChanged,
            AccountLocked,
            InvalidSessionToken,
            UnknownError
        }

        public LoginResponseType ResultType;
        public DateTime? SessionEnd;
        public DateTime? PasswordExpiredDate;
        public string URL;

        public LoginResponse()
        {
            //Only needed for serialisation....
        }

        public LoginResponse(string result, DateTime? sessionEnd)
        {
            SessionEnd = sessionEnd;
            if (result.Contains("|"))
            {
                ResultType = LoginResponseType.PasswordOutdated;
                PasswordExpiredDate = DateTime.Parse(result.Split("|")[1]);
            }
            else
            {
                ResultType = Enum.Parse<LoginResponseType>(result);
            }
        }
    }
}