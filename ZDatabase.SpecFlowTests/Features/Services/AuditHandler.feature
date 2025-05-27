Feature: Audit Handler
  AuditHandler should handle audit operations correctly.

  Scenario Outline: AddOperationEntitiesBeforeSaving adds operations for added entities
    Given a new ServicesHistoryFake and <Count> new AuditableFake(s)
    When AddOperationEntitiesBeforeSaving is called
    Then <Count> OperationsHistoryFake(s) should be added for state Added

    Examples:
      | Count |
      | 1     |
      | 2     |

  Scenario Outline: AddOperationEntitiesBeforeSaving adds operations for deleted entities
    Given existing ServicesHistoryFake and <Count> new AuditableFake(s)
    When the entities are deleted and AddOperationEntitiesBeforeSaving is called
    Then <Count> OperationsHistoryFake(s) should be added for state Deleted

    Examples:
      | Count |
      | 1     |
      | 2     |

  Scenario Outline: AddOperationEntitiesBeforeSaving adds operations for updated entities
    Given existing ServicesHistoryFake and <Count> new AuditableFake(s)
    When the entities are updated and AddOperationEntitiesBeforeSaving is called
    Then <Count> OperationsHistoryFake(s) should be added for state Modified

    Examples:
      | Count |
      | 1     |
      | 2     |

  Scenario Outline: AddOperationEntitiesBeforeSaving does throws MissingServiceHistoryException when there is no service history
    Given <Count> new AuditableFake(s) and no ServicesHistoryFake
    When AddOperationEntitiesBeforeSaving is called
    Then a MissingServiceHistoryException should be thrown

    Examples:
      | Count |
      | 1     |
      | 2     |

  Scenario: RefreshAuditedEntries throws DuplicatedServiceAuditHistoryException
    Given two ServicesHistoryFake and one AuditableFake
    When RefreshAuditedEntries is called
    Then a DuplicatedServiceAuditHistoryException should be thrown

  Scenario: RefreshAuditedEntries does not throw when there is a single history service
    Given one ServicesHistoryFake and one AuditableFake
    When RefreshAuditedEntries is called
    Then no exception should be thrown