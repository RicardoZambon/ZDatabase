Feature: DateTimeUtcGenerator
  DateTimeUtcGenerator should generate the current UTC DateTime correctly.

  Scenario: GeneratesTemporaryValues returns false
    Given a new DateTimeUtcGenerator
    Then GeneratesTemporaryValues should be false

  Scenario: Next returns current UTC DateTime
    Given a new DateTimeUtcGenerator and an EntityEntry
    When Next is called
    Then the result should be the current UTC DateTime
