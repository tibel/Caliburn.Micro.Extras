namespace Samples.ContentHost.ViewModels {
    using Caliburn.Micro;
    using System;
    using System.Linq;

    public class ShellViewModel : Conductor<Screen>.Collection.OneActive, IShell {
        public string Status { get; set; }

        private TimeSpan lastElapsed;

        protected override void OnInitialize() {
            Items.Add(new TextViewModel {DisplayName = "Info tab 1"});
            Items.Add(new TextViewModel {DisplayName = "Info tab 2"});

            ActivateItem(Items.First());
        }

        public void AddScreen() {
            var newScreen = new MyScreenViewModel {DisplayName = string.Concat("Tab ", Items.Count + 1)};
            Items.Add(newScreen);
        }

        public void RemoveScreen() {
            if (Items.Count < 1) {
                return;
            }

            var screenToRemove = Items.Last();
            DeactivateItem(screenToRemove, true);
            Items.Remove(screenToRemove);
        }

        public void MoveToNext(object view) {
            var currentIndex = Items.IndexOf(ActiveItem);
            var nextIndex = currentIndex != Items.Count - 1 ? currentIndex + 1 : 0;

            var startTimestamp = DateTime.Now;

            ActivateItem(Items[nextIndex]);

            var element = view as System.Windows.UIElement;
            if (element != null)
                element.UpdateLayout();

            var finishTimestamp = DateTime.Now;

            var elapsed = finishTimestamp - startTimestamp;

            Status = string.Format("Elapsed {0:0} ms (diff {1:0} ms)", elapsed.TotalMilliseconds,
                                   (elapsed - lastElapsed).TotalMilliseconds);
            NotifyOfPropertyChange(() => Status);
            lastElapsed = elapsed;
        }
    }
}
