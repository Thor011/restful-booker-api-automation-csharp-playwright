# RestfulBooker API Automation - C# + Playwright

[![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-10.0-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Playwright](https://img.shields.io/badge/Playwright-1.57.0-2EAD33?logo=playwright)](https://playwright.dev/dotnet/)
[![NUnit](https://img.shields.io/badge/NUnit-3.x-22B14C?logo=nunit)](https://nunit.org/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

> **Comprehensive API test automation framework for RestfulBooker API using C#, Playwright, and NUnit with BDD-style Gherkin feature files**

## 📋 Overview

A production-ready API testing framework demonstrating modern test automation practices with **63 comprehensive test scenarios** covering CRUD operations, authentication, data validation, filtering, error handling, and end-to-end workflows. Built with Playwright's API testing capabilities to bypass rate limiting through browser-like HTTP requests.

### Key Features

✅ **Playwright API Testing** - Uses Playwright's API request context instead of traditional HTTP clients  
✅ **BDD with Gherkin** - 9 feature files with business-readable scenarios  
✅ **63 Test Scenarios** - Comprehensive coverage including edge cases  
✅ **94% Pass Rate** - 59/63 tests passing (4 failures reveal actual API bugs)  
✅ **Multi-Platform CI/CD** - GitHub Actions, Azure Pipelines, and GitLab CI configurations  
✅ **Clean Architecture** - Organized helpers, models, and test structure  
✅ **Detailed Logging** - Custom test logger for debugging and reporting  
✅ **Data Generation** - Dynamic test data creation utilities  

## 🚀 Quick Start

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or higher
- [Git](https://git-scm.com/downloads)

### Installation

```bash
# Clone the repository
git clone https://github.com/YOUR_USERNAME/restful-booker-api-automation-csharp-playwright.git
cd restful-booker-api-automation-csharp-playwright

# Restore NuGet packages
dotnet restore

# Install Playwright browsers
dotnet tool install --global Microsoft.Playwright.CLI
playwright install --with-deps

# Build the solution
dotnet build
```

### Run Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test category
dotnet test --filter "FullyQualifiedName~BookingCrudTests"
```

## 📁 Project Structure

```
RestfulBookerApiTests.Tests/
├── Config/
│   └── TestConfig.cs                 # API configuration and settings
├── Documentation/
│   ├── TEST_SUITE_SUMMARY.md         # Comprehensive test documentation
│   └── QUICK_REFERENCE.md            # Quick reference guide
├── Features/                          # Gherkin BDD feature files
│   ├── Authentication.feature
│   ├── BookingCrud.feature
│   ├── DataValidation.feature
│   ├── EndToEndScenarios.feature
│   ├── ErrorHandling.feature
│   ├── SearchAndFiltering.feature
│   ├── Performance.feature
│   ├── Idempotency.feature
│   ├── BoundaryTesting.feature
│   └── README.md
├── Helpers/
│   ├── ApiHelper.cs                   # Playwright API client wrapper
│   ├── TestDataGenerator.cs          # Dynamic test data generation
│   └── TestLogger.cs                  # Custom logging utility
├── Models/
│   └── Booking.cs                     # Data models for API requests/responses
├── Scenarios/
│   └── EndToEndScenarioTests.cs       # Complete workflow tests
└── Tests/
    ├── AuthenticationTests.cs         # Authentication & authorization (7 tests)
    ├── BookingCrudTests.cs            # CRUD operations (10 tests)
    ├── DataValidationTests.cs         # Data validation (15 tests)
    ├── ErrorHandlingTests.cs          # Error scenarios (10 tests)
    └── FilteringTests.cs              # Search & filtering (10 tests)
```

## 🧪 Test Coverage

| Test Category | Count | Description |
|--------------|-------|-------------|
| **CRUD Operations** | 10 | Create, Read, Update, Delete booking operations |
| **Authentication** | 7 | Token-based and Basic auth scenarios |
| **Data Validation** | 15 | Unicode, boundaries, edge cases, date handling |
| **Filtering & Search** | 10 | Name, date, and multi-parameter filtering |
| **Error Handling** | 10 | Missing fields, invalid types, XSS, SQL injection |
| **End-to-End Scenarios** | 6 | Complete booking lifecycle workflows |
| **Performance** | 2 | Response time and concurrent requests |
| **Total** | **63** | Comprehensive API coverage |

### Pass Rate: 94% (59/63)

**Failing Tests** (reveal actual API validation bugs):
- `TCERR002` - API accepts invalid data types
- `TCERR004` - XSS payload not sanitized (security issue)
- `TCERR008` - Negative prices accepted
- `TCFILTER005` - Multiple parameter filtering broken

## 🛠️ Technology Stack

- **Framework:** .NET 6.0
- **Language:** C# 10.0
- **Test Framework:** NUnit 3.x
- **API Client:** Microsoft.Playwright 1.57.0
- **Assertions:** FluentAssertions 6.x
- **JSON:** Newtonsoft.Json 13.x
- **CI/CD:** GitHub Actions, Azure Pipelines, GitLab CI

## 🌐 API Under Test

**RestfulBooker API** - https://restful-booker.herokuapp.com

A RESTful web service for booking hotel rooms, providing endpoints for:
- Health check (`GET /ping`)
- Authentication (`POST /auth`)
- Booking CRUD operations (`GET/POST/PUT/PATCH/DELETE /booking`)
- Filtering (`GET /booking?firstname=X&lastname=Y`)

## 📊 CI/CD Pipelines

Three pipeline configurations included:

### GitHub Actions (`.github/workflows/dotnet.yml`)
- Runs on Ubuntu and Windows runners
- Automatic browser installation
- Test result publishing with artifacts

### Azure Pipelines (`azure-pipelines.yml`)
- Multi-stage pipeline (Build + Test)
- Code coverage collection
- Cross-platform testing

### GitLab CI (`.gitlab-ci.yml`)
- Build, Linux, and Windows test stages
- Nightly scheduled runs
- JUnit report integration

## 🎯 Why Playwright for API Testing?

Initially built with RestSharp, the project was migrated to **Playwright's API testing** capabilities to solve HTTP 418 rate limiting issues. Playwright mimics real browser requests with proper User-Agent headers and browser fingerprints, successfully bypassing bot detection while maintaining all API testing functionality.

**Benefits:**
- Browser-like HTTP requests bypass rate limiting
- Built-in request/response interception
- Automatic retry and timeout handling
- Consistent with modern web automation

## 📖 Documentation

- **[TEST_SUITE_SUMMARY.md](RestfulBookerApiTests.Tests/Documentation/TEST_SUITE_SUMMARY.md)** - Detailed test scenarios and coverage
- **[QUICK_REFERENCE.md](RestfulBookerApiTests.Tests/Documentation/QUICK_REFERENCE.md)** - Quick setup and run guide
- **[Features README](RestfulBookerApiTests.Tests/Features/README.md)** - Gherkin feature file documentation

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Links

- [RestfulBooker API Documentation](https://restful-booker.herokuapp.com/apidoc/)
- [Playwright .NET Documentation](https://playwright.dev/dotnet/)
- [NUnit Documentation](https://docs.nunit.org/)
- [FluentAssertions Documentation](https://fluentassertions.com/)

## 📧 Contact

**GitHub:** [@YOUR_USERNAME](https://github.com/YOUR_USERNAME)

---

⭐ **Star this repository** if you find it helpful!

**Keywords:** api testing, automation, csharp, dotnet, playwright, nunit, restful-api, bdd, gherkin, ci-cd, github-actions, azure-pipelines, gitlab-ci, test-automation, qa-automation
