namespace DataIntegrationTool.Application.Interfaces
{
    public interface IDataCleaner<T> where T : class
    {
        void Clean(IEnumerable<T> raws);
        void Clean(T raw);
    }
}
