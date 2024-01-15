using System.IO;
using Microsoft.AspNetCore.Http;

namespace SBN.Lib.IO
{
    public interface IFileSystemFacade
    {
        FileStream LoadAsStream(string path);
        void SaveStream(string path, Stream stream);
        void SaveFormFile(string path, IFormFile formFile);
        void Delete(string path);
    }
}