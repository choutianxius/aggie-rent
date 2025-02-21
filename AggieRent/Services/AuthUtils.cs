using System.Net.Mail;
using System.Text.RegularExpressions;
using AggieRent.DataAccess;
using AggieRent.Models;
using Isopoh.Cryptography.Argon2;

namespace AggieRent.Services
{
    public partial class AuthUtils
    {
        [GeneratedRegex(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[~!@#$%^&*()_+])[A-Za-z\d~!@#$%^&*()_+]{8,}$"
        )]
        private static partial Regex PasswordRegex();

        private static readonly Regex passwordRegex = PasswordRegex();

        [GeneratedRegex(@"\s")]
        private static partial Regex WhiteSpaceRegex();

        private static readonly Regex whiteSpaceRegex = WhiteSpaceRegex();

        public static void ValidateRegistrationCredentials<TUser>(
            string email,
            string password,
            IBaseRepository<TUser> repository
        )
            where TUser : BaseUser
        {
            string normalizedEmail = NormalizeEmail(email);
            if (!ValidateEmail(normalizedEmail))
                throw new ArgumentException("Invalid email format");

            // Lazy execute
            var existingUser = repository.GetAll().FirstOrDefault(u => u.Email == normalizedEmail);
            if (existingUser != null)
                throw new ArgumentException("Email already in use");

            if (!ValidatePassword(password))
                throw new ArgumentException(
                    "Invalid password! Password must be at least 8 symbols long, with at least 1 lower case character, 1 upper case character, 1 symbol and 1 number"
                );
        }

        public static string NormalizeEmail(string email)
        {
            return whiteSpaceRegex.Replace(email, "").ToLower();
        }

        public static bool ValidateEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verify the format of a password during registration
        /// </summary>
        /// <returns>
        /// true if input password is in valid format, false otherwise
        /// </returns>
        public static bool ValidatePassword(string password)
        {
            return passwordRegex.IsMatch(password);
        }

        public static string HashPassword(string plainText)
        {
            var hashedPassword = Argon2.Hash(plainText);
            return hashedPassword;
        }

        public static bool Verify(string plainText, string hash)
        {
            return Argon2.Verify(hash, plainText);
        }
    }
}
