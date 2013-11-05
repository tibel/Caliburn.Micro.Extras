namespace Caliburn.Micro.Extras {
    using System.IO;

    /// <summary>
    /// Interface for the Save File service.
    /// </summary>
    public interface ISaveFileSerivce {
        /// <summary>
        /// Gets or sets the default file name extension applied to files that are saved.
        /// </summary>
        string DefaultExt { get; set; }

        /// <summary>
        /// Gets or sets a filter string that specifies the files types and descriptions to display.
        /// </summary>
        string Filter { get; set; }

        /// <summary>
        /// Gets the file name for the selected file.
        /// </summary>
        string SafeFileName { get; }

#if NET
        /// <summary>
        /// Gets or sets a value indicating whether this instance prompts the user for permission to create a file if the user specifies a file that does not exist.
        /// </summary>
        bool CreatePrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance displays a warning if the user specifies the name of a file that already exists.
        /// </summary>
        bool OverwritePrompt { get; set; }

        /// <summary>
        ///  Gets or sets the initial directory displayed by the file dialog box.
        /// </summary>
        string InitialDirectory { get; set; }

        /// <summary>
        /// Gets or sets a string shown in the title bar of the file dialog.
        /// </summary> 
        string Title { get; set; }
#endif

        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns><c>true</c> if a file is selected; otherwise <c>false</c>.</returns>
        bool DetermineFile();

        /// <summary>
        /// Opens the file specified by the <see cref="SafeFileName"/> property.
        /// </summary>
        /// <returns>A read-write stream for the file.</returns>
        Stream OpenFile();
    }
}
