namespace Caliburn.Micro.Extras {
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// A Caliburn.Micro Result that lets you open a file.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public abstract class OpenFileResult<TResult> :
#if !SILVERLIGHT || SL5 || WP8
        IResult<TResult> {
#else
        IResult {
#endif

        readonly bool multiselect;
        readonly string title;
        string fileTypeFilter;
        string initialDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileResult&lt;TResult&gt;"/> class.
        /// </summary>
        /// <param name="multiselect">Determines wether it is allows to select multiple files.</param>
        /// <param name="title">The title of the dialog.</param>
        protected OpenFileResult(bool multiselect, string title = null) {
            this.multiselect = multiselect;
            this.title = title;
        }

        /// <summary>
        /// Gets the opened file(s).
        /// </summary>
        public TResult Result { get; protected set; }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(ActionExecutionContext context) {
            var openFileService = IoC.Get<IOpenFileService>();
            openFileService.Multiselect = multiselect;
            openFileService.Filter = fileTypeFilter;

#if NET
            openFileService.Title = title;
            openFileService.InitialDirectory = initialDirectory;
#endif

            var fileSelected = openFileService.DetermineFile();
            OnCompleted(openFileService, new ResultCompletionEventArgs {WasCancelled = !fileSelected});
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        /// <summary>
        /// Handles the completion of the execution.
        /// </summary>
        /// <param name="openFileService">The open file service.</param>
        /// <param name="args">The <see cref="ResultCompletionEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCompleted(IOpenFileService openFileService, ResultCompletionEventArgs args) {
            Completed(this, args);
        }

        /// <summary>
        /// Create file filter for the dialog.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public OpenFileResult<TResult> FilterFiles(string filter) {
            fileTypeFilter = filter;
            return this;
        }

        /// <summary>
        /// Sets the initial <paramref name = "directory" /> of the dialog
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns></returns>
        public OpenFileResult<TResult> In(string directory) {
            if (!Directory.Exists(directory))
                throw new ArgumentException(string.Format("Directory '{0}' doesn't exist", directory), "directory");

            initialDirectory = directory;
            return this;
        }

        /// <summary>
        /// Open a single file.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <returns></returns>
        public static OpenFileResult<FileInfo> OneFile(string title = null) {
            return new OneFileResult(title);
        }

        /// <summary>
        /// Open multiple files.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <returns></returns>
        public static OpenFileResult<IEnumerable<FileInfo>> MultipleFiles(string title = null) {
            return new MultipleFilesResult(title);
        }

        class OneFileResult : OpenFileResult<FileInfo> {
            public OneFileResult(string title) : base(false, title) { }

            protected override void OnCompleted(IOpenFileService openFileService, ResultCompletionEventArgs args) {
                if (!args.WasCancelled)
                    Result = openFileService.File;

                base.OnCompleted(openFileService, args);
            }
        }

        class MultipleFilesResult : OpenFileResult<IEnumerable<FileInfo>> {
            public MultipleFilesResult(string title) : base(true, title) {
                Result = new FileInfo[0];
            }

            protected override void OnCompleted(IOpenFileService openFileService, ResultCompletionEventArgs args) {
                if (!args.WasCancelled)
                    Result = openFileService.Files;

                base.OnCompleted(openFileService, args);
            }
        }
    }
}
