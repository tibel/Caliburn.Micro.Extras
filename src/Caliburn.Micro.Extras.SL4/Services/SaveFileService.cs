namespace Caliburn.Micro.Extras {
    using System.IO;

#if SILVERLIGHT
    using System.Windows.Controls;
#else
    using Microsoft.Win32;
#endif

    /// <summary>
    /// Service to save files.
    /// </summary>
    public class SaveFileService : ISaveFileSerivce {
        readonly SaveFileDialog saveFileDialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileService"/> class.
        /// </summary>
        public SaveFileService() {
            saveFileDialog = new SaveFileDialog();
        }

        /// <summary>
        /// Gets or sets the default file name extension applied to files that are saved.
        /// </summary>
        public string DefaultExt
        {
            get { return saveFileDialog.DefaultExt; }
            set { saveFileDialog.DefaultExt = value; }
        }

        /// <summary>
        /// Gets or sets a filter string that specifies the files types and descriptions to display.
        /// </summary>
        public string Filter
        {
            get { return saveFileDialog.Filter; }
            set { saveFileDialog.Filter = value; }
        }

        /// <summary>
        /// Gets the file name for the selected file.
        /// </summary>
        public string SafeFileName
        {
            get { return saveFileDialog.SafeFileName; }
        }

#if NET
        /// <summary>
        ///  Gets or sets the initial directory displayed by the file dialog box.
        /// </summary>
        public string InitialDirectory
        {
            get { return saveFileDialog.InitialDirectory; }
            set { saveFileDialog.InitialDirectory = value; }
        }

        /// <summary>
        /// Gets or sets a string shown in the title bar of the file dialog.
        /// </summary> 
        public string Title
        {
            get { return saveFileDialog.Title; }
            set { saveFileDialog.Title = value; }
        }
#endif

        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if a file is selected; otherwise <c>false</c>.
        /// </returns>
        public bool DetermineFile() {
            return saveFileDialog.ShowDialog().GetValueOrDefault();
        }

        /// <summary>
        /// Opens the file specified by the <see cref="SafeFileName" /> property.
        /// </summary>
        /// <returns>
        /// A read-write stream for the file.
        /// </returns>
        public Stream OpenFile() {
            return saveFileDialog.OpenFile();
        }
    }
}
