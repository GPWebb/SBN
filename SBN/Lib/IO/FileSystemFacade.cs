using System.IO;
using Microsoft.AspNetCore.Http;

namespace SBN.Lib.IO
{
    public class FileSystemFacade : IFileSystemFacade
    {
        public void Delete(string path)
        {
            File.Delete(path);
        }

        public FileStream LoadAsStream(string path)
        {
            return File.OpenRead(path);
        }

        public void SaveStream(string path, Stream stream)
        {
            CreateDirectoryIfRequired(path.Substring(0, path.LastIndexOf("\\")));

            var file = File.OpenWrite(path);
            stream.CopyTo(file);
            file.Close();
        }

        public void SaveFormFile(string path, IFormFile formFile)
        {
            CreateDirectoryIfRequired(path.Substring(0, path.LastIndexOf("\\")));

            using(var stream = File.Create(path))
            {
                formFile.CopyTo(stream);
            }
        }

        public void CreateDirectoryIfRequired(string path)
        {
            var pathParts = path.Split("\\");

            var compiledParts = pathParts[0];

            for(var i=1; i< pathParts.Length; i++)
            {
                compiledParts += $"\\{pathParts[i]}";

                if(!Directory.Exists(compiledParts))
                {
                    Directory.CreateDirectory(compiledParts);
                }
            }
        }
    }
}
