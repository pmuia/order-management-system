using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Interfaces;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.Infrastructure.Repositories
{
	public class SecurityRepository : ISecurityRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;

		public SecurityRepository(ApplicationDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		public async Task<Client> AuthenticateClient(string apiKey, string appSecret)
		{
			var client = await _context.Clients
				.FirstOrDefaultAsync(c => c.ApiKey == apiKey && c.AppSecret == appSecret && c.IsActive);

			if (client != null)
			{
				client.LastAccessAt = DateTime.UtcNow;
				await _context.SaveChangesAsync();
			}

			return client;
		}

		public (string token, long expires) CreateAccessToken(Client client)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
			var expires = DateTime.UtcNow.AddMinutes(client.AccessTokenLifetimeInMins);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.NameIdentifier, client.ClientId.ToString()),
					new Claim(ClaimTypes.Name, client.Name),
					new Claim(ClaimTypes.Role, client.Role.ToString())
				}),
				Expires = expires,
				Issuer = _configuration["Jwt:Issuer"],
				Audience = _configuration["Jwt:Audience"],
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(key),
					SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return (tokenHandler.WriteToken(token), new DateTimeOffset(expires).ToUnixTimeSeconds());
		}
	}
}
