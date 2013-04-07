namespace Caliburn.Micro.Extras {
    using System.Collections.Generic;
    using Windows.UI.Xaml;

    /// <summary>
    /// A service that manages windows.
    /// </summary>
    public interface IWindowManager : ISettingsWindowManager {
        /// <summary>
        /// Shows a popup dialog for the specified model relative to the <paramref name="placementTarget"/>.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="placementTarget">The placement target.</param>
        /// <param name="viewSettings">The optional dialog settings.</param> 
        void ShowFlyout(object rootModel, UIElement placementTarget, IDictionary<string, object> viewSettings = null);
    }
}
