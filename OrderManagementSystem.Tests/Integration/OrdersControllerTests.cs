using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using OrderManagementSystem.API;
using OrderManagementSystem.Application.Services;
using OrderManagementSystem.Domain.Entities;
using Xunit;

namespace OrderManagementSystem.Tests.Integration
{
    public class OrdersControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public OrdersControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateOrder_ValidOrder_ReturnsCreatedOrder()
        {
			// Arrange
			var order = new Order
			{
				CustomerId = Guid.NewGuid(),
				CreatedBy = "Admin",
				TrackingNumber = "1",
				OrderItems = new List<OrderLine>
				{
					new OrderLine
					{
						ProductName = "Test Product",
						UnitPrice = 100,
						Quantity = 1,
						CreatedBy = "Admin"
					}
				}
			};

			var content = new StringContent(
                JsonSerializer.Serialize(order),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/orders", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdOrder = JsonSerializer.Deserialize<Order>(responseContent);
            Assert.NotNull(createdOrder);
            Assert.Equal(order.CustomerId, createdOrder.CustomerId);
            Assert.Equal(OrderStatus.Created, createdOrder.Status);
        }

        [Fact]
        public async Task UpdateOrderStatus_ValidTransition_ReturnsUpdatedOrder()
        {
            // Arrange
            // First create an order
            var order = new Order
            {
                CustomerId = Guid.NewGuid(),
                CreatedBy = "Admin",
                TrackingNumber = "1",
                OrderItems = new List<OrderLine>
                {
                    new OrderLine
                    {
                        ProductName = "Test Product",
                        UnitPrice = 100,
                        Quantity = 1,
                        CreatedBy = "Admin"
                    }
                }
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(order),
                Encoding.UTF8,
                "application/json");

            var createResponse = await _client.PostAsync("/api/orders", createContent);
            var createdOrder = JsonSerializer.Deserialize<Order>(
                await createResponse.Content.ReadAsStringAsync());

            // Act
            var updateContent = new StringContent(
                JsonSerializer.Serialize(OrderStatus.Processing),
                Encoding.UTF8,
                "application/json");

            var updateResponse = await _client.PutAsync(
                $"/api/orders/{createdOrder.OrderId}/status",
                updateContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            var updatedOrder = JsonSerializer.Deserialize<Order>(
                await updateResponse.Content.ReadAsStringAsync());
            Assert.Equal(OrderStatus.Processing, updatedOrder.Status);
        }

        [Fact]
        public async Task GetOrderAnalytics_ReturnsAnalytics()
        {
            // Act
            var response = await _client.GetAsync("/api/orders/analytics");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var analytics = JsonSerializer.Deserialize<OrderAnalytics>(
                await response.Content.ReadAsStringAsync());
            Assert.NotNull(analytics);
        }
    }
} 