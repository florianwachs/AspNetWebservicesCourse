using Akka.Actor;
using Akka.Configuration;
using Akka.DI.Extensions.DependencyInjection;
using AkkaDocumentGenerator.Actors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AkkaDocumentGenerator
{
    public class ActorRegistration
    {
        public const string ActorSystemName = "DocumentGenerationSystem";
        public static IServiceCollection Register(IServiceCollection services)
        {
            var akkaConfPath = Path.Combine(AppContext.BaseDirectory, "akka.conf");
            var configString = File.ReadAllText(akkaConfPath);
            var config = ConfigurationFactory.ParseString(configString);

            services.AddSingleton(provider =>
            {
                var actorSystem = ActorSystem.Create(ActorSystemName, config);
                actorSystem.UseServiceProvider(provider);

                return actorSystem;
            });

            services.AddSingleton<ReportGenerationControllerActor>();
            services.AddTransient<ReportGeneratorActor>();

            services.AddSingleton<ActorReferences.ReportGenerationControllerActorProvider>(provider =>
            {
                var actorSystem = provider.GetRequiredService<ActorSystem>();
                var props = ReportGenerationControllerActor.CreateProps(actorSystem);
                var actor = actorSystem.ActorOf(props);
                return () => actor;
            });

            return services;
        }

        public static void UseActorSystem(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                var logger = app.ApplicationServices.GetService<ILogger<Startup>>();
                logger.LogInformation("Starting ActorSystem");
                StartActors(app.ApplicationServices);
            });

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                var logger = app.ApplicationServices.GetService<ILogger<Startup>>();
                logger.LogInformation("Trying to stop actor system");
                try
                {
                    app.ApplicationServices.GetService<ActorSystem>().Terminate().Wait();
                    logger.LogInformation("actor system stopped");
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "Failed to gracefully shutdown actor system");
                }
            });
        }

        private static void StartActors(IServiceProvider applicationServices)
        {
            // Requesting the actor starts the actor
            applicationServices.GetRequiredService<ActorReferences.ReportGenerationControllerActorProvider>();
        }
    }
}
