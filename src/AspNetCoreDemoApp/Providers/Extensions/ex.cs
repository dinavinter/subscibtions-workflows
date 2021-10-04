using System;
using System.Collections.Generic;
using AspNetCoreDemoApp.Providers.Activities;
using AspNetCoreDemoApp.Providers.ActivityTypes;
using AspNetCoreDemoApp.Providers.Commands;
using AspNetCoreDemoApp.Providers.Events;
using AspNetCoreDemoApp.Providers.Tasks;
using Elsa.Activities.Conductor;
using Elsa.Activities.Conductor.Options;
using Elsa.Activities.Conductor.Services;
using Elsa.Options;
using Elsa.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using ICommandsProvider = AspNetCoreDemoApp.Providers.Commands.ICommandsProvider;
using IEventsProvider = AspNetCoreDemoApp.Providers.Events.IEventsProvider;
using ITasksProvider = AspNetCoreDemoApp.Providers.Tasks.ITasksProvider;

namespace AspNetCoreDemoApp.Providers.Extensions
{
        public static class ElsaOptionsBuilderExtensions
    {
        public static ElsaOptionsBuilder AddXStateActivities(this ElsaOptionsBuilder elsa, Action<StateOptions>? configureOptions = default, Action<IHttpClientBuilder>? configureHttpClient = default)
        {
            var services = elsa.Services;

            if (configureOptions != null)
                services.Configure(configureOptions);

            services.ConfigureCommandsHttpClient(configureHttpClient);
            services.ConfigureTasksHttpClient(configureHttpClient);

            services
                .AddCommandsProvider<OptionsCommandsProvider>()
                .AddEventsProvider<OptionsEventsProvider>()
                .AddTasksProvider<OptionsTasksProvider>()
                .AddActivityTypeProvider<ActionActivityTypeProvider>()
                .AddActivityTypeProvider<EventActivityTypeProvider>()
                .AddActivityTypeProvider<ServiceActivityTypeProvider>()
                .AddSingleton<Scoped<IEnumerable<ICommandsProvider>>>()
                .AddSingleton<Scoped<IEnumerable<IEventsProvider>>>()
                .AddSingleton<Scoped<IEnumerable<ITasksProvider>>>();

            elsa
                .AddActivitiesFrom<ExecuteAction>();

            return elsa;
        }

        public static IServiceCollection AddCommandsProvider<T>(this IServiceCollection services) where T : class, ICommandsProvider => services.AddScoped<ICommandsProvider, T>();
        public static IServiceCollection AddEventsProvider<T>(this IServiceCollection services) where T : class, IEventsProvider => services.AddScoped<IEventsProvider, T>();
        public static IServiceCollection AddTasksProvider<T>(this IServiceCollection services) where T : class, ITasksProvider => services.AddScoped<ITasksProvider, T>();

        private static void ConfigureCommandsHttpClient(this IServiceCollection services, Action<IHttpClientBuilder>? configureHttpClient = default) => services.ConfigureHttpClient<ApplicationCommandsClient>(o => o.CommandsHookUrl, configureHttpClient);
        private static void ConfigureTasksHttpClient(this IServiceCollection services, Action<IHttpClientBuilder>? configureHttpClient = default) => services.ConfigureHttpClient<ApplicationTasksClient>(o => o.CommandsHookUrl, configureHttpClient);

        private static void ConfigureHttpClient<T>(this IServiceCollection services, Func<ConductorOptions, Uri> baseAddress, Action<IHttpClientBuilder>? configureHttpClient = default) where T : class
        {
            var httpClientBuilder = services.AddHttpClient<T>((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<ConductorOptions>>().Value;
                httpClient.BaseAddress = baseAddress(options);
            });

            if (configureHttpClient == null)
                httpClientBuilder.AddTransientHttpErrorPolicy(x => x.WaitAndRetryAsync(10, retryCount => TimeSpan.FromSeconds(Math.Pow(2, retryCount))));
            else
                configureHttpClient(httpClientBuilder);
        }
    }

}