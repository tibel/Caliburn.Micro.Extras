namespace Caliburn.Micro.Extras {
    using System;
    using System.Linq;

    /// <summary>
    /// Base class for all module bootstrappers.
    /// </summary>
    public abstract class ModuleBootstrapperBase : IModuleBootstrapper {
        /// <summary>
        /// Gets or sets the IoC container.
        /// </summary>
        public PhoneContainer Container { get; set; }

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public virtual void Initialize() {
            if (Container == null)
                throw new InvalidOperationException("Container has to be initialized.");

            ConfigureStorageMechanismsAndWorkers(Container);
            Configure(Container);
        }

        /// <summary>
        /// Identify, load and configure all instances of <see cref="IStorageMechanism"/> and <see cref="IStorageHandler"/> 
        /// that are defined in the assembly associated with this bootstrapper.
        /// </summary>
        /// <param name="phoneContainer">The currently configured <see cref="PhoneContainer"/>.</param>
        /// <remarks>
        /// Caliburn Micro will automatically load storage handlers and storage mechanisms from the assemblies configured
        /// in <see cref="AssemblySource.Instance"/> when <see cref="PhoneContainer.RegisterPhoneServices"/> is first invoked.
        /// Since the purpose of this bootstrapper is to allow the delayed loading of assemblies, it makes sense to locate
        /// the storage handlers alongside the view models in the same assembly. 
        /// </remarks>
        private void ConfigureStorageMechanismsAndWorkers(PhoneContainer phoneContainer) {
            var coordinator = (StorageCoordinator) (phoneContainer.GetInstance(typeof (StorageCoordinator), null));
            var assembly = GetType().Assembly;

            phoneContainer.AllTypesOf<IStorageMechanism>(assembly);
            phoneContainer.AllTypesOf<IStorageHandler>(assembly);

            phoneContainer.GetAllInstances(typeof (IStorageMechanism)).
                           Where(m => ReferenceEquals(m.GetType().Assembly, assembly)).
                           Apply(m => coordinator.AddStorageMechanism((IStorageMechanism) m));

            phoneContainer.GetAllInstances(typeof (IStorageHandler)).
                           Where(h => ReferenceEquals(h.GetType().Assembly, assembly)).
                           Apply(h => coordinator.AddStorageHandler((IStorageHandler) h));
        }

        /// <summary>
        /// Override to setup the IoC container for this module.
        /// </summary>
        /// <param name="container">The parent IoC container.</param>
        protected abstract void Configure(PhoneContainer container);
    }
}
