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
            this.company = company;
        }

        [Required(ErrorMessage = @"Name is required.")]
        public string CName {
            get { return company.Name; }
            set {
                company.Name = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage = @"Address is required.")]
        public string Address {
            get { return company.Address; }
            set {
                company.Address = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage = @"Website is required.")]
        [RegularExpression(@"^http(s)?://[a-z0-9-]+(.[a-z0-9-]+)*(:[0-9]+)?(/.*)?$", ErrorMessage = @"The format of the web address is not valid")]
        public string Website {
            get { return company.Website; }
            set {
                company.Website = value;
                OnPropertyChanged();
            }
        }

        public string Contact {
            get { return company.Contact; }
            set {
                company.Contact = value;
                OnPropertyChanged();
            }
        }

        public void Save() {
        }

        public bool CanSave {
            get { return validation.Errors.Count == 0; }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") {
            NotifyOfPropertyChange(propertyName);
            validation.Validate(this, propertyName);
            NotifyOfPropertyChange("CanSave");
        }

        #region Validation

        private readonly DataErrorInfoAdapter<ShellViewModel> validation = new DataErrorInfoAdapter<ShellViewModel>();

        public string Error {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName] {
            get { return validation.GetPropertyError(columnName); }
        }

        #endregion
    }
}
