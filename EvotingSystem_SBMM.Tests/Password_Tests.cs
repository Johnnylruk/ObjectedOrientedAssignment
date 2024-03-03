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
    private Mock<IUsersRepository> _userRepository;
    private Mock<IUserSession> _userSession;
    private Mock<IEmail> _email;
    private PasswordController _passwordController;

    public Password_Tests()
    {
        _userRepository = new Mock<IUsersRepository>();
        _userSession = new Mock<IUserSession>();
        _email = new Mock<IEmail>();
        _passwordController = new PasswordController(_userRepository.Object, _userSession.Object, _email.Object);
    }

    private List<UserModel> GetSampleUserList()
    {
        return Builder<UserModel>.CreateListOfSize(10).Build().ToList();
    }

    private ChangePasswordModel getSampleUserPasswordModel()
    {
        return Builder<ChangePasswordModel>.CreateNew().Build();
    }
    private UserModel GetSampleUser()
    {
        return Builder<UserModel>.CreateNew().Build();
    }

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
        var user = getSampleUserPasswordModel();
        var userSession = GetSampleUser();
        _userRepository.Setup(repo => repo.ChangePassword(user)).Verifiable();
        _userSession.Setup(repo => repo.GetUserSession()).Returns(userSession);
        user.Id = userSession.Id;
        _passwordController.ModelState.AddModelError("PropertyName", "Error message");

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
    
}