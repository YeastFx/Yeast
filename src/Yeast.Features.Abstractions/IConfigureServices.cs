using Microsoft.Extensions.DependencyInjection;

namespace Yeast.Features.Abstractions
{
    public interface IConfigureServices
    {
        void ConfigureServices(IServiceCollection services);
    }
}
