using System;
using System.Collections.Generic;
using System.Linq;
using Distancify.LitiumAddOns.Foundation.Extensions;
using Distancify.SerilogExtensions;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.Foundation;
using Litium.Owin.Lifecycle;
using Litium.Runtime;

namespace Distancify.LitiumAddOns.Foundation.Fields
{
    public abstract class FieldAndTemplateSetup<TArea> : IStartupTask where TArea : IArea
    {
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly FieldDefinitionService _fieldDefinitionService;

        protected FieldAndTemplateSetup(FieldTemplateService fieldTemplateService, FieldDefinitionService fieldDefinitionService)
        {
            _fieldTemplateService = fieldTemplateService;
            _fieldDefinitionService = fieldDefinitionService;
        }

        public void Start()
        {
            try
            {
                using (Solution.Instance.SystemToken.Use())
                {
                    SetupFields(GetFields());
                    SetupTemplates(GetTemplates());
                }
            }
            catch (Exception ex)
            {
                this.Log().Error(ex, "An exception occurred when setting up fields and templates");
            }
        }

        protected virtual List<FieldDefinition<TArea>> GetFields()
        {
            return new List<FieldDefinition<TArea>>();
        }

        protected virtual List<FieldTemplate<TArea>> GetTemplates()
        {
            return new List<FieldTemplate<TArea>>();
        }

        private void SetupFields(List<FieldDefinition<TArea>> fields)
        {
            foreach (var field in fields)
            {
                try
                {
                    SetupField(field);
                }
                catch (Exception ex)
                {
                    this.Log().Error(ex, "An exception occurred when setting up the field with ID {FieldID}", field.Id);
                }
            }
        }

        private void SetupTemplates(List<FieldTemplate<TArea>> templates)
        {
            foreach (var template in templates)
            {
                try
                {
                    SetupTemplate(template);
                }
                catch (Exception ex)
                {
                    this.Log().Error(ex, "An exception occurred when setting up the template with ID {TemplateID}", template.Id);
                }
            }
        }



        private void SetupTemplate(FieldTemplate<TArea> newFieldTemplate)
        {
            var currentFieldTemplate = _fieldTemplateService.Get<FieldTemplate<TArea>>(newFieldTemplate.Id);

            if (currentFieldTemplate is null)
            {
                _fieldTemplateService.Create(newFieldTemplate);
                this.Log().Debug("Created field template {Id}", currentFieldTemplate.Id);
            }
            else UpdateFieldTemplate(newFieldTemplate, currentFieldTemplate);
        }

        private void UpdateFieldTemplate(FieldTemplate<TArea> newFieldTemplate, FieldTemplate<TArea> currentFieldTemplate)
        {
            currentFieldTemplate = currentFieldTemplate.MakeWritableClone() as FieldTemplate<TArea>;
            var dirty = false;

            if (UpdateLocalizations(newFieldTemplate, currentFieldTemplate))
            {
                dirty = true;
            }

            if (UpdateFieldGroups(newFieldTemplate, currentFieldTemplate))
            {
                dirty = true;
            }

            if (dirty)
            {
                _fieldTemplateService.Update(currentFieldTemplate);
                this.Log().Debug("Updated field template {Id}", currentFieldTemplate.Id);
            }
        }

        private bool UpdateLocalizations(FieldTemplate<TArea> newFieldTemplate, FieldTemplate<TArea> currentFieldTemplate)
        {
            var dirty = false;

            foreach (var item in newFieldTemplate.Localizations)
            {
                if (!currentFieldTemplate.Localizations.Any(l => l.Key.Equals(item.Key)) ||
                    !currentFieldTemplate.Localizations[item.Key].Name.Equals(item.Value.Name))
                {
                    currentFieldTemplate.Localizations[item.Key].Name = item.Value.Name;
                    dirty = true;
                }
            }

            return dirty;
        }

        private bool UpdateFieldGroups(FieldTemplate<TArea> newFieldTemplate, FieldTemplate<TArea> currentFieldTemplate)
        {
            var dirty = false;

            if (newFieldTemplate is OrganizationFieldTemplate)//TODO: We need one of these per template type or a more clever and generic solution if one exists.
            {
                if (UpdateFieldGroups((newFieldTemplate as OrganizationFieldTemplate).FieldGroups, (currentFieldTemplate as OrganizationFieldTemplate).FieldGroups))
                {
                    dirty = true;
                }
            }

            return dirty;
        }

        private bool UpdateFieldGroups(ICollection<FieldTemplateFieldGroup> newFieldTemplateFieldGroups, ICollection<FieldTemplateFieldGroup> currentFieldTemplateFieldGroups)
        {
            var dirty = false;

            if (newFieldTemplateFieldGroups != null)
            {
                if (currentFieldTemplateFieldGroups == null)
                {
                    currentFieldTemplateFieldGroups = new List<FieldTemplateFieldGroup>();
                }

                foreach (var newGroup in newFieldTemplateFieldGroups)
                {
                    var currentGroup = currentFieldTemplateFieldGroups.FirstOrDefault(g => g.Id.Equals(newGroup.Id));

                    if (currentGroup == null)
                    {
                        currentFieldTemplateFieldGroups.Add(newGroup);
                        dirty = true;
                    }
                    else if (currentGroup.UpdateLocalizationsAndAddNewFields(newGroup))
                    {
                        dirty = true;
                    }
                }
            }

            return dirty;
        }

        private void SetupField(FieldDefinition newField)
        {
            var currentField = _fieldDefinitionService.Get<TArea>(newField.Id);

            if (currentField is null)
            {
                _fieldDefinitionService.Create(newField);
                this.Log().Debug("Created field {FieldDefinitionId}", currentField.Id);
            }
            else if (!currentField.FieldType.Equals(newField.FieldType))
            {
                this.Log().Error("Cannot update field definition type of {FieldDefinitionId} from {CurrentFieldType} to {NewFieldType}", currentField.Id, currentField.FieldType, newField.FieldType);
            }
            else UpdateField(newField, currentField);
        }

        private void UpdateField(FieldDefinition newField, FieldDefinition currentField)
        {
            var dirty = false;
            currentField = currentField.MakeWritableClone();

            foreach (var item in newField.Localizations)
            {
                if (!currentField.Localizations.Any(l => l.Key.Equals(item.Key)) ||
                    !currentField.Localizations[item.Key].Name.Equals(item.Value.Name))
                {
                    currentField.Localizations[item.Key].Name = item.Value.Name;
                    dirty = true;
                }
            }

            if (dirty)
            {
                _fieldDefinitionService.Update(currentField);
                this.Log().Debug("Updated field {FieldDefinitionId}", currentField.Id);
            }
        }
    }
}