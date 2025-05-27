Feature: Operations History ConfigurationServicesHistoryFake
  OperationsHistory configuration should be correct in the model.

  Scenario: ServiceHistoryID property is a foreign key to 
    Given the model is built for OperationsHistoryFake
    Then the property 'ServiceHistoryID' should be a foreign key to ServicesHistoryFake with navigation 'ServiceHistory' and delete behavior NoAction