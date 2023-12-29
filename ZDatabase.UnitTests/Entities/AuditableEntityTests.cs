using Microsoft.EntityFrameworkCore.Metadata;
using ZDatabase.Entities;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes;

namespace ZDatabase.UnitTests.Entities
{
    public class AuditableEntityTests
    {
        [Fact]
        public void CreatedOn_Pass_HasDefaultValue()
        {
            // Arrange
            IProperty? createdOnProperty;

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            createdOnProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityBase.CreatedOn));

            createdOnProperty.Should().NotBeNull();

            createdOnProperty!.ValueGenerated.Should().Be(ValueGenerated.OnAdd);
        }


        [Fact]
        public void CreatedBy_Pass()
        {
            // Arrange
            AuditableEntity<UsersEntityFake, long> auditableEntity = new AuditableEntityFake
            {
                CreatedBy = new UsersEntityFake(),
            };

            // Act

            // Assert
            auditableEntity.CreatedBy.Should().NotBeNull();
        }

        [Fact]
        public void CreatedByID_Pass()
        {
            // Arrange
            AuditableEntity<UsersEntityFake, long> auditableEntity = new AuditableEntityFake
            {
                CreatedByID = new Random().Next(1, 999_999),
            };

            // Act

            // Assert
            auditableEntity.CreatedByID.Should().BeGreaterThan(0);
        }

        [Fact]
        public void CreatedOn_Pass()
        {
            // Arrange
            DateTime initialDateTime = DateTime.Now;

            AuditableEntity<UsersEntityFake, long> auditableEntity = new AuditableEntityFake
            {
                CreatedOn = DateTime.Now,
            };

            // Act

            // Assert
            auditableEntity.CreatedOn.Should().BeAfter(initialDateTime);
            auditableEntity.CreatedOn.Should().BeBefore(DateTime.Now);
        }

        [Fact]
        public void LastChangedBy_Pass()
        {
            // Arrange
            AuditableEntity<UsersEntityFake, long> auditableEntity = new AuditableEntityFake
            {
                LastChangedBy = new UsersEntityFake(),
            };

            // Act

            // Assert
            auditableEntity.LastChangedBy.Should().NotBeNull();
        }

        [Fact]
        public void LastChangedByID_Pass()
        {
            // Arrange
            AuditableEntity<UsersEntityFake, long> auditableEntity = new AuditableEntityFake
            {
                LastChangedByID = new Random().Next(1, 999_999),
            };

            // Act

            // Assert
            auditableEntity.LastChangedByID.Should().BeGreaterThan(0);
        }

        [Fact]
        public void LastChangedOn_Pass()
        {
            // Arrange
            DateTime initialDateTime = DateTime.Now;

            AuditableEntity<UsersEntityFake, long> auditableEntity = new AuditableEntityFake
            {
                LastChangedOn = DateTime.Now,
            };

            // Act

            // Assert
            auditableEntity.LastChangedOn.Should().BeAfter(initialDateTime);
            auditableEntity.LastChangedOn.Should().BeBefore(DateTime.Now);
        }
    }
}