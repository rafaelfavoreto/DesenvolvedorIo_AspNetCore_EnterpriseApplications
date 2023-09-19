namespace NSE.Identidade.API.Configuration
{
    public static class ApiConfig
    {
        public static WebApplicationBuilder AddApiConfig(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            return builder;
        }
    }
}
