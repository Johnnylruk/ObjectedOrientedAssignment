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

public class VoteFPTP_Tests
{
    private readonly Mock<IUserSession>  _userSession;
    private readonly Mock<IVoteRepository>  _voteRepository;
    private readonly Mock<ICandidateRepository>  _candidateRepository;
    private readonly Mock<IEventRepository>  _eventRepository;
    private VoteController _voteController;

    public VoteFPTP_Tests()
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

    #region SubmiteVote

    [Fact]
    public void SubmitVoteFPTP_ShouldReturn_SuccessSubmitVote()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var voter = GetSampleVoter();
        var simpleEvent = GetSampleEvent();
        var candidate = GetSampleCandidate();

        _userSession.Setup(repo => repo.GetVoterSession()).Returns(voter);
        _eventRepository.Setup(repo => repo.GetActivityEvent()).Returns(simpleEvent);
        _voteRepository.Setup(repo => repo.SubmitVote(It.IsAny<VoteModel>())).Verifiable();
        _voteController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["SuccessMessage"] = "Vote submitted successfully!"
        };
        // Act
        var result = _voteController.SubmitVoteFPTP(candidate.Id);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>(); 
        _voteController.TempData["SuccessMessage"].Should().Be("Vote submitted successfully!");
        _voteRepository.Verify(repo => repo.SubmitVote(It.IsAny<VoteModel>()), Times.Once); // Verify SubmitVote is called once
    }
    
    [Fact]
    public void SubmitVoteFPTP_ShouldHandle_ErrorSubmittingVote()
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
    public void ResultFPTP_Returns_ErrorMessage_When_NoVotes()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var eventModel = GetSampleEvent();
        var candidates = GetSampleCandidateList;
        
        _eventRepository.Setup(repo => repo.GetEventById(eventModel.EventId)).Returns(eventModel);
        _candidateRepository.Setup(repo => repo.GetAll()).Returns(candidates);
        _voteRepository.Setup(repo => repo.GetVoteCountForCandidate(It.IsAny<int>(), eventModel.EventId)).Returns(0);
        _voteController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

        // Act
        var result = _voteController.VotesResultFPTP(eventModel.EventId);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        _voteController.TempData["ErrorMessage"].Should().Be("There are no votes to display results");
    }
    [Fact]
    public void ResultFPTP_Returns_Results_When_VotesExist()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var eventModel = GetSampleEvent();
        var candidates = GetSampleCandidateList;
        var voteCounts = new Dictionary<int, int> { { 1, 10 }, { 2, 5 }, { 3, 3 } };

        _eventRepository.Setup(repo => repo.GetEventById(eventModel.EventId)).Returns(eventModel);
        _candidateRepository.Setup(repo => repo.GetAll()).Returns(candidates);
        _voteRepository.Setup(repo => repo.GetVoteCountForCandidate(It.IsAny<int>(), eventModel.EventId))
            .Returns((int candidateId, int eventId) => voteCounts.ContainsKey(candidateId) ? voteCounts[candidateId] : 0);
        _voteController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

        // Act
        var result = _voteController.VotesResultFPTP(eventModel.EventId) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be("IndexResultsFPTP");
    }
    #endregion
    

    
}