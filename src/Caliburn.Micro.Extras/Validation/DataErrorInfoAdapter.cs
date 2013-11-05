namespace Caliburn.Micro.Extras {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;

    /// <summary>
    /// Adapter for <see cref="IDataErrorInfo"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public class DataErrorInfoAdapter<TViewModel> where TViewModel : IDataErrorInfo {
        private readonly IDictionary<string, IList<string>> validationErrors = new Dictionary<string, IList<string>>();

        /// <summary>
        /// Validates all properties.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public bool ValidateAll(TViewModel instance) {
            var errors = DataAnnotationsValidator.Validate(instance);

            validationErrors.Clear();
            foreach (var error in errors) {
                if (error.Value.Count > 0)
                    validationErrors.Add(error);
            }

            return validationErrors.Count == 0;
        }

        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="onValidation">The on validation callback.</param>
        /// <returns></returns>
        public bool Validate(TViewModel instance, string propertyName, Action<string, IList<string>> onValidation = null) {
            var values = DataAnnotationsValidator.Validate(instance, propertyName);

            if (onValidation != null)
                onValidation(propertyName, values);

            if (values.Count == 0)
                validationErrors.Remove(propertyName);
            else
                validationErrors[propertyName] = values;

            return values.Count == 0;
        }

        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="property">The property.</param>
        /// <param name="onValidation">The on validation callback.</param>
        /// <returns></returns>
        public bool Validate<TProperty>(TViewModel instance, Expression<Func<TProperty>> property,
                                        Action<string, IList<string>> onValidation = null) {
            return Validate(instance, property.GetMemberInfo().Name, onValidation);
        }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        public IDictionary<string, IList<string>> Errors {
            get { return validationErrors; }
        }

        /// <summary>
        /// Gets the property error.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public string GetPropertyError(string propertyName) {
            IList<string> errors;
            if (Errors.TryGetValue(propertyName, out errors))
                return string.Join(Environment.NewLine, errors);
            return string.Empty;
        }

        /// <summary>
        /// Gets the property error.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public string GetPropertyError<TProperty>(Expression<Func<TProperty>> property) {
            return GetPropertyError(property.GetMemberInfo().Name);
        }

        /// <summary>
        /// Determines whether specified property has any errors.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public bool HasPropertyError(string propertyName) {
            IList<string> errors;
            if (Errors.TryGetValue(propertyName, out errors))
                return errors.Count > 0;
            return false;
        }

        /// <summary>
        /// Determines whether specified property has any errors.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public bool HasPropertyError<TProperty>(Expression<Func<TProperty>> property) {
            return HasPropertyError(property.GetMemberInfo().Name);
        }
    }
}
