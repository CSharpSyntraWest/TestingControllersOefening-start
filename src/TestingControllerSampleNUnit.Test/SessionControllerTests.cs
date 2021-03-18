using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingControllersSample.Controllers;
using TestingControllersSample.Core.Interfaces;
using TestingControllersSample.Core.Model;
using TestingControllersSample.ViewModels;

namespace TestingControllerSampleNUnit.Test
{
    public class SessionControllerTests
    {
        private List<BrainstormSession> _sessions;
        [SetUp]
        public void Initialize()
        {
            _sessions = GetTestSessions();
        }
        private List<BrainstormSession> GetTestSessions()
        {
            var sessions = new List<BrainstormSession>();
            sessions.Add(new BrainstormSession()
            {
                DateCreated = new DateTime(2020, 7, 2),
                Id = 1,
                Name = "Brainstorming opdracht 1"
            });
            sessions.Add(new BrainstormSession()
            {
                DateCreated = new DateTime(2020, 7, 1),
                Id = 2,
                Name = "Brainstorming opdracht 2"
            });
            return sessions;
        }
        #region snippet_SessionControllerTests
        [Test]
        public async Task IndexReturnsARedirectToIndexHomeWhenIdIsNull()
        {
            // Arrange
            var controller = new SessionController(sessionRepository: null);

            // Act
            var result = await controller.Index(id: null);

            // Assert
            
            Assert.That(result,Is.TypeOf<RedirectToActionResult>());
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.AreEqual("Home", redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [Test]
        public async Task IndexReturnsContentWithSessionNotFoundWhenSessionNotFound()
        {
            // Arrange
            int testSessionId = 1;
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
                .ReturnsAsync((BrainstormSession)null);
            var controller = new SessionController(mockRepo.Object);

            // Act
            var result = await controller.Index(testSessionId);

            // Assert
            Assert.That(result,Is.TypeOf<ContentResult>());
            var contentResult = result as ContentResult;
            Assert.AreEqual("Session not found.", contentResult.Content);
        }

        [Test]
        public async Task IndexReturnsViewResultWithStormSessionViewModel()
        {
            // Arrange
            int testSessionId = 1;
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
                .ReturnsAsync(_sessions.FirstOrDefault(
                    s => s.Id == testSessionId));
            var controller = new SessionController(mockRepo.Object);

            // Act
            var result = await controller.Index(testSessionId);

            // Assert
            Assert.That(result,Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.ViewData.Model,Is.TypeOf<StormSessionViewModel>());
            var model = viewResult.ViewData.Model as StormSessionViewModel;
            Assert.AreEqual("Brainstorming opdracht 1", model.Name);
            Assert.AreEqual(2, model.DateCreated.Day);
            Assert.AreEqual(testSessionId, model.Id);
        }
        #endregion

       
    }
}
