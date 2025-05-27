Feature: Services History Configuration
  ServicesHistory configuration should be correct in the model.

  Scenario: ChangedByID property has after save behavior Ignore, foreign key, and value generator
    Given the model is built for ServicesHistoryFake
    Then the property 'ChangedByID' should have after save behavior Ignore
    And the property 'ChangedByID' should be a foreign key to UsersFake with navigation 'ChangedBy' and delete behavior NoAction
    And the property 'ChangedByID' should have value generated on add
    And the property 'ChangedByID' should use the 'CurrentUserGenerator'