Feature: ZDbContext
  Scenarios for auditing, service history, and operations history using Fake entities.

Scenario: Create proxy for ClientsFake
	When I create a proxy for ClientsFake
	Then the proxy should be created successfully

####################################################################################################
## Non-auditable entity
####################################################################################################

Scenario: Non-auditable entity - INSERT
	Given a new EntityFake is added to the context with properties
		| Property | Value |
		| Display  | Test  |
	When I call SaveChanges
	Then I read the EntityFake with ID 1 should have the properties
		| Property  | Value |
		| Display   | Test  |
		| IsDeleted | false |

Scenario: Non-auditable entity - UPDATE
	Given a new EntityFake is added to the context with properties
		| Property | Value |
		| Display  | Test  |
	And I call SaveChanges
	When I read the EntityFake with ID 1 and update with properties
		| Property | Value        |
		| Display  | Updated Test |
	And I call SaveChanges
	Then I read the EntityFake with ID 1 should have the properties
		| Property  | Value        |
		| Display   | Updated Test |
		| IsDeleted | false        |

Scenario: Non-uditable entity - DELETE
	Given a new EntityFake is added to the context with properties
		| Property | Value |
		| Display  | Test  |
	And I call SaveChanges
	When I delete the EntityFake with ID 1
	And I call SaveChanges
	Then I read the EntityFake with ID 1 should have the properties
		| Property  | Value |
		| Display   | Test  |
		| IsDeleted | true  |

####################################################################################################
## Auditable entity (Single entity)
####################################################################################################

Scenario: Auditable entity (Single entity) - Fails without Service History
	Given a new ClientsFake is added to the context with properties
		| Property | Value |
		| Name     | John  |
	When I call SaveChanges
	Then a MissingServiceHistoryException should be thrown

Scenario: Auditable entity (Single entity) - INSERT
	Given I save the current time as <insert datetime>
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value      |
		| Name     | AddService |
	And a new ClientsFake is added to the context with properties
		| Property | Value |
		| Name     | John  |
	When I call SaveChanges
	Then I read the ClientsFake with ID 1 should have the properties
		| Property        | Value             |
		| CreatedByID     | <current user>    |
		| CreatedOn       | <insert datetime> |
		| IsDeleted       | false             |
		| LastChangedByID | <current user>    |
		| LastChangedOn   | <insert datetime> |
		| Name            | John              |
	And I read the ServicesHistoryFake with ID 1 should have the properties
		| Property    | Value             |
		| ChangedByID | <current user>    |
		| ChangedOn   | <insert datetime> |
		| Name        | AddService        |
	And I read the OperationsHistoryFake with ID 1 should have the properties
		| Property         | Value       |
		| EntityID         | 1           |
		| EntityName       | ClientsFake |
		| OperationType    | Added       |
		| ServiceHistoryID | 1           |
		| TableName        | ClientsFake |

Scenario: Auditable entity (Single entity) - UPDATE
	Given I save the current time as <insert datetime>
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value      |
		| Name     | AddService |
	And a new ClientsFake is added to the context with properties
		| Property | Value |
		| Name     | John  |
	And I call SaveChanges
	When I save the current time as <update datetime>
	And I read the ClientsFake with ID 1 and update with properties
		| Property | Value    |
		| Name     | John Doe |
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value         |
		| Name     | UpdateService |
	And I call SaveChanges
	Then I read the ClientsFake with ID 1 should have the properties
		| Property        | Value             |
		| CreatedByID     | <current user>    |
		| CreatedOn       | <insert datetime> |
		| IsDeleted       | false             |
		| LastChangedByID | <current user>    |
		| LastChangedOn   | <update datetime> |
		| Name            | John Doe          |
	And I read the ServicesHistoryFake with ID 2 should have the properties
		| Property    | Value             |
		| ChangedByID | <current user>    |
		| ChangedOn   | <update datetime> |
		| Name        | UpdateService     |
	And I read the OperationsHistoryFake with ID 2 should have the properties
		| Property         | Value       |
		| EntityID         | 1           |
		| EntityName       | ClientsFake |
		| OperationType    | Modified    |
		| ServiceHistoryID | 2           |
		| TableName        | ClientsFake |

