Feature: Auditable Entity Base Configuration
  AuditableEntityBase configuration should be correct in the model.

  Scenario: CreatedOn property has after save behavior Ignore and value generator
    Given the model is built for AuditableFake
    Then the property 'CreatedOn' should have after save behavior Ignore
    And the property 'CreatedOn' should have value generated on add
    And the property 'CreatedOn' should use the 'DateTimeUtcGenerator'

  Scenario: LastChangedOn property has before and after save behavior Save and value generator
    Given the model is built for AuditableFake
    Then the property 'LastChangedOn' should have after save behavior Save
    And the property 'LastChangedOn' should have before save behavior Save
    And the property 'LastChangedOn' should have value generated on add or update
    And the property 'LastChangedOn' should use the 'DateTimeUtcGenerator'