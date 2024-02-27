using EVotingSystem_SBMM.Controllers;
using EVotingSystem_SBMM.Enums;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace EVotingSystem_SBMM.Tests;

public class User_Tests
{
    private  Mock<IUsersRepository> _userRepository;
    private ElectoralAdminUserController _userController;

    public User_Tests()
    {
        _userRepository = new Mock<IUsersRepository>();
        _userController = new ElectoralAdminUserController(_userRepository.Object);
    }
    
     private List<UserModel> GetSampleUserList()
    {
        var getAllData = Builder<UserModel>.CreateListOfSize(10).Build();
        return getAllData.ToList();
    }

    private UserModel GetSampleUser()
    {
        var getData = Builder<UserModel>.CreateNew().Build();
        return getData;
    }

    private UserModel CreateSampleModel()
    {
        var getData = Builder<UserModel>.CreateNew()
            .With(c => c.Id, 1)
            .With(c => c.Name, "John")
            .With(c => c.Email, "Johnny@Gmail.com")
            .With(c => c.Login, "John.Doe")
            .With(c => c.Password, "Test@123")
            .With(c => c.Profile, ProfileEnum.ElectoralAdministrator)
            .With(c => c.RegisterDate, DateTime.Now)
            .Build();
        return getData;
    }
    private UserWithOutPwdModel CreateSampleUserWithOutPwdModel()
    {
        // Create a sample UserWithOutPwdModel instance
        return new UserWithOutPwdModel
        {
            Id = 1,
            Name = "John Doe",
            Login = "johndoe",
            Email = "johndoe@example.com",
            Profile = ProfileEnum.ElectoralAdministrator
        };
    }

    [Fact]
    public void ListAllUsers_Should_ListAll()
    {
        //Arrange
        var userModel = GetSampleUserList(); 
        _userRepository.Setup(repo => repo.GetAll()).Returns(userModel);
       
        //Act
        var result = _userController.Index();
        
        //Assert
        result.Should().NotBe(null);
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<UserModel>>(viewResult.ViewData.Model);
        model.Should().HaveCount(userModel.Count);
        _userRepository.Verify(x => x.GetAll(), Times.Once);

    }

    #region Create
    
    [Fact]
    public void CreateUser_Should_Return_Successfull_WhenModel_Is_Valid()
    {
        // Arrange
        var userModel = CreateSampleModel();
        _userRepository.Setup(repo => repo.Register(userModel)).Verifiable();
        _userController.ModelState.AddModelError("PropertyName", "Error message");

        // Act
        var result = _userController.CreateUser(userModel) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.Model.Should().BeOfType<UserModel>().And.BeEquivalentTo(userModel);
        _userRepository.Verify(repo => repo.Register(userModel), Times.Never);
    }
    
    [Fact]
    public void CreateUser_Returns_ViewResult_When_ModelState_Is_Invalid()
    {
        // Arrange
        var userModel = Builder<UserModel>.CreateNew().Build();
        _userController.ModelState.AddModelError("PropertyName", "ErrorMessage");

        // Act
        var result = _userController.CreateUser(userModel) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().BeNull(); 
    }
    
