using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;

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
                options.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;

                // Enum'ları integer (0, 1, 2) yerine string ("Created", "InProgress") olarak dönüştürür.
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

                //değeri null olan tüm propertyleri JSON çıktısından tamamen siler.
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        return services;
    }
}