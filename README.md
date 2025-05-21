# Order Management System

Order management system built with .NET 8, PostgreSQL, and JWT authentication.

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

## Prerequisites

- .NET 8 SDK
- PostgreSQL 15 or later
- Visual Studio 2022 or VS Code

## Database Setup

1. Install PostgreSQL
2. Create a new database:
```sql
CREATE DATABASE order_management_system;
```

3. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=order_management_system;Username=your_username;Password=your_password"
  }
}
```

## Seeded Data

The system comes pre-seeded with the following data:

### Admin Client
- ClientId: Auto-generated
- ApiKey: "admin-api-key"
- AppSecret: "admin-app-secret"
- Role: Admin
- AccessTokenLifetimeInMins: 60
- AuthorizationCodeLifetimeInMins: 5

### Sample Customers


### Sample Orders
- Each customer has 2-3 orders with different statuses
- Orders include tracking numbers in format: ORD-yyyyMMdd-sequence
- Order statuses: Created, Processing, Shipped, Delivered, Cancelled
- Each order contains 1-2 order lines with products

## Authentication

### Getting an Access Token

1. Make a POST request to `/api/auth/token`:
```http
POST /api/auth/token
Content-Type: application/json

{
    "apiKey": "admin-api-key",
    "appSecret": "admin-app-secret"
}
```

2. The response will include:
```json
{
    "accessToken": "your.jwt.token",
    "expiresIn": 3600
}
```

3. Use the token in subsequent requests:
```http
GET /api/v1/orders
Authorization: Bearer your.jwt.token
```

## API Versions

The API supports multiple versions:

### V1 (Default)
- Basic CRUD operations for orders
- Analytics without status filtering
- Standard endpoints

### V2
- Enhanced order filtering by status
- Improved analytics with status filtering
- Additional query parameters

Access V2 endpoints using:
- URL: `/api/v2/orders`
- Header: `x-api-version: 2.0`
- Media Type: `application/json;v=2.0`

## Running the Application

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run the following commands:
```bash
dotnet restore
dotnet build
dotnet run
```

4. The application will:
   - Apply database migrations
   - Seed the initial data
   - Start the web server

5. Access the API documentation at:
   - Swagger UI: `https://localhost:5001`
   - API Endpoints: `https://localhost:5001/api/v1/orders`

## Development

### Database Migrations
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Adding New Features
1. Create new migrations for database changes
2. Update the seed data if needed
3. Add new API versions for breaking changes
4. Update the documentation

## Security

- JWT authentication is required for all endpoints
- Role-based authorization (Admin, Manager, User)
- Secure password hashing
- API key and secret for client authentication
- HTTPS enabled by default

## Logging

The application includes comprehensive logging:
- Console logging for development
- Debug logging for detailed information
- Event source logging for Windows Event Log
- Structured logging with parameters
- Exception handling and logging

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
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request