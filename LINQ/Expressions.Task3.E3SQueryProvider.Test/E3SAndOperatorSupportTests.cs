/*
 * This task is a bit harder than the previous two.
 * Feel free to change the E3SLinqProvider and any other classes if needed.
 * Possibly, after these changes you will need to rewrite existing tests to make them work again =).
 *
 * The task: implement support of && operator for IQueryable. The final request generated by FTSRequestGenerator, should
 * imply the following rules: https://kb.epam.com/display/EPME3SDEV/Telescope+public+REST+for+data#TelescopepublicRESTfordata-FTSRequestSyntax
 */

using System;
using System.Linq;
using System.Linq.Expressions;
using Expressions.Task3.E3SQueryProvider.Helpers;
using Expressions.Task3.E3SQueryProvider.Models.Entities;
using Expressions.Task3.E3SQueryProvider.Services;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Expressions.Task3.E3SQueryProvider.Test
{
    public class E3SAndOperatorSupportTests
    {

        #region private 

        private static readonly IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        private static readonly string _baseUrl = config["api:apiBaseUrl"];

        #endregion

        #region SubTask 3: AND operator support

        [Fact]
        public void TestAndQueryable()
        {
            // Arrange
            var translator = new ExpressionToFtsRequestTranslator();
            Expression<Func<IQueryable<EmployeeEntity>, IQueryable<EmployeeEntity>>> expression
                = query => query.Where(e => e.Workstation == "EPRUIZHW006" && e.Manager.StartsWith("John"));

            var translated = translator.Translate(expression);

            var expectedResultSource = "Workstation:(EPRUIZHW006);Manager:(John*)";
            var generator = new FtsRequestGenerator(_baseUrl);

            var expectedResult = generator.GenerateRequestUrl(typeof(EmployeeEntity), expectedResultSource);

            // Act
            var request = generator.GenerateRequestUrl(typeof(EmployeeEntity), translated);

            // Assert
            Assert.Equal(expectedResult, request);
        }

        #endregion
    }
}
