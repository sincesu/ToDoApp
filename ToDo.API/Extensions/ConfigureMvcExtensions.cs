using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace ToDo.API.Extensions;

public static class ConfigureMvcExtensions
{
    public static IServiceCollection AddCustomControllers(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                // DTO'da olmayan veri gelince hata fırlatmayı tetikler
                options.JsonSerializerOptions.UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow;
                
                //değeri null olan tüm propertyleri JSON çıktısından tamamen siler.
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        return services;
    }
}