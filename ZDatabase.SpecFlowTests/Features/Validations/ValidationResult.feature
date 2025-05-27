Feature: Validation Result
  ValidationResult should handle errors and validation correctly.

  Scenario: SetError adds error to new key
    Given a new ValidationResult
    When SetError is called with a new key and message
    Then the error should be added under the key

  Scenario: SetError adds error to existing key
    Given a ValidationResult with an existing key
    When SetError is called with the same key and a new message
    Then the error list for the key should contain both messages

  Scenario: ValidateEntityErrors throws exception when there are errors
    Given a ValidationResult with errors and an entity
    When ValidateEntityErrors is called
    Then an EntityValidationFailureException should be thrown

  Scenario: ValidateEntityErrors does not throw when there are no errors
    Given a ValidationResult with no errors and an entity
    When ValidateEntityErrors is called
    Then no exception should be thrown

  Scenario: ValidateEntityErrors (generic) throws exception when there are errors
    Given a ValidationResult with errors and an entity
    When ValidateEntityErrors is called with entity key
    Then an EntityValidationFailureException should be thrown

  Scenario: ValidateEntityErrors (generic) does not throw when there are no errors
    Given a ValidationResult with no errors and an entity
    When ValidateEntityErrors is called with entity key
    Then no exception should be thrown

  Scenario: ValidateErrors throws exception when there are errors
    Given a ValidationResult with errors
    When ValidateErrors is called
    Then a ValidationFailureException should be thrown

  Scenario: ValidateErrors does not throw when there are no errors
    Given a ValidationResult with no errors
    When ValidateErrors is called
    Then no exception should be thrown