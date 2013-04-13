using Caliburn.Micro;
using Caliburn.Micro.Extras;

namespace Samples.HelloWP71.Module2 {
    public class AnotherViewModel : Screen {
        private readonly IMessageService messageService;

        public AnotherViewModel(IMessageService messageService) {
            this.messageService = messageService;
            base.DisplayName = "another";
        }

        public void Testi() {
            messageService.Show("Module2 says hello");
        }
    }
}
