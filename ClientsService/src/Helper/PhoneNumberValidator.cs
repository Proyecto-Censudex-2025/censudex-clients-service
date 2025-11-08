using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhoneNumbers;

namespace ClientsService.src.Helper
{
    public static class PhoneNumberValidator
    {
        public static bool IsValidChileanPhone(string phoneNumber)
        {
            try
            {
                var phoneUtil = PhoneNumberUtil.GetInstance();
                var number = phoneUtil.Parse(phoneNumber, "CL");
                
                return phoneUtil.IsValidNumberForRegion(number, "CL");
            }
            catch
            {
                return false;
            }
        }
    }
}