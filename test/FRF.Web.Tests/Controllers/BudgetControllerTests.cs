using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class BudgetControllerTests
    {
        private readonly Mock<IBudgetService> _budgetService;

        private readonly BudgetController _classUnderTest;

        public BudgetControllerTests()
        {
            _budgetService = new Mock<IBudgetService>();
            _classUnderTest = new BudgetController(_budgetService.Object);
        }

        [Fact]
        public async Task GetBudget_ReturnsOk()
        {
            //Arrange
            var budget = 10m;
            var projectId = 1;
            _budgetService.Setup(mock => mock.GetBudget(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<decimal>(budget));

            //Act
            var result = await _classUnderTest.GetBudget(projectId);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<decimal>(okResult.Value);
            Assert.Equal(budget, returnValue);
            _budgetService.Verify(mock => mock.GetBudget(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetBudget_ReturnsNotFound()
        {
            //Arrange
            var projectId = 1;
            _budgetService.Setup(mock => mock.GetBudget(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<decimal>(new Error(ErrorCodes.ProjectNotExists, "[Mock] Message")));

            //Act
            var result = await _classUnderTest.GetBudget(projectId);

            //Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            _budgetService.Verify(mock => mock.GetBudget(It.IsAny<int>()), Times.Once);
        }
    }
}
