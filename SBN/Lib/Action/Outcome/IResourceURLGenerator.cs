using System.Xml.Linq;
using Microsoft.AspNetCore.Http;

namespace SBN.Lib.Action.Outcome
{
    public interface IResourceURLGenerator
    {
        string Generate(PathString path, IQueryCollection query, string APIRoute, XElement outputParameters);
    }
}