Scenario: Auditable entity (Single entity) - DELETE
	Given I save the current time as <insert datetime>
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value      |
		| Name     | AddService |
	And a new ClientsFake is added to the context with properties
		| Property | Value |
		| Name     | John  |
	And I call SaveChanges
	When I save the current time as <delete datetime>
	And I delete the ClientsFake with ID 1
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value         |
		| Name     | DeleteService |
	And I call SaveChanges
	Then I read the ClientsFake with ID 1 should have the properties
		| Property        | Value             |
		| CreatedByID     | <current user>    |
		| CreatedOn       | <insert datetime> |
		| IsDeleted       | true              |
		| LastChangedByID | <current user>    |
		| LastChangedOn   | <delete datetime> |
		| Name            | John              |
	And I read the ServicesHistoryFake with ID 2 should have the properties
		| Property    | Value             |
		| ChangedByID | <current user>    |
		| ChangedOn   | <delete datetime> |
		| Name        | DeleteService     |
	And I read the OperationsHistoryFake with ID 2 should have the properties
		| Property         | Value       |
		| EntityID         | 1           |
		| EntityName       | ClientsFake |
		| OperationType    | Deleted     |
		| ServiceHistoryID | 2           |
		| TableName        | ClientsFake |

####################################################################################################
## Auditable entity (Relation entity)
####################################################################################################

Scenario: Auditable entity (Relation entity) - INSERT
	Given a new ServicesHistoryFake is added to the context with properties
		| Property | Value      |
		| Name     | AddService |
	And a new ClientsFake is added to the context with properties
		| Property | Value |
		| Name     | John  |
	And I call SaveChanges
	When I save the current time as <insert datetime>
	And a new PurchasesFake is added to the context with properties
		| Property     | Value      |
		| ClientID     | 1          |
		| PurchaseDate | 2025-01-01 |
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value              |
		| Name     | AddRelationService |
	And I call SaveChanges
	Then I read the PurchasesFake with ID 1 should have the properties
		| Property        | Value             |
		| ClientID        | 1                 |
		| CreatedByID     | <current user>    |
		| CreatedOn       | <insert datetime> |
		| IsDeleted       | false             |
		| LastChangedByID | <current user>    |
		| LastChangedOn   | <insert datetime> |
		| PurchaseDate    | 2025-01-01        |
	And I read the ServicesHistoryFake with ID 2 should have the properties
		| Property    | Value              |
		| ChangedByID | <current user>     |
		| ChangedOn   | <insert datetime>  |
		| Name        | AddRelationService |
	And I read the OperationsHistoryFake with ID 2 should have the properties
		| Property         | Value         |
		| EntityID         | 1             |
		| EntityName       | PurchasesFake |
		| OperationType    | Added         |
		| ServiceHistoryID | 2             |
		| TableName        | PurchasesFake |
	And I read the OperationsHistoryFake with ID 3 should have the properties
		| Property         | Value       |
		| EntityID         | 1           |
		| EntityName       | ClientsFake |
		| OperationType    | Unchanged   |
		| ServiceHistoryID | 2           |
		| TableName        | ClientsFake |

Scenario: Auditable entity (Relation entity) - UPDATE
	Given I save the current time as <insert datetime>
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value      |
		| Name     | AddService |
	And a new ClientsFake is added to the context with properties
		| Property | Value |
		| Name     | John  |
	And a new PurchasesFake is added to the context with properties
		| Property     | Value      |
		| ClientID     | 1          |
		| PurchaseDate | 2025-01-01 |
	And I call SaveChanges
	When I save the current time as <update datetime>
	And I read the PurchasesFake with ID 1 and update with properties
		| Property     | Value      |
		| PurchaseDate | 2025-02-01 |
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value                 |
		| Name     | UpdateRelationService |
	And I call SaveChanges
	Then I read the PurchasesFake with ID 1 should have the properties
		| Property        | Value             |
		| ClientID        | 1                 |
		| CreatedByID     | <current user>    |
		| CreatedOn       | <insert datetime> |
		| IsDeleted       | false             |
		| LastChangedByID | <current user>    |
		| LastChangedOn   | <update datetime> |
		| PurchaseDate    | 2025-02-01        |
	And I read the ServicesHistoryFake with ID 2 should have the properties
		| Property    | Value                 |
		| ChangedByID | <current user>        |
		| ChangedOn   | <update datetime>     |
		| Name        | UpdateRelationService |
	And I read the OperationsHistoryFake with ID 3 should have the properties
		| Property         | Value         |
		| EntityID         | 1             |
		| EntityName       | PurchasesFake |
		| OperationType    | Modified      |
		| ServiceHistoryID | 2             |
		| TableName        | PurchasesFake |
	And I read the OperationsHistoryFake with ID 4 should have the properties
		| Property         | Value       |
		| EntityID         | 1           |
		| EntityName       | ClientsFake |
		| OperationType    | Unchanged   |
		| ServiceHistoryID | 2           |
		| TableName        | ClientsFake |

