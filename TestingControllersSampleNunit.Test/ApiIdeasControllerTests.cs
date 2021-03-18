using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestingControllersSample.Api;
using TestingControllersSample.ClientModels;
using TestingControllersSample.Core.Interfaces;
using TestingControllersSample.Core.Model;

namespace TestingControllersSampleNunit.Test
{
    public class ApiIdeasControllerTests
    {
        [SetUp]
        public void Setup()
        {
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
            //Assert.IsType<BadRequestObjectResult>(result);//xUnit Assert methode moet vervangen worden door gelijke Assert in NUnit:
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        #endregion
        //Oefening: zet de xUnit test methoden om naar gelijkaardige NUnit test methoden

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
            // Assert.IsType<NotFoundObjectResult>(result); xUnit

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
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
            var result = await controller.Create(newIdea);

            // Assert
            //var okResult = Assert.IsType<OkObjectResult>(result); xUnit      
            Assert.That(result, Is.TypeOf<OkObjectResult>()); //of Assert.IsInstanceOf<OkObjectResult>(result); Nunit
            var okResult = result as OkObjectResult;
            //var returnSession = Assert.IsType<BrainstormSession>(okResult.Value); xUnit
            Assert.That(okResult.Value, Is.TypeOf<BrainstormSession>());//of Assert.IsInstanceOf<....
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
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
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
                .ReturnsAsync(GetTestSession());
            var controller = new IdeasController(mockRepo.Object);

            // Act
            var result = await controller.ForSession(testSessionId);

            // Assert
           Assert.IsInstanceOf<OkObjectResult>(result);
           var okResult = result as OkObjectResult;
           Assert.IsInstanceOf<List<IdeaDTO>>(okResult.Value);
           var returnValue = okResult.Value as List<IdeaDTO>;
           var idea = returnValue.FirstOrDefault();
           Assert.AreEqual("One", idea.Name);
        }
        #endregion

        //#region snippet_ForSessionActionResult_ReturnsNotFoundObjectResultForNonexistentSession
        //[Fact]
        //public async Task ForSessionActionResult_ReturnsNotFoundObjectResultForNonexistentSession()
        //{
        //    // Arrange
        //    var mockRepo = new Mock<IBrainstormSessionRepository>();
        //    var controller = new IdeasController(mockRepo.Object);
        //    var nonExistentSessionId = 999;

        //    // Act
        //    var result = await controller.ForSessionActionResult(nonExistentSessionId);

        //    // Assert
        //    var actionResult = Assert.IsType<ActionResult<List<IdeaDTO>>>(result);
        //    Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        //}
        //#endregion

        //#region snippet_ForSessionActionResult_ReturnsIdeasForSession
        //[Fact]
        //public async Task ForSessionActionResult_ReturnsIdeasForSession()
        //{
        //    // Arrange
        //    int testSessionId = 123;
        //    var mockRepo = new Mock<IBrainstormSessionRepository>();
        //    mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
        //        .ReturnsAsync(GetTestSession());
        //    var controller = new IdeasController(mockRepo.Object);

        //    // Act
        //    var result = await controller.ForSessionActionResult(testSessionId);

        //    // Assert
        //    var actionResult = Assert.IsType<ActionResult<List<IdeaDTO>>>(result);
        //    var returnValue = Assert.IsType<List<IdeaDTO>>(actionResult.Value);
        //    var idea = returnValue.FirstOrDefault();
        //    Assert.Equal("One", idea.Name);
        //}
        //#endregion

        //#region snippet_CreateActionResult_ReturnsBadRequest_GivenInvalidModel
        //[Fact]
        //public async Task CreateActionResult_ReturnsBadRequest_GivenInvalidModel()
        //{
        //    // Arrange & Act
        //    var mockRepo = new Mock<IBrainstormSessionRepository>();
        //    var controller = new IdeasController(mockRepo.Object);
        //    controller.ModelState.AddModelError("error", "some error");

        //    // Act
        //    var result = await controller.CreateActionResult(model: null);

        //    // Assert
        //    var actionResult = Assert.IsType<ActionResult<BrainstormSession>>(result);
        //    Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        //}
        //#endregion

        //#region snippet_CreateActionResult_ReturnsNotFoundObjectResultForNonexistentSession
        //[Fact]
        //public async Task CreateActionResult_ReturnsNotFoundObjectResultForNonexistentSession()
        //{
        //    // Arrange
        //    var nonExistentSessionId = 999;
        //    string testName = "test name";
        //    string testDescription = "test description";
        //    var mockRepo = new Mock<IBrainstormSessionRepository>();
        //    var controller = new IdeasController(mockRepo.Object);

        //    var newIdea = new NewIdeaModel()
        //    {
        //        Description = testDescription,
        //        Name = testName,
        //        SessionId = nonExistentSessionId
        //    };

        //    // Act
        //    var result = await controller.CreateActionResult(newIdea);

        //    // Assert
        //    var actionResult = Assert.IsType<ActionResult<BrainstormSession>>(result);
        //    Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        //}
        //#endregion

        //#region snippet_CreateActionResult_ReturnsNewlyCreatedIdeaForSession
        //[Fact]
        //public async Task CreateActionResult_ReturnsNewlyCreatedIdeaForSession()
        //{
        //    // Arrange
        //    int testSessionId = 123;
        //    string testName = "test name";
        //    string testDescription = "test description";
        //    var testSession = GetTestSession();
        //    var mockRepo = new Mock<IBrainstormSessionRepository>();
        //    mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
        //        .ReturnsAsync(testSession);
        //    var controller = new IdeasController(mockRepo.Object);

        //    var newIdea = new NewIdeaModel()
        //    {
        //        Description = testDescription,
        //        Name = testName,
        //        SessionId = testSessionId
        //    };
        //    mockRepo.Setup(repo => repo.UpdateAsync(testSession))
        //        .Returns(Task.CompletedTask)
        //        .Verifiable();

        //    // Act
        //    var result = await controller.CreateActionResult(newIdea);

        //    // Assert
        //    var actionResult = Assert.IsType<ActionResult<BrainstormSession>>(result);
        //    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        //    var returnValue = Assert.IsType<BrainstormSession>(createdAtActionResult.Value);
        //    mockRepo.Verify();
        //    Assert.Equal(2, returnValue.Ideas.Count());
        //    Assert.Equal(testName, returnValue.Ideas.LastOrDefault().Name);
        //    Assert.Equal(testDescription, returnValue.Ideas.LastOrDefault().Description);
        //}
        //#endregion

        private BrainstormSession GetTestSession()
        {
            var session = new BrainstormSession()
            {
                DateCreated = new DateTime(2020, 7, 2),
                Id = 1,
                Name = "Test One"
            };

            var idea = new Idea() { Name = "One" };
            session.AddIdea(idea);
            return session;
        }
    }
}