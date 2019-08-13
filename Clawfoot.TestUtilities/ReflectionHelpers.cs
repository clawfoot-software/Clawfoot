using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clawfoot.TestUtilities
{
    public static class ReflectionHelpers
    {
        /// <summary>
        /// Gets a collection of 
        /// </summary>
        /// <param name="modelAssembly">The assembly the models are contained in</param>
        /// <param name="modelNamespace">The namespace the models are contained in</param>
        /// <param name="comparateAssembly">The assembly for the comparates for these models</param>
        /// <param name="comparateNamespace">The namespace for the comparates for these models</param>
        /// <param name="childModelNamespacesOnly">If you only need to compare models and comparates from namespaces under the provided ones</param>
        /// <param name="modelNameModifications">The modifications should be performed on model names, such as removing 'Dto' or 'Entity' from the name.</param>
        /// <param name="propertyFilter">A filter that can be applied to the properties of each model. can be used to filter propertes by things like attributes.</param>
        /// <returns></returns>
        public static IEnumerable<ModelPropertyToTypeComparisonModel> GetModelPropertyComparisonModels(
            string modelAssembly, 
            string modelNamespace, 
            string comparateAssembly, 
            string comparateNamespace, 
            bool childModelNamespacesOnly = false, 
            Func<string, string> modelNameModifications = null, 
            Func<List<PropertyInfo>, List<PropertyInfo>> propertyFilter = null)
        {
            Dictionary<string, Type> models = GetNameSpaceClasses(modelAssembly, modelNamespace, childModelNamespacesOnly);
            Dictionary<string, Type> comparates = GetNameSpaceClasses(comparateAssembly, comparateNamespace);

            foreach (KeyValuePair<string, Type> model in models)
            {
                string modelName = model.Value.Name;
                if (!(modelNameModifications is null))
                {
                    modelName = modelNameModifications(modelName);
                }

                if (comparates.ContainsKey(modelName))
                {
                    List<PropertyInfo> modelProperties = model.Value.GetProperties().Where(x => x.CanWrite).ToList();
                    if (!(propertyFilter is null))
                    {
                        modelProperties = propertyFilter(modelProperties);
                    }

                    foreach (PropertyInfo property in modelProperties)
                    {
                        yield return new ModelPropertyToTypeComparisonModel(model.Value, property, comparates[modelName]);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a dictionary of class/struct names for the given namespace and all nested namespaces
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="namespaceName"></param>
        /// <param name="childNameSpacesOnly">If you only need child namespaces classes, not the parent namespace</param>
        /// <returns></returns>
        public static Dictionary<string, Type> GetNameSpaceClasses(string assemblyName, string namespaceName, bool childNameSpacesOnly = false)
        {
            Assembly assembly = Assembly.Load(assemblyName);

            if (childNameSpacesOnly)
            {
                return GetChildNameSpaceClasses(assemblyName, namespaceName);
            }

            return assembly
                .GetTypes()
                .Where(x => x.IsClass || x.IsValueType)
                .Where(x => x.Namespace == namespaceName)
                .Where(x => !x.IsNested)
                .ToDictionary(x => x.Name);

        }

        private static Dictionary<string, Type> GetChildNameSpaceClasses(string assemblyName, string namespaceName)
        {
            Assembly entityAssembly = Assembly.Load(assemblyName);

            return entityAssembly
                .GetTypes()
                .Where(x => x.IsClass || x.IsValueType)
                .Where(x => x.Namespace.Contains(namespaceName))
                .Where(x => x.Namespace != namespaceName)
                .Where(x => !x.IsNested)
                .ToDictionary(x => x.Name);

        }
    }
}