Scenario: Auditable entity (Relation entity) - DELETE
	Given I save the current time as <insert datetime>
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value      |
		| Name     | AddService |
	And a new ClientsFake is added to the context with properties
		| Property | Value |
		| Name     | John  |
	And a new PurchasesFake is added to the context with properties
		| Property     | Value      |
		| ClientID     | 1          |
		| PurchaseDate | 2025-01-01 |
	And I call SaveChanges
	When I save the current time as <delete datetime>
	And I delete the PurchasesFake with ID 1
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value                 |
		| Name     | DeleteRelationService |
	And I call SaveChanges
	Then I read the PurchasesFake with ID 1 should have the properties
		| Property        | Value             |
		| ClientID        | 1                 |
		| CreatedByID     | <current user>    |
		| CreatedOn       | <insert datetime> |
		| IsDeleted       | true              |
		| LastChangedByID | <current user>    |
		| LastChangedOn   | <delete datetime> |
		| PurchaseDate    | 2025-01-01        |
	And I read the ServicesHistoryFake with ID 2 should have the properties
		| Property    | Value                 |
		| ChangedByID | <current user>        |
		| ChangedOn   | <delete datetime>     |
		| Name        | DeleteRelationService |
	And I read the OperationsHistoryFake with ID 3 should have the properties
		| Property         | Value         |
		| EntityID         | 1             |
		| EntityName       | PurchasesFake |
		| OperationType    | Deleted       |
		| ServiceHistoryID | 2             |
		| TableName        | PurchasesFake |
	And I read the OperationsHistoryFake with ID 4 should have the properties
		| Property         | Value       |
		| EntityID         | 1           |
		| EntityName       | ClientsFake |
		| OperationType    | Unchanged   |
		| ServiceHistoryID | 2           |
		| TableName        | ClientsFake |

####################################################################################################
## Auditable entity (Child entity)
####################################################################################################

Scenario: Auditable entity (Child entity) - INSERT
	Given a new ServicesHistoryFake is added to the context with properties
		| Property | Value      |
		| Name     | AddService |
	And a new ClientsFake is added to the context with properties
		| Property | Value |
		| Name     | John  |
	And a new PurchasesFake is added to the context with properties
		| Property     | Value      |
		| ClientID     | 1          |
		| PurchaseDate | 2025-01-01 |
	And I call SaveChanges
	When I save the current time as <insert datetime>
	And a new PurchasesItemsFake is added to the context with properties
		| Property   | Value |
		| ProductID  | 1     |
		| PurchaseID | 1     |
		| Quantity   | 5     |
		| UnitPrice  | 10    |
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value           |
		| Name     | AddChildService |
	And I call SaveChanges
	Then I read the PurchasesItemsFake with ID 1 should have the properties
		| Property        | Value             |
		| CreatedByID     | <current user>    |
		| CreatedOn       | <insert datetime> |
		| IsDeleted       | false             |
		| LastChangedByID | <current user>    |
		| LastChangedOn   | <insert datetime> |
		| ProductID       | 1                 |
		| PurchaseID      | 1                 |
		| Quantity        | 5                 |
		| UnitPrice       | 10                |
	And I read the ServicesHistoryFake with ID 2 should have the properties
		| Property    | Value             |
		| ChangedByID | <current user>    |
		| ChangedOn   | <insert datetime> |
		| Name        | AddChildService   |
	And I read the OperationsHistoryFake with ID 3 should have the properties
		| Property         | Value              |
		| EntityID         | 1                  |
		| EntityName       | PurchasesItemsFake |
		| OperationType    | Added              |
		| ServiceHistoryID | 2                  |
		| TableName        | PurchasesItemsFake |
	And I read the OperationsHistoryFake with ID 4 should have the properties
		| Property         | Value         |
		| EntityID         | 1             |
		| EntityName       | PurchasesFake |
		| OperationType    | Unchanged     |
		| ServiceHistoryID | 2             |
		| TableName        | PurchasesFake |
	And I read the OperationsHistoryFake with ID 5 should have the properties
		| Property         | Value       |
		| EntityID         | 1           |
		| EntityName       | ClientsFake |
		| OperationType    | Unchanged   |
		| ServiceHistoryID | 2           |
		| TableName        | ClientsFake |

