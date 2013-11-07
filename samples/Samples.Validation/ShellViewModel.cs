using Caliburn.Micro;
using Caliburn.Micro.Extras;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Samples.Validation {
    public class ShellViewModel : Screen, IShell, IDataErrorInfo {
        private readonly Company company;

        public ShellViewModel(Company company) {
            validation.Validators.Add(new DataAnnotationsValidator(GetType()));
            this.company = company;
        }

        [Required(ErrorMessage = @"Name is required.")]
        public string CName {
            get { return company.Name; }
            set {
                company.Name = value;
                OnPropertyChanged(value);
            }
        }

        [Required(ErrorMessage = @"Address is required.")]
        public string Address {
            get { return company.Address; }
            set {
                company.Address = value;
                OnPropertyChanged(value);
            }
        }

        [Required(ErrorMessage = @"Website is required.")]
        [RegularExpression(@"^http(s)?://[a-z0-9-]+(.[a-z0-9-]+)*(:[0-9]+)?(/.*)?$", ErrorMessage = @"The format of the web address is not valid")]
        public string Website {
            get { return company.Website; }
            set {
                company.Website = value;
                OnPropertyChanged(value);
            }
        }

        public string Contact {
            get { return company.Contact; }
            set {
                company.Contact = value;
                OnPropertyChanged(value);
            }
        }

        public IResult Save() {
            return new MessengerResult("Your changes where saved.")
                .Caption("Save")
                .Image(MessageImage.Information);
        }

        public bool CanSave {
            get { return !validation.HasErrors; }
        }

        protected void OnPropertyChanged(object value, [CallerMemberName] string propertyName = "") {
            validation.ValidateProperty(propertyName, value);
            NotifyOfPropertyChange(propertyName);
            NotifyOfPropertyChange(() => CanSave);
        }

        #region Validation

        private readonly ValidationAdapter validation = new ValidationAdapter();

        public string Error {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName] {
            get { return string.Join(Environment.NewLine, validation.GetPropertyError(columnName)); }
        }

        #endregion
    }
}
