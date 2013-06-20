namespace Samples.ContentHost {
    using System;
    using System.Globalization;

    public class Customer {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string State { get; set; }

        public DateTime Born { get; set; }

        public DateTime LastOrderPlaced { get; set; }
        public int LastOrderItemsCount { get; set; }
        public decimal LastOrderPrice { get; set; }

        public Customer() {
        }

        public Customer(int ordinalNumber) {
            Id = ordinalNumber;
            FirstName = string.Concat("Johnny ", ordinalNumber);
            LastName = string.Concat("McNavarra ", ordinalNumber);

            Street = string.Concat("Blue Road", ordinalNumber);
            StreetNumber = ordinalNumber.ToString(CultureInfo.InvariantCulture);

            City = string.Concat("New Valley ", ordinalNumber);
            Zip = string.Concat(ordinalNumber, " ", ordinalNumber);
            State = string.Concat("California ", ordinalNumber);

            Born = DateTime.Now.AddYears(-1*ordinalNumber);
            LastOrderPlaced = DateTime.Now.AddDays(-1*ordinalNumber);
            LastOrderItemsCount = ordinalNumber;
            LastOrderPrice = ordinalNumber*123;
        }
    }
}
