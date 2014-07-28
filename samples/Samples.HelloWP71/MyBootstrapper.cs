using System;
using System.Collections.Generic;
using System.Reflection;
using Caliburn.Micro;
using Caliburn.Micro.Extras;

namespace Samples.HelloWP71 {
    public class MyBootstrapper : PhoneBootstrapperBase {
        PhoneContainer container;

        public MyBootstrapper() {
            Initialize();
        }

        protected override void Configure() {
            container = new PhoneContainer();
            container.RegisterPhoneServices(RootFrame);
            container.Singleton<IMessageService, MessageService>();

            //NOTE: don't register anything from the module assemblies here
            container.PerRequest<MainPageViewModel>();
            
            //NOTE: install the conventions
            ModuleConventions.Install();
        }

        protected override IEnumerable<Assembly> SelectAssemblies() {
            //NOTE: don't reference the module assemblies here
            return base.SelectAssemblies();
        }

        protected override object GetInstance(Type service, string key) {
            //NOTE: initialize the assembly (module)
            if (service != null)
                ModuleConventions.InitializeAssembly(service.Assembly);
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service) {
            //NOTE: initialize the assembly (module)
            ModuleConventions.InitializeAssembly(service.Assembly);
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance) {
            container.BuildUp(instance);
        }
    }
}
