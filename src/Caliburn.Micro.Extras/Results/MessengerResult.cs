namespace Caliburn.Micro.Extras {
    using System;
    using System.Linq;
    
    /// <summary>
    /// A Caliburn.Micro Result that lets you show messages.
    /// </summary>
    public class MessengerResult : IResult<MessageResult> {
        readonly string message;
        string caption = "";
        MessageButton button = MessageButton.OK;
        MessageImage image = MessageImage.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessengerResult"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessengerResult(string message) {
            this.message = message;
            Result = MessageResult.None;
        }

        /// <summary>
        /// Gets the message
        /// </summary>
        public MessageResult Result { get; protected set; }

        /// <summary>
        /// Sets the caption.
        /// </summary>
        /// <param name="text">The caption text.</param>
        /// <returns></returns>
        public MessengerResult Caption(string text = "") {
            caption = text;
            return this;
        }

        /// <summary>
        /// Sets the button.
        /// </summary>
        /// <param name="buttons">The button.</param>
        /// <returns></returns>
        public MessengerResult Buttons(MessageButton buttons = MessageButton.OK) {
            button = buttons;
            return this;
        }

        /// <summary>
        /// Sets the image.
        /// </summary>
        /// <param name="icon">The image.</param>
        /// <returns></returns>
        public MessengerResult Image(MessageImage icon = MessageImage.None) {
            image = icon;
            return this;
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(CoroutineExecutionContext context) {
            var messageService = (IMessageService) IoC.GetAllInstances(typeof (IMessageService)).FirstOrDefault() ??
                                 new MessageService();
            Result = messageService.Show(message, caption, button, image);
            Completed(this, new ResultCompletionEventArgs());
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}
