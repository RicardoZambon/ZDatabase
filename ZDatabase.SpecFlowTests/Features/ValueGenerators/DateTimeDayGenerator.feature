Feature: DateTimeDayGenerator
  DateTimeDayGenerator should generate the current day correctly.

  Scenario: GeneratesTemporaryValues returns false
    Given a new DateTimeDayGenerator
    Then GeneratesTemporaryValues should be false

  Scenario: Next returns current day
    Given a new DateTimeDayGenerator and an EntityEntry
    When Next is called
    Then the result should be the current day