Scenario: Auditable entity (Child entity) - UPDATE
	Given I save the current time as <insert datetime>
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value      |
		| Name     | AddService |
	And a new ClientsFake is added to the context with properties
		| Property | Value |
		| Name     | John  |
	And a new PurchasesFake is added to the context with properties
		| Property     | Value      |
		| ClientID     | 1          |
		| PurchaseDate | 2025-01-01 |
	And a new PurchasesItemsFake is added to the context with properties
		| Property   | Value |
		| ProductID  | 1     |
		| PurchaseID | 1     |
		| Quantity   | 5     |
		| UnitPrice  | 10    |
	And I call SaveChanges
	When I save the current time as <update datetime>
	And I read the PurchasesItemsFake with ID 1 and update with properties
		| Property  | Value |
		| ProductID | 2     |
		| Quantity  | 10    |
		| UnitPrice | 50    |
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value              |
		| Name     | UpdateChildService |
	And I call SaveChanges
	Then I read the PurchasesItemsFake with ID 1 should have the properties
		| Property        | Value             |
		| CreatedByID     | <current user>    |
		| CreatedOn       | <insert datetime> |
		| IsDeleted       | false             |
		| LastChangedByID | <current user>    |
		| LastChangedOn   | <update datetime> |
		| ProductID       | 2                 |
		| PurchaseID      | 1                 |
		| Quantity        | 10                |
		| UnitPrice       | 50                |
	And I read the ServicesHistoryFake with ID 2 should have the properties
		| Property    | Value              |
		| ChangedByID | <current user>     |
		| ChangedOn   | <update datetime>  |
		| Name        | UpdateChildService |
	And I read the OperationsHistoryFake with ID 4 should have the properties
		| Property         | Value              |
		| EntityID         | 1                  |
		| EntityName       | PurchasesItemsFake |
		| OperationType    | Modified           |
		| ServiceHistoryID | 2                  |
		| TableName        | PurchasesItemsFake |
	And I read the OperationsHistoryFake with ID 5 should have the properties
		| Property         | Value         |
		| EntityID         | 1             |
		| EntityName       | PurchasesFake |
		| OperationType    | Unchanged     |
		| ServiceHistoryID | 2             |
		| TableName        | PurchasesFake |
	And I read the OperationsHistoryFake with ID 6 should have the properties
		| Property         | Value       |
		| EntityID         | 1           |
		| EntityName       | ClientsFake |
		| OperationType    | Unchanged   |
		| ServiceHistoryID | 2           |
		| TableName        | ClientsFake |

