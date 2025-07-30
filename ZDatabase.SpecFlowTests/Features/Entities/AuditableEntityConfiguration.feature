Feature: Auditable Entity Configuration
  AuditableEntity configuration should be correct in the model.

  Scenario: CreatedByID property has after save behavior Ignore, foreign key, and value generator
    Given the model is built for 'AuditableFake'
    Then the property 'CreatedByID' should have 'after' save behavior Ignore
    And the property 'CreatedByID' should be a foreign key to 'UsersFake' with navigation 'CreatedBy' and delete behavior NoAction
    And the property 'CreatedByID' should have value generated OnAdd
    And the property 'CreatedByID' should use the generator 'CurrentUserGenerator'

  Scenario: LastChangedByID property has before and after save behavior Save, foreign key, and value generator
    Given the model is built for 'AuditableFake'
    Then the property 'LastChangedByID' should have 'after' save behavior Save
    And the property 'LastChangedByID' should have 'before' save behavior Save
    And the property 'LastChangedByID' should be a foreign key to 'UsersFake' with navigation 'LastChangedBy' and delete behavior NoAction
    And the property 'LastChangedByID' should have value generated OnAddOrUpdate
    And the property 'LastChangedByID' should use the generator 'CurrentUserGenerator'

  Scenario: Query filter is configured for IsDeleted
    Given the model is built for 'EntityFake'
    Then the query filter should check the 'IsDeleted' property