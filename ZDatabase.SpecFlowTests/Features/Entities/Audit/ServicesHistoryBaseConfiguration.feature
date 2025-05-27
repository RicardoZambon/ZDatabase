Feature: Services History Base Configuration
  ServicesHistoryBase configuration should be correct in the model.

  Scenario: ChangedOn property has after save behavior Ignore and value generator
    Given the model is built for ServicesHistoryFake
    Then the property 'ChangedOn' should have after save behavior Ignore
    And the property 'ChangedOn' should have value generated on add
    And the property 'ChangedOn' should use the 'DateTimeUtcGenerator'

  Scenario: Name property has after save behavior Ignore
    Given the model is built for ServicesHistoryFake
    Then the property 'Name' should have after save behavior Ignore