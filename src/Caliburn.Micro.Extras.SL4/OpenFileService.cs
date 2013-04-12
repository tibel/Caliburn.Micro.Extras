namespace Caliburn.Micro.Extras {
    using System.Collections.Generic;
    using System.IO;

#if SILVERLIGHT
    using System.Windows.Controls;
#else
    using System.Linq;
    using Microsoft.Win32;
#endif

    /// <summary>
    /// Service to open files.
    /// </summary>
    public class OpenFileService : IOpenFileService {
        private readonly OpenFileDialog openFileDialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileService"/> class.
        /// </summary>
        public OpenFileService() {
            openFileDialog = new OpenFileDialog();
        }

        /// <summary>
        /// Gets a <see cref="FileInfo" /> object for the selected file. If multiple files are selected, returns the first selected file.
        /// </summary>
        public FileInfo File {
            get {
#if SILVERLIGHT
                return openFileDialog.File;
#else
                return new FileInfo(openFileDialog.FileName);
#endif
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="FileInfo" /> objects for the selected files.
        /// </summary>
        public IEnumerable<FileInfo> Files {
            get {
#if SILVERLIGHT
                return openFileDialog.Files;
#else
                return openFileDialog.FileNames.Select(name => new FileInfo(name));
#endif
            }
        }

        /// <summary>
        /// Gets or sets a filter string that specifies the file types and descriptions to display.
        /// </summary>
        public string Filter {
            get { return openFileDialog.Filter; }
            set { openFileDialog.Filter = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is allows to select multiple files.
        /// </summary>
        public bool Multiselect {
            get { return openFileDialog.Multiselect; }
            set { openFileDialog.Multiselect = value; }
        }

        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if a file is selected; otherwise <c>false</c>.
        /// </returns>
        public bool DetermineFile() {
            return openFileDialog.ShowDialog().GetValueOrDefault();
        }
    }
}
