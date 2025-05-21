using Moq;
using OrderManagementSystem.Application.Services;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enum;
using OrderManagementSystem.Domain.Interfaces;
using Xunit;

namespace OrderManagementSystem.Tests.Services
{
	public class DiscountServiceTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly DiscountService _discountService;

        public DiscountServiceTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _discountService = new DiscountService(_customerRepositoryMock.Object);
        }

        [Fact]
        public async Task CalculateDiscount_PlatinumCustomer_ReturnsHighestDiscount()
        {
            // Arrange
            var customer = new Customer
            {
                FirstName = "Paul",
                LastName =  "Muia",
                Email = "paul@gmail.com",
                CustomerId = Guid.NewGuid(),
                Segment = CustomerSegment.Platinum,
                TotalSpent = 15000,
                LastOrderDate = DateTime.UtcNow.AddDays(-30),
				CreatedBy = "Admin",
			};

            var order = new Order
            {
                CustomerId = customer.CustomerId,
				CreatedBy = "Admin",
                TrackingNumber = "1",
				OrderItems = new List<OrderLine>
                {
                    new OrderLine { Quantity = 25, UnitPrice = 100,CreatedBy = "Admin", }
                }
            };

            _customerRepositoryMock.Setup(x => x.GetByIdAsync(customer.CustomerId))
                .ReturnsAsync(customer);

            // Act
            var discount = await _discountService.CalculateDiscountAsync(order);

            // Assert
            Assert.Equal(0.25m, discount); // Should return bulk order discount (25%)
        }

        [Fact]
        public async Task CalculateDiscount_NewCustomer_ReturnsNoDiscount()
        {
            // Arrange
            var customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
				FirstName = "Paul",
				LastName = "Muia",
				Email = "paul@gmail.com",
				Segment = CustomerSegment.Regular,
                TotalSpent = 0,
                LastOrderDate = null,
				CreatedBy = "Admin",
			};

            var order = new Order
            {
                CustomerId = customer.CustomerId,
                TrackingNumber ="1",
				CreatedBy = "Admin",
				OrderItems = new List<OrderLine>
                {
                    new OrderLine { Quantity = 1, UnitPrice = 100,CreatedBy = "Admin", }
                }
            };

            _customerRepositoryMock.Setup(x => x.GetByIdAsync(customer.CustomerId))
                .ReturnsAsync(customer);

            // Act
            var discount = await _discountService.CalculateDiscountAsync(order);

            // Assert
            Assert.Equal(0m, discount);
        }

        [Fact]
        public async Task CalculateDiscount_LoyalCustomer_ReturnsLoyaltyDiscount()
        {
            // Arrange
            var customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
				FirstName = "Paul",
				LastName = "Muia",
				Email = "paul@gmail.com",
                Segment = CustomerSegment.Gold,
                TotalSpent = 12000,
                LastOrderDate = DateTime.UtcNow.AddDays(-60),
				CreatedBy = "Admin",
			};

            var order = new Order
            {
                CustomerId = customer.CustomerId,
				CreatedBy = "Admin",
                TrackingNumber="1",
				OrderItems = new List<OrderLine>
                {
                    new OrderLine { Quantity = 1, UnitPrice = 100,CreatedBy = "Admin", }
                }
            };

            _customerRepositoryMock.Setup(x => x.GetByIdAsync(customer.CustomerId))
                .ReturnsAsync(customer);

            // Act
            var discount = await _discountService.CalculateDiscountAsync(order);

            // Assert
            Assert.Equal(0.20m, discount); // Should return loyalty discount (20%)
        }

        [Fact]
        public async Task GetCustomerSegmentDiscount_PlatinumCustomer_ReturnsCorrectDiscount()
        {
            // Act
            var discount = await _discountService.GetCustomerSegmentDiscountAsync(CustomerSegment.Platinum);

            // Assert
            Assert.Equal(0.15m, discount);
        }

        [Fact]
        public async Task GetBulkOrderDiscount_LargeOrder_ReturnsCorrectDiscount()
        {
            // Arrange
            var order = new Order
            {
				CreatedBy = "Admin",
				TrackingNumber = "1",
				OrderItems = new List<OrderLine>
                {
                    new OrderLine { Quantity = 25, UnitPrice = 100,CreatedBy = "Admin", }
                }
            };

            // Act
            var discount = await _discountService.GetBulkOrderDiscountAsync(order);

            // Assert
            Assert.Equal(0.25m, discount);
        }
    }
} 