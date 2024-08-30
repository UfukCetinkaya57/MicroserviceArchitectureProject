# Online Education Platform

This project is an online education platform based on microservice architecture. The project aims to provide a flexible, scalable, and sustainable structure by using different databases and technologies.

## Technologies Used

- **.NET Core 5.0:** Forms the core framework of the project.
- **ASP.NET Core MVC:** Used in the web application layer.
- **Entity Framework Core:** Used for database operations.
- **AutoMapper:** Used for object mapping.
- **FluentValidation:** Used for data validation.
- **Refit:** Used for HTTP-based communication between microservices.
- **RabbitMQ:** Used as the messaging infrastructure between microservices.
- **MassTransit:** Used for integration with RabbitMQ.
- **Ocelot:** Serves as the API Gateway.
- **Polly:** Used for error handling and retry mechanisms.
- **Docker:** Used for containerizing and managing microservices.
- **Kubernetes:** Used for container orchestration and management.

## Databases Used

Each microservice manages its data using its specific database:

- **SQL Server:** Used in the Order and Payment services.
- **MongoDB:** Used for the Catalog service.
- **PostgreSQL:** Used for the User service.
- **In-Memory Database:** Used for testing and in some services for fast data access.
- **Redis:** Used as a caching mechanism.

## Docker Integration

The project is containerized using Docker. You can run the project using Docker with the following steps:

1. If Docker is not installed, download and install Docker.
2. The project directory contains a `Dockerfile` and a `docker-compose.yml` file.
3. Open the terminal or command prompt and navigate to the project directory.
4. Run the command `docker-compose up` to start all the microservices.

Docker will download all dependencies and run the microservices in their respective containers.

## Project Purpose and Features

This project aims to build the infrastructure of an online education platform as an educational project. The technologies used in this project focus on modern software development practices such as microservices architecture and messaging infrastructure.

### Features

- User registration and authentication
- Course creation, update, and deletion
- Cart and payment processing
- Order tracking and management
- Photo upload and deletion (PhotoStock service)
- Messaging and integration between microservices
