# Capstone Documentation

This folder contains all the documentation for the Capstone microservices platform, organized by category for easy navigation.

## 📁 Documentation Structure

### 🚀 DevOps Documentation (`devops/`)
- **[DEPLOYMENT-STRATEGY.md](devops/DEPLOYMENT-STRATEGY.md)** - Complete deployment strategy and architecture guide
- **[SECRETS-MANAGEMENT.md](devops/SECRETS-MANAGEMENT.md)** - MongoDB credentials and secrets management

### 🔧 Services Documentation (`services/`)
- **[README.md](services/README.md)** - Catalog service documentation and API reference
- **[RBAC-COMPREHENSIVE.md](services/RBAC-COMPREHENSIVE.md)** - Complete Role-Based Access Control system design and implementation

### 🧪 Testing Documentation (`testing/`)
- **[README.md](testing/README.md)** - Postman collections and testing overview
- **[NEWMAN_TEST_FIXES.md](testing/NEWMAN_TEST_FIXES.md)** - Newman API testing analysis and fixes
- **[Newman-API-Testing-Report-2025-11-13.md](testing/Newman-API-Testing-Report-2025-11-13.md)** - Detailed Newman test execution report
- **[TEST-DATA-REFERENCE.md](testing/TEST-DATA-REFERENCE.md)** - Test data reference guide

### 🐙 GitHub Documentation (`github/`)
- **[copilot-instructions.md](github/copilot-instructions.md)** - GitHub Copilot configuration and instructions

## 🎯 Quick Navigation

### 🚀 Getting Started
1. **Architecture Overview**: [`devops/DEPLOYMENT-STRATEGY.md`](devops/DEPLOYMENT-STRATEGY.md)
2. **Setup Secrets**: [`devops/SECRETS-MANAGEMENT.md`](devops/SECRETS-MANAGEMENT.md)
3. **Test APIs**: [`testing/README.md`](testing/README.md)

### 🔍 Service Development
1. **Service Architecture**: [`services/README.md`](services/README.md)
2. **Authentication/RBAC**: [`services/RBAC-COMPREHENSIVE.md`](services/RBAC-COMPREHENSIVE.md)
3. **API Testing**: [`testing/NEWMAN_TEST_FIXES.md`](testing/NEWMAN_TEST_FIXES.md)

### 🛠️ Infrastructure Management
1. **Deployment Guide**: [`devops/DEPLOYMENT-STRATEGY.md`](devops/DEPLOYMENT-STRATEGY.md)
2. **Secrets Management**: [`devops/SECRETS-MANAGEMENT.md`](devops/SECRETS-MANAGEMENT.md)
3. **Authentication System**: [`services/RBAC-COMPREHENSIVE.md`](services/RBAC-COMPREHENSIVE.md)

---

## 📊 Architecture Overview

The Capstone platform follows a microservices architecture with:

- **4 Active Services**: Authentication, Catalog, CRM, Cart
- **Local Kubernetes**: Streamlined deployment strategy
- **Shared MongoDB**: Single instance for efficiency
- **Newman Testing**: Comprehensive API validation
- **PowerShell Jobs**: Automated deployment and secrets management

## 🔗 Related Resources

- **Source Code**: `../src/services/`
- **Deployment Scripts**: `../devops/kubernetes/`
- **Automation Jobs**: `../devops/jobs/`
- **Postman Collections**: `../postman-collections/`

---

*Last Updated: November 13, 2025*
*Documentation moved to centralized location for better organization*