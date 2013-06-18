using Caliburn.Micro;
using Caliburn.Micro.Extras;

namespace Samples.HelloWP71.Module2
{
    public class Bootstrapper : ModuleBootstrapperBase
    {
        protected override void Configure(PhoneContainer container)
        {
            container.PerRequest<AnotherViewModel>();
        }
    }
}
