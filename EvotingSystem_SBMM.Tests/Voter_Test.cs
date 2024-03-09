using EVotingSystem_SBMM.Controllers;
using EVotingSystem_SBMM.Enums;
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

public class Voter_Test
{
    private readonly Mock<IVotersRepository> _voterRepository;
    private readonly Mock<IEmail> _email;
    private ElectoralAdminVoterController _voterController;
    private Email email;

    public Voter_Test()
    {
        _voterRepository = new Mock<IVotersRepository>();
        _email = new Mock<IEmail>();
        _voterController = new ElectoralAdminVoterController(_voterRepository.Object, _email.Object);

    }
    private List<VoterModel> GetSampleNotPendingVoterList()
    {
        return Builder<VoterModel>.CreateListOfSize(10)
            .All().With(v => v.IsPending = false)
            .Build().ToList();
    }
    private List<VoterModel> GetSampleIsPendingVoterList()
    {
        return Builder<VoterModel>.CreateListOfSize(10)
            .All().With(v => v.IsPending)
            .Build().ToList();
    }
    private VoterModel GetSampleVoter()
    {
        var getData = Builder<VoterModel>.CreateNew().Build();
        return getData;
    }

    private VoterModel CreateSampleModel()
    {
        var getData = Builder<VoterModel>.CreateNew()
            .With(c => c.Id, 1)
            .With(c => c.Name, "John")
            .With(c => c.City, "Leeds")
            .With(c => c.Address, "Somewhere in Leeds")
            .With(c => c.IsPending, false)
            .With(c => c.Email, "algumacoisa@gmail.com")
            .With(c => c.Login, "algumacoisa")
            .With(c => c.Passport, "123456")
            .With(c => c.Password, "Test@123")
            .With(c => c.Mobile, "2345878")
            .With(c => c.Profile, ProfileEnum.Voter)
            .With(c => c.BirthDate, DateTime.Now.AddYears(-16))
            .With(c => c.IsPending, true)
            .Build();
        return getData;
    }

    [Fact]
    public void List_All_Not_Pending_Voters_Should_ListAll()
    {
        //Arrange
        var voters = GetSampleNotPendingVoterList(); 
        _voterRepository.Setup(repo => repo.GetAll()).Returns(voters);
       
        //Act
        var result = _voterController.Index();
        
        //Assert
        result.Should().NotBeNull().And.BeOfType<ViewResult>(); 
        var viewResult = result.Should().BeOfType<ViewResult>().Subject; 
         viewResult.Model.Should().BeOfType<List<VoterModel>>(); 
         var model = viewResult.Model.As<List<VoterModel>>(); 
         model.Should().HaveCount(voters.Count); 
         _voterRepository.Verify(x => x.GetAll(), Times.Once); // 
    }
    
    [Fact]
    public void List_All_Pending_Voters_Should_ListAll()
    {
        //Arrange
        var voters = GetSampleIsPendingVoterList(); 
        _voterRepository.Setup(repo => repo.GetAll()).Returns(voters);
       
        //Act
        var result = _voterController.PendingVoters();
        
        //Assert
        result.Should().NotBeNull().And.BeOfType<ViewResult>(); 
        var viewResult = result.Should().BeOfType<ViewResult>().Subject; 
        viewResult.Model.Should().BeOfType<List<VoterModel>>(); 
        var model = viewResult.Model.As<List<VoterModel>>(); 
        model.Should().NotHaveSameCount(voters); 
        _voterRepository.Verify(x => x.GetAll(), Times.Once); // 
    }
    #region RequestsToApprove
    
    [Fact]
    public void ApproveVoter_Should_Return_Successfull_When_Request_Approved()
    {
            // Arrange
            var voterId = 1; // Set the voter ID for testing purposes
            var voter = CreateSampleModel();
            var httpContext = new DefaultHttpContext();
            _voterRepository.Setup(repo => repo.GetVoterById(voterId)).Returns(voter); // Setup the repository mock to return the sample voter when GetVoterById is called
            _voterRepository.Setup(repo => repo.ApproveVoterRequest(voter)).Returns(voter);
            var emailMessage = "Your voter request has been approved";
            _voterController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["SuccessMessage"] = "Voter has been approved."
            };
            
            // Act
            var result = _voterController.ApproveVoter(voter, voterId);

