using System;
using System.Collections.Generic;
using System.Text;

namespace Eprocurement.Domain.Entities
{
    public class RegisterSupplierRequest 
    {
        public string CorporateName { get; private set; }
        public string DocumentNumber { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public bool IsActive { get; private set; }

        public RegisterSupplierRequest(string corporateName, string documentNumber, string email, string phone)
        {
            CorporateName = corporateName;
            DocumentNumber = documentNumber;
            Email = email;
            Phone = phone;
            IsActive = true;
        }
    }
}
