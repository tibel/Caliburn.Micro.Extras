namespace Caliburn.Micro.Extras {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Interface for module bootstrappers.
    /// </summary>
    public interface IModuleBootstrapper {
        /// <summary>
        /// Initializes the module.
        /// </summary>
        void Initialize();
    }

    /// <summary>
    /// Conventions installer for ExternalModules support.
    /// </summary>
    public static class ModuleConventions {
        static readonly List<string> InitializedAssemblies = new List<string>();

        /// <summary>
        /// Installs the conventions needed for <see cref="IModuleBootstrapper"/>.
        /// </summary>
        public static void Install() {
            var locateTypeForViewType = ViewModelLocator.LocateTypeForViewType;
            ViewModelLocator.LocateTypeForViewType = (viewType, searchForInterface) => {
#if WinRT
                InitializeAssembly(viewType.GetTypeInfo().Assembly);
#else
                InitializeAssembly(viewType.Assembly);
#endif
                return locateTypeForViewType(viewType, searchForInterface);
            };

            var locateTypeForModelType = ViewLocator.LocateTypeForModelType;
            ViewLocator.LocateTypeForModelType = (modelType, displayLocation, context) => {
#if WinRT
                InitializeAssembly(modelType.GetTypeInfo().Assembly);
#else
                InitializeAssembly(modelType.Assembly);
#endif
                return locateTypeForModelType(modelType, displayLocation, context);
            };
        }

        /// <summary>
        /// Initializes the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public static void InitializeAssembly(Assembly assembly) {
            if (InitializedAssemblies.Contains(assembly.FullName)) return;
            InitializedAssemblies.Add(assembly.FullName);

            if (!AssemblySource.Instance.Contains(assembly))
                AssemblySource.Instance.Add(assembly);

            var bootstrappers = from type in assembly.GetExportedTypes()
                                where
#if WinRT
                                    !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface &&
#else
                                    !type.IsAbstract && !type.IsInterface &&
#endif
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
