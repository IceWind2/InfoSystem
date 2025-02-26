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

            foreach ( var property in entityProperties )
            {
                if (property.GetValue(entity)!.ToString()!.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class FilterProperty : Attribute { }
}
