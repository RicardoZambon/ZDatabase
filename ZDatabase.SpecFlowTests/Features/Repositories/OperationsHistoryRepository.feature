Feature: Operations History Repository
  OperationsHistoryRepository should manage operation history records correctly.

  Scenario: AddOperationHistoryAsync adds a new operation history
    Given a new OperationsHistoryFake
    When AddOperationHistoryAsync is called
    Then the operation history should be added to the database

  Scenario: ListOperations returns empty for invalid service history ID
    Given a new ServicesHistoryFake and a new AuditableFake
    When ListOperations is called with an invalid service history ID
    Then the result should be empty

  Scenario: ListOperations returns correct operation history
    Given a new ServicesHistoryFake and a new AuditableFake
    When ListOperations is called with the service history ID
    Then the result should contain one operation history with correct entity and service history IDs and entity name