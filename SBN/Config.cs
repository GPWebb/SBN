using AlpineRed.DB;
using Microsoft.Extensions.DependencyInjection;
using SBN.Lib;
using SBN.Lib.Action;
using SBN.Lib.Action.Data;
using SBN.Lib.Action.Data.Serialise;
using SBN.Lib.Action.JsonConvert;
using SBN.Lib.Action.Outcome;
using SBN.Lib.Action.Output;
using SBN.Lib.Analytics;
using SBN.Lib.Asset;
using SBN.Lib.Asset.Process;
using SBN.Lib.DB;
using SBN.Lib.DB.Lib;
using SBN.Lib.Drawing;
using SBN.Lib.IO;
using SBN.Lib.Page;
using SBN.Lib.Page.Call;
using SBN.Lib.Page.Merge;
using SBN.Lib.Page.Merge.TokenReplace;
using SBN.Lib.Page.Outcome;
using SBN.Lib.Page.Outcome.ErrorOutcome;
using SBN.Lib.Page.Render;
using SBN.Lib.Page.Render.Util;
using SBN.Lib.Request;
using SBN.Lib.Sys;
using SBN.Lib.Xml.XPath;
using SBN.Lib.Xml.XslTransform;

namespace SBN
{
    public static class Config
    {
        public static void RegisterComponents(IServiceCollection services)
        {
            RegisterAction(services);
            RegisterAnalytics(services);
            RegisterAsset(services);
            RegisterDB(services);
            RegisterDrawing(services);
            RegisterIO(services);
            RegisterPage(services);
            RegisterRequest(services);
            RegisterSys(services);
            RegisterXml(services);

            services.AddSingleton<IErrorWrapper, ErrorWrapper>();
            services.AddTransient<ISessionTokenAccessor, SessionTokenAccessor>();
            services.AddSingleton<ISettingCacher, SettingCacher>();
            services.AddTransient<Lib.ISettings, Lib.Settings>();
            services.AddTransient<IStringHasher, StringHasher>();

            RegisterExternal(services);
        }

        private static void RegisterAnalytics(IServiceCollection services)
        {
            services.AddTransient<IGeolocator, Geolocator>();
            services.AddTransient<ILocationDataParser, LocationDataParser>();
        }

        #region "Action"
        private static void RegisterAction(IServiceCollection services)
        {
            RegisterActionData(services);
            RegisterActionJsonConvert(services);
            RegisterActionOutcome(services);
            RegisterActionOutput(services);

            services.AddTransient<IApiActionCaller, ApiActionCaller>();
            services.AddTransient<IApiActionDbCaller, ApiActionDbCaller>();
        }

        private static void RegisterActionData(IServiceCollection services)
        {
            RegisterActionDataSerialise(services);

            services.AddTransient<IActionDataBuilder, ActionDataBuilder>();
            services.AddSingleton<IActionDataCache, ActionDataCache>();
            services.AddTransient<IActionDataLoader, ActionDataLoader>();
            services.AddTransient<IActionDataMapper, ActionDataMapper>();
            services.AddTransient<IApiDataLoader, ApiDataLoader>();
            services.AddTransient<IExtractActionData, ExtractActionData>();
        }

        private static void RegisterActionDataSerialise(IServiceCollection services)
        {
            services.AddTransient<IActionDataSerialiser, ActionDataSerialiser>();
            services.AddTransient<IActionDataSerialiseStrategy, ActionDataSerialiseSingleStrategy>();
            services.AddTransient<IActionDataSerialiseStrategy, ActionDataSerialiseXmlStrategy>();
            services.AddTransient<IActionDataSerialiseStrategy, ActionDataSerialiseManyStrategy>();
            services.AddTransient<IActionDataSerialiseStrategy, ActionDataSerialiseEmptyStrategy>();
        }

        private static void RegisterActionJsonConvert(IServiceCollection services)
        {
            services.AddTransient<IJsonConverter, JsonConverter>();
            services.AddTransient<IJsonDecorator, JsonDecorator>();
        }

        private static void RegisterActionOutcome(IServiceCollection services)
        {
            services.AddTransient<IApiActionOutcomeDataPopulator, ApiActionOutcomeDataPopulator>();
            services.AddTransient<IBuildApiActionOutcome, BuildApiActionOutcome>();
            services.AddTransient<IBuildFromOutcomeResponse, BuildFromOutcomeResponse>();
            services.AddTransient<IOutcomeBuilderDataPopulator, OutcomeBuilderDataPopulator>();
            services.AddTransient<IOutcomeBuilderXmlPopulator, OutcomeBuilderXmlPopulator>();
            services.AddTransient<IResourceURLGenerator, ResourceURLGenerator>();
            services.AddTransient<ITransformActionData, TransformActionData>();
        }

