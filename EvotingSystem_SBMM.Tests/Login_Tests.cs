using EVotingSystem_SBMM.Controllers;
using EVotingSystem_SBMM.Helper;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace EVotingSystem_SBMM.Tests
{
    public class Login_Tests
    {
        private readonly Mock<IUserSession> _userSession;
        private readonly Mock<ILoginRepository> _loginRepository;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly Mock<IPasswordHandle> _passwordHandle;
        private LoginController _loginController; 

        public Login_Tests()
        {
            _userSession = new Mock<IUserSession>();
            _loginRepository = new Mock<ILoginRepository>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _passwordHandle = new Mock<IPasswordHandle>();
            _loginController = new LoginController(_userSession.Object, _loginRepository.Object, _httpContextAccessor.Object, _passwordHandle.Object);
        }
        
        private UserModel GetSampleUser()
        {
            return Builder<UserModel>.CreateNew().Build();
        }
        private VoterModel GetSampleVoter()
        {
            return Builder<VoterModel>.CreateNew().Build();
        }
        private LoginModel GetSampleLoginUser()
        {
            return Builder<LoginModel>.CreateNew().Build();
        }
        #region Index_LogOut
        
        [Fact]
        public void Index_Returns_ViewResult_When_UserSession_Is_Null()
        {
            // Arrange
            var userSession = GetSampleUser();
            _userSession.Setup(repo => repo.GetUserSession()).Equals(null);
            
            // Act
            var result = _loginController.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            _userSession.Setup(repo => repo.GetUserSession()).Returns(null as UserModel);
        }

        
        [Fact]
        public void Index_RedirectsTo_Home_Index_Action_When_UserSession_Is_Not_Null()
        {
            // Arrange
            var userSession = GetSampleUser();
            _userSession.Setup(repo => repo.GetUserSession()).Returns(userSession);

            // Act
            var result = _loginController.Index() as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result?.ControllerName.Should().Be("Home");
            result?.ActionName.Should().Be("Index");
        }

        [Fact]
        public void LoggedOut_Returns_RedirectToAction_Index_Login_Controller()
        {
            // Arrange
            var userSession = GetSampleUser();
            _userSession.Setup(repo => repo.GetUserSession()).Returns(userSession);
            _userSession.Setup(repo => repo.RemoveLoginSession());
            // Act
            var result = _loginController.LoggedOut() as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result?.ControllerName.Should().Be("Login");
            result?.ActionName.Should().Be("Index");
            _userSession.Verify(repo => repo.RemoveLoginSession(), Times.Once);
        }
        #endregion

        #region Profile

        [Fact]
        public void FindProfile_Returns_UserWhenUserFound()
        {
            // Arrange
            var user = GetSampleUser();
            _loginRepository.Setup(repo => repo.GetUserByLogin(user.Login)).Returns(user);

            var hashPassword = user.Password.GenerateHash();
            _passwordHandle.Setup(ph => ph.ValidatePassword(hashPassword, user.Password)).Returns(true);

            // Act
            var result = _loginController.FindProfile(user.Login, user.Password);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UserModel>(result);
            Assert.Equal(user, result);
        }
        
        [Fact]
        public void FindProfile_Returns_NullWhenUserNotFound()
        {
            // Arrange
            var user = GetSampleUser();
            user.Login = "";
            _loginRepository.Setup(repo => repo.GetUserByLogin(user.Login)).Returns(user);

            var hashPassword = user.Password.GenerateHash();
            _passwordHandle.Setup(ph => ph.ValidatePassword(hashPassword, user.Password)).Returns(true);

            // Act
            var result = _loginController.FindProfile(user.Login, user.Password);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UserModel>(result);
            Assert.Equal(user, result);
        }
        [Fact]
        public void FindProfile_Returns_VoterProfileWhenVoterFound()
        {
            // Arrange
            var voter = GetSampleVoter();
            _loginRepository.Setup(repo => repo.GetVoterByLogin(voter.Login)).Returns(voter);

            var hashPassword = voter.Password.GenerateHash();
            _passwordHandle.Setup(ph => ph.ValidatePassword(hashPassword, voter.Password)).Returns(true);

            // Act
            var result = _loginController.FindProfile(voter.Login, voter.Password);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<VoterModel>(result);
            Assert.Equal(voter, result);
        }
        
        [Fact]
        public void FindProfile_Returns_VoterProfileNullWhenVoterIsPending()
        {
            // Arrange
            var voter = GetSampleVoter();
            voter.IsPending = true;
            _loginRepository.Setup(repo => repo.GetVoterByLogin(voter.Login)).Returns(voter);

            var hashPassword = voter.Password.GenerateHash();
            _passwordHandle.Setup(ph => ph.ValidatePassword(hashPassword, voter.Password)).Returns(true);

            // Act
            var result = _loginController.FindProfile(voter.Login, voter.Password);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Login

        [Fact]
        public void Login_Returns_HomeWhenUserLogIn()
        {
            // Arrange
            var loginModel = GetSampleLoginUser();
            var user = new UserModel { Login = loginModel.Login, Password = loginModel.Password }; 
            _loginRepository.Setup(repo => repo.GetUserByLogin(loginModel.Login)).Returns(user);
            _passwordHandle.Setup(ph => ph.ValidatePassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            var result = _loginController.LogIn(loginModel) as RedirectToActionResult;

            // Assert
            _userSession.Verify(repo => repo.CreateSession(user, null), Times.Once);
            result.Should().NotBeNull();
            result.ControllerName.Should().Be("Home");
            result.ActionName.Should().Be("Index");
        }
        
        [Fact]
        public void Login_Returns_AccessVoterWhenUserLogIn()
        {
            // Arrange
            var loginModel = GetSampleLoginUser();
            var voter = new VoterModel { Login = loginModel.Login, Password = loginModel.Password }; 
            _loginRepository.Setup(repo => repo.GetVoterByLogin(loginModel.Login)).Returns(voter);
            _passwordHandle.Setup(ph => ph.ValidatePassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            var result = _loginController.LogIn(loginModel) as RedirectToActionResult;

            // Assert
            _userSession.Verify(repo => repo.CreateSession(null, voter), Times.Once);
            result.Should().NotBeNull();
            result.ControllerName.Should().Be("AccessVoter");
            result.ActionName.Should().Be("Index");
        }   
        [Fact]
        public void Login_Returns_ErrorMessageWhenProfileIsNull()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var loginModel = GetSampleLoginUser();
            loginModel.ApprovalPending = true;
            var voter = new VoterModel { Login = "", Password = "loginModel.Password" }; 
            
            _loginRepository.Setup(repo => repo.GetVoterByLogin(loginModel.Login)).Returns(voter);
            _passwordHandle.Setup(ph => ph.ValidatePassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            _loginController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
                   {
                        ["ErrorMessage"] = "Approval Pending, you will receive an email once you have been approved."
                   };
            // Act
            var result = _loginController.LogIn(loginModel) as ViewResult;

            // Assert
            result.ViewName.Should().Be("Index");
            _loginController.TempData["ErrorMessage"].Should().Be("Approval Pending, you will receive an email once you have been approved.");
        }
        
        [Fact]
        public void Login_Returns_ErrorMessageWhenUserOrPasswordIsInvalid()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var loginModel = GetSampleLoginUser();
            var user = new UserModel { Login = loginModel.Login, Password = loginModel.Password };
            loginModel.IsInvalidCredentials = true;
            _loginRepository.Setup(repo => repo.GetUserByLogin(loginModel.Login)).Returns(user);
            _passwordHandle.Setup(ph => ph.ValidatePassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
    
            _loginController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["ErrorMessage"] = "Invalid Password."
            };
    
            // Act
            var result = _loginController.LogIn(loginModel) as ViewResult;

            // Assert
            result.ViewName.Should().Be("Index");
            _loginController.TempData["ErrorMessage"].Should().Be("Invalid Password.");
        }
        [Fact]
        public void Login_Returns_ErrorMessageWhenPasswordIsInvalid()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var loginModel = GetSampleLoginUser();
            var user = new UserModel { Login = loginModel.Login, Password = loginModel.Password };
            loginModel.IsInvalidCredentials = false;
            loginModel.ApprovalPending = false;
            _loginRepository.Setup(repo => repo.GetUserByLogin(loginModel.Login)).Returns(user);
            _passwordHandle.Setup(ph => ph.ValidatePassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
    
            _loginController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["ErrorMessage"] = "Invalid User/Password."
            };
    
            // Act
            var result = _loginController.LogIn(loginModel) as ViewResult;

            // Assert
            result.ViewName.Should().Be("Index");
            _loginController.TempData["ErrorMessage"].Should().Be("Invalid User/Password.");
        }
        #endregion
        
       
    }
}
