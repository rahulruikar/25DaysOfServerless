using Day5.Interfaces;
using Day5.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Day5.Startup))]

namespace Day5
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
           .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
           .AddEnvironmentVariables()
           .Build();

            builder.Services.AddSingleton(config);
            builder.Services.AddScoped<ISentimentAnalysisService, SentimentAnalysisService>();
            builder.Services.AddScoped<ITextTranslationService, TextTranslationService>();
        }
    }
}