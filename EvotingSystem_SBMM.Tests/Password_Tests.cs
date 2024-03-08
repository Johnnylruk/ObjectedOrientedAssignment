using System.Collections.Immutable;
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

namespace EVotingSystem_SBMM.Tests;

public class Password_Tests
{
    private readonly Mock<IUsersRepository> _userRepository;
    private readonly Mock<IUserSession> _userSession;
    private readonly Mock<IEmail> _email;
    private readonly Mock<IPasswordHandle> _passwordHandle;
    private PasswordController _passwordController;

    public Password_Tests()
    {
        _userRepository = new Mock<IUsersRepository>();
        _userSession = new Mock<IUserSession>();
        _email = new Mock<IEmail>();
        _passwordHandle = new Mock<IPasswordHandle>();
        _passwordController = new PasswordController(_userRepository.Object, _userSession.Object, _email.Object, _passwordHandle.Object);
    }

    private ResetPasswordModel GetSampleResetPassword()
    {
        return Builder<ResetPasswordModel>.CreateNew().Build();
    }

    private ChangePasswordModel getSampleUserPasswordModel()
    {
        return Builder<ChangePasswordModel>.CreateNew().Build();
    }
    private UserModel GetSampleUser()
    {
        return Builder<UserModel>.CreateNew().Build();
    }

    #region Change Password
    
    [Fact]
    public void ChangeUserPassword_Should_Return_RedirectToAction_Create_When_Changed_Successfully()
    {
        //Arrange
        var httpContext = new DefaultHttpContext();
        var user = getSampleUserPasswordModel();
        var userSession = GetSampleUser();
        _userRepository.Setup(repo => repo.ChangePassword(user)).Verifiable();
        _userSession.Setup(repo => repo.GetUserSession()).Returns(userSession);
        user.Id = userSession.Id;
        _passwordController.ModelState.Clear(); 
        _passwordController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["SuccessMessage"] = "Password has been successful updated."
        };
        //Act
        var result = _passwordController.ChangeUserPassword(user) as RedirectToActionResult;
        
        //Assert
        result.Should().NotBe(null);
        result.ActionName.Should().Be("ChangePassword");
        _passwordController.TempData["SuccessMessage"].Should().Be("Password has been successful updated.");
    }
    [Fact]
    public void ChangeUserPassword_Should_Return_Successfull_WhenModel_Is_Valid()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var user = getSampleUserPasswordModel();
        var userSession = GetSampleUser();
        _userRepository.Setup(repo => repo.ChangePassword(user)).Verifiable();
        _userSession.Setup(repo => repo.GetUserSession()).Returns(userSession);
        user.Id = userSession.Id;
        _passwordController.ModelState.AddModelError("PropertyName", "Error message");
        _passwordController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["SuccessMessage"] = "Password has been successful updated."
        };
        // Act
        var result = _passwordController.ChangeUserPassword(user) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.Model.Should().BeOfType<ChangePasswordModel>().And.BeEquivalentTo(user);
        _userRepository.Verify(repo => repo.ChangePassword(user), Times.Never);
    }
    
    [Fact]
    public void ChangeUserPassword_Returns_ViewResult_When_ModelState_Is_Invalid()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var user = getSampleUserPasswordModel();
        var userSession = GetSampleUser();
        user.Id = userSession.Id;
        _userSession.Setup(repo => repo.GetUserSession()).Returns(userSession);
        _passwordController.ModelState.AddModelError("PropertyName", "ErrorMessage");
        _passwordController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["ErrorMessage"] = "Error trying to update your password."
        };
        // Act
        var result = _passwordController.ChangeUserPassword(user) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be("ChangePassword"); 
        _passwordController.TempData["ErrorMessage"].Should().Be("Error trying to update your password.");

    }
    
    #endregion

    #region SendPasswordLink

    [Fact]
    public void SendPasswordLink_Should_Return_RedirectToAction_When_Send_Successfully()
    {
        //Arrange
        var httpContext = new DefaultHttpContext();
        var user = GetSampleUser();
        var resetPasswordUser = GetSampleResetPassword();
        
        string subject = "EVoting System SBMM - New Password";
        _passwordHandle.Setup(repo => repo.GenerateNewPassword());
        string message = $"Your new password is: {user.Password}";
        
        _userRepository.Setup(repo => repo.GetByLoginAndEmail(resetPasswordUser.Login, resetPasswordUser.Email)).Returns(user);
        _email.Setup(repo => repo.SendEmailLink(user.Email, subject, message)).Returns(true).Verifiable();
        
        _userRepository.Setup(repo => repo.UpdateUser(user)).Returns(user);
       
        _passwordController.ModelState.Clear(); 
        _passwordController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["SuccessMessage"] = "We have sent a new password to your email."
        };
        //Act
        var result = _passwordController.SendResetPasswordLink(resetPasswordUser) as RedirectToActionResult;
        
        //Assert
        result.Should().NotBe(null);
        result.ActionName.Should().Be("Index");
        _passwordController.TempData["SuccessMessage"].Should().Be("We have sent a new password to your email.");
    }
    [Fact]
    public void SendPasswordLink_Should_Set_Error_Message_When_Send_Fails()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var resetPasswordModel = GetSampleResetPassword();
        var user = GetSampleUser();

        _userRepository.Setup(repo => repo.GetByLoginAndEmail(resetPasswordModel.Login, resetPasswordModel.Email)).Returns(user);
        _passwordController.ModelState.AddModelError("PropertyName", "ErrorMessage");
        _passwordController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["ErrorMessage"] = "We could not reset your password."
        };
        // Act
        var result = _passwordController.SendResetPasswordLink(resetPasswordModel) as ViewResult;
    
        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be("ChangePassword");
    
        // Verify that the TempData contains the expected error message
        _passwordController.TempData["ErrorMessage"].Should().Be("We could not reset your password.");
    }

    #endregion
    
    
}