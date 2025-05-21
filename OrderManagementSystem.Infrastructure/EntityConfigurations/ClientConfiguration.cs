using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Domain.Entities;

namespace OrderManagementSystem.Infrastructure.EntityConfigurations
{
	public class ClientConfiguration : IEntityTypeConfiguration<Client>
	{
		public void Configure(EntityTypeBuilder<Client> builder)
		{
			builder.ToTable("Clients");
			builder.HasKey(x => x.ClientId);
			builder.Property(x => x.ClientId).ValueGeneratedNever();

			builder.Property(x => x.ClientId).ValueGeneratedNever();
			builder.Property(x => x.AppSecret).HasMaxLength(128);
		}
	}
}
