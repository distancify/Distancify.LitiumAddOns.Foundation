using System;

namespace Distancify.LitiumAddOns.Foundation.Models
{
    public class AddressModel
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string CareOf { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public object CustomData { get; set; }//Customers.Address only
        public string HouseExtension { get; set; }
        public string HouseNumber { get; set; }
        public string State { get; set; }
        public Guid SystemId { get; set; }
        public string ZipCode { get; set; }//"Zip" in ECommerce.Addresses.Address
        public string PhoneNumber { get; set; }//"Phone" in ECommerce.Addresses.Address

        //ECommerce.Addresses.Address only:
        public string Email { get; set; }
        public string Fax { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }
        public string OrganizationName { get; set; }
        public string Title { get; set; }

        public AddressModel() { }

        public AddressModel(Litium.Customers.Address address)
        {
            MapFrom(address);
        }

        public AddressModel(Litium.Foundation.Modules.ECommerce.Addresses.Address address)
        {
            MapFrom(address);
        }

        public virtual void MapFrom(Litium.Customers.Address address)
        {
            Address1 = address.Address1;
            Address2 = address.Address2;
            CareOf = address.CareOf;
            City = address.City;
            Country = address.Country;
            CustomData = address.CustomData;
            HouseExtension = address.HouseExtension;
            HouseNumber = address.HouseNumber;
            State = address.State;
            SystemId = address.SystemId;
            ZipCode = address.ZipCode;
            PhoneNumber = address.PhoneNumber;
        }

        public virtual void MapFrom(Litium.Foundation.Modules.ECommerce.Addresses.Address address)
        {
            Address1 = address.Address1;
            Address2 = address.Address2;
            CareOf = address.CareOf;
            City = address.City;
            Country = address.Country;
            HouseExtension = address.HouseExtension;
            HouseNumber = address.HouseNumber;
            State = address.State;
            SystemId = address.ID;
            ZipCode = address.Zip;
            PhoneNumber = address.Phone;
            Email = address.Email;
            Fax = address.Fax;
            FirstName = address.FirstName;
            LastName = address.LastName;
            MobilePhone = address.MobilePhone;
            OrganizationName = address.OrganizationName;
            Title = address.Title;
        }
    }
}