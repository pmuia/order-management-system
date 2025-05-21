using System.Threading.Tasks;
using OrderManagementSystem.Domain.Entities;

namespace OrderManagementSystem.Domain.Interfaces
{
	public interface ISecurityRepository
	{
		Task<Client> AuthenticateClient(string apiKey, string appSecret);
		(string token, long expires) CreateAccessToken(Client client);
	}
}
