namespace Samples.ContentHost.ViewModels {
    using Caliburn.Micro;
    using System.Collections.Generic;
    using System.Linq;

    public class MyScreenViewModel : Screen {
        public IEnumerable<Customer> Data { get; set; }

        protected override void OnInitialize() {
            Data = GenerateCustomers(150).ToList();
            NotifyOfPropertyChange(() => Data);
        }

        private static IEnumerable<Customer> GenerateCustomers(int count) {
            for (var i = 1; i <= count; i++) {
                yield return new Customer(i);
            }
        }
    }
}
