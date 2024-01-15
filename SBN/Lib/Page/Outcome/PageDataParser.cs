using Microsoft.AspNetCore.Http;
using SBN.Lib.DB.Lib;
using SBN.Lib.Page.Request;
using System;
using System.Data;
using System.Linq;

namespace SBN.Lib.Page.Outcome
{
    public partial class PageDataParser : IPageDataParser
    {
        private const int EarliestActionDataStart = 5;
        private const int DocumentsTables = 2;

        private readonly IPageDocumentParser _pageDocumentParser;
        private readonly IDocumentTypeResponseParser _documentTypeResponseParser;
        private readonly IActionDataSetParser _actionDataParser;
        private readonly IPageTable _pageTable;

        public PageDataParser(IPageDocumentParser pageDocumentParser,
            IDocumentTypeResponseParser documentTypeResponseParser,
            IActionDataSetParser actionDataParser,
            IPageTable pageTable)
        {
            _pageDocumentParser = pageDocumentParser;
            _documentTypeResponseParser = documentTypeResponseParser;
            _actionDataParser = actionDataParser;
            _pageTable = pageTable;
        }

        public PageData Parse(DataSet pageDataSet, HttpContext context, Guid sessionToken)
        {
            var pageData = new PageData();

            pageData.Title = _pageTable.Read(pageDataSet, PageDataTables.Header)
                .Rows[0]
                .Field<string>("PageTitle");

            pageData.CacheExpiry = _pageTable.Read(pageDataSet, PageDataTables.Header)
                .Rows[0]
                .Field<DateTime?>("CacheExpiry");

            pageData.ProfileData = _pageTable.Read(pageDataSet, PageDataTables.Profile)
                .Rows[0];

            pageData.Template = _pageTable.Read(pageDataSet, PageDataTables.Header)
                .Rows[0]
                .XmlField("Template");

            pageData.PageReferences = _pageTable.Read(pageDataSet, PageDataTables.Header)
                .Rows[0]
                .Field<string>("Reference");

            pageData.PageParts = _pageTable.Read(pageDataSet, PageDataTables.Part)
                .Select()
                .Select(p => new PagePart(p))
                .ToList();

            var requestParameters = _pageTable.Read(pageDataSet, PageDataTables.Parameters)
                .Select()
                .Select(p => new ValidatedRequestParam(p.Field<string>("ParamName"),
                    p.Field<string>("ParamType"),
                    p.Field<string>("DefaultValue"),
                    p.Field<decimal?>("MinValue"),
                    p.Field<decimal?>("MaxValue"),
                    p.Field<bool>("RequiredField"),
                    p.Field<string>("Value")))
                .ToList();

            pageData.ValidatedRequest = new ValidatedRequest(requestParameters, context, usePost: false);

            pageData.PagePermissions = _pageTable.Read(pageDataSet, PageDataTables.Permissions)
                    .Select()
                    .Select(pm => pm.Field<string>("PermissionName"));

            pageData.PageHasDocuments = _pageTable.Read(pageDataSet, PageDataTables.Document)
                    .Columns[0]
                    .Caption == "DocumentTypeID";

            if (pageData.PageHasDocuments)
            {
                pageData.PageDocuments = _pageDocumentParser.Parse(_pageTable.Read(pageDataSet, PageDataTables.Document));
                pageData.DocumentTypeResponse = _documentTypeResponseParser.Parse(_pageTable.Read(pageDataSet, PageDataTables.DocumentTypeResponse));
            }

            pageData.ActionData = _actionDataParser.ParseActionData(pageDataSet,
                EarliestActionDataStart + (pageData.PageHasDocuments ? DocumentsTables : 0),
                sessionToken);

            return pageData;
        }
    }
}
