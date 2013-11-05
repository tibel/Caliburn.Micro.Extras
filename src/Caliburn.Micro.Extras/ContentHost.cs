namespace Caliburn.Micro.Extras {
    using Micro;
    using System.Linq;
#if WinRT
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
#else
    using System.Windows;
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Custom <see cref="ContentControl "/> that caches all views so that it can quickly switch between them.
    /// </summary>
    /// <remarks>
    /// Models have to implement <see cref="IDeactivate"/> so views can be cached.
    /// </remarks>
    public class ContentHost : Grid {
        static ContentHost() {
            ConventionManager.AddElementConvention<ContentHost>(CurrentModelProperty, "CurrentModel", "Loaded");
        }

        /// <summary>
        /// Gets or sets the current model.
        /// </summary>
        public object CurrentModel {
            get { return GetValue(CurrentModelProperty); }
            set { SetValue(CurrentModelProperty, value); }
        }

        /// <summary>
        /// A dependency property for attaching a model to the UI.
        /// </summary>
        public static readonly DependencyProperty CurrentModelProperty =
            DependencyProperty.Register("CurrentModel", typeof (object), typeof (ContentHost),
                                        new PropertyMetadata(null, (s, e) => ((ContentHost) s).OnCurrentModelChanged(e)));

        void OnCurrentModelChanged(DependencyPropertyChangedEventArgs e) {
            var currentView = Children.OfType<UIElement>().FirstOrDefault(v => v.Visibility == Visibility.Visible);
            var newView = GetView(e.NewValue);
            BringToFront(newView, e.NewValue);
            SendToBack(currentView, e.OldValue);
        }

        UIElement GetView(object viewModel) {
            if (viewModel == null)
                return null;

            UIElement view = Children
                .OfType<FrameworkElement>()
                .FirstOrDefault(fe => ReferenceEquals(fe.DataContext, viewModel));
            if (view != null)
                return view;

            var context = View.GetContext(this);
            view = ViewLocator.LocateForModel(viewModel, this, context);
            ViewModelBinder.Bind(viewModel, view, context);
            return view;
        }

        void BringToFront(UIElement view, object viewModel) {
            if (view != null) {
                view.Visibility = Visibility.Visible;

                if (!Children.Contains(view)) {
                    SubscribeDeactivation(viewModel);
                    Children.Add(view);
                }
            }
        }

        void SendToBack(UIElement view, object viewModel) {
            if (view != null) {
                view.Visibility = Visibility.Collapsed;

                if (!(viewModel is IDeactivate)) {
                    Children.Remove(view);
                }
            }
        }

        void SubscribeDeactivation(object viewModel) {
            var deactivatable = viewModel as IDeactivate;
            if (deactivatable != null) {
                deactivatable.Deactivated += OnViewModelDeactivated;
            }
        }

        void OnViewModelDeactivated(object sender, DeactivationEventArgs e) {
            if (e.WasClosed) {
                var deactivatable = (IDeactivate) sender;
                deactivatable.Deactivated -= OnViewModelDeactivated;

                var view = Children
                    .OfType<FrameworkElement>()
                    .FirstOrDefault(fe => ReferenceEquals(fe.DataContext, sender));

                if (view != null) {
                    Children.Remove(view);
                }
            }
        }
    }
}
