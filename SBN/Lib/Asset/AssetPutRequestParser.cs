using Microsoft.AspNetCore.Http;

namespace SBN.Lib.Asset
{
    public class AssetPutRequestParser : IAssetPutRequestParser
    {
        //public async Task<Stream> StreamFromFormFileData(IFormFile formFile)
        //{
        //    using (var inputStream = new MemoryStream())
        //    {
        //        await formFile.CopyToAsync(inputStream);
        //        //// stream to byte array
        //        //byte[] array = new byte[inputStream.Length];
        //        //inputStream.Seek(0, SeekOrigin.Begin);
        //        //inputStream.Read(array, 0, array.Length);
        //        //// get file name
        //        //string fName = formFile.FileName;

        //        return inputStream;
        //    }
        //}

        public AssetPutRequest Parse(HttpRequest request)
        {
            return new AssetPutRequest
            {
                Name = request.Form["AssetName"],
                Description = request.Form["Description"],
                AssetType = request.Form["AssetType"],
                AssetPath = request.Form["AssetPath"],
                File = request.Form.Files[0],
                ClientFilename = request.Form.Files[0].FileName,
                MIMEType = request.Form.Files[0].ContentType
            };
        }
    }
}
