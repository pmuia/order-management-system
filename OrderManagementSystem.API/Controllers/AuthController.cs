using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Domain.Interfaces;
using OrderManagementSystem.Domain.Models;

namespace OrderManagementSystem.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly ISecurityRepository _securityRepository;

		public AuthController(ISecurityRepository securityRepository)
		{
			_securityRepository = securityRepository;
		}

		/// <summary>
		/// Generates a JWT Bearer access token that can be used to authorize subsequent requests.      
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, AllowAnonymous, Route("token")]
		[Produces(MediaTypeNames.Application.Json), Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(typeof(ResponseObject<TokenDto>), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> CreateToken([FromBody, Required] TokenRequest request)
		{
			var client = await _securityRepository.AuthenticateClient(request.ApiKey, request.AppSecret);
			if (client is null) return Forbid();

			var (token, expires) = _securityRepository.CreateAccessToken(client);

			return Ok(new ResponseObject<TokenDto> 
			{ 
				Data = new[] 
				{ 
					new TokenDto 
					{ 
						AccessToken = token, 
						Expires = expires, 
						TokenType = "Bearer" 
					} 
				} 
			});
		}
	}
}
