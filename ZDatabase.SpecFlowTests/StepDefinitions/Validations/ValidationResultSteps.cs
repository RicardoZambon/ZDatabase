using FluentAssertions;
using Reqnroll;
using ZDatabase.Exceptions;
using ZDatabase.SpecFlowTests.Fakes.Entities;
using ZDatabase.Validations;

namespace ZDatabase.SpecFlowTests.StepDefinitions.Validations
{
    [Binding]
    [Scope(Feature = "Validation Result")]
    public class ValidationResultSteps
    {
        private ValidationResult _validationResult;
        private string _key;
        private string _message;
        private EntityFake _entity;
        private Exception _caughtException;

        [Given("a new ValidationResult")]
        public void GivenANewValidationResult()
        {
            _validationResult = new ValidationResult();
        }

        [When("SetError is called with a new key and message")]
        public void WhenSetErrorIsCalledWithANewKeyAndMessage()
        {
            _key = "Key";
            _message = "Message";
            _validationResult.SetError(_key, _message);
        }

        [Then("the error should be added under the key")]
        public void ThenTheErrorShouldBeAddedUnderTheKey()
        {
            _validationResult.Errors.Should().ContainKey(_key);
            _validationResult.Errors[_key].Should().Contain(_message);
        }

        [Given("a ValidationResult with an existing key")]
        public void GivenAValidationResultWithAnExistingKey()
        {
            _validationResult = new ValidationResult();
            _key = "Key";
            _validationResult.SetError(_key, "ExistingMessage");
        }

        [When("SetError is called with the same key and a new message")]
        public void WhenSetErrorIsCalledWithTheSameKeyAndANewMessage()
        {
            _message = "Message";
            _validationResult.SetError(_key, _message);
        }

        [Then("the error list for the key should contain both messages")]
        public void ThenTheErrorListForTheKeyShouldContainBothMessages()
        {
            _validationResult.Errors[_key].Should().Contain("ExistingMessage");
            _validationResult.Errors[_key].Should().Contain(_message);
        }

        [Given("a ValidationResult with errors and an entity")]
        public void GivenAValidationResultWithErrorsAndAnEntity()
        {
            _validationResult = new ValidationResult();
            _key = "Key";
            _message = "Message";
            _validationResult.SetError(_key, _message);
            _entity = new EntityFake { ID = 1 };
        }

        [When("ValidateEntityErrors is called")]
        public void WhenValidateEntityErrorsIsCalled()
        {
            try
            {
                _validationResult.ValidateEntityErrors(_entity);
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        [Then("an EntityValidationFailureException should be thrown")]
        public void ThenAnEntityValidationFailureExceptionShouldBeThrown()
        {
            _caughtException.Should().BeOfType<EntityValidationFailureException<long>>();
        }

        [Given("a ValidationResult with no errors and an entity")]
        public void GivenAValidationResultWithNoErrorsAndAnEntity()
        {
            _validationResult = new ValidationResult();
            _entity = new EntityFake { ID = 1 };
        }

        [Then("no exception should be thrown")]
        public void ThenNoExceptionShouldBeThrown()
        {
            _caughtException.Should().BeNull();
        }

        [When("ValidateEntityErrors is called with entity key")]
        public void WhenValidateEntityErrorsIsCalledWithEntityKey()
        {
            try
            {
                _validationResult.ValidateEntityErrors<EntityFake, long>(_entity.ID);
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        [Given("a ValidationResult with errors")]
        public void GivenAValidationResultWithErrors()
        {
            _validationResult = new ValidationResult();
            _key = "Key";
            _message = "Message";
            _validationResult.SetError(_key, _message);
        }

        [When("ValidateErrors is called")]
        public void WhenValidateErrorsIsCalled()
        {
            try
            {
                _validationResult.ValidateErrors();
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        [Then("a ValidationFailureException should be thrown")]
        public void ThenAValidationFailureExceptionShouldBeThrown()
        {
            _caughtException.Should().BeOfType<ValidationFailureException>();
        }

        [Given("a ValidationResult with no errors")]
        public void GivenAValidationResultWithNoErrors()
        {
            _validationResult = new ValidationResult();
        }
    }
}
