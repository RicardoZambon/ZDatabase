Feature: DateTimeYearGenerator
  DateTimeYearGenerator should generate the current year correctly.

  Scenario: GeneratesTemporaryValues returns false
    Given a new DateTimeYearGenerator
    Then GeneratesTemporaryValues should be false

  Scenario: Next returns current year
    Given a new DateTimeYearGenerator and an EntityEntry
    When Next is called
    Then the result should be the current year
