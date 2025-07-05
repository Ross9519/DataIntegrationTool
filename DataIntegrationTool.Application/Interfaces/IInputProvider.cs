namespace DataIntegrationTool.Application.Interfaces
{
    public interface IInputProvider
    {
        Task<IEnumerable<T>> CreateObjectFromInputAsync<T>() where T : class;
    }
}
