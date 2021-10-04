﻿using System.Linq;
using System.Threading.Tasks;
using Elsa;
using Elsa.Activities.Conductor.Models;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;

namespace AspNetCoreDemoApp.Providers.Activities
{
    [Job(
        Category = "state.Services",
         Description = "Sends a task to your application and waits for the application to report the task as completed or cancelled."
    )]
    public class RunService : Activity
    {
        private readonly IEventPublisher _eventPublisher;

        public RunService(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        [ActivityInput(
            Label = "Service",
            Hint = "The service to run.",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid }
        )]
        public string TaskName { get; set; } = default!;

        [ActivityOutput(Hint = "Any input to send along with the task to your application.")]
        public object? Payload { get; set; }

        [ActivityOutput(Hint = "Any input that was received along with the task completion.")]
        public object? Output { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            await _eventPublisher.PublishAsync(new RunTaskModel(TaskName, Payload, context.WorkflowInstance.Id));
            return Suspend();
        }

        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
            var eventModel = context.GetInput<TaskResultModel>()!;
            var outcomes = eventModel.Outcomes;

            if (outcomes?.Any() == false)
                outcomes = new[] { OutcomeNames.Done };

            Output = eventModel.Payload;
            context.LogOutputProperty(this, nameof(Output), Output);
            return base.Outcomes(outcomes!);
        }
    }
}