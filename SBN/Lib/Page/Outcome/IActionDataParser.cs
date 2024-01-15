using SBN.Lib.Action.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace SBN.Lib.Page.Outcome
{
    public interface IActionDataParser
    {
        ActionData ParseSingleAction(
            DataSet actionDataSet, 
            string definition, 
            string transform,
            Guid sessionToken);
    }
}