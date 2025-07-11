namespace DataIntegrationTool.Application.Interfaces
{
    public interface IFileReader
    {
        Stream OpenRead(string path);
        public bool Exists(string path);
    }
}