Scenario: Auditable entity (Child entity) - DELETE
	Given I save the current time as <insert datetime>
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value      |
		| Name     | AddService |
	And a new ClientsFake is added to the context with properties
		| Property | Value |
		| Name     | John  |
	And a new PurchasesFake is added to the context with properties
		| Property     | Value      |
		| ClientID     | 1          |
		| PurchaseDate | 2025-01-01 |
	And a new PurchasesItemsFake is added to the context with properties
		| Property   | Value |
		| ProductID  | 1     |
		| PurchaseID | 1     |
		| Quantity   | 5     |
		| UnitPrice  | 10    |
	And I call SaveChanges
	When I save the current time as <delete datetime>
	And I delete the PurchasesItemsFake with ID 1
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value              |
		| Name     | DeleteChildService |
	And I call SaveChanges
	Then I read the PurchasesItemsFake with ID 1 should have the properties
		| Property        | Value             |
		| CreatedByID     | <current user>    |
		| CreatedOn       | <insert datetime> |
		| IsDeleted       | true              |
		| LastChangedByID | <current user>    |
		| LastChangedOn   | <insert datetime> |
		| ProductID       | 1                 |
		| PurchaseID      | 1                 |
		| Quantity        | 5                 |
		| UnitPrice       | 10                |
	And I read the ServicesHistoryFake with ID 2 should have the properties
		| Property    | Value              |
		| ChangedByID | <current user>     |
		| ChangedOn   | <delete datetime>  |
		| Name        | DeleteChildService |
	And I read the OperationsHistoryFake with ID 4 should have the properties
		| Property         | Value              |
		| EntityID         | 1                  |
		| EntityName       | PurchasesItemsFake |
		| OperationType    | Deleted            |
		| ServiceHistoryID | 2                  |
		| TableName        | PurchasesItemsFake |
	And I read the OperationsHistoryFake with ID 5 should have the properties
		| Property         | Value         |
		| EntityID         | 1             |
		| EntityName       | PurchasesFake |
		| OperationType    | Unchanged     |
		| ServiceHistoryID | 2             |
		| TableName        | PurchasesFake |
	And I read the OperationsHistoryFake with ID 6 should have the properties
		| Property         | Value       |
		| EntityID         | 1           |
		| EntityName       | ClientsFake |
		| OperationType    | Unchanged   |
		| ServiceHistoryID | 2           |
		| TableName        | ClientsFake |

####################################################################################################
## Auditable entity (Many-to-many entity)
####################################################################################################

Scenario: Auditable entity (Many-to-many entity) - INSERT
	Given a new ServicesHistoryFake is added to the context with properties
		| Property | Value      |
		| Name     | AddService |
	And a new ClientsFake is added to the context with properties
		| Property | Value |
		| Name     | John  |
	And a new PurchasesFake is added to the context with properties
		| Property     | Value      |
		| ClientID     | 1          |
		| PurchaseDate | 2025-01-01 |
	And I call SaveChanges
	When I save the current time as <insert datetime>
	And a new AssessmentsFake is added to the context with properties
		| Property  | Value | ForeignKey    | ForeignKey Property |
		| Details   | Test  |               |                     |
		| Purchases | 1     | PurchasesFake | ID                  |
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value                |
		| Name     | AddManyToManyService |
	And I call SaveChanges
	Then I read the AssessmentsFake with ID 1 should have the properties
		| Property        | Value             |
		| CreatedByID     | <current user>    |
		| CreatedOn       | <insert datetime> |
		| IsDeleted       | false             |
		| LastChangedByID | <current user>    |
		| LastChangedOn   | <insert datetime> |
		| Details         | Test              |
	And I read the ServicesHistoryFake with ID 2 should have the properties
		| Property    | Value                |
		| ChangedByID | <current user>       |
		| ChangedOn   | <insert datetime>    |
		| Name        | AddManyToManyService |
	And I read the OperationsHistoryFake with ID 3 should have the properties
		| Property         | Value           |
		| EntityID         | 1               |
		| EntityName       | AssessmentsFake |
		| OperationType    | Added           |
		| ServiceHistoryID | 2               |
		| TableName        | AssessmentsFake |
	And I read the OperationsHistoryFake with ID 4 should have the properties
		| Property         | Value         |
		| EntityID         | 1             |
		| EntityName       | PurchasesFake |
		| OperationType    | Unchanged     |
		| ServiceHistoryID | 2             |
		| TableName        | PurchasesFake |
	And I read the OperationsHistoryFake with ID 5 should have the properties
		| Property         | Value       |
		| EntityID         | 1           |
		| EntityName       | ClientsFake |
		| OperationType    | Unchanged   |
		| ServiceHistoryID | 2           |
		| TableName        | ClientsFake |

