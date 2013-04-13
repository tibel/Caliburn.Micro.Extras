using Caliburn.Micro;
using Caliburn.Micro.Extras;

namespace Samples.HelloWP71.Module1.Feature {
    public class FeaturePageViewModel : Screen {
        private readonly IMessageService messageService;

        public FeaturePageViewModel(IMessageService messageService) {
            this.messageService = messageService;
        }

        public void Testi() {
            messageService.Show("Executing feature");
        }
    }
}
