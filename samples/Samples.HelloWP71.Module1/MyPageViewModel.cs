using Caliburn.Micro;
using Caliburn.Micro.Extras;

namespace Samples.HelloWP71.Module1 {
    public class MyPageViewModel : Screen {
        private readonly IMessageService messageService;

        public MyPageViewModel(IMessageService messageService) {
            this.messageService = messageService;
        }

        public void HelloFromModule1() {
            messageService.Show("Hello from module1");
        }
    }
}
