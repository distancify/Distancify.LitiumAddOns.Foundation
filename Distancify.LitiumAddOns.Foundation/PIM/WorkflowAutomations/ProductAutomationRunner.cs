using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Application.Data;
using Litium.Application.Workflows.Data;
using Litium.Foundation;
using Litium.Products;
using Litium.Workflows;
using Litium.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace Distancify.LitiumAddOns.PIM.WorkflowAutomations
{
    public class ProductAutomationRunner : IAutomationRunner
    {
        private readonly WorkflowService _workflowService;
        private readonly WorkflowEngine _workflowEngine;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SecurityContextService _securityContextService;

        private readonly List<IWorkflowAutomation> _availableAutomations;

        public ProductAutomationRunner(WorkflowService workflowService,
            WorkflowEngine workflowEngine,            
            IServiceScopeFactory serviceScopeFactory,
            IServiceProvider serviceProvider,
            SecurityContextService securityContextService)
        {
            _workflowService = workflowService;
            _workflowEngine = workflowEngine;
            _serviceScopeFactory = serviceScopeFactory;
            _securityContextService = securityContextService;

            _availableAutomations = serviceProvider.GetService<IEnumerable<IWorkflowAutomation>>().ToList();
        }

        public void Run()
        {
            var tasksForAutomatedUser = this.GetTasksForAutomatedUser();
           
            foreach(var task in tasksForAutomatedUser)
            {
                var automation = _availableAutomations.FirstOrDefault(a => a.CanProcess(task));
                if(automation == null)
                {
                    continue;
                }

                var matchingVariants = this.GetVariantsMatchingTask(task);
                foreach(var variant in matchingVariants)
                {
                    automation.Execute(task, variant);
                    Thread.Sleep(100);
                }
            }
        }
        
        private IEnumerable<Workflow.Task> GetTasksForAutomatedUser()
        {
            var userId = _securityContextService.GetPersonSystemId(AutomationConstants.AutomationUserName);
            
            return _workflowService.GetAll()
                .SelectMany(w => w.Tasks.Where(t => t.AssignedLinks.OfType<Workflow.Task.AssignedPersonLink>()
                                                 .Any(z => z.PersonSystemId == userId)))
                .Where(t => _workflowEngine.GetTaskMatchCount(t.SystemId) != 0);
        }

        private IEnumerable<Guid> GetVariantsMatchingTask(Workflow.Task task)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<LitiumDbContext>())
                {
                    return dbContext.Set<WorkflowTaskMatchLinkEntity>()
                        .AsNoTracking()
                        .Where(x => x.EntityType == typeof(Variant).Name &&
                                    x.TaskSystemId.Equals(task.SystemId))
                        .Select(i => i.EntitySystemId)
                        .ToList();
                }
            }
        }
    }
}