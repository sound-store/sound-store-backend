using System.Text.Json;

namespace SoundStore.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                o.JsonSerializerOptions.WriteIndented = true;
            });
            
            services.AddEndpointsApiExplorer();

            services.AddSwaggerDoc();

            return services;
        }

        private static IServiceCollection AddSwaggerDoc(this IServiceCollection services)
        {
            services.AddSwaggerGen(o =>
            {

            });

            return services;
        }
    }
}