Scenario: Auditable entity (Many-to-many entity) - UPDATE
	Given a new ServicesHistoryFake is added to the context with properties
		| Property | Value      |
		| Name     | AddService |
	And a new ClientsFake is added to the context with properties
		| Property | Value |
		| Name     | John  |
	And a new PurchasesFake is added to the context with properties
		| Property     | Value      |
		| ClientID     | 1          |
		| PurchaseDate | 2025-01-01 |
	And I call SaveChanges
	And I save the current time as <insert datetime>
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value         |
		| Name     | Add2ndService |
	And a new AssessmentsFake is added to the context with properties
		| Property  | Value | ForeignKey    | ForeignKey Property |
		| Details   | Test  |               |                     |
		| Purchases | 1     | PurchasesFake | ID                  |
	And I call SaveChanges
	When I save the current time as <update datetime>
	And I read the AssessmentsFake with ID 1 and update with properties
		| Property | Value        |
		| Details  | Another test |
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value                   |
		| Name     | UpdateManyToManyService |
	And I call SaveChanges
	Then I read the AssessmentsFake with ID 1 should have the properties
		| Property        | Value             |
		| CreatedByID     | <current user>    |
		| CreatedOn       | <insert datetime> |
		| IsDeleted       | false             |
		| LastChangedByID | <current user>    |
		| LastChangedOn   | <update datetime> |
		| Details         | Another test      |
	And I read the ServicesHistoryFake with ID 3 should have the properties
		| Property    | Value                   |
		| ChangedByID | <current user>          |
		| ChangedOn   | <update datetime>       |
		| Name        | UpdateManyToManyService |
	And I read the OperationsHistoryFake with ID 7 should have the properties
		| Property         | Value           |
		| EntityID         | 1               |
		| EntityName       | AssessmentsFake |
		| OperationType    | Modified        |
		| ServiceHistoryID | 3               |
		| TableName        | AssessmentsFake |
	And I read the OperationsHistoryFake with ID 8 should have the properties
		| Property         | Value         |
		| EntityID         | 1             |
		| EntityName       | PurchasesFake |
		| OperationType    | Unchanged     |
		| ServiceHistoryID | 3             |
		| TableName        | PurchasesFake |
	And I read the OperationsHistoryFake with ID 9 should have the properties
		| Property         | Value       |
		| EntityID         | 1           |
		| EntityName       | ClientsFake |
		| OperationType    | Unchanged   |
		| ServiceHistoryID | 3           |
		| TableName        | ClientsFake |

Scenario: Auditable entity (Many-to-many entity) - DELETE
	Given a new ServicesHistoryFake is added to the context with properties
		| Property | Value      |
		| Name     | AddService |
	And a new ClientsFake is added to the context with properties
		| Property | Value |
		| Name     | John  |
	And a new PurchasesFake is added to the context with properties
		| Property     | Value      |
		| ClientID     | 1          |
		| PurchaseDate | 2025-01-01 |
	And I call SaveChanges
	And I save the current time as <insert datetime>
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value         |
		| Name     | Add2ndService |
	And a new AssessmentsFake is added to the context with properties
		| Property  | Value | ForeignKey    | ForeignKey Property |
		| Details   | Test  |               |                     |
		| Purchases | 1     | PurchasesFake | ID                  |
	And I call SaveChanges
	When I save the current time as <delete datetime>
	And I delete the AssessmentsFake with ID 1
	And a new ServicesHistoryFake is added to the context with properties
		| Property | Value                   |
		| Name     | DeleteManyToManyService |
	And I call SaveChanges
	Then I read the AssessmentsFake with ID 1 should have the properties
		| Property        | Value             |
		| CreatedByID     | <current user>    |
		| CreatedOn       | <insert datetime> |
		| IsDeleted       | true              |
		| LastChangedByID | <current user>    |
		| LastChangedOn   | <delete datetime> |
		| Details         | Test              |
	And I read the ServicesHistoryFake with ID 3 should have the properties
		| Property    | Value                   |
		| ChangedByID | <current user>          |
		| ChangedOn   | <delete datetime>       |
		| Name        | DeleteManyToManyService |
	And I read the OperationsHistoryFake with ID 7 should have the properties
		| Property         | Value           |
		| EntityID         | 1               |
		| EntityName       | AssessmentsFake |
		| OperationType    | Deleted         |
		| ServiceHistoryID | 3               |
		| TableName        | AssessmentsFake |
	And I read the OperationsHistoryFake with ID 8 should have the properties
		| Property         | Value         |
		| EntityID         | 1             |
		| EntityName       | PurchasesFake |
		| OperationType    | Unchanged     |
		| ServiceHistoryID | 3             |
		| TableName        | PurchasesFake |
	And I read the OperationsHistoryFake with ID 9 should have the properties
		| Property         | Value       |
		| EntityID         | 1           |
		| EntityName       | ClientsFake |
		| OperationType    | Unchanged   |
		| ServiceHistoryID | 3           |
		| TableName        | ClientsFake |