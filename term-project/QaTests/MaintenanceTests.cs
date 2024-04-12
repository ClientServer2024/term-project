using NUnit.Framework;
using System;
using System.Reflection;
using term_project.Models.CRMModels;
using Postgrest.Attributes;

namespace term_project.Tests.Models.CRMModels
{
    [TestFixture]
    public class MaintenanceRequestTests
    {
        private MaintenanceRequest _maintenanceRequest;

        [SetUp]
        public void SetUp()
        {
            _maintenanceRequest = new MaintenanceRequest();
        }

        [Test]
        public void MaintenanceRequest_ShouldBeDecoratedWithTableAttribute()
        {
            var attribute = Attribute.GetCustomAttribute(typeof(MaintenanceRequest), typeof(TableAttribute)) as TableAttribute;

            Assert.That(attribute, Is.Not.Null, "Table attribute should not be null.");
            Assert.That(attribute.Name, Is.EqualTo("MAINTENANCE_REQUEST"), "Table name should be 'MAINTENANCE_REQUEST'.");
        }

        [Test]
        public void MaintenanceRequestId_ShouldBeMarkedAsPrimaryKey()
        {
            var property = typeof(MaintenanceRequest).GetProperty(nameof(MaintenanceRequest.MaintenanceRequestId));
            var attribute = Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute;

            Assert.That(attribute, Is.Not.Null, "PrimaryKey attribute should not be null.");
            Assert.That(attribute.ColumnName, Is.EqualTo("maintenance_request_id"), "Primary key column name should be 'maintenance_request_id'.");
        }

        [Test]
        public void AssetId_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(MaintenanceRequest).GetProperty(nameof(MaintenanceRequest.AssetId));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }

        [Test]
        public void RenterId_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(MaintenanceRequest).GetProperty(nameof(MaintenanceRequest.RenterId));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }

        [Test]
        public void Description_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(MaintenanceRequest).GetProperty(nameof(MaintenanceRequest.Description));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }

        [Test]
        public void Status_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(MaintenanceRequest).GetProperty(nameof(MaintenanceRequest.Status));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }

        [Test]
        public void DueDate_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(MaintenanceRequest).GetProperty(nameof(MaintenanceRequest.DueDate));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }
    }
}
