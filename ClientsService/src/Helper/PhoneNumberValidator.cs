using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhoneNumbers;

namespace ClientsService.src.Helper
{
    public static class PhoneNumberValidator
    {
        /// <summary>
        /// The function `IsValidChileanPhone` checks if a given phone number is valid for Chile.
        /// </summary>
        /// <param name="phoneNumber">The `IsValidChileanPhone` method is used to validate a Chilean
        /// phone number. It uses the `PhoneNumberUtil` class from the libphonenumber library to parse
        /// and validate the phone number for the region "CL" (Chile).</param>
        /// <returns>
        /// The `IsValidChileanPhone` method is returning a boolean value. If the input `phoneNumber` is
        /// a valid Chilean phone number, it will return `true`. If there is an error during the
        /// validation process or the phone number is not valid, it will return `false`.
        /// </returns>
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