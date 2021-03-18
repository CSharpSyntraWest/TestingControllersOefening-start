using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingControllersSample.Api;
using TestingControllersSample.ClientModels;
using TestingControllersSample.Core.Interfaces;
using TestingControllersSample.Core.Model;

namespace TestingControllerSampleNUnit.Test
{
    public class ApiIdeasControllerTests
    {
        public BrainstormSession _session;
        [SetUp]
        public void Setup()
        {
            _session = GetTestSession();
        }
        private BrainstormSession GetTestSession()
        {
            var session = new BrainstormSession()
            {
                DateCreated = new DateTime(2021, 1, 10),
                Id = 1,
                Name = "Test One"
            };

            var idea = new Idea() { Name = "One" };
            session.AddIdea(idea);
            return session;
        }
        #region snippet_ApiIdeasControllerTests1
        [Test]
        public async Task Create_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange & Act
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            var controller = new IdeasController(mockRepo.Object);
            controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await controller.Create(model: null);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        #endregion

        #region snippet_ApiIdeasControllerTests2
        [Test]
        public async Task Create_ReturnsHttpNotFound_ForInvalidSession()
        {
            // Arrange
            int testSessionId = 123;
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
                .ReturnsAsync((BrainstormSession)null);
            var controller = new IdeasController(mockRepo.Object);

            // Act
            var result = await controller.Create(new NewIdeaModel());

            // Assert
            Assert.That(result,Is.TypeOf<NotFoundObjectResult>());
        }
        #endregion

        #region snippet_ApiIdeasControllerTests3
        [Test]
        public async Task Create_ReturnsNewlyCreatedIdeaForSession()
        {
            // Arrange
            int testSessionId = 123;
            string testName = "test name";
            string testDescription = "test description";
            
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
                .ReturnsAsync(_session);
            var controller = new IdeasController(mockRepo.Object);

            var newIdea = new NewIdeaModel()
            {
                Description = testDescription,
                Name = testName,
                SessionId = testSessionId
            };
            mockRepo.Setup(repo => repo.UpdateAsync(_session))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await controller.Create(newIdea);

            // Assert
            Assert.That(result,Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value,Is.TypeOf<BrainstormSession>());
            var returnSession = okResult.Value as BrainstormSession;
            mockRepo.Verify();
            Assert.AreEqual(2, returnSession.Ideas.Count());
            Assert.AreEqual(testName, returnSession.Ideas.LastOrDefault().Name);
            Assert.AreEqual(testDescription, returnSession.Ideas.LastOrDefault().Description);
        }
        #endregion

        #region snippet_ApiIdeasControllerTests4
        [Test]
        public async Task ForSession_ReturnsHttpNotFound_ForInvalidSession()
        {
            // Arrange
            int testSessionId = 123;
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
                .ReturnsAsync((BrainstormSession)null);
            var controller = new IdeasController(mockRepo.Object);

            // Act
            var result = await controller.ForSession(testSessionId);

            // Assert
            Assert.That(result,Is.TypeOf<NotFoundObjectResult>());
            var notFoundObjectResult = result as NotFoundObjectResult;
            Assert.AreEqual(testSessionId, notFoundObjectResult.Value);
        }
        #endregion

        #region snippet_ApiIdeasControllerTests5
        [Test]
        public async Task ForSession_ReturnsIdeasForSession()
        {
            // Arrange
            int testSessionId = 123;
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
                .ReturnsAsync(_session);
            var controller = new IdeasController(mockRepo.Object);

            // Act
            var result = await controller.ForSession(testSessionId);

            // Assert
            Assert.That(result,Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value,Is.TypeOf<List<IdeaDTO>>()) ;
            var returnValue = okResult.Value as List<IdeaDTO>;
           var idea = returnValue.FirstOrDefault();
            Assert.AreEqual("One", idea.Name);
        }
        #endregion

