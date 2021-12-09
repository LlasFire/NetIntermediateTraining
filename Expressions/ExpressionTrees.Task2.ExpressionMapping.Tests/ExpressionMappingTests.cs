using ExpressionTrees.Task2.ExpressionMapping.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ExpressionTrees.Task2.ExpressionMapping.Tests
{
    [TestClass]
    public class ExpressionMappingTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var mapGenerator = new MappingGenerator();
            var mapper = mapGenerator.Generate<Foo, Bar>();
            var prepareModel = new Foo() { Name = "dsafsdgf", Id = Guid.NewGuid() };

            // Act
            var res = mapper.Map(prepareModel);

            // Assert
            Assert.AreEqual(prepareModel.Name, res.Name);
            Assert.AreEqual(prepareModel.Id, res.Id);
        }

        [TestMethod]
        public void TestMethod2()
        {
            // Arrange
            var mapGenerator = new MappingGenerator();
            var mapper = mapGenerator.Generate<Foo, Bar>();
            var prepareModel = new Foo() { BreakProperty = "Los Angeles", Id = Guid.NewGuid(), Street = "fdrsgdg", Count = 5145 };

            // Act
            var res = mapper.Map(prepareModel);

            // Assert
            Assert.AreEqual(prepareModel.City, res.City);
            Assert.AreEqual(prepareModel.Id, res.Id);
            Assert.AreEqual(prepareModel.Count, res.Count);
        }

        [TestMethod]
        public void TestMethod3()
        {
            // Arrange
            var mapGenerator = new MappingGenerator();
            var mapper = mapGenerator.Generate<Foo, Bar>();
            var prepareModel = new Foo() { Name = "khuikh", Id = Guid.NewGuid(), BreakProperty = "rtrtrtrtr"};

            // Act
            var res = mapper.Map(prepareModel);

            // Assert
            Assert.AreEqual(prepareModel.Name, res.Name);
            Assert.AreEqual(prepareModel.Id, res.Id);
            Assert.AreEqual(0, res.BreakProperty);
        }

        [TestMethod]
        public void TestMethod4()
        {
            // Arrange
            var mapGenerator = new MappingGenerator();
            var mapper = mapGenerator.Generate<Foo, Bar>();
            var prepareModel = new Foo() { Name = "khuikh", Id = Guid.NewGuid(), BreakProperty = "rtrtrtrtr", ForeignId = Guid.NewGuid(), Tratarara = 120 };

            // Act
            var res = mapper.Map(prepareModel);

            // Assert
            Assert.AreEqual(prepareModel.Name, res.Name);
            Assert.AreEqual(prepareModel.Id, res.Id);
            Assert.AreEqual(0, res.BreakProperty);
            Assert.AreEqual(prepareModel.ForeignId.ToString(), res.Foreign);
            Assert.AreEqual(prepareModel.Tratarara, res.TRA);
        }
    }
}
