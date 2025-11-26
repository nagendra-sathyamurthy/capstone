# Food Delivery Application (FDA) - Capstone Project

A comprehensive microservices-based food delivery platform built with .NET 8, Node.js, React, and MongoDB.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Technologies](#technologies)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [Deployment](#deployment)
- [API Documentation](#api-documentation)
- [Project Structure](#project-structure)
- [Development Workflow](#development-workflow)
- [Testing](#testing)
- [Contributing](#contributing)

## 🎯 Overview

The Food Delivery Application is a full-stack microservices platform that enables:
- **Customers** to browse restaurants, order food, and track deliveries
- **Restaurant Owners** to manage menus, track orders, and manage staff
- **Operators** to process and fulfill orders
- **Kitchen Workers** to prepare orders
- **Delivery Agents** to deliver orders
- **IT Admins** to manage the entire platform

## 🏗 Architecture

The application follows a microservices architecture with the following services:

### Backend Services (.NET 8)
- **Authentication Service** (Port 8081) - User authentication and JWT token management
- **CRM Service** (Port 8083) - Customer relationship management and user profiles
- **Catalog Service** (Port 8082) - Restaurant and menu catalog management
- **Cart Service** (Port 8084) - Shopping cart management
- **Order Service** (Port 8085) - Order processing and management

### Gateway & Frontend
- **Gateway Service** (Port 5000) - Node.js/Express API Gateway with routing and proxy
- **Customer App** (Port 3000) - React 18 + TypeScript customer-facing application

### Database
- **MongoDB** (Port 27017) - NoSQL database for all services

## 🛠 Technologies

### Backend
- .NET 8.0
- ASP.NET Core Web API
- MongoDB Driver
- JWT Authentication
- Swagger/OpenAPI

### Frontend
- React 18
- TypeScript
- Axios
- React Router
- Material-UI / Custom Components

### Gateway
- Node.js 20
- Express.js
- HTTP Proxy Middleware

### DevOps
- Docker & Docker Compose
- Kubernetes (Local & Production)
- PowerShell Scripts
- GitHub Actions (Planned)

## 📦 Prerequisites

Before you begin, ensure you have the following installed:

### Required
- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 20+** - [Download](https://nodejs.org/)
- **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop)
- **Git** - [Download](https://git-scm.com/)

### Optional
- **MongoDB Compass** - For database management
- **Postman** - For API testing
- **Visual Studio Code** - Recommended IDE
- **Kubernetes (kubectl)** - For Kubernetes deployments

## 📥 Installation

### 1. Clone the Repository

```bash
git clone https://github.com/nagendra-sathyamurthy/capstone.git
cd capstone
```

### 2. Install Backend Dependencies

```bash
# Restore all .NET projects
dotnet restore fda/src/services/capstone.sln
```

### 3. Install Frontend Dependencies

```bash
# Install Gateway dependencies
cd fda/src/gateway
npm install

# Install Customer App dependencies
cd ../customer-app
npm install

cd ../../..
```

## ⚙️ Configuration

### Environment Variables

#### Customer App
Edit `fda/src/customer-app/.env.production`:
```env
REACT_APP_GATEWAY_URL=http://localhost:5000
```

#### Services
Each service uses `appsettings.json` and `appsettings.Development.json`:
- MongoDB connection strings
- JWT secret keys
- CORS settings
- Service ports

### MongoDB Configuration

MongoDB connection strings are configured in each service's `appsettings.json`:
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://mongodb:27017",
    "DatabaseName": "ServiceNameDb"
  }
}
```

## 🚀 Running the Application

### Option 1: Using Docker Compose (Recommended)

```bash
# Build and start all services
cd fda/devops/docker
docker-compose -f docker-compose-working.yml up --build

# Or run in detached mode
docker-compose -f docker-compose-working.yml up -d --build
```

**Access the application:**
- Customer App: http://localhost:3000
- Gateway API: http://localhost:5000
- Authentication API: http://localhost:8081/swagger
- Catalog API: http://localhost:8082/swagger
- CRM API: http://localhost:8083/swagger
- Cart API: http://localhost:8084/swagger
- Order API: http://localhost:8085/swagger

**Stop the services:**
```bash
docker-compose -f docker-compose-working.yml down
```

### Option 2: Using VS Code Tasks

The project includes VS Code tasks for building and running services:

```bash
# Build all services
Ctrl+Shift+P -> Tasks: Run Task -> build-all

# Run individual services in watch mode
Ctrl+Shift+P -> Tasks: Run Task -> watch-authentication
Ctrl+Shift+P -> Tasks: Run Task -> watch-catalog
Ctrl+Shift+P -> Tasks: Run Task -> watch-crm
Ctrl+Shift+P -> Tasks: Run Task -> watch-cart
```

### Option 3: Manual Run

**Start MongoDB:**
```bash
docker run -d -p 27017:27017 --name mongodb mongo:latest
```

**Start Backend Services:**
```bash
# Authentication Service
cd fda/src/services/authentication/API
dotnet run

# Repeat for other services (CRM, Catalog, Cart, Order)
```

**Start Gateway:**
```bash
cd fda/src/gateway
npm start
```

**Start Customer App:**
```bash
cd fda/src/customer-app
npm start
```

## 🌐 Deployment

### Local Kubernetes Deployment

```powershell
# Deploy to local Kubernetes (Docker Desktop)
cd fda/devops/jobs
.\deploy-local-k8s.ps1

# Cleanup
.\cleanup-local-k8s.ps1
```

### Production Kubernetes Deployment

```powershell
# Deploy to production cluster
cd fda/devops/jobs
.\deploy-production.ps1

# Cleanup
.\cleanup-environment.ps1
```

### Deployment Scripts

Located in `fda/devops/jobs/`:
- `deploy-local-k8s.ps1` - Deploy to local Kubernetes
- `deploy-production.ps1` - Deploy to production
- `cleanup-local-k8s.ps1` - Clean up local deployment
- `cleanup-environment.ps1` - Clean up production deployment
- `setup-local-user-secrets.ps1` - Set up local secrets
- `apply-secrets.ps1` - Apply secrets to Kubernetes

### Docker Images

Build individual service images:
```bash
# Authentication Service
docker build -t fda-authentication:latest ./fda/src/services/authentication

# Gateway Service
docker build -t fda-gateway:latest ./fda/src/gateway

# Customer App
docker build -t fda-customer-app:latest ./fda/src/customer-app
```

## 📚 API Documentation

### Swagger UI

Each service exposes Swagger UI for API documentation:
- Authentication: http://localhost:8081/swagger
- Catalog: http://localhost:8082/swagger
- CRM: http://localhost:8083/swagger
- Cart: http://localhost:8084/swagger
- Order: http://localhost:8085/swagger

### Postman Collections

Postman collections are available in `fda/postman-collections/`:
- `Authentication-Service.postman_collection.json`
- `Catalog-Service.postman_collection.json`
- `CRM-Service.postman_collection.json`
- `Cart-Service.postman_collection.json`
- `Order-Service.postman_collection.json`
- `Capstone-Workflow.postman_collection.json`

**Environments:**
- `Capstone-Local-Environment.postman_environment.json`
- `Capstone-Production-Environment.postman_environment.json`

## 📁 Project Structure

```
capstone/
├── fda/                                    # Main application folder
│   ├── devops/                            # DevOps configurations
│   │   ├── docker/                        # Docker configurations
│   │   │   └── docker-compose-working.yml # Docker Compose file
│   │   ├── jobs/                          # Deployment scripts
│   │   │   ├── deploy-local-k8s.ps1
│   │   │   ├── deploy-production.ps1
│   │   │   └── cleanup-*.ps1
│   │   └── kubernetes/                    # Kubernetes manifests
│   │       ├── local/                     # Local K8s configs
│   │       └── production/                # Production K8s configs
│   ├── docs/                              # Documentation
│   │   ├── services/                      # Service documentation
│   │   ├── devops/                        # DevOps documentation
│   │   └── testing/                       # Testing documentation
│   ├── postman-collections/               # Postman API collections
│   └── src/                               # Source code
│       ├── services/                      # Backend microservices
│       │   ├── authentication/            # Authentication service
│       │   ├── catalog/                   # Catalog service
│       │   ├── crm/                       # CRM service
│       │   ├── cart/                      # Cart service
│       │   └── order/                     # Order service
│       ├── gateway/                       # API Gateway (Node.js)
│       └── customer-app/                  # Customer frontend (React)
├── .vscode/                               # VS Code configurations
│   ├── tasks.json                         # Build tasks
│   └── launch.json                        # Debug configurations
├── .gitignore                             # Git ignore rules
└── README.md                              # This file
```

### Service Structure

Each .NET service follows this structure:
```
service-name/
├── API/                    # Web API project
│   ├── Controllers/        # API controllers
│   ├── Program.cs          # Application entry point
│   └── appsettings.json    # Configuration
├── DataAccess/             # Data access layer
│   ├── Repositories/       # Repository pattern
│   └── MongoDbContext.cs   # Database context
├── Models/                 # Domain models and DTOs
└── service-name.sln        # Solution file
```

## 🔄 Development Workflow

### Branching Strategy

- `master` - Production-ready code
- `develop` - Integration branch for features
- `feature/*` - Feature branches (branch from develop)
- `fix/*` - Bug fix branches (branch from develop)

### Workflow

1. Create a feature branch from `develop`:
   ```bash
   git checkout develop
   git pull origin develop
   git checkout -b feature/your-feature-name
   ```

2. Make your changes and commit:
   ```bash
   git add .
   git commit -m "feat: your feature description"
   ```

3. Push to remote:
   ```bash
   git push -u origin feature/your-feature-name
   ```

4. Create a Pull Request to `develop` branch

5. After review and merge, delete the feature branch

### Building Projects

```bash
# Build all services
dotnet build fda/src/services/capstone.sln

# Build individual services
dotnet build fda/src/services/authentication/authentication.sln
dotnet build fda/src/services/catalog/Catalog.sln
dotnet build fda/src/services/crm/crm.sln
dotnet build fda/src/services/cart/Cart.sln
dotnet build fda/src/services/order/Order.sln

# Clean build artifacts
dotnet clean fda/src/services/capstone.sln
```

## 🧪 Testing

### Running Tests with Newman

```bash
# Install Newman globally
npm install -g newman newman-reporter-htmlextra

# Run authentication tests
newman run fda/postman-collections/Authentication-Service.postman_collection.json \
  -e fda/postman-collections/Capstone-Local-Environment.postman_environment.json \
  -r htmlextra --reporter-htmlextra-export test-results/authentication-report.html
```

### Manual Testing

1. Start all services using Docker Compose
2. Import Postman collections from `fda/postman-collections/`
3. Import the local environment
4. Execute requests in the workflow collection

## 🤝 Contributing

### Code Style

- **C#**: Follow Microsoft C# coding conventions
- **TypeScript/JavaScript**: Follow Airbnb JavaScript Style Guide
- **Naming**: Use meaningful, descriptive names

### Commit Messages

Follow conventional commits format:
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `refactor:` - Code refactoring
- `test:` - Adding tests
- `chore:` - Maintenance tasks

### Pull Request Process

1. Ensure all tests pass
2. Update documentation if needed
3. Request review from team members
4. Address review comments
5. Merge after approval

## 📝 Additional Documentation

- [Local Development Setup](fda/docs/LOCAL-DEVELOPMENT-SETUP.md)
- [Deployment Strategy](fda/docs/devops/DEPLOYMENT-STRATEGY.md)
- [Secrets Management](fda/docs/devops/SECRETS-MANAGEMENT.md)
- [RBAC Comprehensive Guide](fda/docs/services/RBAC-COMPREHENSIVE.md)
- [Testing Guide](fda/docs/testing/README.md)

## 📧 Contact

For questions or support, please contact the development team.

## 📄 License

This project is part of a capstone project and is for educational purposes.

---

**Last Updated:** November 26, 2025
