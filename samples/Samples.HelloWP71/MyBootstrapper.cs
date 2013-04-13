using System;
using System.Collections.Generic;
using System.Reflection;
using Caliburn.Micro;
using Caliburn.Micro.Extras.ExternalModules;

namespace Samples.HelloWP71 {
    public class MyBootstrapper : PhoneBootstrapper {
        private PhoneContainer container;

        protected override void Configure() {
            container = new PhoneContainer(RootFrame);
            container.RegisterPhoneServices();

            //NOTE: don't register anything from the module assemblies here
            container.PerRequest<MainPageViewModel>();

            //NOTE: install the conventions
            ModuleConventions.Install();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            //NOTE: don't reference the module assemblies here
            return base.SelectAssemblies();
        }

        protected override object GetInstance(Type service, string key) {
            //NOTE: use extension that also does module initialization
            return container.GetInstanceWithModuleInitialization(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service) {
            //NOTE: use extension that also does module initialization
            return container.GetAllInstancesWithModuleInitialization(service);
        }

        protected override void BuildUp(object instance) {
            container.BuildUp(instance);
        }
    }
}
