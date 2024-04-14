using EVotingSystem_SBMM.Enums;
using EVotingSystem_SBMM.Helper;
using EVotingSystem_SBMM.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace EVotingSystem_SBMM.Tests
{
    public class UserSession_Tests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly UserSession _userSession;

        public UserSession_Tests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();
            mockHttpContext.SetupGet(x => x.Session).Returns(mockSession.Object);
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

            _userSession = new UserSession(_mockHttpContextAccessor.Object);
            
        }

        [Fact]
        public void CreateSession_ShouldSetUserSession_WhenUserModelProvided()
        {
            // Arrange
            var userModel = Builder<UserModel>.CreateNew().Build();

            // Act
            _userSession.CreateSession(userModel, null);

            // Assert
            string expectedSessionValue = JsonConvert.SerializeObject(userModel);
            _mockHttpContextAccessor.Verify(
                x => x.HttpContext.Session.Set("userLoggedSession", It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public void CreateSession_ShouldSetVoterSession_WhenVoterModelProvided()
        {
            // Arrange
            var voterModel = Builder<VoterModel>.CreateNew().Build();

            // Act
            _userSession.CreateSession(null, voterModel);

            // Assert
            string expectedSessionValue = JsonConvert.SerializeObject(voterModel);
            _mockHttpContextAccessor.Verify(
                x => x.HttpContext.Session.Set("userLoggedSession", It.IsAny<byte[]>()), Times.Once);
        }
        [Fact]
        public void RemoveLoginSession_ShouldRemoveUserSession()
        {
            // Arrange
            var mockSession = new Mock<ISession>();
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext.Session).Returns(mockSession.Object);

            // Act
            _userSession.RemoveLoginSession();

            // Assert
            mockSession.Verify(x => x.Remove("userLoggedSession"), Times.Once);
        }
    }
}
