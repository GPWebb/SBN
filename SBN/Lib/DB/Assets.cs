using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AlpineRed.DB;
using SBN.Lib.Asset;

namespace SBN.Lib.DB
{
    public class Assets : IAssets
    {
        private readonly IConnector _connector;
        private readonly IDBUtil _dbUtil;

        public Assets(IConnector connector,
            IDBUtil dbUtil)
        {
            _connector = connector;
            _dbUtil = dbUtil;
        }

        public HttpStatusCode AssetErrorHandler(SqlException dbex)
        {
            var errorNumber = dbex.Number.ToString();
            if (errorNumber.Length == 6 && errorNumber.StartsWith("580"))
            {
                return Enum.Parse<HttpStatusCode>(errorNumber.Substring(3));
            }

            throw dbex;
        }

        private AssetResponse GetFromDataRow(DataRow result)
        {
            return new AssetResponse
            {
                AssetCallStatus = result.Field<HttpStatusCode>("AssetCallStatus"),
                AssetID = result.Field<int>("AssetID"),
                AssetVariantID = result.Field<int>("AssetTypeVariantID"),
                Name = result.Field<string>("AssetName"),
                Description = result.Field<string>("AssetDescription"),
                AssetBasePath = result.Field<string>("AssetBasePath"),
                BasePath = result.Field<string>("BasePath"),
                BaseVariantPath = result.Field<string>("BaseVariantPath"),
                VariantPath = result.Field<string>("VariantPath"),
                VariantSuffix = result.Field<string>("VariantSuffix"),
                VariantParameters = result.Field<string>("VariantParameters"),
                Attachment = result.Field<bool>("Attachment"),
                ClientFilename = result.Field<string>("ClientFilename"),
                MIMEType = result.Field<string>("MIMEType"),
                LastUpdated = result.Field<DateTime>("LastUpdated")
            };
        }

        public async Task<AssetResponse> Get(Guid sessionToken, string assetPath)
        {
            try
            {
                var connection = _dbUtil.Connection;
                var command = (SqlCommand)_dbUtil.GetAuthenticatedCommand(connection, sessionToken, "assets.Asset_Read");

                command.Parameters.Add(Util.CreateParameter("Path", SqlDbType.NVarChar, assetPath));

                var result = await _connector.GetDataRow(connection, command);
                if (result == null) return new AssetResponse { AssetCallStatus = HttpStatusCode.NotFound };

                return GetFromDataRow(result);
            }
            catch(SqlException dbex)
            {
                return new AssetResponse { AssetCallStatus = AssetErrorHandler(dbex) };
            }
        }

        public async Task<AssetDeleteResponse> Delete(Guid sessionToken, string path)
        {
            try
            {
                var connection = _dbUtil.Connection;
                var command = (SqlCommand)_dbUtil.GetAuthenticatedCommand(connection, sessionToken, "assets.Asset_Delete");

                command.Parameters.Add(Util.CreateParameter("Path", SqlDbType.NVarChar, path));

                var result = await _connector.GetDataSet(connection, command);

                return new AssetDeleteResponse
                {
                    StatusCode = Enum.Parse<HttpStatusCode>(result.Tables[0].Rows[0].Field<int>("StatusCode").ToString()),
                    FilePaths = result.Tables[1].Select().Select(r => r.Field<string>("Path"))
                };
            }
            catch (SqlException dbex)
            {
                return new AssetDeleteResponse { StatusCode = AssetErrorHandler(dbex) };
            }
        }

        public async Task<AssetCreatedResponse> Add(Guid sessionToken, 
            string assetType, 
            string assetPath, 
            string name, 
            string description, 
            string clientFilename, 
            string MIMEType)
        {
            try
            {
                var connection = _dbUtil.Connection;
                var command = (SqlCommand)_dbUtil.GetAuthenticatedCommand(connection, sessionToken, "assets.Asset_Add");

                command.Parameters.Add(Util.CreateParameter("AssetType", SqlDbType.NVarChar, assetType));
                command.Parameters.Add(Util.CreateParameter("AssetPath", SqlDbType.NVarChar, assetPath));
                command.Parameters.Add(Util.CreateParameter("Name", SqlDbType.NVarChar, name));
                command.Parameters.Add(Util.CreateParameter("Description", SqlDbType.NVarChar, description));
                command.Parameters.Add(Util.CreateParameter("ClientFilename", SqlDbType.NVarChar, clientFilename));
                command.Parameters.Add(Util.CreateParameter("MIMEType", SqlDbType.NVarChar, MIMEType));

                var result = await _connector.GetDataRow(connection, command);

                return new AssetCreatedResponse
                {
                    StatusCode = HttpStatusCode.Created,
                    AssetID = result.Field<int>("AssetID"),
                    Path = result.Field<string>("Path")
                };
            }
            catch(SqlException dbex)
            {
                return new AssetCreatedResponse { StatusCode = AssetErrorHandler(dbex), ErrorMessage = dbex.Message };
            }
        }

        public async Task<AssetResponse> GetByIDAndVariant(Guid sessionToken, int ID, string variantSuffix)
        {
            try
            {
                var connection = _dbUtil.Connection;
                var command = (SqlCommand)_dbUtil.GetAuthenticatedCommand(connection, sessionToken, "assets.Asset_ReadByIDAndVariant");

                command.Parameters.Add(Util.CreateParameter("AssetID", SqlDbType.Int, ID));
                command.Parameters.Add(Util.CreateParameter("VariantSuffix", SqlDbType.NVarChar, variantSuffix));

                var result = await _connector.GetDataRow(connection, command);
                if (result == null) return new AssetResponse { AssetCallStatus = HttpStatusCode.NotFound };

                return GetFromDataRow(result);
            }
            catch (SqlException dbex)
            {
                return new AssetResponse { AssetCallStatus = AssetErrorHandler(dbex) };
            }
        }

        public async Task UpdateVariantFilePath(Guid sessionToken, AssetResponse assetResult)
        {
            var connection = _dbUtil.Connection;
            var command = (SqlCommand)_dbUtil.GetAuthenticatedCommand(connection, sessionToken, "assets.Asset_UpdateVariantFilePath");

            command.Parameters.Add(Util.CreateParameter("AssetID", SqlDbType.NVarChar, assetResult.AssetID));
            command.Parameters.Add(Util.CreateParameter("AssetVariantID", SqlDbType.NVarChar, assetResult.AssetVariantID));
            command.Parameters.Add(Util.CreateParameter("VariantPath", SqlDbType.NVarChar, assetResult.VariantPath));

            await _connector.ExecuteCmd_NoReturn(connection, command);
        }
    }
}
