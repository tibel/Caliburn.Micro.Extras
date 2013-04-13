namespace Caliburn.Micro.Extras.ExternalModules {
    /// <summary>
    /// Conventions installer for ExternalModules support.
    /// </summary>
    public static class ModuleConventions {
        /// <summary>
        /// Installs the conventions needed for <see cref="IModuleBootstrapper"/>.
        /// </summary>
        public static void Install() {
            var locateTypeForViewType = ViewModelLocator.LocateTypeForViewType;
            ViewModelLocator.LocateTypeForViewType = (viewType, searchForInterface) => {
                IoCExtensions.InitializeAssembly(viewType.Assembly);
                return locateTypeForViewType(viewType, searchForInterface);
            };

            var locateTypeForModelType = ViewLocator.LocateTypeForModelType;
            ViewLocator.LocateTypeForModelType = (modelType, displayLocation, context) => {
                IoCExtensions.InitializeAssembly(modelType.Assembly);
                return locateTypeForModelType(modelType, displayLocation, context);
            };
        }
    }
}