            // Assert
            result.Should().NotBeNull(); // Ensure that the result is not null
            _voterRepository.Verify(repo => repo.ApproveVoterRequest(voter), Times.Once);
            _email.Verify(e => e.SendEmailLink(voter.Email, "EVoting System SBMM -", emailMessage), Times.Once);
            _voterController.TempData["SuccessMessage"].Should().Be("Voter has been approved.");

    }

    [Fact] public void ApproveVoter_Returns_ViewResult_When_Voter_Is_Null()
    {
        // Arrange
        var voterId = 1; // Set the voter ID for testing purposes
        VoterModel voter = null;
        var httpContext = new DefaultHttpContext();
        _voterRepository.Setup(repo => repo.GetVoterById(voterId)).Returns(voter); // Setup the repository mock to return the sample voter when GetVoterById is called
        _voterController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["ErrorMessage"] = "Ops, could not find a voter."
        };
        // Act
        var result = _voterController.ApproveVoter(voter, voterId);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>(); 
        var redirectToActionResult = result as RedirectToActionResult;
        redirectToActionResult.ActionName.Should().Be("Index");
        _voterController.TempData["ErrorMessage"].Should().Be("Ops, could not find a voter.");

    }
 [Fact]
    public void RefuseVoter_Should_Return_Successfull_When_Request_Refused()
    {
            // Arrange
            var voterId = 1; // Set the voter ID for testing purposes
            var voter = CreateSampleModel();
            var httpContext = new DefaultHttpContext();
            _voterRepository.Setup(repo => repo.GetVoterById(voterId)).Returns(voter); // Setup the repository mock to return the sample voter when GetVoterById is called
            _voterRepository.Setup(repo => repo.DenyVoterRequest(voter)).Returns(voter);
            var emailMessage = "Your voter request has been refused";
            _voterController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["SuccessMessage"] = "Voter has been refused and deleted from database."
            };
            
            // Act
            var result = _voterController.RefuseVoter(voter, voterId);

            // Assert
            result.Should().NotBeNull(); // Ensure that the result is not null
            _voterRepository.Verify(repo => repo.DenyVoterRequest(voter), Times.Once);
            _voterController.TempData["SuccessMessage"].Should().Be("Voter has been refused and deleted from database.");
            _email.Verify(e => e.SendEmailLink(voter.Email, "EVoting System SBMM -", emailMessage), Times.Once);

    }

    [Fact] public void RefuseVoter_Returns_ViewResult_When_Voter_Is_Null()
    {
        // Arrange
        var voterId = 1; // Set the voter ID for testing purposes
        VoterModel voter = null;
        var httpContext = new DefaultHttpContext();
        _voterRepository.Setup(repo => repo.GetVoterById(voterId)).Returns(voter); 
        _voterController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["ErrorMessage"] = "Ops, could not find a voter."
        };
        // Act
        var result = _voterController.RefuseVoter(voter, voterId);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>(); 
        var redirectToActionResult = result as RedirectToActionResult;
        redirectToActionResult.ActionName.Should().Be("Index");
        _voterController.TempData["ErrorMessage"].Should().Be("Ops, could not find a voter.");

    }
    #endregion
    #region Edit

    [Fact]
    public void EditVoter_Should_Return_Voter_By_Id()
    {
        //Arrange
        var Voter = GetSampleVoter();
        _voterRepository.Setup(repo => repo.GetVoterById(1)).Returns(Voter);
        
        //Act 
        var result = _voterController.EditVoter(1);
        
        //Assert
        result.Should().NotBe(null);
        result.Should().BeOfType<ViewResult>();
        _voterRepository.Verify(x => x.GetVoterById(1), Times.Once);
    }
    
    [Fact]
    public void EditVoter_Should_Return_Successfull_WhenModel_Is_Valid()
    {
        //Arrange
         var voter = CreateSampleModel();
         _voterRepository.Setup(repo => repo.UpdateVoter(voter)).Verifiable();
        _voterController.ModelState.AddModelError("PropertyName", "Error message");

        //Act
        var result = _voterController.EditVoter(voter) as ViewResult;

        //Assert
        result.Should().NotBeNull();
        result.Model.Should().BeOfType<VoterModel>().And.BeEquivalentTo(voter);
        _voterRepository.Verify(repo => repo.UpdateVoter(voter), Times.Never);
    }
     [Fact]
    public void EditVoter_Returns_ViewResult_When_ModelState_Is_Invalid()
    {
        // Arrange
        var VoterModel = Builder<VoterModel>.CreateNew().Build();
        _voterController.ModelState.AddModelError("PropertyName", "ErrorMessage");

        // Act
        var result = _voterController.EditVoter(VoterModel) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().BeNull(); 
    }

      [Fact]
      public void EditVoter_Returns_RedirectToAction_Index_When_Updated_Successfully()
      {
          // Arrange
          var httpContext = new DefaultHttpContext();
          var VoterModel = Builder<VoterModel>.CreateNew().Build();
          _voterRepository.Setup(repo => repo.UpdateVoter(VoterModel)).Verifiable();
          _voterController.ModelState.Clear(); // Ensure ModelState is valid
          _voterController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
          {
              ["SuccessMessage"] = "Voter has been updated."
          };

          // Act
          var result = _voterController.EditVoter(VoterModel) as RedirectToActionResult;

          // Assert
          result.Should().NotBeNull();
          result.ActionName.Should().Be("Index");
          _voterController.TempData["SuccessMessage"].Should().Be("Voter has been updated.");
      }

        [Fact]
        public void EditVoter_Returns_RedirectToAction_Index_When_Updating_Fails()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var VoterModel = Builder<VoterModel>.CreateNew().Build();
            _voterRepository.Setup(repo => repo.UpdateVoter(VoterModel)).Verifiable();
            _voterController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["ErrorMessage"] = "Voter not found."
            };
            //Act
            var result = _voterController.EditVoter(VoterModel) as RedirectToActionResult;

            //Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _voterController.TempData["ErrorMessage"].Should().Be("Voter not found.");
        }

          #endregion

          #region Delete

          [Fact]
          public void DeleteVoter_Should_Return_Voter_By_Id()
          {
              //Arrange
              var Voter = GetSampleVoter();
              _voterRepository.Setup(repo => repo.GetVoterById(1)).Returns(Voter);

              //Act
              var result = _voterController.DeleteVoter(1);

              //Assert
              result.Should().NotBe(null);
              result.Should().BeOfType<ViewResult>();
              _voterRepository.Verify(x => x.GetVoterById(1), Times.Once);
          }

            [Fact]
            public void DeleteVoter_Should_Return_Successfull_WhenModel_Is_Valid()
            {
                //Arrange
                var Voter = CreateSampleModel();
                _voterRepository.Setup(repo => repo.DeleteVoter(Voter.Id)).Verifiable();
                _voterController.ModelState.AddModelError("PropertyName", "Error message");

                //Act
                var result = _voterController.DeleteVoter(Voter) as ViewResult;

                //Assert
                result.Should().NotBeNull();
                result.Model.Should().BeOfType<VoterModel>().And.BeEquivalentTo(Voter);
                _voterRepository.Verify(repo => repo.DeleteVoter(Voter.Id), Times.Never);
            }

        [Fact]
            public void DeleteVoter_Returns_RedirectToAction_Index_When_Deleted_Successfully()
          {
              // Arrange
              var httpContext = new DefaultHttpContext();
              var VoterModel = Builder<VoterModel>.CreateNew().Build();
              _voterRepository.Setup(repo => repo.DeleteVoter(VoterModel.Id)).Returns(true);
              _voterController.ModelState.Clear(); // Ensure ModelState is valid
              _voterController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
              {
                  ["SuccessMessage"] = "Voter has been deleted."
              };

              // Act
              var result = _voterController.DeleteVoter(VoterModel) as RedirectToActionResult;

              // Assert
              result.Should().NotBeNull();
              result.ActionName.Should().Be("Index");
              _voterController.TempData["SuccessMessage"].Should().Be("Voter has been deleted.");
          }

                [Fact]
                public void DeleteVoter_Returns_RedirectToAction_Index_When_Deletion_Fails()
                {
                    //Arrange
                    var httpContext = new DefaultHttpContext();
                    var VoterModel = Builder<VoterModel>.CreateNew().Build();
                    _voterRepository.Setup(repo => repo.DeleteVoter(VoterModel.Id)).Returns(false);
                    _voterController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
                    {
                        ["ErrorMessage"] = "Voter not found."
                    };
                    //Act
                    var result = _voterController.DeleteVoter(VoterModel) as RedirectToActionResult;

                    //Assert
                    result.Should().NotBeNull();
                    result.ActionName.Should().Be("Index");
                    _voterController.TempData["ErrorMessage"].Should().Be("Voter not found.");
                }

                      [Fact]
                      public void DeleteVoter_Returns_ViewResult_When_ModelState_Is_Invalid()
                      {
                          // Arrange
                          var VoterModel = Builder<VoterModel>.CreateNew().Build();
                          _voterController.ModelState.AddModelError("PropertyName", "ErrorMessage");

                          // Act
                          var result = _voterController.DeleteVoter(VoterModel) as ViewResult;

                          // Assert
                          result.Should().NotBeNull();
                          result.ViewName.Should().BeNull(); // Ensure it returns the default view
                      }
                      #endregion

}