using Caliburn.Micro;
using Caliburn.Micro.Extras.ExternalModules;
using Samples.HelloWP71.Module1.Feature;

namespace Samples.HelloWP71.Module1
{
    public class Bootstrapper : ModuleBootstrapperBase
    {
        protected override void Configure(PhoneContainer container)
        {
            container.PerRequest<MyPageViewModel>();
            container.PerRequest<FeaturePageViewModel>();
        }
    }
}
