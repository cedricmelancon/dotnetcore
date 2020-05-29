using System;
using System.Collections.Generic;

namespace UserApplication.Data.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<AddressModel> Addresses { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
    }
}
