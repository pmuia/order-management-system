using System.Threading.Tasks;

namespace OrderManagementSystem.Domain.Interfaces
{
    public interface ISeed
    {
        Task SeedDefaults();
    }
} 