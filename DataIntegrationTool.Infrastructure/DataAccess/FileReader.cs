using DataIntegrationTool.Application.Interfaces;

namespace DataIntegrationTool.Infrastructure.DataAccess
{
    public class FileReader : IFileReader
    {
        public Stream OpenRead(string path)
            => new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        public bool Exists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            return File.Exists(path);
        }
    }
}