        #region snippet_ForSessionActionResult_ReturnsNotFoundObjectResultForNonexistentSession
        [Test]
        public async Task ForSessionActionResult_ReturnsNotFoundObjectResultForNonexistentSession()
        {
            // Arrange
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            var controller = new IdeasController(mockRepo.Object);
            var nonExistentSessionId = 999;

            // Act
            var result = await controller.ForSessionActionResult(nonExistentSessionId);

            // Assert
            Assert.That(result,Is.TypeOf<ActionResult<List<IdeaDTO>>>());
            var actionResult = result as ActionResult<List<IdeaDTO>>;
            Assert.That(actionResult.Result,Is.TypeOf<NotFoundObjectResult>());
        }
        #endregion

        #region snippet_ForSessionActionResult_ReturnsIdeasForSession
        [Test]
        public async Task ForSessionActionResult_ReturnsIdeasForSession()
        {
            // Arrange
            int testSessionId = 123;
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
                .ReturnsAsync(GetTestSession());
            var controller = new IdeasController(mockRepo.Object);

            // Act
            var result = await controller.ForSessionActionResult(testSessionId);

            // Assert
            Assert.That(result,Is.TypeOf<ActionResult<List<IdeaDTO>>>());
            var actionResult =result as ActionResult<List<IdeaDTO>>;
            Assert.That(actionResult.Value,Is.TypeOf<List<IdeaDTO>>());
            var returnValue = actionResult.Value as List<IdeaDTO>;
            var idea = returnValue.FirstOrDefault();
            Assert.AreEqual("One", idea.Name);
        }
        #endregion

        #region snippet_CreateActionResult_ReturnsBadRequest_GivenInvalidModel
        [Test]
        public async Task CreateActionResult_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange & Act
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            var controller = new IdeasController(mockRepo.Object);
            controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await controller.CreateActionResult(model: null);

            // Assert
            Assert.That(result,Is.TypeOf<ActionResult<BrainstormSession>>());
            var actionResult = result as ActionResult<BrainstormSession>;
            Assert.That(actionResult.Result,Is.TypeOf<BadRequestObjectResult>());
        }
        #endregion

        #region snippet_CreateActionResult_ReturnsNotFoundObjectResultForNonexistentSession
        [Test]
        public async Task CreateActionResult_ReturnsNotFoundObjectResultForNonexistentSession()
        {
            // Arrange
            var nonExistentSessionId = 999;
            string testName = "test name";
            string testDescription = "test description";
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            var controller = new IdeasController(mockRepo.Object);

            var newIdea = new NewIdeaModel()
            {
                Description = testDescription,
                Name = testName,
                SessionId = nonExistentSessionId
            };

            // Act
            var result = await controller.CreateActionResult(newIdea);

            // Assert
            Assert.That(result,Is.TypeOf<ActionResult<BrainstormSession>>());
            var actionResult = result as ActionResult<BrainstormSession>;
            Assert.That(actionResult.Result,Is.TypeOf<NotFoundObjectResult>());
        }
        #endregion

        #region snippet_CreateActionResult_ReturnsNewlyCreatedIdeaForSession
        [Test]
        public async Task CreateActionResult_ReturnsNewlyCreatedIdeaForSession()
        {
            // Arrange
            int testSessionId = 123;
            string testName = "test name";
            string testDescription = "test description";
            var testSession = GetTestSession();
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
                .ReturnsAsync(testSession);
            var controller = new IdeasController(mockRepo.Object);

            var newIdea = new NewIdeaModel()
            {
                Description = testDescription,
                Name = testName,
                SessionId = testSessionId
            };
            mockRepo.Setup(repo => repo.UpdateAsync(testSession))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await controller.CreateActionResult(newIdea);

            // Assert
            Assert.That(result,Is.TypeOf<ActionResult<BrainstormSession>>());
            var actionResult = result as ActionResult<BrainstormSession>;
            Assert.That(actionResult.Result,Is.TypeOf<CreatedAtActionResult>());
            var createdAtActionResult = actionResult.Result as CreatedAtActionResult;
            Assert.That(createdAtActionResult.Value,Is.TypeOf<BrainstormSession>());
            var returnValue = createdAtActionResult.Value as BrainstormSession;
            mockRepo.Verify();
            Assert.AreEqual(2, returnValue.Ideas.Count());
            Assert.AreEqual(testName, returnValue.Ideas.LastOrDefault().Name);
            Assert.AreEqual(testDescription, returnValue.Ideas.LastOrDefault().Description);
        }
        #endregion


    }
}
