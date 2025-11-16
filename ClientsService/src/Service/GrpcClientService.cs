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
        /* The above code is a C# class defining a service called `GrpcClientService`. It has two
        private readonly fields: `_clientRepository` of type `IClientRepository` and `_logger` of
        type `ILogger<GrpcClientService>`. The class has a constructor that takes in an
        `IClientRepository` and an `ILogger<GrpcClientService>` as parameters and assigns them to
        the respective fields. This class is likely used to interact with gRPC clients and log
        information using the provided logger. */
        private readonly IClientRepository _clientRepository;
        private readonly ILogger<GrpcClientService> _logger;

        public GrpcClientService(IClientRepository clientRepository, ILogger<GrpcClientService> logger)
        {
            _clientRepository = clientRepository;
            _logger = logger;
        }

        /// <summary>
        /// This C# function retrieves all clients, checks for admin role permission, and returns client
        /// information in a response object.
        /// </summary>
        /// <param name="GetAllClientsRequest">The `GetAllClients` method you provided is an
        /// asynchronous method that retrieves all clients and returns a response containing client
        /// information in a specific format. Here's a breakdown of the parameters and the method
        /// implementation:</param>
        /// <param name="ServerCallContext">The `ServerCallContext` parameter in the `GetAllClients`
        /// method represents the context of the server-side call. It provides information about the
        /// incoming call, such as metadata, deadline, cancellation token, peer identity, etc. In your
        /// code snippet, you are using the `ServerCallContext`</param>
        /// <returns>
        /// The method `GetAllClients` returns a `Task` that will eventually contain a
        /// `GetAllClientsResponse` object. This response object contains a list of `ClientDto` objects
        /// representing clients retrieved from the `_clientRepository`.
        /// </returns>
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

        /// <summary>
        /// The function `GetClient` retrieves client information based on the provided request and
        /// returns a response with the client details or an error message if an exception occurs.
        /// </summary>
        /// <param name="GetClientRequest">The `GetClientRequest` parameter is the request object that
        /// contains the information needed to retrieve a client. It likely includes a unique identifier
        /// such as the client's ID.</param>
        /// <param name="ServerCallContext">The `ServerCallContext` parameter in the `GetClient` method
        /// represents the context of the server-side call. It provides information about the incoming
        /// call such as metadata, cancellation signals, and authentication details. This context is
        /// used to interact with the gRPC server during the processing of the request.</param>
        /// <returns>
        /// The GetClient method returns a GetClientResponse object. This response object contains
        /// information about a client, including whether the client was found, and details about the
        /// client such as their ID, full name, email, username, activity status, birthdate, address,
        /// telephone number, and registration date. If the client is not found in the repository, the
        /// response will indicate that the client was not found by
        /// </returns>
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

        /// <summary>
        /// This C# function retrieves filtered client data, ensuring the user has the "Admin" role, and
        /// handles exceptions appropriately.
        /// </summary>
        /// <param name="GetClientsFilteredRequest">GetClientsFilteredRequest is a class that contains
        /// the parameters for filtering clients. It typically includes properties such as Name, Email,
        /// IsActive, Username, etc., which are used to filter the list of clients based on the
        /// specified criteria.</param>
        /// <param name="ServerCallContext">The `ServerCallContext` parameter in the
        /// `GetClientsFiltered` method represents the context of the server-side call. It provides
        /// information about the incoming call such as the method being called, the deadline for the
        /// call, metadata associated with the call, and the peer's identity.</param>
        /// <returns>
        /// The method `GetClientsFiltered` is returning a `Task` that will eventually contain a
        /// `GetClientsFilteredResponse` object. This response object contains a list of `ClientDto`
        /// objects that represent clients filtered based on the criteria provided in the
        /// `GetClientsFilteredRequest` parameter.
        /// </returns>
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

        /// <summary>
        /// The function `EnableDisableClient` enables or disables a client based on the request and
        /// returns a response indicating success or failure.
        /// </summary>
        /// <param name="EnableDisableClientRequest">The `EnableDisableClient` method you provided is an
        /// asynchronous method that enables or disables a client based on the request received. Here's
        /// a breakdown of the parameters involved:</param>
        /// <param name="ServerCallContext">The `ServerCallContext` parameter in the
        /// `EnableDisableClient` method represents the context of the server-side call. It provides
        /// information about the incoming call such as metadata, cancellation tokens, and
        /// authentication details. In this method, the `ServerCallContext` is used to check if the
        /// caller has</param>
        /// <returns>
        /// The `EnableDisableClient` method returns an `EnableDisableClientResponse` object. This
        /// object contains a `Success` property indicating whether the operation was successful, and a
        /// `Message` property providing additional information about the operation outcome.
        /// </returns>
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

        /// <summary>
        /// The UpdateClient function in C# updates client information based on the provided request and
        /// returns a response indicating success or failure.
        /// </summary>
        /// <param name="UpdateClientRequest">The `UpdateClientRequest` class seems to contain
        /// properties for updating client information such as name, surname, email, username,
        /// birthdate, address, telephone number, and password.</param>
        /// <param name="ServerCallContext">The `ServerCallContext` parameter in the `UpdateClient`
        /// method is typically used in gRPC services to provide contextual information about the
        /// ongoing RPC call. It contains metadata about the call, such as the deadline, cancellation
        /// token, peer information, and authentication details.</param>
        /// <returns>
        /// The `UpdateClient` method returns a `Task` that will eventually yield an
        /// `UpdateClientResponse` object. This response object contains information about the success
        /// or failure of the client update operation, along with a message and details of the updated
        /// client if successful.
        /// </returns>
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

        /// <summary>
        /// The RegisterClient function registers a new client based on the provided request data and
        /// returns a response indicating success or failure.
        /// </summary>
        /// <param name="RegisterClientRequest">The `RegisterClient` method you provided is an
        /// asynchronous method that registers a client based on the information provided in the
        /// `RegisterClientRequest`. Here's a breakdown of the parameters used in the method:</param>
        /// <param name="ServerCallContext">The `RegisterClient` method you provided is an asynchronous
        /// method that registers a client based on the information provided in the
        /// `RegisterClientRequest` object. Here's a breakdown of the parameters used in the
        /// method:</param>
        /// <returns>
        /// The RegisterClient method returns a RegisterClientResponse object. This object contains a
        /// success flag indicating whether the registration was successful or not, a message providing
        /// information about the registration status, and a ClientDto object representing the
        /// registered client details such as ID, full name, email, username, birthdate, address,
        /// telephone number, and registration date.
        /// </returns>
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
        /// <summary>
        /// The function `HasRole` checks if a user has a specific role based on claims in the server
        /// call context.
        /// </summary>
        /// <param name="ServerCallContext">The `ServerCallContext` parameter represents the context of
        /// a server call in gRPC. It provides information about the incoming call, such as metadata,
        /// authentication details, and cancellation signals.</param>
        /// <param name="role">The `role` parameter in the `HasRole` method is a string that represents
        /// the role you want to check for in the user's claims.</param>
        /// <returns>
        /// The `HasRole` method is returning a boolean value indicating whether the user associated
        /// with the provided `ServerCallContext` has the specified role. It checks if the user's claims
        /// include the specified role and returns `true` if the role is found, and `false` otherwise.
        /// </returns>
        private bool HasRole(ServerCallContext context, string role)
        {
            var claims = GetClaimsFromContext(context);
            return claims?.IsInRole(role) ?? false;
        }

        /// <summary>
        /// The function `GetClaimsFromContext` extracts JWT token from metadata and returns the user
        /// from the HTTP context if available.
        /// </summary>
        /// <param name="ServerCallContext">ServerCallContext is a class representing the context of a
        /// server call in gRPC. It contains information about the incoming request, such as headers,
        /// cancellation tokens, and other metadata.</param>
        /// <returns>
        /// The method `GetClaimsFromContext` is returning a `ClaimsPrincipal` object extracted from the
        /// HTTP context user if a valid JWT token is found in the request headers. If the token is not
        /// found or is invalid, it returns `null`.
        /// </returns>
        private ClaimsPrincipal? GetClaimsFromContext(ServerCallContext context)
        {
            // Extract JWT token from metadata
            var authHeader = context.RequestHeaders.GetValue("authorization");
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }

            // You would validate the token here using your TokenService
            return context.GetHttpContext()?.User;
        }
    }
}
