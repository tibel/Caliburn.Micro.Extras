namespace Caliburn.Micro.Extras {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Helper class to use <see cref="System.ComponentModel.DataAnnotations"/> attributes for validation.
    /// </summary>
    public static class DataAnnotationsValidator {
        static class Helpers<TViewModel> {
            public static readonly Dictionary<string, Func<TViewModel, object>> PropertyGetters =
                (from p in typeof (TViewModel).GetProperties()
                 where p.GetAttributes<ValidationAttribute>(true).ToArray().Length != 0
                 select p
                ).ToDictionary(p => p.Name, GetValueGetter);

            public static readonly Dictionary<string, ValidationAttribute[]> Validators =
                (from p in typeof (TViewModel).GetProperties()
                 let attrs = p.GetAttributes<ValidationAttribute>(true).ToArray()
                 where attrs.Length != 0
                 select new KeyValuePair<string, ValidationAttribute[]>(p.Name, attrs)
                ).ToDictionary(p => p.Key, p => p.Value);

            private static Func<TViewModel, object> GetValueGetter(PropertyInfo property) {
                var instance = Expression.Parameter(typeof (TViewModel), "i");
                var cast = Expression.TypeAs(Expression.Property(instance, property), typeof (object));
                return (Func<TViewModel, object>) Expression.Lambda(cast, instance).Compile();
            }
        }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static IDictionary<string, IList<string>> Validate<TViewModel>(TViewModel instance) {
            return Helpers<TViewModel>.PropertyGetters.Keys.ToDictionary(p => p, p => Validate(instance, p));
        }

        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static IList<string> Validate<TViewModel>(TViewModel instance, string propertyName) {
            Func<TViewModel, object> valueGetter;
            if (!Helpers<TViewModel>.PropertyGetters.TryGetValue(propertyName, out valueGetter))
                return new List<string>();

            var value = valueGetter(instance);

#if SILVERLIGHT
            var context = new ValidationContext(instance, null, null);
            return (from v in Helpers<TViewModel>.Validators[propertyName]
                    where v.GetValidationResult(value, context) != ValidationResult.Success
                    select v.FormatErrorMessage(propertyName)).ToList();
#elif WinRT
            var context = new ValidationContext(instance);
            return (from v in Helpers<TViewModel>.Validators[propertyName]
                    where v.GetValidationResult(value, context) != ValidationResult.Success
                    select v.FormatErrorMessage(propertyName)).ToList();
#else
            return (from v in Helpers<TViewModel>.Validators[propertyName]
                    where !v.IsValid(value)
                    select v.FormatErrorMessage(propertyName)).ToList();
#endif
        }

        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static IList<string> Validate<TViewModel, TProperty>(TViewModel instance, Expression<Func<TProperty>> property) {
            return Validate(instance, property.GetMemberInfo().Name);
        }
    }
}
