Feature: CurrentUserGenerator
  CurrentUserGenerator should generate user IDs correctly.

  Scenario: GeneratesTemporaryValues returns false
    Then GeneratesTemporaryValues should be false

  Scenario: Return default when service is not found
    Given a database context
    And a new EntityEntry
    When Next is called
    Then the result should be default

  Scenario: Next throws exception from user provider
    Given a CurrentUserService that throws an exception
    And a database context
    When Next is called
    Then an exception should be thrown

  Scenario: Next returns current user ID
    Given a CurrentUserService that returns 123
    And a database context
    And a new EntityEntry
    When Next is called
    Then the result should be the current user ID

  Scenario: Next returns default when CurrentUserID is null
	Given a CurrentUserService that returns a null
    And a database context
    And a new EntityEntry
    When Next is called
    Then the result should be default