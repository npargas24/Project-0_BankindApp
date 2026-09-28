using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace BankingApp
{
    public abstract class User
    {
        #region Properties
        public int UserID {get; set;}
        public string Username {get; set;}
        public bool MustChangePassword {get; set;}
        protected string PasswordHash {get; set;}

        private static readonly PasswordHasher<User> PasswordHasher = new();

        #endregion

        #region Methods
        // public void Login()
        // {
        //     System.Console.WriteLine("You logged in!");
        // }

        //Validate Login
        //Change Password
        //Set password?
        //Create Password Hash
        public void SetPassword(string password)
        {
            if (password.Length < 8){
                throw new ArgumentException("Password must be at least 8 characters.");
            }

            if (!password.Any(char.IsUpper)){
                throw new ArgumentException("Password must contain an uppercase letter.");
            }

            if (!password.Any(char.IsDigit)){
                throw new ArgumentException("Password must contain a number.");
            }

            if (!password.Any(char.IsSymbol) && !password.Any(char.IsPunctuation)){
                throw new ArgumentException("Password must contain a special character.");
            }
            

            PasswordHash = PasswordHasher.HashPassword(this, password);
        }

        public bool VerifyPassword(string password)
        {
            var result = PasswordHasher.VerifyHashedPassword(
                this,
                PasswordHash,
                password
            );

            if (result == PasswordVerificationResult.Success ||
                result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GenerateTemporaryPassword()
        {
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string specialCharacters = "!@#$%&*?";

            const string allCharacters =
                uppercase + lowercase + numbers + specialCharacters;

            char[] password = new char[10];

            // Guarantee at least one of each required character type
            password[0] = uppercase[RandomNumberGenerator.GetInt32(uppercase.Length)];
            password[1] = lowercase[RandomNumberGenerator.GetInt32(lowercase.Length)];
            password[2] = numbers[RandomNumberGenerator.GetInt32(numbers.Length)];
            password[3] = specialCharacters[RandomNumberGenerator.GetInt32(specialCharacters.Length)];

            // Fill the remaining characters randomly
            for (int i = 4; i < password.Length; i++)
            {
                password[i] =
                    allCharacters[RandomNumberGenerator.GetInt32(allCharacters.Length)];
            }

            // Shuffle so the first four characters aren't always predictable types
            RandomNumberGenerator.Shuffle(password);

            return new string(password);
        }

        #endregion

    }
}