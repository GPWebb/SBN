using System.Collections.Generic;
using System.Xml.Linq;

namespace SBN.Lib.Page
{
    public interface IPermissionsApplier
    {
        XElement Apply(IEnumerable<string> permissions, XElement pageTemplate);
    }
}