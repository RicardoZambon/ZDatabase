Feature: DateTimeTimeSpanGenerator
  DateTimeTimeSpanGenerator should generate the current TimeSpan correctly.

  Scenario: GeneratesTemporaryValues returns false
    Given a new DateTimeTimeSpanGenerator
    Then GeneratesTemporaryValues should be false

  Scenario: Next returns current TimeSpan
    Given a new DateTimeTimeSpanGenerator and an EntityEntry
    When Next is called
    Then the result should be the current TimeSpan
