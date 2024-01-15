using SBN.Lib.Page.Request;
using System;
using System.Collections.Generic;
using System.Data;

namespace SBN.Lib.Action.Data
{
    public interface IActionDataMapper
    {
        IEnumerable<ActionData> Map(DataTable[] data, Guid sessionToken);
    }
}