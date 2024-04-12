using NUnit.Framework;
using System;
using System.Reflection;
using term_project.Models.CRMModels;
using Postgrest.Attributes;

namespace term_project.Tests.Models.CRMModels
{
    [TestFixture]
    public class ApplicantTests
    {
        private Applicant _applicant;

        [SetUp]
        public void SetUp()
        {
            _applicant = new Applicant();
        }

        [Test]
        public void Applicant_ShouldBeDecoratedWithTableAttribute()
        {
            var attribute = Attribute.GetCustomAttribute(typeof(Applicant), typeof(TableAttribute)) as TableAttribute;

            Assert.That(attribute, Is.Not.Null, "Table attribute should not be null.");
            Assert.That(attribute.Name, Is.EqualTo("APPLICANT"), "Table name should be 'APPLICANT'.");
        }

        [Test]
        public void ApplicantId_ShouldBeMarkedAsPrimaryKey()
        {
            var property = typeof(Applicant).GetProperty(nameof(Applicant.ApplicantId));
            var attribute = Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute;

            Assert.That(attribute, Is.Not.Null, "PrimaryKey attribute should not be null.");
            Assert.That(attribute.ColumnName, Is.EqualTo("applicant_id"), "Primary key column name should be 'applicant_id'.");
        }



        [Test]
        public void FirstName_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(Applicant).GetProperty(nameof(Applicant.FirstName));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }

        [Test]
        public void LastName_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(Applicant).GetProperty(nameof(Applicant.LastName));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }
    }
}
