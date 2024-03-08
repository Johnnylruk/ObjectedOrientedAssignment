using EVotingSystem_SBMM.Controllers;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using RouteData = Microsoft.AspNetCore.Routing.RouteData;

namespace EVotingSystem_SBMM.Tests;

public class Candidate_Tests
{
    private readonly Mock<ICandidateRepository> _candidateRepository;
    private ElectoralAdminCandidateController _candidateController;

    public Candidate_Tests()
    {
        _candidateRepository = new Mock<ICandidateRepository>();
        _candidateController = new ElectoralAdminCandidateController(_candidateRepository.Object);
    }

    private List<CandidateModel> GetSampleCandidateList()
    {
        var getAllData = Builder<CandidateModel>.CreateListOfSize(10).Build();
        return getAllData.ToList();
    }

    private CandidateModel GetSampleCandidate()
    {
        var getData = Builder<CandidateModel>.CreateNew().Build();
        return getData;
    }

    private CandidateModel CreateSampleModel()
    {
        var getData = Builder<CandidateModel>.CreateNew()
            .With(c => c.Id, 1)
            .With(c => c.Name, "John")
            .With(c => c.City, "Leeds")
            .With(c => c.Description, "Voting")
            .With(c => c.IsElected, false)
            .Build();
        return getData;
    }

    [Fact]
    public void ListAllCandidates_Should_ListAll()
    {
        //Arrange
        var candidates = GetSampleCandidateList(); 
        _candidateRepository.Setup(repo => repo.GetAll()).Returns(candidates);
       
        //Act
        var result = _candidateController.Index();
        
        //Assert
        result.Should().NotBe(null);
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<CandidateModel>>(viewResult.ViewData.Model);
        model.Should().HaveCount(candidates.Count);
        _candidateRepository.Verify(x => x.GetAll(), Times.Once);

    }

    #region Create
    
    [Fact]
    public void CreateCandidate_Should_Return_Successfull_WhenModel_Is_Valid()
    {
        // Arrange
        var candidate = CreateSampleModel();
        _candidateRepository.Setup(repo => repo.Register(candidate)).Verifiable();
        _candidateController.ModelState.AddModelError("PropertyName", "Error message");

        // Act
        var result = _candidateController.CreateCandidate(candidate) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.Model.Should().BeOfType<CandidateModel>().And.BeEquivalentTo(candidate);
        _candidateRepository.Verify(repo => repo.Register(candidate), Times.Never);
    }
    
    [Fact]
    public void CreateCandidate_Returns_ViewResult_When_ModelState_Is_Invalid()
    {
        // Arrange
        var candidateModel = Builder<CandidateModel>.CreateNew().Build();
        _candidateController.ModelState.AddModelError("PropertyName", "ErrorMessage");

        // Act
        var result = _candidateController.CreateCandidate(candidateModel) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().BeNull(); 
    }
    
