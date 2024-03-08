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
    
    [Fact]
    public void SubmitVoteFPTP_ShouldReturn_SuccessSubmitVote()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var voter = GetSampleVoter();
        var simpleEvent = GetSampleEvent();
        var candidate = GetSampleCandidate();
        var vote = GetSampleVote();

        _userSession.Setup(repo => repo.GetVoterSession()).Returns(voter);
        _eventRepository.Setup(repo => repo.GetActivityEvent()).Returns(simpleEvent);
        _voteRepository.Setup(repo => repo.SubmitVote(It.IsAny<VoteModel>())).Verifiable(); // Ensure SubmitVote is called
        _voteController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["SuccessMessage"] = "Vote submitted successfully!"
        };
        // Act
        var result = _voteController.SubmitVoteFPTP(candidate.Id);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>(); // Assuming it should return a redirect
        _voteController.TempData["SuccessMessage"].Should().Be("Vote submitted successfully!");
        _voteRepository.Verify(repo => repo.SubmitVote(It.IsAny<VoteModel>()), Times.Once); // Verify SubmitVote is called once
    }

    
}