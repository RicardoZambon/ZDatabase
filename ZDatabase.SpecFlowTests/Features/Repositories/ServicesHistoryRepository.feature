Feature: Services History Repository
  ServicesHistoryRepository should manage service history records correctly.

  Scenario: AddServiceHistoryAsync adds a new service history
    Given a new ServicesHistoryFake
    When AddServiceHistoryAsync is called
    Then the service history should be added to the database

  Scenario: ListServicesAsync throws exception for invalid entity ID
    Given a new ServicesHistoryFake and a new AuditableFake
    When ListServicesAsync is called with an invalid entity ID
    Then an EntityNotFoundException should be thrown

  Scenario: ListServicesAsync returns correct service history
    Given two service histories and two auditable entities
    When ListServicesAsync is called for the first auditable entity
    Then only the first service history should be returned