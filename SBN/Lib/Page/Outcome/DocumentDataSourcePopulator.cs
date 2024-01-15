using SBN.Lib.Action;
using SBN.Lib.Sys;
using SBN.Lib.Xml.XPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SBN.Lib.Page.Outcome
{
    public class DocumentDataSourcePopulator : IDocumentDataSourcePopulator
    {
        private readonly IApiActionDbCaller _apiActionDbCaller;
        private readonly ISessionTokenAccessor _sessionTokenAccessor;
        private readonly IXPathFacade _xpathFacade;

        public DocumentDataSourcePopulator(IApiActionDbCaller apiActionDbCaller,
            ISessionTokenAccessor sessionTokenAccessor,
            IXPathFacade xpathFacade)
        {
            _apiActionDbCaller = apiActionDbCaller;
            _sessionTokenAccessor = sessionTokenAccessor;
            _xpathFacade = xpathFacade;
        }

        public async IAsyncEnumerable<PageDocument> Populate(IEnumerable<PageDocument> pageDocuments)
        {
            foreach (var pageDocument in pageDocuments)
            {
                XElement element;

                do
                {
                    element = _xpathFacade
                        .SelectElements(pageDocument.Document, "//*[@DataSource][1]")
                        .FirstOrDefault();

                    if (element == null) break;

                    var dataSource = element.Attribute("DataSource").Value;
                    dataSource = dataSource.Replace($"{{{element.Name}}}", element.Value);

                    try
                    {
                        var result = await _apiActionDbCaller.Call(_sessionTokenAccessor.SessionToken(),
                            dataSource,
                            "GET",
                            "/",
                            decorateForJson: false,
                            null);

                        if (!result.Body.StatusCode.IsSuccessStatusCode())
                        {
                            throw new RestCallException(dataSource, "GET", result.Body.StatusCode, result.Body.Message);
                        }

                        if (element.Name == result.Body.Data.Name)
                        {
                            var elementReference = element.Value;
                            result.Body.Data.Add(new XAttribute($"__{element.Name}", elementReference));
                            element.ReplaceWith(result.Body.Data);
                        }
                        else
                        {
                            element.RemoveAttributes();
                            element.Value = "";
                            element.Add(result.Body.Data);
                        }
                    }
                    catch (RestCallException rex)
                    {
                        throw new Exception($"Error populating datasource from '{rex.Uri}': {rex.Message}", rex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error populating datasource: {ex.Message}", ex);
                    }
                } while (element != null);

                yield return pageDocument;
            }
        }
    }
}
