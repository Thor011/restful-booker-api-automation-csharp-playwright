# Feature Files - Gherkin Scenarios

This directory contains Gherkin feature files that describe test scenarios in a business-readable format. These files are written using the **Gherkin syntax** and can be understood by both technical and non-technical stakeholders.

## What is Gherkin?

Gherkin is a business-readable, domain-specific language that lets you describe software behavior without detailing how that functionality is implemented. It uses a simple syntax with keywords like:

- **Feature**: High-level description of a software feature
- **Scenario**: Concrete example of business rule
- **Given**: Preconditions or context
- **When**: Action or event
- **Then**: Expected outcome
- **And**: Additional conditions or outcomes
- **But**: Negative condition or outcome

## Feature Files Overview

### 1. BookingCrud.feature
**Test Count**: 10 scenarios  
**Focus**: Basic booking operations (Create, Read, Update, Delete)  
**Business Value**: Ensures core booking functionality works correctly

**Key Scenarios**:
- Creating new bookings
- Retrieving booking information
- Updating bookings with authentication
- Deleting bookings
- Handling non-existent bookings

---

### 2. Authentication.feature
**Test Count**: 7 scenarios  
**Focus**: Security and access control  
**Business Value**: Protects bookings from unauthorized modifications

**Key Scenarios**:
- Token-based authentication
- Authorization for updates and deletions
- Rejecting unauthorized access
- Basic authentication support

---

### 3. DataValidation.feature
**Test Count**: 15 scenarios  
**Focus**: Data quality and integrity  
**Business Value**: Ensures only valid data is stored in the system

**Key Scenarios**:
- Unicode character support
- Price validation
- Date validation (leap years, boundaries)
- Name length validation
- Security (preventing command injection)

---

### 4. SearchAndFiltering.feature
**Test Count**: 10 scenarios  
**Focus**: Finding bookings quickly  
**Business Value**: Helps staff locate specific reservations efficiently

**Key Scenarios**:
- Search by guest name (first, last, or both)
- Filter by dates (check-in, check-out)
- Handle special characters and encoding
- Case sensitivity handling

---

### 5. ErrorHandling.feature
**Test Count**: 11 scenarios  
**Focus**: System stability and security  
**Business Value**: Protects system from invalid data and attacks

**Key Scenarios**:
- Missing or invalid data handling
- Security testing (XSS, SQL injection, command injection)
- Invalid date formats
- Large payload handling

---

### 6. Performance.feature
**Test Count**: 12 scenarios  
**Focus**: System responsiveness  
**Business Value**: Ensures good user experience under load

**Key Scenarios**:
- Response time validation (< 3 seconds)
- Concurrent request handling
- Average response time measurement
- Complete workflow performance

---

### 7. Idempotency.feature
**Test Count**: 7 scenarios  
**Focus**: Consistent behavior  
**Business Value**: Prevents duplicate actions from causing problems

**Key Scenarios**:
- Consistent GET responses
- Idempotent PUT operations
- Graceful duplicate DELETE handling
- Concurrent request consistency

---

### 8. BoundaryTesting.feature
**Test Count**: 25 scenarios  
**Focus**: Edge cases and limits  
**Business Value**: Ensures system handles extreme values correctly

**Key Scenarios**:
- Minimum/maximum ID values
- Very long names and text
- Invalid dates and formats
- Zero and negative values
- Malformed requests

---

### 9. EndToEndScenarios.feature
**Test Count**: 6 scenarios  
**Focus**: Complete workflows  
**Business Value**: Validates real-world usage patterns

**Key Scenarios**:
- Complete booking lifecycle
- Multiple booking management
- Guest check-in process
- Booking modification workflow
- Search and retrieval workflow

---

## How to Read These Files

### Example Scenario Breakdown:

```gherkin
Scenario: Create a new booking successfully
  Given I have valid guest information
  And I have valid check-in and check-out dates
  When I create a new booking
  Then the booking should be created successfully
  And I should receive a unique booking ID
  And the booking details should match my request
```

**Reading this**:
- **Given** = Starting conditions (what we have)
- **When** = Action being performed (what we do)
- **Then** = Expected results (what should happen)

### Scenario Outline Example:

```gherkin
Scenario Outline: Validate price boundaries
  Given I have a booking with price of <price>
  When I create the booking
  Then the booking should be created successfully

  Examples:
    | price   |
    | 0       |
    | 1       |
    | 999999  |
```

This scenario runs multiple times with different values from the Examples table.

## Total Test Coverage

| Feature File | Scenarios | Business Area |
|--------------|-----------|---------------|
| BookingCrud | 10 | Core Operations |
| Authentication | 7 | Security |
| DataValidation | 15 | Data Quality |
| SearchAndFiltering | 10 | Search |
| ErrorHandling | 11 | Stability |
| Performance | 12 | Speed |
| Idempotency | 7 | Consistency |
| BoundaryTesting | 25 | Edge Cases |
| EndToEndScenarios | 6 | Workflows |
| **TOTAL** | **103** | **Complete Coverage** |

## Benefits for Non-Technical Stakeholders

1. **Easy to Read**: Written in plain English
2. **Business Focused**: Describes what the system does, not how
3. **Clear Examples**: Concrete scenarios that are easy to understand
4. **Traceable**: Each scenario maps to actual tests
5. **Documentation**: Serves as living documentation of system behavior

## Benefits for Technical Team

1. **Shared Understanding**: Bridge between business and technical teams
2. **Test Scenarios**: Clear specification of what to test
3. **Regression Suite**: Documents expected behavior for regression testing
4. **Acceptance Criteria**: Can be used as acceptance criteria for features
5. **Automation Ready**: Can be automated using tools like SpecFlow (for .NET)

## Next Steps

While these feature files serve as excellent documentation and specification, you can:

1. **Use as is**: Reference documentation for understanding test coverage
2. **Implement SpecFlow**: Convert to automated BDD tests (requires SpecFlow NuGet package)
3. **Share with Stakeholders**: Use in meetings to discuss system behavior
4. **Acceptance Criteria**: Use scenarios when defining new features

## Related Files

The actual test implementations are in:
- `Tests/BookingCrudTests.cs`
- `Tests/AuthenticationTests.cs`
- `Tests/DataValidationTests.cs`
- `Tests/FilteringTests.cs`
- `Tests/ErrorHandlingTests.cs`
- `Tests/PerformanceTests.cs`
- `Tests/IdempotencyTests.cs`
- `Tests/BoundaryAndEdgeCaseTests.cs`
- `Scenarios/EndToEndScenarioTests.cs`

Each feature file scenario corresponds to one or more actual test methods in these files.
