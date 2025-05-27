Feature: DateTimeMonthGenerator
  DateTimeMonthGenerator should generate the current month correctly.

  Scenario: GeneratesTemporaryValues returns false
    Given a new DateTimeMonthGenerator
    Then GeneratesTemporaryValues should be false

  Scenario: Next returns current month
    Given a new DateTimeMonthGenerator and an EntityEntry
    When Next is called
    Then the result should be the current month
