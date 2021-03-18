using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestingControllersSample.Controllers;
using TestingControllersSample.Core.Interfaces;
using TestingControllersSample.Core.Model;
using TestingControllersSample.ViewModels;

namespace TestingControllerSampleNUnit.Test
{
    public class HomeControllerTests
    {
        public List<BrainstormSession> _sessions;
        [SetUp]
        public void Setup()
        {
            _sessions = GetTestSessions();
        }
        private static List<BrainstormSession> GetTestSessions()
        {
            var sessions = new List<BrainstormSession>();
            sessions.Add(new BrainstormSession()
            {
                DateCreated = new DateTime(2021, 3, 15),
                Id = 1,
                Name = "Sessie 1"
            });
            sessions.Add(new BrainstormSession()
            {
                DateCreated = new DateTime(2021, 3, 14),
                Id = 2,
                Name = "Sessie twee"
            });
            return sessions;
        }
        [Test]
        public async Task Index_ReturnsAViewResult_WithAListOfBrainstormSessions()
        {
            // Arrange
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.ListAsync())
                .ReturnsAsync(_sessions);
            var controller = new HomeController(mockRepo.Object);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<IEnumerable<StormSessionViewModel>>(
                viewResult.ViewData.Model);
            var model = viewResult.ViewData.Model as IEnumerable<StormSessionViewModel>;
            Assert.AreEqual(2, model.Count());
        }
        public async Task IndexPost_ReturnsBadRequestResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.ListAsync())
                .ReturnsAsync(_sessions);
            var controller = new HomeController(mockRepo.Object);
            controller.ModelState.AddModelError("SessionName", "Required");
            var newSession = new HomeController.NewSessionModel();

            // Act
            var result = await controller.Index(newSession);

            // Assert
            Assert.That(result,Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsInstanceOf<SerializableError>(badRequestResult.Value);

        }
        public async Task IndexPost_ReturnsARedirectAndAddsSession_WhenModelStateIsValid()
        {
            // Arrange
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.AddAsync(It.IsAny<BrainstormSession>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            var controller = new HomeController(mockRepo.Object);
            var newSession = new HomeController.NewSessionModel()
            {
                SessionName = "Test Name"
            };

            // Act
            var result = await controller.Index(newSession);

            // Assert
            Assert.That(result,Is.TypeOf<RedirectToActionResult>());
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            mockRepo.Verify();
        }
    }
}