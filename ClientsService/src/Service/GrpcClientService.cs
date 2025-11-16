using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClientsService.Grcp;
using ClientsService.src.DTOs;
using ClientsService.src.Helper;
using ClientsService.src.Interface;
using Grpc.Core;

namespace ClientsService.src.Service
{
    public class GrpcClientService : ClientService.ClientServiceBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly ILogger<GrpcClientService> _logger;

        public GrpcClientService(IClientRepository clientRepository, ILogger<GrpcClientService> logger)
        {
            _clientRepository = clientRepository;
            _logger = logger;
        }

        public override async Task<GetAllClientsResponse> GetAllClients(
            GetAllClientsRequest request, 
            ServerCallContext context)
        {
            try
            {
                // Check if user has Admin role from metadata
                if (!HasRole(context, "Admin"))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Admin role required"));
                }

                var clients = await _clientRepository.GetAll();
                var response = new GetAllClientsResponse();
                
                foreach (var client in clients)
                {
                    response.Clients.Add(new ClientDto
                    {
                        Id = client.Id,
                        Fullname = $"{client.Name} {client.Surename}",
                        Email = client.Email,
                        Username = client.Username,
                        IsActive = client.isActive,
                        Birthdate = client.Birthdate.ToString("yyyy-MM-dd"),
                        Address = client.Address,
                        TelephoneNumber = client.TelephoneNumber,
                        RegistrationDate = client.RegistrationDate.ToString("yyyy-MM-dd")
                    });
                }

                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllClients");
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }

        public override async Task<GetClientResponse> GetClient(
            GetClientRequest request, 
            ServerCallContext context)
        {
            try
            {
                var client = await _clientRepository.GetClient(request.Id);
                
                if (client == null)
                {
                    return new GetClientResponse { Found = false };
                }

                return new GetClientResponse
                {
                    Found = true,
                    Client = new ClientDto
                    {
                        Id = client.Id,
                        Fullname = $"{client.Name} {client.Surename}",
                        Email = client.Email,
                        Username = client.Username,
                        IsActive = client.isActive,
                        Birthdate = client.Birthdate.ToString("yyyy-MM-dd"),
                        Address = client.Address,
                        TelephoneNumber = client.TelephoneNumber,
                        RegistrationDate = client.RegistrationDate.ToString("yyyy-MM-dd")
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetClient");
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }

        public override async Task<GetClientsFilteredResponse> GetClientsFiltered(
            GetClientsFilteredRequest request, 
            ServerCallContext context)
        {
            try
            {
                if (!HasRole(context, "Admin"))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Admin role required"));
                }

                var query = new QueryObject
                {
                    Name = request.Name,
                    Email = request.Email,
                    isActive = request.HasIsActive ? request.IsActive : null,
                    Username = request.Username
                };

                var clients = await _clientRepository.GetClients(query);
                var response = new GetClientsFilteredResponse();

                foreach (var client in clients)
                {
                    response.Clients.Add(new ClientDto
                    {
                        Id = client.Id,
                        Fullname = $"{client.Name} {client.Surename}",
                        Email = client.Email,
                        Username = client.Username,
                        IsActive = client.isActive,
                        Birthdate = client.Birthdate.ToString("yyyy-MM-dd"),
                        Address = client.Address,
                        TelephoneNumber = client.TelephoneNumber,
                        RegistrationDate = client.RegistrationDate.ToString("yyyy-MM-dd")
                    });
                }

                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetClientsFiltered");
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }

        public override async Task<EnableDisableClientResponse> EnableDisableClient(
            EnableDisableClientRequest request, 
            ServerCallContext context)
        {
            try
            {
                if (!HasRole(context, "Admin"))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Admin role required"));
                }

                await _clientRepository.EnableDisableClient(request.Id);
                
                return new EnableDisableClientResponse
                {
                    Success = true,
                    Message = "Client status updated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EnableDisableClient");
                return new EnableDisableClientResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public override async Task<UpdateClientResponse> UpdateClient(
            UpdateClientRequest request, 
            ServerCallContext context)
        {
            try
            {
                // Extract claims from token in metadata
                var claims = GetClaimsFromContext(context);
                if (claims == null)
                {
                    throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid token"));
                }

                var updateDto = new UpdateClientDto
                {
                    Name = request.Name,
                    Surename = request.Surename,
                    Email = request.Email,
                    Username = request.Username,
                    Birthdate = !string.IsNullOrEmpty(request.Birthdate) 
                        ? DateOnly.Parse(request.Birthdate) 
                        : null,
                    Address = request.Address,
                    TelephoneNumber = request.TelephoneNumber,
                    Password = request.Password
                };

                var client = await _clientRepository.UpdateClient(updateDto, claims);

                if (client == null)
                {
                    return new UpdateClientResponse
                    {
                        Success = false,
                        Message = "Client not found"
                    };
                }

                return new UpdateClientResponse
                {
                    Success = true,
                    Message = "Client updated successfully",
                    Client = new ClientDto
                    {
                        Id = client.Id,
                        Fullname = $"{client.Name} {client.Surename}",
                        Email = client.Email,
                        Username = client.Username,
                        IsActive = client.isActive,
                        Birthdate = client.Birthdate.ToString("yyyy-MM-dd"),
                        Address = client.Address,
                        TelephoneNumber = client.TelephoneNumber,
                        RegistrationDate = client.RegistrationDate.ToString("yyyy-MM-dd")
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateClient");
                return new UpdateClientResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public override async Task<RegisterClientResponse> RegisterClient(
            RegisterClientRequest request, 
            ServerCallContext context)
        {
            try
            {
                var createDto = new CreateClientDto
                {
                    Name = request.Name,
                    Surename = request.Surename,
                    Email = request.Email,
                    Username = request.Username,
                    Birthdate = DateOnly.Parse(request.Birthdate),
                    Address = request.Address,
                    TelephoneNumber = request.TelephoneNumber,
                    Password = request.Password
                };

                var client = await _clientRepository.CreateClient(createDto);

                return new RegisterClientResponse
                {
                    Success = true,
                    Message = "Client registered successfully",
                    Client = new ClientDto
                    {
                        Id = client.Id,
                        Fullname = $"{client.Name} {client.Surename}",
                        Email = client.Email,
                        Username = client.Username,
                        IsActive = client.isActive,
                        Birthdate = client.Birthdate.ToString("yyyy-MM-dd"),
                        Address = client.Address,
                        TelephoneNumber = client.TelephoneNumber,
                        RegistrationDate = client.RegistrationDate.ToString("yyyy-MM-dd")
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterClient");
                return new RegisterClientResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // Helper methods
        private bool HasRole(ServerCallContext context, string role)
        {
            var claims = GetClaimsFromContext(context);
            return claims?.IsInRole(role) ?? false;
        }

        private ClaimsPrincipal? GetClaimsFromContext(ServerCallContext context)
        {
            // Extract JWT token from metadata
            var authHeader = context.RequestHeaders.GetValue("authorization");
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }

            // You would validate the token here using your TokenService
            // For now, returning the user from the HTTP context if available
            return context.GetHttpContext()?.User;
        }
    }
}
