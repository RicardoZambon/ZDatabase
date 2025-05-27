Feature: DateTimeGenerator
  DateTimeGenerator should generate the current DateTime correctly.

  Scenario: GeneratesTemporaryValues returns false
    Given a new DateTimeGenerator
    Then GeneratesTemporaryValues should be false

  Scenario: Next returns current DateTime
    Given a new DateTimeGenerator and an EntityEntry
    When Next is called
    Then the result should be the current DateTime
