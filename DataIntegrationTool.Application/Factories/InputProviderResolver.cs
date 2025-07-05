using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Interfaces;

namespace DataIntegrationTool.Application.Factories
{
    public delegate IInputProvider InputProviderResolver(InputSourceConfig config);
}