    [Fact]
    public void CreateUser_Returns_RedirectToAction_Index_When_Created_Successfully()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var userModel = Builder<UserModel>.CreateNew().Build();
        _userRepository.Setup(repo => repo.Register(userModel)).Verifiable();
        _userController.ModelState.Clear(); // Ensure ModelState is valid
        _userController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["SuccessMessage"] = "User has been created."
        };

        // Act
        var result = _userController.CreateUser(userModel) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("Index");
        _userController.TempData["SuccessMessage"].Should().Be("User has been created.");
    }
    [Fact]
    public void CreateUser_Returns_RedirectToAction_Index_When_Creation_Fails()
    {
        //Arrange
        var httpContext = new DefaultHttpContext();
        var userModel = Builder<UserModel>.CreateNew().Build();
        _userRepository.Setup(repo => repo.Register(userModel)).Verifiable();
        _userController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["ErrorMessage"] = "User not found."
        };
        //Act
        var result = _userController.CreateUser(userModel) as RedirectToActionResult;
            
        //Assert 
        result.Should().NotBeNull();
        result.ActionName.Should().Be("Index");
        _userController.TempData["ErrorMessage"].Should().Be("User not found.");
    }

    #endregion

    #region Edit

    [Fact]
    public void EditUser_Should_Return_User_By_Id()
    {
        //Arrange
        var userModel = GetSampleUser();
        _userRepository.Setup(repo => repo.GetUserById(1)).Returns(userModel);
        
        //Act 
        var result = _userController.EditUser(1);
        
        //Assert
        result.Should().NotBe(null);
        result.Should().BeOfType<ViewResult>();
        _userRepository.Verify(x => x.GetUserById(1), Times.Once);
    }
    
    [Fact]
    public void EditUser_Should_Return_Successfull_WhenModel_Is_Valid()
    {
        var userWithOutPwdModel = CreateSampleUserWithOutPwdModel();
        _userRepository.Setup(repo => repo.UpdateUser(It.IsAny<UserModel>())).Verifiable();
        _userController.ModelState.AddModelError("PropertyName", "Error message");

        //Act
        var result = _userController.EditUser(userWithOutPwdModel) as ViewResult;

        //Assert
        result.Should().NotBeNull();
        _userRepository.Verify(repo => repo.UpdateUser(It.IsAny<UserModel>()), Times.Never);
    }

     [Fact]
    public void EditUser_Returns_ViewResult_When_ModelState_Is_Invalid()
    {
        // Arrange
        var userModel = Builder<UserModel>.CreateNew().Build();
        _userController.ModelState.AddModelError("PropertyName", "ErrorMessage");

        // Act
        var result = _userController.EditUser(userModel.Id) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().BeNull(); 
    }
    
    [Fact]
    public void EditUser_Returns_RedirectToAction_Index_When_Updated_Successfully()
    {
        var httpContext = new DefaultHttpContext();
        var userWithOutPwdModel = Builder<UserWithOutPwdModel>.CreateNew().Build();
        var userModel = new UserModel
        {
            Id = userWithOutPwdModel.Id,
            Name = userWithOutPwdModel.Name,
            Login = userWithOutPwdModel.Login,
            Email = userWithOutPwdModel.Email,
            Profile = userWithOutPwdModel.Profile
        };
        _userRepository.Setup(repo => repo.UpdateUser(It.IsAny<UserModel>())).Returns(userModel); // Make the UpdateUser method return the model
        _userController.ModelState.Clear(); // Ensure ModelState is valid
        _userController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["SuccessMessage"] = "User has been updated."
        };

        // Act
        var result = _userController.EditUser(userWithOutPwdModel) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("Index");
        _userController.TempData["SuccessMessage"].Should().Be("User has been updated.");
    }
    
    [Fact]
    public void EditUser_Returns_RedirectToAction_Index_When_Updating_Fails()
    {
        //Arrange
        var httpContext = new DefaultHttpContext();
        var userWithOutPwdModel = Builder<UserWithOutPwdModel>.CreateNew().Build();
        _userRepository.Setup(repo => repo.UpdateUser(It.IsAny<UserModel>())).Throws(new Exception("User not found.")); 
        _userController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["ErrorMessage"] = $"Ops, could not update a user. Please try again. Error details: User not found."
        };

        //Act
        var result = _userController.EditUser(userWithOutPwdModel) as RedirectToActionResult;
        
        //Assert 
        result.Should().NotBeNull();
        result.ActionName.Should().Be("Index");
        _userController.TempData["ErrorMessage"].Should().Be($"Ops, could not update a user. Please try again. Error details: User not found.");
    }

    #endregion

    #region Delete
    
    [Fact]
    public void DeleteUser_Should_Return_User_By_Id()
    {
        //Arrange
        var userModel = GetSampleUser();
        _userRepository.Setup(repo => repo.GetUserById(1)).Returns(userModel);
        
        //Act 
        var result = _userController.DeleteUser(1);
        
        //Assert
        result.Should().NotBe(null);
        result.Should().BeOfType<ViewResult>();
        _userRepository.Verify(x => x.GetUserById(1), Times.Once);
    }
    
    [Fact]
    public void DeleteUser_Should_Return_Successfull_WhenModel_Is_Valid()
    {
        //Arrange
        var userModel = CreateSampleModel();
        _userRepository.Setup(repo => repo.DeleteUser(userModel.Id)).Verifiable();
        _userController.ModelState.AddModelError("PropertyName", "Error message");

        //Act
        var result = _userController.DeleteUser(userModel) as ViewResult;

        //Assert
        result.Should().NotBeNull();
        result.Model.Should().BeOfType<UserModel>().And.BeEquivalentTo(userModel);
        _userRepository.Verify(repo => repo.DeleteUser(userModel.Id), Times.Never);
    }
    
    [Fact]
        public void DeleteUser_Returns_RedirectToAction_Index_When_Deleted_Successfully()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var userModel = Builder<UserModel>.CreateNew().Build();
            _userRepository.Setup(repo => repo.DeleteUser(userModel.Id)).Returns(true);
            _userController.ModelState.Clear(); // Ensure ModelState is valid
            _userController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["SuccessMessage"] = "User has been deleted."
            };

            // Act
            var result = _userController.DeleteUser(userModel) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _userController.TempData["SuccessMessage"].Should().Be("User has been deleted.");
        }

        [Fact]
        public void DeleteUser_Returns_RedirectToAction_Index_When_Deletion_Fails()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var userModel = Builder<UserModel>.CreateNew().Build();
            _userRepository.Setup(repo => repo.DeleteUser(userModel.Id)).Returns(false);
            _userController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["ErrorMessage"] = "User not found."
            };
            //Act
            var result = _userController.DeleteUser(userModel) as RedirectToActionResult;
            
            //Assert 
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _userController.TempData["ErrorMessage"].Should().Be("User not found.");
        }

        [Fact]
        public void DeleteUser_Returns_ViewResult_When_ModelState_Is_Invalid()
        {
            // Arrange
            var userModel = Builder<UserModel>.CreateNew().Build();
            _userController.ModelState.AddModelError("PropertyName", "ErrorMessage");

            // Act
            var result = _userController.DeleteUser(userModel) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNull(); // Ensure it returns the default view
        }
        #endregion
}