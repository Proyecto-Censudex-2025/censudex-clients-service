using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ClientsService.src.Helper;
using ClientsService.src.Model;

namespace ClientsService.src.Data
{
    public class Seeder
    {
        /// <summary>
        /// Ensures the db is created and seeds it
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDBContext>();
                context.Database.EnsureCreated();

                if (!context.clients.Any())
                {
                    var admin = new Client
                    {
                        Id = Guid.NewGuid().ToString(),
                        Role = "Admin",
                        Name = "Nombre_Admin",
                        Surename = "Apellido_Admin",
                        Email = "Admin@censudex.cl",
                        Username = "AdminUsername",
                        Birthdate = DateOnly.FromDateTime(DateTime.Today).AddYears(-18),
                        Address = "A very real address 123",
                        TelephoneNumber = "+56911111111",
                        Password = PasswordManager.HashPassword("Password123+"),
                        RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
                        isActive = true
                    };
                    context.clients.Add(admin);
                    context.SaveChanges();

                    Random random = new Random();
                    string[] names = ["Aria", "Elias", "Sofia", "Adrian", "Mateo", "Clara"];
                    string[] surenames = ["Morgan", "Carter", "Delgado", "Blake", "Monroe", "Cruz"];
                    string[] usernames = ["PixelVoyager", "LunaNova", "EchoStorm", "CyberFenix", "ShadowPulse",
                     "CrimsonByte", "NeonSpecter", "AquaRift", "ZeroComet", "DataNomad", "SkyBreaker", "MysticWarden",
                     "NovaDrift", "QuantumPanda", "SilentOrbit", "IronNebula", "SolarBreeze", "DarkAtlas", "CrystalLogic", "EchoZen"];
                    string digits = "0123456789";
                    using var rng = RandomNumberGenerator.Create();
                    var number =  new char[8];
                    for (int i = 0; i < 8; i++)
                    {
                        number[i] = GetRandomCharacter(digits, rng);
                    }       
                    for (int i = 1; i < 10; i++)
                    {
                        var client = new Client
                        {
                            Id = Guid.NewGuid().ToString(),
                            Role = "User",
                            Name = names[random.Next(0, 6)],
                            Surename = surenames[random.Next(0, 6)],
                            Email = GenerateCryptoRandomString(random.Next(1, 17)) + "@censudex.cl",
                            Username = usernames[random.Next(0, 20)],
                            Birthdate = DateOnly.FromDateTime(DateTime.Today).AddYears(-18),
                            Address = "A very real address 123",
                            TelephoneNumber = "+569" + number,
                            Password = PasswordManager.HashPassword(GeneratePassword(12)),
                            RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
                            isActive = true
                        };
                        context.clients.Add(client);
                        context.SaveChanges();
                    }
                    context.SaveChanges();
                }
                context.SaveChanges();
            }
        }

        /// </summary>
        /// <param name="length">Length of the string to generate</param>
        /// <param name="chars">Characters to choose from</param>
        /// <returns>Random string</returns>
        private static string GenerateCryptoRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[RandomNumberGenerator.GetInt32(chars.Length)]);
            }

            return result.ToString();
        }
        /// <summary>
        /// Generates a secure random password that meets all specified requirements
        /// </summary>
        /// <param name="length">Password length (minimum 8 characters)</param>
        /// <returns>A secure password meeting all requirements</returns>
        public static string GeneratePassword(int length)
        {

            string lowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
            string uppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string digits = "0123456789";
            string specialCharacters = "!@#$%^&*()_+-=[]{}|;:,.<>?~`";

            string AllCharacters = lowercaseLetters + uppercaseLetters + digits + specialCharacters;

            if (length < 8)
                throw new ArgumentException("Password length must be at least 8 characters", nameof(length));

            using var rng = RandomNumberGenerator.Create();

            var password = new char[length];

            // Step 1: Ensure at least one character from each required category
            password[0] = GetRandomCharacter(lowercaseLetters, rng);
            password[1] = GetRandomCharacter(uppercaseLetters, rng);
            password[2] = GetRandomCharacter(digits, rng);
            password[3] = GetRandomCharacter(specialCharacters, rng);

            // Step 2: Fill the remaining positions with random characters from all sets
            for (int i = 4; i < length; i++)
            {
                password[i] = GetRandomCharacter(AllCharacters, rng);
            }

            // Step 3: Shuffle the password to randomize the positions
            ShuffleArray(password, rng);

            return new string(password);
        }

        /// <summary>
        /// Gets a random character from the specified character set
        /// </summary>
        private static char GetRandomCharacter(string characterSet, RandomNumberGenerator rng)
        {
            var buffer = new byte[4];
            rng.GetBytes(buffer);
            var randomValue = BitConverter.ToUInt32(buffer, 0);
            var index = (int)(randomValue % characterSet.Length);
            return characterSet[index];
        }
        
        /// <summary>
        /// Shuffles an array using Fisher-Yates algorithm with cryptographically secure randomness
        /// </summary>
        private static void ShuffleArray<T>(T[] array, RandomNumberGenerator rng)
        {
            var buffer = new byte[4];
            
            for (int i = array.Length - 1; i > 0; i--)
            {
                rng.GetBytes(buffer);
                var randomValue = BitConverter.ToUInt32(buffer, 0);
                var j = (int)(randomValue % (i + 1));
                
                // Swap elements
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
    }
}