using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientsService.src.DTOs;
using ClientsService.src.Helper;
using ClientsService.src.Interface;
using ClientsService.src.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientsService.src.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {

        /* This code snippet is defining a constructor for the `ClientController` class in C#. The
        constructor takes an `IClientRepository` object as a parameter and assigns it to the private
        readonly field `_clientRepository`. This is a common practice in C# to use constructor
        injection for dependency injection. By passing the `IClientRepository` object to the
        constructor, the `ClientController` class can use the methods and properties of the
        `IClientRepository` interface throughout its implementation. */
        private readonly IClientRepository _clientRepository;
        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        /// <summary>
        /// The function handles a POST request for user login, validating the input and returning the
        /// logged-in client or an error message.
        /// </summary>
        /// <param name="LoginDto">LoginDto is a data transfer object (DTO) that typically contains the
        /// necessary information for a user to log in, such as username and password. In this case, the
        /// LoginDto object is being passed as a parameter to the Login method in a controller for
        /// handling user login requests.</param>
        /// <returns>
        /// The Login method in the controller is returning an IActionResult. If the ModelState is not
        /// valid, it returns a BadRequest with the ModelState. If the login is successful, it returns
        /// an Ok response with the client information. If an exception occurs during the login process,
        /// it returns a StatusCode 500 with the exception message.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var client = await _clientRepository.Login(loginDto);

                return Ok(client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    } 
}