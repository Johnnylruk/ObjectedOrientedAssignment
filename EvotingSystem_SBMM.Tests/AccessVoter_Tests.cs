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

public class AccessVoter_Tests
{
    private readonly Mock<IVotersRepository> _votersRepository;
    private readonly Mock<IUserSession>  _userSession;
    private readonly Mock<IVoteRepository>  _voteRepository;
    private readonly Mock<ICandidateRepository>  _candidateRepository;
    private readonly Mock<IEventRepository>  _eventRepository;
    private AccessVoterController _accessVoterController;
    
    public AccessVoter_Tests()
    {
        _votersRepository = new Mock<IVotersRepository>();
        _candidateRepository = new Mock<ICandidateRepository>();
        _eventRepository = new Mock<IEventRepository>();
        _userSession = new Mock<IUserSession>();
        _voteRepository = new Mock<IVoteRepository>();
        _accessVoterController = new AccessVoterController(_votersRepository.Object, _candidateRepository.Object,
            _eventRepository.Object, _userSession.Object,
            _voteRepository.Object);
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
    private List<EventModel> GetSampleEventList()
    {
        return Builder<EventModel>.CreateListOfSize(10).Build().ToList();
    } 
    private List<VoterModel> GetSampleVotersList()
    {
        return Builder<VoterModel>.CreateListOfSize(10).Build().ToList();
    }
    private VoterModel GetSampleVoter()
    {
        return Builder<VoterModel>.CreateNew().Build(); 
    }
  

    #region AccessBallot

    [Fact]
    public void AccessBallot_ShouldReturn_BallotSTV_View_When_EventActive_And_NotVoted()
    {
        // Arrange
        var candidate = GetSampleCandidate();
        var voter = GetSampleVoter();
        var simpleEvent = GetSampleEvent();
        var candidates = GetSampleCandidateList();
        var voterCity = "SampleCity";

        _eventRepository.Setup(repo => repo.GetEventById(simpleEvent.EventId)).Returns(simpleEvent);
        _votersRepository.Setup(repo => repo.GetVoterCity()).Returns(voterCity);
        _userSession.Setup(repo => repo.GetVoterSession()).Returns(voter);
        _candidateRepository.Setup(repo => repo.GetAll()).Returns(candidates);
        _voteRepository.Setup(repo => repo.HasVoted(voter.Id, simpleEvent.EventId)).Returns(false);

        // Act
        var result = _accessVoterController.Ballot(simpleEvent.EventId, candidate.Id);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.ViewName.Should().Be("BallotSTV");
    }
    [Fact]
    public void Ballot_Returns_RedirectToAction_Index_Home_When_EventIsNull()
    {
        // Arrange
        var eventId = 1;
        EventModel eventModel = null;
        _eventRepository.Setup(repo => repo.GetEventById(eventId)).Returns(eventModel);

        // Act
        var result = _accessVoterController.Ballot(eventId, 1);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Index");
    }

    [Fact]
    public void Ballot_Returns_Index_View_When_CandidatesOrVoterCityIsNull()
    {
        // Arrange
        var eventId = 1;
        var eventModel = new EventModel { EventId = eventId };
        _eventRepository.Setup(repo => repo.GetEventById(eventId)).Returns(eventModel);
        List<CandidateModel> candidates = null;
        string voterCity = null;
        _candidateRepository.Setup(repo => repo.GetAll()).Returns(candidates);
        _votersRepository.Setup(repo => repo.GetVoterCity()).Returns(voterCity);

        // Act
        var result = _accessVoterController.Ballot(eventId, 1);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.ViewName.Should().Be("Index");
    }
    [Fact]
    public void Ballot_Returns_BallotFPTP_View_When_EventIsFPTPAndHasNotVoted()
    {
        // Arrange
        var eventId = 1;
        var candidateId = 1;
        var eventModel = new EventModel { EventId = eventId, EventType = EventTypeEnum.FPTP, City = "City" };
        _eventRepository.Setup(repo => repo.GetEventById(eventId)).Returns(eventModel);
        var candidates = new List<CandidateModel> { new CandidateModel { Id = 1, City = "City" } };
        _candidateRepository.Setup(repo => repo.GetAll()).Returns(candidates);
        _votersRepository.Setup(repo => repo.GetVoterCity()).Returns("City");
        _userSession.Setup(repo => repo.GetVoterSession()).Returns(new VoterModel { Id = 1 });
        _voteRepository.Setup(repo => repo.HasVoted(1, eventId)).Returns(false);

        // Act
        var result = _accessVoterController.Ballot(eventId, candidateId);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.ViewName.Should().Be("BallotFPTP");
    }

    [Fact]
    public void Ballot_Returns_Index_View_When_EventIsFPTPAndHasAlreadyVoted()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var eventId = 1;
        var candidateId = 1;
        var eventModel = new EventModel { EventId = eventId, EventType = EventTypeEnum.FPTP, City = "City" };
        _eventRepository.Setup(repo => repo.GetEventById(eventId)).Returns(eventModel);
        var candidates = new List<CandidateModel> { new CandidateModel { Id = 1, City = "City" } };
        _candidateRepository.Setup(repo => repo.GetAll()).Returns(candidates);
        _votersRepository.Setup(repo => repo.GetVoterCity()).Returns("City");
        _userSession.Setup(repo => repo.GetVoterSession()).Returns(new VoterModel { Id = 1 });
        _voteRepository.Setup(repo => repo.HasVoted(1, eventId)).Returns(true);
        _accessVoterController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["ErrorMessage"] = "There is no available event for you or you have voted in this event before."
        };
        // Act
        var result = _accessVoterController.Ballot(eventId, candidateId);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.ViewName.Should().Be("Index");
        _accessVoterController.TempData["ErrorMessage"].Should().Be("There is no available event for you or you have voted in this event before."); 
    }
    #endregion

    #region AccessingViews

    [Fact]
    public void Register_ShouldReturn_View()
    {
        // Act
        var result = _accessVoterController.Register();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void UpdateVoter_ShouldReturn_View()
    {
        // Arrange
        var voterId = 1;
        var voterModel = new VoterModel { Id = voterId };
        _userSession.Setup(repo => repo.GetVoterSession()).Returns(new VoterModel { Id = voterId });
        _votersRepository.Setup(repo => repo.GetVoterById(voterId)).Returns(voterModel);

        // Act
        var result = _accessVoterController.UpdateVoter(voterId);

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void VoteStatus_ShouldReturn_View()
    {
        // Arrange
        var voterId = 1;
        var allEvents = GetSampleEventList();
        _userSession.Setup(repo => repo.GetVoterSession()).Returns(new VoterModel { Id = voterId });
        _eventRepository.Setup(repo => repo.GetAll()).Returns(allEvents);

        // For each event, setup HasVoted to return true for the specified voterId
        foreach (var ev in allEvents)
        {
            _voteRepository.Setup(repo => repo.HasVoted(voterId, ev.EventId)).Returns(true);
        }
        // Act
        var result = _accessVoterController.VoteStatus();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }
    #endregion

    [Fact]
    public void RequestRegister_ShouldReturn_BirthDayError_WhenUserCannotVote()
    {
        // Arrange
        var voters = GetSampleVotersList();
        var newVoter = GetSampleVoter();
        newVoter.BirthDate = DateTime.Now.AddYears(-14);
        _votersRepository.Setup(repo => repo.GetAll()).Returns(voters);

        var hashedPassport = Cryptography.GenerateHash(newVoter.Passport);
        newVoter.Passport = hashedPassport;

        var hashedPassportsInDatabase = voters.Select(v => Cryptography.GenerateHash(v.Passport)).ToList();

        // Act
        var result = _accessVoterController.RequestRegister(newVoter) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be("Register");
        result.ViewData["BirthDateError"].Should().Be("Voter is not older enough to vote.");
    }
    [Fact]
    public void RequestRegister_ShouldReturn_LoginError_WhenLoginDuplicate()
    {
        // Arrange
        var voters = GetSampleVotersList();
        var newVoter = GetSampleVoter();
        newVoter.BirthDate = DateTime.Now.AddYears(-20);
        newVoter.Mobile = "123456554";
        _votersRepository.Setup(repo => repo.GetAll()).Returns(voters);

        // Act
        var result = _accessVoterController.RequestRegister(newVoter) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be("Register");
        result.ViewData["LoginError"].Should().Be("Login is already registered.");
    }


}
    