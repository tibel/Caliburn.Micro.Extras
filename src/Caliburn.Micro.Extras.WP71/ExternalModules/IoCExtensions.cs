namespace Caliburn.Micro.Extras.ExternalModules {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extensions for the <see cref="PhoneContainer"/> to support <see cref="IModuleBootstrapper"/> initialization.
    /// </summary>
    public static class IoCExtensions {
        static readonly Dictionary<string, Assembly> InitializedAssemblies = new Dictionary<string, Assembly>();

        /// <summary>
        /// Requests all instances of a given type and intializes the module containing <paramref name="service"/> type.
        /// </summary>
        /// <param name="container">The IoC container.</param>
        /// <param name="service">The service.</param>
        /// <returns>All the instances or an empty enumerable if none are found.</returns>
        public static IEnumerable<object> GetAllInstancesWithModuleInitialization(this PhoneContainer container, Type service) {
            InitializeAssembly(service.Assembly);
            return container.GetAllInstances(service);
        }

        /// <summary>
        /// Requests an instance and intializes the module containing <paramref name="service"/> type.
        /// </summary>
        /// <param name="container">The IoC container.</param>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <returns>The instance, or null if a handler is not found.</returns>
        public static object GetInstanceWithModuleInitialization(this PhoneContainer container, Type service, string key) {
            if (service != null)
                InitializeAssembly(service.Assembly);
            return container.GetInstance(service, key);
        }

        internal static void InitializeAssembly(Assembly assembly) {
            if (InitializedAssemblies.ContainsKey(assembly.FullName))
                return;

            InitializedAssemblies.Add(assembly.FullName, assembly);

            if (!AssemblySource.Instance.Contains(assembly))
                AssemblySource.Instance.Add(assembly);

            var bootstrappers = from type in assembly.GetExportedTypes()
                                where
                                    !type.IsAbstract && !type.IsInterface &&
                                    typeof (IModuleBootstrapper).IsAssignableFrom(type)
                                select type;

            foreach (var bootstrapper in bootstrappers) {
                var moduleInitializer = (IModuleBootstrapper) Activator.CreateInstance(bootstrapper);
                IoC.BuildUp(moduleInitializer);
                moduleInitializer.Initialize();
            }
        }
    }
}
