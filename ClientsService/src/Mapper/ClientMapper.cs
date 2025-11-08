using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientsService.src.DTOs;
using ClientsService.src.Model;

namespace ClientsService.src.Mapper
{
    public static class ClientMapper
    {
        /// <summary>
        /// Method that turns a Client into a VisualizeClientDto
        /// </summary>
        /// <param name="client"></param>
        /// <returns>The VisualizeClientDto</returns>
        public static VisualizeClientDto ToVisualizeClientDtoFromClient(this Client client)
        {
            return new VisualizeClientDto
            {
                Id = client.Id,
                Fullname = $"{client.Name} {client.Surename}",
                Email = client.Email,
                Username = client.Username,
                isActive = client.isActive,
                Birthdate = client.Birthdate,
                Address = client.Address,
                TelephoneNumber = client.TelephoneNumber,
                RegistrationDate = client.RegistrationDate
            };
        }
        /// <summary>
        /// Method that lets a list of Clients be turned into a list of VisualizeClientDtos
        /// </summary>
        /// <param name="clients"></param>
        /// <returns>The list of VisualizeClientDto</returns>
        public static IEnumerable<VisualizeClientDto> ToDtoEnumerable(this IEnumerable<Client> clients)
        {
            return clients.Select(client => client.ToVisualizeClientDtoFromClient());
        }
    }
}