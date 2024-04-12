using NUnit.Framework;
using System;
using System.Reflection;
using term_project.Models.CRMModels;
using Postgrest.Attributes;

namespace term_project.Tests.Models.CRMModels
{
    [TestFixture]
    public class AssetTests
    {
        private Asset _asset;

        [SetUp]
        public void SetUp()
        {
            _asset = new Asset();
        }

        [Test]
        public void Asset_ShouldBeDecoratedWithTableAttribute()
        {
            var attribute = Attribute.GetCustomAttribute(typeof(Asset), typeof(TableAttribute)) as TableAttribute;

            Assert.That(attribute, Is.Not.Null, "Table attribute should not be null.");
            Assert.That(attribute.Name, Is.EqualTo("ASSET"), "Table name should be 'ASSET'.");
        }

        [Test]
        public void AssetId_ShouldBeMarkedAsPrimaryKey()
        {
            var property = typeof(Asset).GetProperty(nameof(Asset.AssetId));
            var attribute = Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute;

            Assert.That(attribute, Is.Not.Null, "PrimaryKey attribute should not be null.");
            Assert.That(attribute.ColumnName, Is.EqualTo("asset_id"), "Primary key column name should be 'asset_id'.");
        }

        [Test]
        public void Type_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(Asset).GetProperty(nameof(Asset.Type));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }

        [Test]
        public void Status_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(Asset).GetProperty(nameof(Asset.Status));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }

        [Test]
        public void ApplicationCount_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(Asset).GetProperty(nameof(Asset.ApplicationCount));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }

        [Test]
        public void Rate_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(Asset).GetProperty(nameof(Asset.Rate));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }
    }
}
