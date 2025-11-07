using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BCrypt.Net;

namespace ClientsService.src.Helper
{
    public static class PasswordManager
    {
        private const int DefaultWorkFactor = 12;

        /// <summary>
        /// Hashes a password using BCrypt with default work factor
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <returns>BCrypt hash of the password</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            return BCrypt.Net.BCrypt.HashPassword(password, DefaultWorkFactor);
        }

        /// <summary>
        /// Hashes a password using BCrypt with custom work factor
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <param name="workFactor">The cost factor</param>
        /// <returns>BCrypt hash of the password</returns>
        public static string HashPassword(string password, int workFactor)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            if (workFactor < 4 || workFactor > 31)
                throw new ArgumentException("Work factor must be between 4 and 31", nameof(workFactor));

            return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
        }

        /// <summary>
        /// Verifies a password against a BCrypt hash
        /// </summary>
        /// <param name="password">The plain text password to verify</param>
        /// <param name="hash">The BCrypt hash to verify against</param>
        /// <returns>True if the password matches the hash, false otherwise</returns>
        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (string.IsNullOrEmpty(hash))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch (Exception)
            {
                // Invalid hash format or other BCrypt error
                return false;
            }
        }

        /// <summary>
        /// Checks if a hash needs to be rehashed (if work factor has changed)
        /// </summary>
        /// <param name="hash">The existing hash to check</param>
        /// <param name="targetWorkFactor">The desired work factor</param>
        /// <returns>True if rehashing is needed</returns>
        public static bool NeedsRehash(string hash, int targetWorkFactor = DefaultWorkFactor)
        {
            if (string.IsNullOrEmpty(hash))
                return true;

            try
            {
                // Extract the work factor from the existing hash
                var parts = hash.Split('$');
                if (parts.Length < 4)
                    return true;

                if (int.TryParse(parts[2], out int currentWorkFactor))
                {
                    return currentWorkFactor < targetWorkFactor;
                }

                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Checks the vaidity of a password
        /// </summary>
        /// <param name="password">The existing password to check</param>
        /// <returns>True if it is a valid password</returns>
        public static bool IsValidPassword(string password)
        {

            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            var specialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?";
            // At least 8 characters
            if (password.Length < 8 || !password.Any(char.IsUpper) || !password.Any(char.IsLower)
                || !password.Any(char.IsDigit) || !password.Any(c => specialChars.Contains(c)))
            {
                return false;
            }

            return true;

        }
    }
}