        private static void RegisterActionOutput(IServiceCollection services)
        {
            services.AddTransient<IOutputDefinitionParser, OutputDefinitionParser>();
            services.AddTransient<IOutputDefinitionValidator, OutputDefinitionValidator>();
            services.AddTransient<IOutputSetDefinitionValidator, OutputSetDefinitionValidator>();
        }
        #endregion

        #region "Asset"
        public static void RegisterAsset(IServiceCollection services)
        {
            RegisterAssetProcess(services);

            services.AddTransient<IAssetCallStrategy, AssetCallDeleteStrategy>();
            services.AddTransient<IAssetCallStrategy, AssetCallGetStrategy>();
            services.AddTransient<IAssetCallStrategy, AssetCallPutStrategy>();
            services.AddTransient<IAssetCallStrategy, AssetCallUnsupportedtrategy>();

            services.AddTransient<IAssetPutRequestParser, AssetPutRequestParser>();
            services.AddTransient<IAssetRetriever, AssetRetriever>();




        }

        private static void RegisterAssetProcess(IServiceCollection services)
        {
            services.AddTransient<IAssetProcessor, AssetProcessor>();

            services.AddTransient<IAssetProcessorStrategy, ImageAssetProcessorStrategy>();
            services.AddTransient<IAssetProcessorStrategy, UnsupportedAssetProcessorStrategy>();

            services.AddTransient<IAssetVariantPersister, AssetVariantPersister>();
            services.AddTransient<IImageVariantParameterParser, ImageVariantParameterParser>();
        }
        #endregion

        #region "DB"
        private static void RegisterDB(IServiceCollection services)
        {
            RegisterDBLib(services);

            services.AddTransient<IActions, Actions>();
            services.AddTransient<IAnalytics, Analytics>();
            services.AddTransient<IAssets, Assets>();
            services.AddTransient<IDBLogger, DBLogger>();
            services.AddTransient<ILogins, Logins>();
            services.AddTransient<IPages, Pages>();
            services.AddTransient<ISession, Session>();
            services.AddTransient<Lib.DB.ISettings, Lib.DB.Settings>();
            services.AddTransient<ISiteText, SiteText>();
        }

        private static void RegisterDBLib(IServiceCollection services)
        {
            services.AddTransient<IConnectionStringReader, ConnectionStringReader>();
            services.AddTransient<IDBUtil, DBUtil>();
            services.AddTransient<IXMLReader, XMLReader>();
        }
        #endregion

        private static void RegisterDrawing(IServiceCollection services)
        {
            services.AddTransient<IImageCodecFromImageFormat, ImageCodecFromImageFormat>();
            services.AddTransient<IImageFormatParser, ImageFormatParser>();
            services.AddSingleton<IImageProcessor, ImageProcessor>();
        }

        private static void RegisterIO(IServiceCollection services)
        {
            services.AddTransient<IFileSystemFacade, FileSystemFacade>();
            services.AddTransient<IStreamConverter, StreamConverter>();
        }

        #region "Page"
        private static void RegisterPage(IServiceCollection services)
        {
            RegisterPageCall(services);
            RegisterPageMerge(services);
            RegisterPageOutcome(services);
            RegisterPageRender(services);

            services.AddTransient<IPermissionsApplier, PermissionsApplier>();
            services.AddTransient<IProfileDataCacher, ProfileDataCacher>();
            services.AddTransient<IReferenceApplier, ReferenceApplier>();
        }

        private static void RegisterPageCall(IServiceCollection services)
        {
            services.AddTransient<IEstablishSessionToken, EstablishSessionToken>();
            services.AddTransient<IPageCaller, PageCaller>();
            services.AddTransient<IPageDbCaller, PageDbCaller>();
            services.AddTransient<IPageFromCache, PageFromCache>();
            services.AddTransient<IPageFromDB, PageFromDB>();
        }

        private static void RegisterPageMerge(IServiceCollection services)
        {
            RegisterPageMergeTokenReplace(services);

            services.AddTransient<IDataMergeParameterParser, DataMergeParameterParser>();
            services.AddTransient<INonDataMerger, NonDataMerger>();
            services.AddTransient<IXmlDataMerger, XmlDataMerger>();
        }

