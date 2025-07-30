Feature: Operations History Base Configuration
  OperationsHistoryBase configuration should be correct in the model.

  Scenario Outline: Property has after save behavior Ignore
    Given the model is built for 'OperationsHistoryFake'
    Then the property '<Property>' should have 'after' save behavior Ignore

    Examples:
      | Property         |
      | EntityID         |
      | EntityName       |
      | NewValues        |
      | OldValues        |
      | OperationType    |
      | ServiceHistoryID |
      | TableName        |