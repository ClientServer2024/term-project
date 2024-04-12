using NUnit.Framework;
using System;
using System.Reflection;
using term_project.Models.CRMModels;
using Postgrest.Attributes;


namespace term_project.Tests.Models.CRMModels
{
    [TestFixture]
    public class ApplianceTests
    {
        private Appliance _appliance;

        [SetUp]
        public void SetUp()
        {
            _appliance = new Appliance();
        }

        [Test]
        public void Appliance_ShouldBeDecoratedWithTableAttribute()
        {
            var attribute = Attribute.GetCustomAttribute(typeof(Appliance), typeof(TableAttribute)) as TableAttribute;

            Assert.That(attribute, Is.Not.Null, "Table attribute should not be null.");
            Assert.That(attribute.Name, Is.EqualTo("APPLIANCE"), "Table name should be 'APPLIANCE'.");
        }

        [Test]
        public void ApplianceId_ShouldBeMarkedAsPrimaryKey()
        {
            var property = typeof(Appliance).GetProperty(nameof(Appliance.ApplianceId));
            var attribute = Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute;

            Assert.That(attribute, Is.Not.Null, "PrimaryKey attribute should not be null.");
            Assert.That(attribute.ColumnName, Is.EqualTo("appliance_id"), "Primary key column name should be 'appliance_id'.");
        }

        [Test]
        public void AssetId_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(Appliance).GetProperty(nameof(Appliance.AssetId));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }

        [Test]
        public void Make_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(Appliance).GetProperty(nameof(Appliance.Make));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }

        [Test]
        public void Model_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(Appliance).GetProperty(nameof(Appliance.Model));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }
    }
}
