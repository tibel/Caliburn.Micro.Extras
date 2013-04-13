namespace Caliburn.Micro.Extras {
    using System;
    using System.Linq;
    using System.Windows;

#if !SILVERLIGHT || SL5 || WP8
    using System.Threading.Tasks;
#endif

    /// <summary>
    /// Message service that implements the <see cref="IMessageService"/> by using the <see cref="MessageBox"/> class.
    /// </summary>
    public class MessageService : IMessageService {
        static MessageResult TranslateMessageBoxResult(MessageBoxResult result) {
            var value = result.ToString();
            return (MessageResult)Enum.Parse(typeof(MessageResult), value, true);
        }

#if NET
		static MessageBoxImage TranslateMessageImage(MessageImage image) {
            var value = image.ToString();
            return (MessageBoxImage)Enum.Parse(typeof(MessageBoxImage), value, true);
		}

        static Window GetActiveWindow() {
            if (Application.Current == null) {
                return null;
            }

            var active = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            return active ?? Application.Current.MainWindow;
        }
#endif

        static MessageBoxButton TranslateMessageButton(MessageButton button) {
            try {
                var value = button.ToString();
                return (MessageBoxButton) Enum.Parse(typeof (MessageBoxButton), value, true);
            }
            catch (Exception) {
                throw new NotSupportedException(string.Format("Unfortunately, the default MessageBox class of does not support '{0}' button.", button));
            }
        }

        /// <summary>
        /// Shows the specified message and returns the result.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="icon">The icon.</param>
        /// <returns>The <see cref="MessageResult"/>.</returns>
        public MessageResult Show(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None) {
#if WinRT
            return ShowMessageBox(message, caption, button, icon).Result;
#else
            return ShowMessageBox(message, caption, button, icon);
#endif
        }

#if !SILVERLIGHT || SL5 || WP8
        /// <summary>
        /// Shows the specified message and allows to await for the message to complete.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="icon">The icon.</param>
        /// <returns>A Task containing the <see cref="MessageResult"/>.</returns>
        public Task<MessageResult> ShowAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None) {
#if WinRT
            return ShowMessageBox(message, caption, button, icon);
#else
            var taskSource = new TaskCompletionSource<MessageResult>();
            try {
                var result = ShowMessageBox(message, caption, button, icon);
                taskSource.SetResult(result);
            }
            catch (Exception ex) {
                taskSource.SetException(ex);
            }
            return taskSource.Task;
#endif
        }
#endif

#if WinRT
        static async Task<MessageResult> ShowMessageBox(string message, string caption, MessageButton button, MessageImage icon) {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");
            if (icon != MessageImage.None)
                throw new NotSupportedException("Unfortunately, the default MessageBox class does not support setting an icon.");

            var messageBoxButton = TranslateMessageButton(button);
            var result = await MessageBox.ShowAsync(message, caption, messageBoxButton);
            return TranslateMessageBoxResult(result);
        }
#else
        static MessageResult ShowMessageBox(string message, string caption, MessageButton button, MessageImage icon) {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");

            var result = MessageBoxResult.None;
            var messageBoxButton = TranslateMessageButton(button);

#if NET
            var messageBoxImage = TranslateMessageImage(icon);

            var activeWindow = GetActiveWindow();
            if (activeWindow != null) {
                result = MessageBox.Show(activeWindow, message, caption, messageBoxButton, messageBoxImage);
            }
            else {
                result = MessageBox.Show(message, caption, messageBoxButton, messageBoxImage);
            }
#else
            result = MessageBox.Show(message, caption, messageBoxButton);
#endif

            return TranslateMessageBoxResult(result);
        }
#endif
    }
}
