namespace Caliburn.Micro.Extras {
    using System;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// A Caliburn.Micro Result that lets you save a file.
    /// </summary>
    public class SaveFileResult : IResult<Stream> {

        Lazy<Stream> lazyStream;
        readonly string title;
        string fileTypeFilter;
        string initialDirectory;
        string defaultFileExtension;
#if NET
        bool promptForOverwrite;
        bool promptForCreate;
#endif

        /// <summary>
        /// Gets the opened file(s).
        /// </summary>
        public Stream Result {
            get { return lazyStream.Value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileResult"/> class.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        protected SaveFileResult(string title = null) {
            this.title = title;
            lazyStream = new Lazy<Stream>(() => null);
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(ActionExecutionContext context) {
            var saveFileService = (ISaveFileService) IoC.GetAllInstances(typeof (ISaveFileService)).FirstOrDefault() ??
                                  new SaveFileService();
            saveFileService.Filter = fileTypeFilter;
            saveFileService.DefaultExt = defaultFileExtension;

#if NET
            saveFileService.Title = title;
            saveFileService.InitialDirectory = initialDirectory;
            saveFileService.CreatePrompt = promptForCreate;
            saveFileService.OverwritePrompt = promptForOverwrite;
#endif

            var fileSelected = saveFileService.DetermineFile();
            OnCompleted(saveFileService, new ResultCompletionEventArgs { WasCancelled = !fileSelected });
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        /// <summary>
        /// Handles the completion of the execution.
        /// </summary>
        /// <param name="saveFileSerivce">The save file service.</param>
        /// <param name="args">The <see cref="ResultCompletionEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCompleted(ISaveFileService saveFileSerivce, ResultCompletionEventArgs args) {
            if (!args.WasCancelled)
                lazyStream = new Lazy<Stream>(saveFileSerivce.OpenFile);

            Completed(this, args);
        }

        /// <summary>
        /// Create file filter for the dialog.
        /// </summary>
        /// <param name="filter">The file type filter.</param>
        /// <param name="defaultExtension">The default file name extension applied to files that are saved.</param>
        /// <returns></returns>
        public SaveFileResult FilterFiles(string filter, string defaultExtension) {
            fileTypeFilter = filter;
            defaultFileExtension = defaultExtension;
            return this;
        }

        /// <summary>
        /// Sets the initial <paramref name = "directory" /> of the dialog
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns></returns>
        public SaveFileResult In(string directory) {
            initialDirectory = directory;
            return this;
        }

#if NET
        /// <summary>
        /// Ask the user for permission if the file will be overriden.
        /// </summary>
        /// <returns></returns>
        public SaveFileResult PromptForOverwrite() {
            promptForOverwrite = true;
            return this;
        }

        /// <summary>
        /// Ask the user for permission if a new file will be created.
        /// </summary>
        /// <returns></returns>
        public SaveFileResult PromptForCreate() {
            promptForCreate = true;
            return this;
        }
#endif

        /// <summary>
        /// Save a single file.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <returns></returns>
        public static SaveFileResult OneFile(string title = null) {
            return new SaveFileResult(title);
        }
    }
}
