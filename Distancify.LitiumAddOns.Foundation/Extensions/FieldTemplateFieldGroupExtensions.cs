using Litium.FieldFramework;
using System.Linq;

namespace Distancify.LitiumAddOns.Extensions
{
    public static class FieldTemplateFieldGroupExtensions
    {
        public static bool UpdateLocalizationsAndAddNewFields(this FieldTemplateFieldGroup currentGroup, FieldTemplateFieldGroup newGroup)
        {
            var dirty = false;

            if (currentGroup.UpdateLocalizations(newGroup))
            {
                dirty = true;
            }

            if (currentGroup.AddNewFields(newGroup))
            {
                dirty = true;
            }

            return dirty;
        }

        public static bool UpdateLocalizations(this FieldTemplateFieldGroup currentGroup, FieldTemplateFieldGroup newGroup)
        {
            var dirty = false;

            foreach (var item in newGroup.Localizations)
            {
                if (!currentGroup.Localizations.Any(l => l.Key.Equals(item.Key)) ||
                    !currentGroup.Localizations[item.Key].Name.Equals(item.Value.Name))
                {
                    currentGroup.Localizations[item.Key].Name = item.Value.Name;
                    dirty = true;
                }
            }

            return dirty;
        }

        public static bool AddNewFields(this FieldTemplateFieldGroup currentGroup, FieldTemplateFieldGroup newGroup)
        {
            var dirty = false;

            foreach (var field in newGroup.Fields)
            {
                if (!currentGroup.Fields.Contains(field))
                {
                    currentGroup.Fields.Add(field);
                    dirty = true;
                }
            }

            return dirty;
        }
    }
}
