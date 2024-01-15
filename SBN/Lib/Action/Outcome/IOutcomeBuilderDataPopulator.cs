using System.Data;
using System.Xml.Linq;

namespace SBN.Lib.Action.Outcome
{
    public interface IOutcomeBuilderDataPopulator
    {
        void PopulateDataResponse(DataSet outcomeData, XElement outputDefinition, ApiActionOutcome apiActionOutcome, bool decorateForJson);
    }
}