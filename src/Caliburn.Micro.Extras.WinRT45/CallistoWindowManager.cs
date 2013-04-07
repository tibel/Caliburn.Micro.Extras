namespace Caliburn.Micro.Extras {
    using Callisto.Controls;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Windows.UI.Xaml;

    /// <summary>
    /// An implementation of the <see cref="IWindowManager" /> using Callisto
    /// </summary>
    public class CallistoWindowManager : CallistoSettingsWindowManager, IWindowManager {
        /// <summary>
        /// Shows a popup dialog for the specified model relative to the <paramref name="placementTarget"/>.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="placementTarget">The placement target.</param>
        /// <param name="viewSettings">The optional dialog settings.</param> 
        public void ShowFlyout(object rootModel, UIElement placementTarget, IDictionary<string, object> viewSettings = null) {
            var view = ViewLocator.LocateForModel(rootModel, null, null);
            ViewModelBinder.Bind(rootModel, view, null);

            var flyout = new Flyout
                {
                    Content = view,
                    PlacementTarget = placementTarget,
                };

            ApplySettings(flyout, viewSettings);
            flyout.IsOpen = true;

            var deactivator = rootModel as IDeactivate;
            if (deactivator != null) {
                EventHandler<object> closed = null;
                closed = (s, e) => {
                    deactivator.Deactivate(true);
                    flyout.Closed -= closed;
                };

                flyout.Closed += closed;
            }

            var activator = rootModel as IActivate;
            if (activator != null) {
                activator.Activate();
            }
        }

        static bool ApplySettings(object target, IEnumerable<KeyValuePair<string, object>> settings) {
            if (settings == null)
                return false;

            var type = target.GetType();

            foreach (var pair in settings) {
                var propertyInfo = type.GetRuntimeProperty(pair.Key);

                if (propertyInfo != null)
                    propertyInfo.SetValue(target, pair.Value, null);
            }

            return true;
        }
    }
}
