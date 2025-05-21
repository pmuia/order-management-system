using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enum;
using OrderManagementSystem.Domain.Interfaces;
using OrderManagementSystem.Infrastructure.Services;

namespace OrderManagementSystem.Infrastructure.Data
{
    public class Seed : ISeed
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly TrackingNumberGenerator _trackingNumberGenerator;

        public Seed(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _trackingNumberGenerator = new TrackingNumberGenerator(context);
        }

        public async Task SeedDefaults()
        {
            if (!await _context.Clients.AnyAsync())
            {
                var adminClient = new Client
                {
                    ClientId = Guid.NewGuid(),
                    ApiKey = _configuration["DefaultAdmin:ApiKey"] ?? "admin-api-key",
                    AppSecret = _configuration["DefaultAdmin:AppSecret"] ?? "admin-app-secret",
                    Name = "Admin Client",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    Role = Roles.Admin,
                    AccessTokenLifetimeInMins = 60,
                    AuthorizationCodeLifetimeInMins = 60,
                    CreatedBy = "System"
                };

                await _context.Clients.AddAsync(adminClient);
            }

            if (!await _context.Customers.AnyAsync())
            {
                var customers = new[]
                {
                    new Customer
                    {
                        CustomerId = Guid.NewGuid(),
                        FirstName = "Paul",
                        LastName = "Muia",
                        Email = "paul.muia@gmail.com",
                        Phone = "123-456-7890",
                        Segment = CustomerSegment.Gold,
                        TotalSpent = 5000,
                        OrderCount = 10,
                        LastOrderDate = DateTime.UtcNow.AddDays(-5),
                        CreatedBy = "System"
                    },
                    new Customer
                    {
                        CustomerId = Guid.NewGuid(),
                        FirstName = "Jane",
                        LastName = "Jane",
                        Email = "jane.jane@gmail.com",
                        Phone = "098-765-4321",
                        Segment = CustomerSegment.Platinum,
                        TotalSpent = 15000,
                        OrderCount = 25,
                        LastOrderDate = DateTime.UtcNow.AddDays(-2),
                        CreatedBy = "System"
                    },
                    new Customer
                    {
                        CustomerId = Guid.NewGuid(),
                        FirstName = "Andrew",
                        LastName = "Mutuku",
                        Email = "andrew.mutuku@gmail.com",
                        Phone = "555-555-5555",
                        Segment = CustomerSegment.Regular,
                        TotalSpent = 500,
                        OrderCount = 2,
                        LastOrderDate = DateTime.UtcNow.AddDays(-30),
                        CreatedBy = "System"
                    }
                };

                await _context.Customers.AddRangeAsync(customers);
            }

            if (!await _context.Orders.AnyAsync())
            {
                var customers = await _context.Customers.ToListAsync();
                var orders = new List<Order>();

                await _trackingNumberGenerator.InitializeLastSequenceAsync();

                foreach (var customer in customers)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var order = new Order
                        {
                            OrderId = Guid.NewGuid(),
                            CustomerId = customer.CustomerId,
                            OrderDate = DateTime.UtcNow.AddDays(-i * 10),
                            Status = (OrderStatus)(i % 4), 
                            TotalAmount = 100 * (i + 1),
                            DiscountedAmount = 90 * (i + 1),
                            TrackingNumber = await _trackingNumberGenerator.GenerateTrackingNumberAsync(),
                            CreatedBy = "System"
                        };

                        order.OrderItems = new List<OrderLine>
                        {
                            new OrderLine
                            {
                                OrderLineId = Guid.NewGuid(),
                                ProductName = $"Product {i + 1}",
                                Quantity = i + 1,
                                UnitPrice = 50,
                                TotalPrice = 50 * (i + 1),
                                CreatedBy = "System"
                            }
                        };

                        orders.Add(order);
                    }
                }

                await _context.Orders.AddRangeAsync(orders);
            }

            await _context.SaveChangesAsync();
        }
    }
} 