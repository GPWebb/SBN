using System;
using System.Xml.Linq;
using System.Xml.Xsl;
using Microsoft.Extensions.Caching.Memory;

namespace SBN.Lib.Xml.XslTransform
{
    public class TransformCacher : ITransformCacher
    {
        private IMemoryCache _cachedTransforms;
        private readonly IStringHasher _stringHasher;
        private readonly ITransformCompiler _transformCompiler;

        public TransformCacher(IMemoryCache memoryCache,
            IStringHasher stringHasher,
            ITransformCompiler transformCompiler)
        {
            _cachedTransforms = memoryCache;
            _stringHasher = stringHasher;
            _transformCompiler = transformCompiler;
        }

        public XslCompiledTransform GetTransformer(XElement transform)
        {
            var hash = _stringHasher.HashString(transform.ToString());

            if (!_cachedTransforms.TryGetValue(hash, out XslCompiledTransform compiledTransform))
            {
                compiledTransform = _transformCompiler.CompileTransform(transform);

                _cachedTransforms.Set(hash, compiledTransform, TimeSpan.FromMinutes(60));
            }

            return compiledTransform;
        }
    }
}
