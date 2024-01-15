using System.Collections.Generic;
using SBN.Lib.Action.Data;

namespace SBN.Lib.Page.Outcome
{
    public interface IDynamicPagePartPopulator
    {
        IEnumerable<PagePart> Populate(IEnumerable<PagePart> parts, List<ActionData> actionData);
    }
}