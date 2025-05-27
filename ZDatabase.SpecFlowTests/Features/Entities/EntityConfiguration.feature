Feature: Entity Configuration
  Entity configuration should be correct in the model.

  Scenario: Entity ID is configured as key and generated on add
    Given the model is built for EntityFake
    Then the property 'ID' should be configured as key
    And the property 'ID' should have value generated on add

  Scenario: IsDeleted property has default value false
    Given the model is built for EntityFake
    Then the property 'IsDeleted' should have default value false

  Scenario: Query filter is configured for IsDeleted
    Given the model is built for EntityFake
    Then the query filter should check the 'IsDeleted' property