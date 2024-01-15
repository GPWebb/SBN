using System.Data;
using SBN.Models;

namespace SBN.Lib.Action.Outcome
{
    public interface IOutcomeBuilderXmlPopulator
    {
        //bool PopulateXmlResponse(DataSet outcomeData, bool populated, ApiActionOutcome apiActionOutcome);
        bool PopulateXmlResponse(DataTable inputData, bool populated, Response response);
    }
}