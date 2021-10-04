using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using AspNetCoreDemoApp.Providers.Models;

namespace AspNetCoreDemoApp.Providers
{
    public class StateOptions
    {

            public StateOptions()
            {
              this.SerializerSettings = new JsonSerializerSettings();
              this.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            }

            public Uri ActionsHookUrl { get; set; } = (Uri) null;

            public Uri ServicesHookUrl { get; set; } = (Uri) null;

            public JsonSerializerSettings SerializerSettings { get; set; }

            public ICollection<CommandDefinition> Actions { get; set; } = (ICollection<CommandDefinition>) new List<CommandDefinition>();

            public ICollection<EventDefinition> Events { get; set; } = (ICollection<EventDefinition>) new List<EventDefinition>();

            public ICollection<TaskDefinition> Services { get; set; } = (ICollection<TaskDefinition>) new List<TaskDefinition>();

    }
}