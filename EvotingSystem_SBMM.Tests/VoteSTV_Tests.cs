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

public class VoteSTV_Tests
{
    
    private readonly Mock<IUserSession>  _userSession;
    private readonly Mock<IVoteRepository>  _voteRepository;
    private readonly Mock<ICandidateRepository>  _candidateRepository;
    private readonly Mock<IEventRepository>  _eventRepository;
    private VoteController _voteController;

    public VoteSTV_Tests()
    {
        _userSession = new Mock<IUserSession>();
        _voteRepository = new Mock<IVoteRepository>();
        _candidateRepository = new Mock<ICandidateRepository>();
        _eventRepository = new Mock<IEventRepository>();
        _voteController = new VoteController(_userSession.Object, _voteRepository.Object, _candidateRepository.Object, _eventRepository.Object);
    }

    private CandidateModel GetSampleCandidate()
    {
        return Builder<CandidateModel>.CreateNew().Build();
    }
    private List<CandidateModel> GetSampleCandidateList()
    {
        return Builder<CandidateModel>.CreateListOfSize(10).Build().ToList();
    }
    private EventModel GetSampleEvent()
    {
        return Builder<EventModel>.CreateNew().Build();
    }

    private VoterModel GetSampleVoter()
    {
        return Builder<VoterModel>.CreateNew().Build(); 
    }
    private VoteModel GetSampleVote()
    {
        return Builder<VoteModel>.CreateNew().Build(); 
    }
    
    private VotePreferenceModel GetSamplePreferenceVote()
    {
        return Builder<VotePreferenceModel>.CreateNew().Build(); 
    } 
    private List<VotePreferenceModel> GetSamplePreferenceVoteList()
    {
        return Builder<VotePreferenceModel>.CreateListOfSize(10).Build().ToList(); 
    }

    #region SubmiteVote

    [Fact]
        public void SubmitVoteSTV_Returns_SuccessfulVoteSubmission()
        {

            // Arrange
            var httpContext = new DefaultHttpContext();
            var voter = GetSampleVoter();
            var eventModel = GetSampleEvent();
            var vote = GetSampleVote();
            vote.Preferences = new List<VotePreferenceModel> { GetSamplePreferenceVote(), GetSamplePreferenceVote() }; // Mock preferences
            var expectedRedirectToAction = new RedirectToActionResult("Index", "AccessVoter", null);
            _voteController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["SuccessMessage"] = "Votes submitted successfully!"
            };

            _userSession.Setup(repo => repo.GetVoterSession()).Returns(voter);
            _eventRepository.Setup(repo => repo.GetEventById(eventModel.EventId)).Returns(eventModel);

            // Act
            var result = _voteController.SubmitVoteSTV(vote, eventModel.EventId) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedRedirectToAction);
            _voteRepository.Verify(repo => repo.SubmitVote(vote), Times.Once);
            _voteController.TempData["SuccessMessage"].Should().Be("Votes submitted successfully!");

        }

        [Fact]
        public void SubmitVoteSTV_Returns_Error_When_RankOrCandidateIdIsNotProvided()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var voter = GetSampleVoter();
            var eventModel = GetSampleEvent();
            var vote = GetSampleVote(); // Mock vote without preferences
            _voteController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["ErrorMessage"] = "Please rank all candidates before submitting your vote."
            };
            
            _userSession.Setup(repo => repo.GetVoterSession()).Returns(voter);
            _eventRepository.Setup(repo => repo.GetEventById(eventModel.EventId)).Returns(eventModel);

            // Act
            var result = _voteController.SubmitVoteSTV(vote, eventModel.EventId) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Error");
            result.ControllerName.Should().Be("Home");
            _voteRepository.Verify(repo => repo.SubmitVote(It.IsAny<VoteModel>()), Times.Never);
            _voteController.TempData["ErrorMessage"].Should().Be("Please rank all candidates before submitting your vote.");
        }
       
        [Fact]
        public void SubmitVoteSTV_ShouldHandle_ErrorSubmittingVote()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var voter = GetSampleVoter();
            var simpleEvent = GetSampleEvent();
            var candidate = GetSampleCandidate();

            _userSession.Setup(repo => repo.GetVoterSession()).Returns(voter);
            _eventRepository.Setup(repo => repo.GetActivityEvent()).Returns(simpleEvent);
            _voteRepository.Setup(repo => repo.SubmitVote(It.IsAny<VoteModel>())).Throws(new Exception("Simulated error"));
            _voteController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["ErrorMessage"] = "Failed to submit vote: Simulated error"
            };
            // Act
            var result = _voteController.SubmitVoteFPTP(candidate.Id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>(); 
            _voteRepository.Verify(repo => repo.SubmitVote(It.IsAny<VoteModel>()), Times.Once); 
            _voteController.TempData["ErrorMessage"].Should().Be("Failed to submit vote: Simulated error"); 
        }
    #endregion

    #region Results

    [Fact]
    public void VotesResultSTV_Returns_Error_When_NoVotes()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var eventModel = GetSampleEvent();
        var candidates = GetSampleCandidateList();
        _candidateRepository.Setup(repo => repo.GetAll()).Returns(candidates);
        _eventRepository.Setup(repo => repo.GetEventById(eventModel.EventId)).Returns(eventModel);
        _voteRepository.Setup(repo => repo.GetAllVotesPreferential()).Returns(new List<VotePreferenceModel>());
        _voteController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["ErrorMessage"] = "There are no votes to display results"
        };
        // Act
        var result = _voteController.VotesResultSTV(eventModel.EventId) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("Index");
        result.ControllerName.Should().Be("Event");
        _voteController.TempData["ErrorMessage"].Should().Be("There are no votes to display results");
    }

    [Fact]
    public void VotesResultSTV_Returns_View_WithVoteResults()
    {
        // Arrange
        var eventModel = GetSampleEvent();
        var httpContext = new DefaultHttpContext();
        var candidates = GetSampleCandidateList;
        var votePreferences = GetSamplePreferenceVoteList();
        _eventRepository.Setup(repo => repo.GetEventById(eventModel.EventId)).Returns(eventModel);
        _candidateRepository.Setup(repo => repo.GetAll()).Returns(candidates);
        _voteRepository.Setup(repo => repo.GetAllVotesPreferential()).Returns(votePreferences);

        // Act
        var result = _voteController.VotesResultSTV(eventModel.EventId) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be("IndexResultsSTV");
    }
    #endregion
}