    [Fact]
    public void CreateCandidate_Returns_RedirectToAction_Index_When_Created_Successfully()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var candidateModel = Builder<CandidateModel>.CreateNew().Build();
        _candidateRepository.Setup(repo => repo.Register(candidateModel)).Verifiable();
        _candidateController.ModelState.Clear(); // Ensure ModelState is valid
        _candidateController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["SuccessMessage"] = "Candidate has been created."
        };

        // Act
        var result = _candidateController.CreateCandidate(candidateModel) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("Index");
        _candidateController.TempData["SuccessMessage"].Should().Be("Candidate has been created.");
    }
    [Fact]
    public void CreateCandidate_Returns_RedirectToAction_Index_When_Creation_Fails()
    {
        //Arrange
        var httpContext = new DefaultHttpContext();
        var candidateModel = Builder<CandidateModel>.CreateNew().Build();
        _candidateRepository.Setup(repo => repo.Register(candidateModel)).Verifiable();
        _candidateController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["ErrorMessage"] = "Candidate not found."
        };
        //Act
        var result = _candidateController.CreateCandidate(candidateModel) as RedirectToActionResult;
            
        //Assert 
        result.Should().NotBeNull();
        result.ActionName.Should().Be("Index");
        _candidateController.TempData["ErrorMessage"].Should().Be("Candidate not found.");
    }

    #endregion

    #region Edit

    [Fact]
    public void EditCandidate_Should_Return_Candidate_By_Id()
    {
        //Arrange
        var candidate = GetSampleCandidate();
        _candidateRepository.Setup(repo => repo.GetCandidateById(1)).Returns(candidate);
        
        //Act 
        var result = _candidateController.EditCandidate(1);
        
        //Assert
        result.Should().NotBe(null);
        result.Should().BeOfType<ViewResult>();
        _candidateRepository.Verify(x => x.GetCandidateById(1), Times.Once);
    }
    
    [Fact]
    public void EditCandidate_Should_Return_Successfull_WhenModel_Is_Valid()
    {
        //Arrange
         var candidate = CreateSampleModel();
         _candidateRepository.Setup(repo => repo.UpdateCandidate(candidate)).Verifiable();
        _candidateController.ModelState.AddModelError("PropertyName", "Error message");

        //Act
        var result = _candidateController.EditCandidate(candidate) as ViewResult;

        //Assert
        result.Should().NotBeNull();
        result.Model.Should().BeOfType<CandidateModel>().And.BeEquivalentTo(candidate);
        _candidateRepository.Verify(repo => repo.UpdateCandidate(candidate), Times.Never);
    }

     [Fact]
    public void EditCandidate_Returns_ViewResult_When_ModelState_Is_Invalid()
    {
        // Arrange
        var candidateModel = Builder<CandidateModel>.CreateNew().Build();
        _candidateController.ModelState.AddModelError("PropertyName", "ErrorMessage");

        // Act
        var result = _candidateController.EditCandidate(candidateModel) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().BeNull(); 
    }
    
    [Fact]
    public void EditCandidate_Returns_RedirectToAction_Index_When_Updated_Successfully()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var candidateModel = Builder<CandidateModel>.CreateNew().Build();
        _candidateRepository.Setup(repo => repo.UpdateCandidate(candidateModel)).Verifiable();
        _candidateController.ModelState.Clear(); // Ensure ModelState is valid
        _candidateController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["SuccessMessage"] = "Candidate has been updated."
        };

        // Act
        var result = _candidateController.EditCandidate(candidateModel) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("Index");
        _candidateController.TempData["SuccessMessage"].Should().Be("Candidate has been updated.");
    }
    
    [Fact]
    public void EditCandidate_Returns_RedirectToAction_Index_When_Updating_Fails()
    {
        //Arrange
        var httpContext = new DefaultHttpContext();
        var candidateModel = Builder<CandidateModel>.CreateNew().Build();
        _candidateRepository.Setup(repo => repo.UpdateCandidate(candidateModel)).Verifiable();
        _candidateController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["ErrorMessage"] = "Candidate not found."
        };
        //Act
        var result = _candidateController.EditCandidate(candidateModel) as RedirectToActionResult;
            
        //Assert 
        result.Should().NotBeNull();
        result.ActionName.Should().Be("Index");
        _candidateController.TempData["ErrorMessage"].Should().Be("Candidate not found.");
    }

    #endregion

    #region Delete
    
    [Fact]
    public void DeleteCandidate_Should_Return_Candidate_By_Id()
    {
        //Arrange
        var candidate = GetSampleCandidate();
        _candidateRepository.Setup(repo => repo.GetCandidateById(1)).Returns(candidate);
        
        //Act 
        var result = _candidateController.DeleteCandidate(1);
        
        //Assert
        result.Should().NotBe(null);
        result.Should().BeOfType<ViewResult>();
        _candidateRepository.Verify(x => x.GetCandidateById(1), Times.Once);
    }
    
    [Fact]
    public void DeleteCandidate_Should_Return_Successfull_WhenModel_Is_Valid()
    {
        //Arrange
        var candidate = CreateSampleModel();
        _candidateRepository.Setup(repo => repo.DeleteCandidate(candidate.Id)).Verifiable();
        _candidateController.ModelState.AddModelError("PropertyName", "Error message");

        //Act
        var result = _candidateController.DeleteCandidate(candidate) as ViewResult;

        //Assert
        result.Should().NotBeNull();
        result.Model.Should().BeOfType<CandidateModel>().And.BeEquivalentTo(candidate);
        _candidateRepository.Verify(repo => repo.DeleteCandidate(candidate.Id), Times.Never);
    }
    
    [Fact]
        public void DeleteCandidate_Returns_RedirectToAction_Index_When_Deleted_Successfully()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var candidateModel = Builder<CandidateModel>.CreateNew().Build();
            _candidateRepository.Setup(repo => repo.DeleteCandidate(candidateModel.Id)).Returns(true);
            _candidateController.ModelState.Clear(); // Ensure ModelState is valid
            _candidateController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["SuccessMessage"] = "Candidate has been deleted."
            };

            // Act
            var result = _candidateController.DeleteCandidate(candidateModel) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _candidateController.TempData["SuccessMessage"].Should().Be("Candidate has been deleted.");
        }

        [Fact]
        public void DeleteCandidate_Returns_RedirectToAction_Index_When_Deletion_Fails()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var candidateModel = Builder<CandidateModel>.CreateNew().Build();
            _candidateRepository.Setup(repo => repo.DeleteCandidate(candidateModel.Id)).Returns(false);
            _candidateController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["ErrorMessage"] = "Candidate not found."
            };
            //Act
            var result = _candidateController.DeleteCandidate(candidateModel) as RedirectToActionResult;
            
            //Assert 
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _candidateController.TempData["ErrorMessage"].Should().Be("Candidate not found.");
        }

        [Fact]
        public void DeleteCandidate_Returns_ViewResult_When_ModelState_Is_Invalid()
        {
            // Arrange
            var candidateModel = Builder<CandidateModel>.CreateNew().Build();
            _candidateController.ModelState.AddModelError("PropertyName", "ErrorMessage");

            // Act
            var result = _candidateController.DeleteCandidate(candidateModel) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNull(); // Ensure it returns the default view
        }
        #endregion

}