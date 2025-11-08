using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClientsService.src.Data;
using ClientsService.src.DTOs;
using ClientsService.src.Helper;
using ClientsService.src.Interface;
using ClientsService.src.Model;
using Microsoft.EntityFrameworkCore;

namespace ClientsService.src.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDBContext _context;
        /// <summary>
        /// Create conection with the db
        /// </summary>
        /// <param name="context"></param>
        public ClientRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Creates and saves a new client in the db
        /// </summary>
        /// <param name="createClientDto"></param>
        /// <returns>The created client</returns>
        public async Task<Client> CreateClient(CreateClientDto createClientDto)
        {
            if (string.IsNullOrEmpty(createClientDto.Password) || string.IsNullOrEmpty(createClientDto.Email)
                || string.IsNullOrEmpty(createClientDto.Surename) || string.IsNullOrEmpty(createClientDto.Name))
            {
                throw new ArgumentException("All fields are required");
            }

            // Check if it's a valid email
            string email = createClientDto.Email;
            if (!email.ToLower().Contains('@'))
            {
                throw new Exception("Invalid Email");
            }

            string[] verifyEmail = email.Split('@');
            if (verifyEmail[1] != "censudex.cl")
            {
                throw new Exception("Invalid Email");
            }
            // Check if email is already registered
            var emailExists = await _context.clients.AnyAsync(u => u.Email == createClientDto.Email);
            if (emailExists)
            {
                throw new Exception("Email is already registered");
            }
            // Check if it is a valid password
            if (!PasswordManager.IsValidPassword(createClientDto.Password))
            {
                throw new Exception("Invalid password");
            }
            // Check if email is already registered
            var usernameExists = await _context.clients.AnyAsync(u => u.Username == createClientDto.Username);
            if (usernameExists)
            {
                throw new Exception("Username is already registered");
            }

            // Check if the client is at least 18 years old
            if (createClientDto.Birthdate >= DateOnly.FromDateTime(DateTime.Today).AddYears(-18)) throw new Exception("Client needs to be at least 18 years old");

            if (!PhoneNumberValidator.IsValidChileanPhone(createClientDto.TelephoneNumber))
            {
                throw new Exception("Invalid telephone number");
            }

            // make sure it's a new Id
            string newId;
            while (true)
            {
                newId = Guid.NewGuid().ToString();
                var checkClient = await _context.clients.FindAsync(newId);
                if (checkClient == null) break;
            }

            // Create the new client  
            var client = new Client
            {
                Id = newId,
                Role = "User",
                Name = createClientDto.Name,
                Surename = createClientDto.Surename,
                Email = email,
                Password = PasswordManager.HashPassword(createClientDto.Password),
                Username = createClientDto.Username,
                Birthdate = createClientDto.Birthdate,
                Address = createClientDto.Address,
                TelephoneNumber = createClientDto.TelephoneNumber,
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
                isActive = true
            };

            // Save changes
            await _context.clients.AddAsync(client);
            await _context.SaveChangesAsync();
            return client;
        }

        /// <summary>
        /// Changes the state of a client, from active to deactivated or in reverse
        /// </summary>
        /// <param name="Id"></param>
        public async Task EnableDisableClient(string Id)
        {
            var client = await _context.clients.FindAsync(Id);
            if (client == null)
            {
                throw new Exception("Client not found");
            }

            if (client.isActive == true)
            {
                client.isActive = false;
                client.DeactivationDates.Add(DateOnly.FromDateTime(DateTime.Now));
            }
            else
            {
                client.isActive = true;
            }
            await _context.SaveChangesAsync();

        }
        /// <summary>
        /// Gets all the clients from the db
        /// </summary>
        /// <returns>A list with all the clients</returns>
        public async Task<List<Client>> GetAll()
        {
            return await _context.clients.ToListAsync();
        }

        public async Task<Client?> GetClient(string Id)
        {
            return await _context.clients.FindAsync(Id);
        }

        public async Task<List<Client>> GetClients(QueryObject query)
        {
            var clients = _context.clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                clients = clients.Where(u =>
                    EF.Functions.ILike(u.Name, $"%{query.Name}%") ||
                    EF.Functions.ILike(u.Surename, $"%{query.Name}%"));
            }

            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                clients = clients.Where(x => x.Email.Contains(query.Email));
            }

            if (query.isActive.HasValue)
            {
                clients = clients.Where(x => x.isActive == query.isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Username))
            {
                clients = clients.Where(x => x.Username.Contains(query.Username));
            }

            return await clients.ToListAsync();
        }
        /// <summary>
        /// Can update the name, surename, email and password of the client
        /// </summary>
        /// <param name="updateClientDto"></param>
        /// <param name="currentClient"></param>
        /// <returns>The updated client</returns>
        public async Task<Client?> UpdateClient(UpdateClientDto updateClientDto, ClaimsPrincipal currentClient)
        {
            var client = await _context.clients.FindAsync(currentClient.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Check if client exists
            if (client == null)
            {
                throw new Exception("Client not found");
            }

            // Update client email
            if (!string.IsNullOrEmpty(updateClientDto.Email) && updateClientDto.Email != client.Email)
            {
                // Check if it's a valid email
                string email = updateClientDto.Email;
                if (!email.ToLower().Contains('@'))
                {
                    throw new Exception("Invalid Email");
                }
                string[] verifyEmail = email.Split('@');
                if (verifyEmail[1] != "censudex.cl")
                {
                    throw new Exception("Invalid Email");
                }

                // Check if email is already registered
                var emailExists = await _context.clients
                    .AnyAsync(u => u.Email == updateClientDto.Email && u.Id != ClaimTypes.NameIdentifier);

                if (emailExists)
                {
                    throw new Exception("Email is already registered");
                }

                client.Email = updateClientDto.Email;
            }

            // Update client Username
            if (!string.IsNullOrEmpty(updateClientDto.Username))
            {
                var usernameExists = await _context.clients.AnyAsync(u => u.Username == updateClientDto.Username && u.Id != ClaimTypes.NameIdentifier);

                if (usernameExists)
                {
                    throw new Exception("Username is already registered");
                }
                client.Username = updateClientDto.Username; 
            }

            // Check if it is a valid password
            if (!string.IsNullOrEmpty(updateClientDto.Password))
            {
                if (!PasswordManager.IsValidPassword(updateClientDto.Password))
                {
                    throw new Exception("Invalid password");
                }
                client.Password = PasswordManager.HashPassword(updateClientDto.Password);
            }

            // Check if it is a valid Birthdate
            if (updateClientDto.Birthdate.HasValue)
            {
                // Check if the client is at least 18 years old
                if (updateClientDto.Birthdate >= DateOnly.FromDateTime(DateTime.Today).AddYears(-18))
                {
                    throw new Exception("Client needs to be at least 18 years old");
                }
                client.Birthdate = (DateOnly)updateClientDto.Birthdate;
            }

            // Check if it is a valid telephone number
            if (!string.IsNullOrEmpty(updateClientDto.TelephoneNumber))
            {
                if (!PhoneNumberValidator.IsValidChileanPhone(updateClientDto.TelephoneNumber))
                {
                    throw new Exception("Invalid telephone number");
                }
                client.TelephoneNumber = updateClientDto.TelephoneNumber;
            }

            // Update client name
            if (!string.IsNullOrEmpty(updateClientDto.Name)) { client.Name = updateClientDto.Name; }

            // Update client surename
            if (!string.IsNullOrEmpty(updateClientDto.Surename)) { client.Surename = updateClientDto.Surename; }

            // Update client address
            if (!string.IsNullOrEmpty(updateClientDto.Address)) { client.Address = updateClientDto.Address; }

            // Save changes
            await _context.SaveChangesAsync();
            return client;
        }
        /// <summary>
        /// Function for the login of the client
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns>The logged client</returns>
        public async Task<ClientLoginResponse?> Login(LoginDto loginDto)
        {
            try
            {
                var client = await _context.clients
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.isActive == true);
                
                if (client == null)
                {
                    client = await _context.clients.FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.isActive == true);
                    if (client == null)
                    {
                        return new ClientLoginResponse 
                        { 
                            IsValid = false, 
                            ErrorMessage = "Client not found" 
                        };
                    }
                }
                
                // Verify password
                var isPasswordValid = PasswordManager.VerifyPassword(loginDto.Password, client.Password);

                if (!isPasswordValid)
                {
                    return new ClientLoginResponse
                    {
                        IsValid = false,
                        ErrorMessage = "Invalid password"
                    };
                }
                
                return new ClientLoginResponse
                {
                    IsValid = true,
                    Id = client.Id,
                    Email = client.Email,
                    Name = client.Name,
                    Surename = client.Surename,
                    Username = client.Username,
                    Birthdate = client.Birthdate,
                    Address = client.Address,
                    TelephoneNumber = client.TelephoneNumber,
                    Role = client.Role,
                    Claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, client.Id),
                        new Claim(ClaimTypes.Email, client.Email),
                        new Claim(ClaimTypes.Role, client.Role),
                        new Claim(ClaimTypes.GivenName, client.Name),
                        new Claim(ClaimTypes.Surname, client.Surename),
                        new Claim("Username", client.Username),
                        new Claim(ClaimTypes.DateOfBirth, client.Birthdate.ToString()),
                        new Claim(ClaimTypes.StreetAddress, client.Address),
                        new Claim(ClaimTypes.MobilePhone, client.TelephoneNumber),
                        new Claim("fullName", $"{client.Name} {client.Surename}".Trim()),
                        new Claim("registrationDate", client.RegistrationDate.ToString("yyyy-MM-dd"))
                    }
                };
            }
            catch (Exception ex)
            {
                return new ClientLoginResponse 
                { 
                    IsValid = false, 
                    ErrorMessage = "Validation error occurred" 
                };
            }
        }
    }
}