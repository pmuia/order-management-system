# Order Management System

A modern, scalable order management system built with .NET 8, following clean architecture principles.

## Features

- Order management with status tracking
- Customer management
- Discount system
- Analytics and reporting
- Performance optimizations:
  - In-memory caching for frequently accessed data
  - Optimized queries with projections
  - Batch processing for bulk operations
  - Pagination for large result sets
  - Efficient data transfer with DTOs

## Architecture

The solution follows clean architecture principles with the following layers:

- **Domain**: Core business logic and entities
- **Application**: Business rules and use cases
- **Infrastructure**: External concerns (database, caching)
- **API**: HTTP endpoints and controllers

## Performance Optimizations

### Caching
- In-memory caching for frequently accessed data
- Cache invalidation on data modifications
- Configurable cache duration (default: 30 minutes)

### Query Optimization
- Projections to reduce data transfer
- Efficient joins and includes
- Pagination support for large datasets
- Optimized batch operations

### Data Transfer
- Lightweight DTOs for API responses
- Selective data loading
- Efficient serialization

## Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository
```bash
git clone https://github.com/yourusername/OrderManagementSystem.git
```

2. Update the connection string in `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=OrderManagementSystem;Username=your_username;Password=your_password"
  }
}
```

3. Run the database migrations
```bash
dotnet ef database update
```

4. Run the application
```bash
dotnet run
```

## API Endpoints

### Orders
- `GET /api/orders` - Get paginated list of orders
- `GET /api/orders/summaries` - Get lightweight order summaries
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create new order
- `PUT /api/orders/{id}` - Update order
- `DELETE /api/orders/{id}` - Delete order
- `PUT /api/orders/batch/status` - Update multiple order statuses
- `GET /api/orders/analytics` - Get order analytics

### Customers
- `GET /api/customers` - Get list of customers
- `GET /api/customers/{id}` - Get customer by ID
- `POST /api/customers` - Create new customer
- `PUT /api/customers/{id}` - Update customer
- `DELETE /api/customers/{id}` - Delete customer

## Testing

Run the tests using:
```bash
dotnet test
```

## Contributing

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a new Pull Request