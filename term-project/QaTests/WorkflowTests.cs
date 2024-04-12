using NUnit.Framework;
using System;
using System.Reflection;
using term_project.Models.CRMModels;
using Postgrest.Attributes;

namespace term_project.Tests.Models.CRMModels
{
    [TestFixture]
    public class WorkflowTests
    {
        private Workflow _workflow;

        [SetUp]
        public void SetUp()
        {
            _workflow = new Workflow();
        }

        [Test]
        public void Workflow_ShouldBeDecoratedWithTableAttribute()
        {
            var attribute = Attribute.GetCustomAttribute(typeof(Workflow), typeof(TableAttribute)) as TableAttribute;

            Assert.That(attribute, Is.Not.Null, "Table attribute should not be null.");
            Assert.That(attribute.Name, Is.EqualTo("WORKFLOW"), "Table name should be 'WORKFLOW'.");
        }

        [Test]
        public void WorkflowId_ShouldBeMarkedAsPrimaryKey()
        {
            var property = typeof(Workflow).GetProperty(nameof(Workflow.WorkflowId));
            var attribute = Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute;

            Assert.That(attribute, Is.Not.Null, "PrimaryKey attribute should not be null.");
            Assert.That(attribute.ColumnName, Is.EqualTo("workflow_id"), "Primary key column name should be 'workflow_id'.");
        }

        [Test]
        public void MaintenanceRequestId_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(Workflow).GetProperty(nameof(Workflow.MaintenanceRequestId));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }

        [Test]
        public void Status_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(Workflow).GetProperty(nameof(Workflow.Status));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }

        [Test]
        public void AssignedTo_ShouldBeDecoratedWithColumnAttribute()
        {
            var property = typeof(Workflow).GetProperty(nameof(Workflow.AssignedTo));
            var attribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            Assert.That(attribute, Is.Not.Null, "Column attribute should not be null.");
        }
    }
}
