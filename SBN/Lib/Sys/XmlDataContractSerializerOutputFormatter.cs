using System;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;

namespace SBN.Lib.Sys
{
    public class XmlDataContractSerializerOutputFormatter : Microsoft.AspNetCore.Mvc.Formatters.XmlDataContractSerializerOutputFormatter
    {
        //https://stackoverflow.com/questions/38323664/returning-xdocument-from-controller-dotnet-coreclr
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (selectedEncoding == null)
            {
                throw new ArgumentNullException(nameof(selectedEncoding));
            }

            var writerSettings = WriterSettings.Clone();
            writerSettings.Encoding = selectedEncoding;

            // Wrap the object only if there is a wrapping type.
            var value = context.Object;
            var wrappingType = GetSerializableType(context.ObjectType);
            if (wrappingType != null && wrappingType != context.ObjectType)
            {
                var wrapperProvider = WrapperProviderFactories.GetWrapperProvider(new WrapperProviderContext(
                    declaredType: context.ObjectType,
                    isSerialization: true));

                value = wrapperProvider.Wrap(value);
            }

            var dataContractSerializer = GetCachedSerializer(wrappingType);

            using (var textWriter = context.WriterFactory(context.HttpContext.Response.Body, writerSettings.Encoding))
            {
                using (var xmlWriter = CreateXmlWriter(textWriter, writerSettings))
                {
                    // If XDocument, use its own serializer as DataContractSerializer cannot handle XDocuments.
                    if (value is XDocument)
                    {
                        ((XDocument)value).WriteTo(xmlWriter);
                    }
                    else if (value is XElement)
                    {
                        ((XElement)value).WriteTo(xmlWriter);
                    }
                    else
                    {
                        dataContractSerializer.WriteObject(xmlWriter, value);
                    }
                }

                // Perf: call FlushAsync to call WriteAsync on the stream with any content left in the TextWriter's
                // buffers. This is better than just letting dispose handle it (which would result in a synchronous 
                // write).
                await textWriter.FlushAsync();
            }
        }
    }
}
