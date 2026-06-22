using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi; // 💥 DİKKAT: '.Models' KISMI YOK, ARTIK SADECE BU KULLANILIYOR!

namespace ToDo.API.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDo API", Version = "v1" });

                // 1. ADIM: Kilit ikonunu ve token girilecek kutuyu oluşturur
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Paste your JWT token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                // 2. ADIM: Token'ı tüm isteklere zorla ekletir (.NET 10 için document parametreli YENİ SYNTAX)
                c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", document),
                        []
                    }
                });
            });

            return services;
        }
    }
}