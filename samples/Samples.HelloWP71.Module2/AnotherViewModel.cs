using System.Windows;
using Caliburn.Micro;

namespace Samples.HelloWP71.Module2
{
    public class AnotherViewModel : Screen
    {
        public AnotherViewModel() {
            base.DisplayName = "another";
        }

        public void Testi()
        {
            MessageBox.Show("Module2 says hello");
        }
    }
}
