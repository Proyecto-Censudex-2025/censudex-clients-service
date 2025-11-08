using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClientsService.src.DTOs;
using ClientsService.src.Helper;
using ClientsService.src.Model;

namespace ClientsService.src.Interface
{
    public interface IClientRepository
    {
        Task<Client> CreateClient(CreateClientDto createClientDto);
        Task<List<Client>> GetAll();
        Task<Client?> GetClient(string Id);
        Task<Client?> UpdateClient(UpdateClientDto updateClientDto, ClaimsPrincipal currentClient);
        Task EnableDisableClient(string Id);
        Task<List<Client>> GetClients(QueryObject query);
        Task<ClientLoginResponse?> Login(LoginDto loginDto);
    }
}