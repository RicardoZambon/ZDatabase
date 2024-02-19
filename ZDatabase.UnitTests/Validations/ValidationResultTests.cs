using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDatabase.Interfaces;
using ZDatabase.Repositories.Audit.Interfaces;
using ZDatabase.Repositories.Audit;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes.EntitiesFake;
using ZDatabase.Validations;
using ZDatabase.Entities.Audit;
using ZDatabase.Exceptions;
using ZDatabase.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using FluentAssertions.Specialized;

namespace ZDatabase.UnitTests.Validations
{
    /// <summary>
    /// Unit tests for <see cref="ZDatabase.Validations.ValidationResult"/>.
    /// </summary>
    public class ValidationResultTests
    {
        /// <summary>
        /// Test the SetError should add a new message to a new key in the error list.
        /// </summary>
        [Fact]
        public void SetError_Pass_AddsErrorNewKey()
        {
            // Arrange
            string key = "Key";
            string message = "Message";

            ValidationResult validationResult = new();

            // Act
            validationResult.SetError(key, message);

            // Assert
            validationResult.Errors.Should().ContainKey(key);
            validationResult.Errors[key].Should().HaveCount(1);
            validationResult.Errors[key].Should().Contain(message);
        }

        /// <summary>
        /// Test the SetError should add a new message to an existing key in the error list.
        /// </summary>
        [Fact]
        public void SetError_Pass_AddsErrorExistingKey()
        {
            // Arrange
            string existingMessage = "Message";
            string key = "Key";
            string message = "Message";

            ValidationResult validationResult = new();
            validationResult.SetError(key, existingMessage);

            // Act
            validationResult.SetError(key, message);

            // Assert
            validationResult.Errors.Should().ContainKey(key);
            validationResult.Errors[key].Should().HaveCount(2);
            validationResult.Errors[key].Should().Contain(message);
        }

        /// <summary>
        /// Test the ValidateEntityErrors should throw exception for entities when has errors.
        /// </summary>
        [Fact]
        public void ValidateEntityErrors_Fail_ThrowExceptionEntitiesWhenHasErrors()
        {
            // Arrange
            string key = "Key";
            string message = "Message";

            EntityFake entity = new() { ID = 1 };

            ValidationResult validationResult = new();
            validationResult.SetError(key, message);
            
            // Act
            Action act = () =>
            {
                validationResult.ValidateEntityErrors(entity);
            };

            // Assert
            ExceptionAssertions<EntityValidationFailureException<long>> exception = act.Should().Throw<EntityValidationFailureException<long>>()
                 .WithMessage($"Entity '{nameof(EntityFake)} ({entity.ID})' has validation problems.");

            exception.Which.EntityKey.Should().Be(entity.ID);
            exception.Which.ValidationResult.Should().Be(validationResult);

            validationResult.Errors.Should().HaveCount(1);
            validationResult.HasErrors.Should().BeTrue();
        }

        /// <summary>
        /// Test the ValidateEntityErrors should not throw exception for entities without errors.
        /// </summary>
        [Fact]
        public void ValidateEntityErrors_Pass_NotThrowExceptionEntitiesWithoutErrors()
        {
            // Arrange
            EntityFake entity = new() { ID = 1 };

            ValidationResult validationResult = new();

            // Act
            Action act = () =>
            {
                validationResult.ValidateEntityErrors(entity);
            };

            // Assert
            act.Should().NotThrow();

            validationResult.Errors.Should().BeEmpty();
            validationResult.HasErrors.Should().BeFalse();
        }

        /// <summary>
        /// Test the ValidateEntityErrors should throw exception for generic entities when has errors.
        /// </summary>
        [Fact]
        public void ValidateEntityErrors_Fail_ThrowExceptionGenericEntitiesWhenHasErrors()
        {
            // Arrange
            string key = "Key";
            string message = "Message";

            EntityFake entity = new() { ID = 1 };

            ValidationResult validationResult = new();
            validationResult.SetError(key, message);

            // Act
            Action act = () =>
            {
                validationResult.ValidateEntityErrors<EntityFake, long>(entity.ID);
            };

            // Assert
            ExceptionAssertions<EntityValidationFailureException<long>> exception = act.Should().Throw<EntityValidationFailureException<long>>()
                .WithMessage($"Entity '{nameof(EntityFake)} ({entity.ID})' has validation problems.");

            exception.Which.EntityKey.Should().Be(entity.ID);
            exception.Which.ValidationResult.Should().Be(validationResult);

            validationResult.Errors.Should().HaveCount(1);
            validationResult.HasErrors.Should().BeTrue();
        }

        /// <summary>
        /// Test the ValidateEntityErrors should not throw exception for generic entities without errors.
        /// </summary>
        [Fact]
        public void ValidateEntityErrors_Pass_NotThrowExceptionGenericEntitiesWithoutErrors()
        {
            // Arrange
            EntityFake entity = new() { ID = 1 };

            ValidationResult validationResult = new();

            // Act
            Action act = () =>
            {
                validationResult.ValidateEntityErrors<EntityFake, long>(entity.ID);
            };

            // Assert
            act.Should().NotThrow();

            validationResult.Errors.Should().BeEmpty();
            validationResult.HasErrors.Should().BeFalse();
        }

        /// <summary>
        /// Test the ValidateErrors should throw exception when has errors.
        /// </summary>
        [Fact]
        public void ValidateErrors_Fail_ThrowExceptionWhenHasErrors()
        {
            // Arrange
            string key = "Key";
            string message = "Message";

            EntityFake entity = new() { ID = 1 };

            ValidationResult validationResult = new();
            validationResult.SetError(key, message);

            // Act
            Action act = () =>
            {
                validationResult.ValidateErrors();
            };

            // Assert
            act.Should().Throw<ValidationFailureException>()
                .Which.ValidationResult.Should().Be(validationResult);

            validationResult.Errors.Should().HaveCount(1);
            validationResult.HasErrors.Should().BeTrue();
        }

        /// <summary>
        /// Test the ValidateErrors should not throw exception without errors.
        /// </summary>
        [Fact]
        public void ValidateErrors_Pass_NotThrowExceptionWithoutErrors()
        {
            // Arrange
            ValidationResult validationResult = new();

            // Act
            Action act = () =>
            {
                validationResult.ValidateErrors();
            };

            // Assert
            act.Should().NotThrow();

            validationResult.Errors.Should().BeEmpty();
            validationResult.HasErrors.Should().BeFalse();
        }
    }
}