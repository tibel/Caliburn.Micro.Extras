namespace Caliburn.Micro.Extras {
    using System;
    using System.Threading.Tasks;
    using Windows.UI.Popups;

    internal enum MessageBoxButton {
        OK = 0,
        OKCancel = 1,
        YesNoCancel = 3,
        YesNo = 4,
    }

    internal enum MessageBoxResult {
        None = 0,
        OK = 1,
        Cancel = 2,
        Yes = 6,
        No = 7,
    }

    internal static class MessageBox {
        public static async Task<MessageBoxResult> ShowAsync(string message, string caption = "", MessageBoxButton button = MessageBoxButton.OK) {
            var result = MessageBoxResult.None;
            var messageDialog = new MessageDialog(message, caption);

            if (button == MessageBoxButton.OK || button == MessageBoxButton.OKCancel) {
                messageDialog.Commands.Add(new UICommand("OK", cmd => result = MessageBoxResult.OK));
            }

            if (button == MessageBoxButton.YesNo || button == MessageBoxButton.YesNoCancel) {
                messageDialog.Commands.Add(new UICommand("Yes", cmd => result = MessageBoxResult.Yes));
                messageDialog.Commands.Add(new UICommand("No", cmd => result = MessageBoxResult.No));
            }

            if (button == MessageBoxButton.OKCancel || button == MessageBoxButton.YesNoCancel) {
                messageDialog.Commands.Add(new UICommand("Cancel", cmd => result = MessageBoxResult.Cancel));
                messageDialog.CancelCommandIndex = (uint) messageDialog.Commands.Count - 1;
            }

            await messageDialog.ShowAsync();
            return result;
        }
    }
}
