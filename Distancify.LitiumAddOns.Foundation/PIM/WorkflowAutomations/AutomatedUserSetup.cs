using System;
using System.Collections.Generic;
using Distancify.Migrations.Litium.Seeds;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.Security;
using Litium.Web.Administration.WebApi;

namespace Distancify.LitiumAddOns.PIM.WorkflowAutomations
{
    public class AutomatedUserSetup : IAutomatedUserSetup
    {   
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly FieldDefinitionService _fieldDefinitionService;
        private readonly PersonService _personService;
        private readonly GroupService _groupService;

        public AutomatedUserSetup(FieldTemplateService fieldTemplateService,
            FieldDefinitionService fieldDefinitionService,
            PersonService personService,
            GroupService groupService)
        {
            _fieldTemplateService = fieldTemplateService;
            _fieldDefinitionService = fieldDefinitionService;
            _personService = personService;
            _groupService = groupService;
        }

        public void EnsureUserExists()
        {
            var groupTemplate = _fieldTemplateService.Get<GroupFieldTemplate>(AutomationConstants.GroupFieldTemplateId);
            if(groupTemplate == null)
            {
                groupTemplate = CreateGroupFieldTemplate();
            }

            var group = _groupService.Get<StaticGroup>(AutomationConstants.AutomationGroupId);
            if(group == null)
            {
                group = CreateAutomatedUserGroup(groupTemplate);
            }

            var personTemplate = _fieldTemplateService.Get<PersonFieldTemplate>(AutomationConstants.UserFieldTemplateId);
            if (personTemplate == null)
            {
                personTemplate = CreateAutomatedUserTemplate();
            }

            var person = _personService.Get(AutomationConstants.UserId);            
            if (person == null)
            {
                CreateAutomatedUser(personTemplate, group);
            }
        }

        private GroupFieldTemplate CreateGroupFieldTemplate()
        {
            var template = new GroupFieldTemplate(AutomationConstants.GroupFieldTemplateId)
            {
                FieldGroups = new[]
                {
                    new FieldTemplateFieldGroup()
                    {
                        Id = "General",
                        Collapsed = false,
                        Localizations =
                        {
                            [Cultures.sv_SE] = {Name = "Allmänt"},
                            [Cultures.en_US] = {Name = "General"},
                        },
                        Fields =
                        {
                            SystemFieldDefinitionConstants.NameInvariantCulture,
                            SystemFieldDefinitionConstants.Description
                        }
                    }
                }
            };

            template.Localizations.FromDictionary(new Dictionary<string, string>
                                                            {
                                                                { Cultures.sv_SE, AutomationConstants.GroupFieldTemplateName },
                                                                { Cultures.en_US, AutomationConstants.GroupFieldTemplateName }
                                                            }, (x => x.Name));
            _fieldTemplateService.Create(template);

            return _fieldTemplateService.Get<GroupFieldTemplate>(AutomationConstants.GroupFieldTemplateId);         
        }

        private StaticGroup CreateAutomatedUserGroup(GroupFieldTemplate template)
        {
            var group = new StaticGroup(template.SystemId, AutomationConstants.GroupName)
            {
                Id = AutomationConstants.AutomationGroupId
            };

            group.Fields.AddOrUpdateValue(SystemFieldDefinitionConstants.NameInvariantCulture, AutomationConstants.GroupName);
            group.Fields.AddOrUpdateValue(SystemFieldDefinitionConstants.Description, Cultures.sv_SE, AutomationConstants.Description);
            group.Fields.AddOrUpdateValue(SystemFieldDefinitionConstants.Description, Cultures.en_US, AutomationConstants.Description);

            group.AccessControlOperationList = new HashSet<AccessControlOperationEntry>(new[]
            {
                new AccessControlOperationEntry(Operations.Function.Products.Content),
                new AccessControlOperationEntry(Operations.Function.Products.Settings),
                new AccessControlOperationEntry(Operations.Function.Products.UI),
            });

            _groupService.Create(group);
            
            return _groupService.Get<StaticGroup>(AutomationConstants.AutomationGroupId);
        }

        private PersonFieldTemplate CreateAutomatedUserTemplate()
        {
            var template = new PersonFieldTemplate(AutomationConstants.UserFieldTemplateId)
            {
                Localizations =
                {
                    [Cultures.sv_SE] = {Name = AutomationConstants.UserFieldTemplateName},
                    [Cultures.en_US] = {Name = AutomationConstants.UserFieldTemplateName},
                },
                FieldGroups = new[]
                {
                    new FieldTemplateFieldGroup()
                    {
                        Id = "General",
                        Collapsed = false,
                        Localizations =
                        {
                            [Cultures.sv_SE] = {Name = "Allmänt"},
                            [Cultures.en_US] = {Name = "General"},
                        },
                        Fields =
                        {
                            SystemFieldDefinitionConstants.FirstName,
                            SystemFieldDefinitionConstants.LastName,
                            SystemFieldDefinitionConstants.Description
                        }
                    }
                }
            };

            _fieldTemplateService.Create(template);

            return _fieldTemplateService.Get<PersonFieldTemplate>(AutomationConstants.UserFieldTemplateId);
        }

        private void CreateAutomatedUser(PersonFieldTemplate template, Group group)
        {
            var person = new Person(template.SystemId)
            {
                Id = AutomationConstants.UserId
            };

            person.Fields.AddOrUpdateValue(SystemFieldDefinitionConstants.FirstName, AutomationConstants.UserFirstName);
            person.Fields.AddOrUpdateValue(SystemFieldDefinitionConstants.LastName, AutomationConstants.UserLastName);
            person.Fields.AddOrUpdateValue(SystemFieldDefinitionConstants.Description, Cultures.sv_SE, AutomationConstants.Description);
            person.Fields.AddOrUpdateValue(SystemFieldDefinitionConstants.Description, Cultures.en_US, AutomationConstants.Description);

            person.LoginCredential.Username = AutomationConstants.AutomationUserName;
            person.LoginCredential.NewPassword = AutomationConstants.AutomatedUserPassword;
            person.LoginCredential.LockoutEnabled = true;
            person.LoginCredential.LockoutEndDate = DateTime.MaxValue;

            person.GroupLinks.Add(new PersonToGroupLink(group.SystemId));

            _personService.Create(person);
        }
    }
}
