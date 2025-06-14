using System;
using System.Linq;
using System.Reflection;

namespace InfoSystem
{
    public static class Filter
    {
        public static bool ContainsFilter(this object entity, string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return true;
            }

            var entityType = entity.GetType();
            var entityProperties = entity.GetType().GetProperties().Where(pr => pr.GetCustomAttribute<FilterProperty>() != null);

            foreach (var property in entityProperties)
            {
                string? propValue = property.GetValue(entity)?.ToString();
                if (propValue == null)
                {
                    continue;
                }

                if (propValue.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class FilterProperty : Attribute { }
}