        private static void RegisterPageMergeTokenReplace(IServiceCollection services)
        {
            services.AddTransient<IBackfillData, BackfillData>();
            services.AddTransient<IDataMerger, DataMerger>();
            services.AddTransient<IGetParameterValue, GetParameterValue>();
            services.AddTransient<IMergeFromDocument, MergeFromDocument>();
            services.AddTransient<IParameterEncodeChecker, ParameterEncodeChecker>();
            services.AddTransient<IParameterReader, ParameterReader>();
            services.AddTransient<IQueryStringParameterReplacer, QueryStringParameterReplacer>();
            services.AddTransient<IQueryStringReplacer, QueryStringReplacer>();
            services.AddTransient<IStandardParameters, StandardParameters>();
            services.AddTransient<ITokenReplacer, TokenReplacer>();
            services.AddTransient<IValueEncoder, ValueEncoder>();
            services.AddTransient<IValueFromData, ValueFromData>();
        }

        private static void RegisterPageOutcome(IServiceCollection services)
        {
            RegisterPageOutcomeErrorOutcome(services);

            services.AddTransient<IActionDataParser, ActionDataParser>();
            services.AddTransient<IActionDataSetParser, ActionDataSetParser>();
            services.AddTransient<IDocumentDataSourcePopulator, DocumentDataSourcePopulator>();
            services.AddTransient<IDocumentTypeResponseParser, DocumentTypeResponseParser>();
            services.AddTransient<IDynamicPagePartPopulator, DynamicPagePartPopulator>();
            services.AddTransient<IHttpResult, HttpResult>();
            services.AddTransient<IPageDataParser, PageDataParser>();
            services.AddTransient<IPageDocumentChecker, PageDocumentChecker>();
            services.AddTransient<IPageDocumentParser, PageDocumentParser>();
            services.AddTransient<IPageOutcome, PageOutcome>();
            services.AddTransient<IPageTable, PageTable>();
        }

        private static void RegisterPageOutcomeErrorOutcome(IServiceCollection services)
        {
            services.AddTransient<IErrorOutcomeBuilder, ErrorOutcomeBuilder>();
            services.AddTransient<IErrorOutcomeBuilderStrategy, UnauthorisedOutcomeBuilderStrategy>();
            services.AddTransient<IErrorOutcomeBuilderStrategy, FallbackErrorOutcomeBuilderStrategy>();
        }

        private static void RegisterPageRender(IServiceCollection services)
        {
            RegisterPageRenderUtil(services);

            services.AddTransient<IDocumentMerger, DocumentMerger>();
            services.AddSingleton<IPageCache, PageCache>();
            services.AddTransient<IPageContentRenderer, PageContentRenderer>();
            services.AddTransient<IPageDocumentRenderer, PageDocumentRenderer>();
            services.AddTransient<IPagePartRenderer, PagePartRenderer>();
            services.AddTransient<IPageRenderer, PageRenderer>();
            services.AddTransient<IPartPreRenderer, PartPreRenderer>();
            services.AddTransient<ISBNContentRenderer, SBNContentRenderer>();

        }

        private static void RegisterPageRenderUtil(IServiceCollection services)
        {
            services.AddTransient<IDocumentCleanup, DocumentCleanup>();
            services.AddTransient<IHtml5Renderer, Html5Renderer>();
            services.AddSingleton<ISiteTextCache, SiteTextCache>();
            services.AddTransient<ISiteTextMerger, SiteTextMerger>();
            services.AddTransient<IURLParameterReplacer, URLParameterReplacer>();
        }
        #endregion

        private static void RegisterRequest(IServiceCollection services)
        {
            services.AddTransient<IAcceptsReader, AcceptsReader>();
            services.AddTransient<IRequestReader, RequestReader>();
            services.AddTransient<IRequestStreamReader, RequestStreamReader>();
        }

        private static void RegisterSys(IServiceCollection services)
        {
            services.AddTransient<ILogger, Logger>();
        }

        #region "XML"
        private static void RegisterXml(IServiceCollection services)
        {
            RegisterXmlPath(services);
            RegisterXmlXslTransform(services);

            services.AddTransient<IXMLSerializerNoNamespace, XMLSerializerNoNamespace>();
            services.AddTransient<IXmlTryReader, XmlTryReader>();
        }

        private static void RegisterXmlPath(IServiceCollection services)
        {
            services.AddTransient<IXPathFacade, XPathFacade>();
        }

        private static void RegisterXmlXslTransform(IServiceCollection services)
        {
            services.AddSingleton<ITransformCacher, TransformCacher>();
            services.AddTransient<ITransformCompiler, TransformCompiler>();
            services.AddTransient<IXmlCompiledTransformer, XmlCompiledTransformer>();
            services.AddTransient<IXmlTransformer, XmlTransformer>();
        }
        #endregion "XML"

        private static void RegisterExternal(IServiceCollection services)
        {
            services.AddTransient<IConnector, Connector>();
        }
    }
}
