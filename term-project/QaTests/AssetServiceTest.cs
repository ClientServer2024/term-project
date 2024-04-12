using NUnit.Framework;
using System;
using System.Reflection;
using term_project.Models.CRMModels;
using Postgrest.Attributes;

namespace term_project.Tests.Models.CRMModels
{
    [TestFixture]
    public class AssetServiceTests
    {
        private AssetService _assetService;

        [SetUp]
        public void SetUp()
        {
            _assetService = new AssetService();
        }

        [Test]
        public void AssetService_ShouldBeDecoratedWithTableAttribute()
        {
            var attribute = Attribute.GetCustomAttribute(typeof(AssetService), typeof(TableAttribute)) as TableAttribute;

            Assert.That(attribute, Is.Not.Null, "Table attribute should not be null.");
            Assert.That(attribute.Name, Is.EqualTo("ASSET_SERVICE"), "Table name should be 'ASSET_SERVICE'.");
        }

        [Test]
        public void AssetServiceId_ShouldBeMarkedAsPrimaryKey()
        {
            var property = typeof(AssetService).GetProperty(nameof(AssetService.AssetServiceId));
            var attribute = Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute;

            Assert.That(attribute, Is.Not.Null, "PrimaryKey attribute should not be null.");
            Assert.That(attribute.ColumnName, Is.EqualTo("asset_service_id"), "Primary key column name should be 'asset_service_id'.");
        }

        [Test]
        public void Name_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(AssetService).GetProperty(nameof(AssetService.Name));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }

        [Test]
        public void Rate_ShouldBeDecoratedWithColumnAttributeAndAllowNulls()
        {
            var property = typeof(AssetService).GetProperty(nameof(AssetService.Rate));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
            Assert.That(property.PropertyType, Is.EqualTo(typeof(float?)), "Rate property should allow null values.");
        }
    }
}
