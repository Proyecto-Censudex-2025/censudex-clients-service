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
        private readonly IClientRepository _clientRepository;
        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _clientRepository.GetAll();
            var clientsDtos = clients.ToDtoEnumerable();
            return Ok(clientsDtos);
        }

        [HttpGet("GetClient")]
        public async Task<IActionResult> GetClient(string Id)
        {
            var client = await _clientRepository.GetClient(Id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client.ToVisualizeClientDtoFromClient());
        }

        [HttpGet("ClientFilter")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetClients([FromQuery] QueryObject query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var clients = await _clientRepository.GetClients(query);
            var clientDtos = clients.Select(u => u.ToVisualizeClientDtoFromClient());
            return Ok(clientDtos);
        }

        [HttpDelete("enable-disable/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnableDisableClient([FromRoute] string Id)
        {
            await _clientRepository.EnableDisableClient(Id);
            return Ok();
        }

        [HttpPatch("update-client")]
        [Authorize]
        public async Task<IActionResult> UpdateClient([FromBody] UpdateClientDto clientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var client = await _clientRepository.UpdateClient(clientDto, User);
                if (client == null)
                {
                    return NotFound();
                }

                return Ok(client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] CreateClientDto createClientDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newClient = await _clientRepository.CreateClient(createClientDto);

                return Ok(newClient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

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