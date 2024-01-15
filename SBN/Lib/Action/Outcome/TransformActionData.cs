using SBN.Lib.Action.Data;
using SBN.Lib.Xml.XslTransform;

namespace SBN.Lib.Action.Outcome
{
    public class TransformActionData : ITransformActionData
    {
        private readonly IXmlCompiledTransformer _xmlCompiledTransformer;
        private readonly ITransformCacher _transformCacher;

        public TransformActionData(IXmlCompiledTransformer xmlCompiledTransformer,
            ITransformCacher transformCacher)
        {
            _xmlCompiledTransformer = xmlCompiledTransformer;
            _transformCacher = transformCacher;
        }

        public void Transform(ActionData actionData, ApiActionOutcome apiActionOutcome)
        {
            if (actionData.Transform != null)
            {
                if (apiActionOutcome.Body.Data != null)
                {
                    apiActionOutcome.Body.Data = _xmlCompiledTransformer.Transform(
                        apiActionOutcome.Body.Data, 
                        _transformCacher.GetTransformer(actionData.Transform));
                }
            }
        }
    }
}
