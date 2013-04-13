using Caliburn.Micro;
using Samples.HelloWP71.Module1;
using Samples.HelloWP71.Module1.Feature;
using Samples.HelloWP71.Module2;

namespace Samples.HelloWP71 {
    public class MainPageViewModel : Screen {
        private readonly INavigationService navigation;
        private Screen another;

        public MainPageViewModel(INavigationService navigation) {
            this.navigation = navigation;
        }

        public Screen Another {
            get { return another; }
            set {
                another = value;
                NotifyOfPropertyChange(() => Another);
            }
        }

        public void GotoModule1() {
            navigation.UriFor<MyPageViewModel>()
                      .Navigate();
        }

        public void GotoFeature() {
            navigation.UriFor<FeaturePageViewModel>()
                      .Navigate();
        }

        public void LoadModule2() {
            var screen = IoC.Get<AnotherViewModel>();
            Another = screen;
        }
    }
}
