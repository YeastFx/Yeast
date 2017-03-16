using Microsoft.AspNetCore.Builder;

namespace Yeast.AspNetCore.Abstractions
{
    public interface IConfigurePipeline
    {
        void Configure(IApplicationBuilder app);
    }
}
