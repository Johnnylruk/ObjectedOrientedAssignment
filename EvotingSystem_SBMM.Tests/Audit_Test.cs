using EVotingSystem_SBMM.Controllers;
using EVotingSystem_SBMM.Data;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json.Linq;

namespace EVotingSystem_SBMM.Tests;

public class Audit_Test
{
    private Mock<IEventRepository> _eventRepository;
    private Mock<IVoteRepository> _voteRepository;
    private AuditController _auditController;

    public Audit_Test()
    {
        _eventRepository = new Mock<IEventRepository>();
        _voteRepository = new Mock<IVoteRepository>();
        _auditController = new AuditController(_eventRepository.Object, _voteRepository.Object);
    }

    private List<EventModel> GetSampleEventList()
    {
        return Builder<EventModel>.CreateListOfSize(10).Build().ToList();
    }
    
    private EventModel GetSampleEvent()
    {
        var getData = Builder<EventModel>.CreateNew().Build();
        return getData;
    }
    
    private List<VoteModel> GetSampleVotes()
    {
        // Provide sample votes here
        return Builder<VoteModel>.CreateListOfSize(10).Build().ToList();
    }
    private List<VotePreferenceModel> GetPreferenceSampleVotes()
    {
        // Provide sample votes here
        return Builder<VotePreferenceModel>.CreateListOfSize(10).Build().ToList();
    }
    [Fact]
    public void GetAllEvent_Should_Return_All_To_Index_View_As_Result()
    {
        //Arrange
        var events = GetSampleEventList();
        _eventRepository.Setup(x => x.GetAll()).Returns(events);

        //Act
        var result = _auditController.Index();
    
        //Assert
        result.Should().NotBe(null);
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<EventModel>>(viewResult.ViewData.Model);
        model.Should().HaveCount(events.Count);
        _eventRepository.Verify(repo => repo.GetAll(), Times.Once);
    }

    [Fact]
    public void AuditVotesPage_Should_Return_AuditForEventId()
    {
        //Arrange
        var eventId = GetSampleEvent();
        _eventRepository.Setup(repo => repo.GetEventById(1)).Returns(eventId).Verifiable();

        //Act
        var result = _auditController.AuditVotesPage(1);
        
        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ViewResult>();
    }
    [Fact]
    public void PreferenceVotesPage_Should_Return_AuditForEventId()
    {
        //Arrange
        var eventId = GetSampleEvent();
        _eventRepository.Setup(repo => repo.GetEventById(1)).Returns(eventId).Verifiable();

        //Act
        var result = _auditController.AuditPreferenceVotesPage(1);
        
        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ViewResult>();
    }

 
    [Fact]
    public void GetVotesForAudit_Should_Return_JsonResult_With_Votes()
    {
        // Arrange
        var eventModel = GetSampleEvent();
        var eventId = eventModel.EventId;
        var votes = GetSampleVotes().Where(v => v.EventId == eventId).ToList(); // Filter votes by event ID
        _eventRepository.Setup(repo => repo.GetEventById(eventId)).Returns(eventModel);
        _voteRepository.Setup(repo => repo.GetVotesByEventId(eventId)).Returns(votes);
        // Act
        var result = _auditController.GetVotesForAudit(eventId);

        // Assert
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        var data = jsonResult.Value; 
        data.Should().NotBeNull();
        var dataProperty = data.GetType().GetProperty("data");
        dataProperty.Should().NotBeNull();
        var votesData = dataProperty.GetValue(data) as IEnumerable<object>;
        votesData.Should().HaveCount(votes.Count);
    }  
    [Fact]
    public void GetPreferenceVotesForAudit_Should_Return_JsonResult_With_Votes()
    {
        // Arrange
        var eventModel = GetSampleEvent();
        var eventId = eventModel.EventId;
        var votes = GetPreferenceSampleVotes().Where(v => v.EventId == eventId).ToList(); // Filter votes by event ID
        _eventRepository.Setup(repo => repo.GetEventById(eventId)).Returns(eventModel);
        _voteRepository.Setup(repo => repo.GetPreferenceVotesByEventId(eventId)).Returns(votes);

        // Act
        var result = _auditController.GetPreferencesVoteForAudit(eventId);

        // Assert
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        var data = jsonResult.Value; 
        data.Should().NotBeNull();
        var dataProperty = data.GetType().GetProperty("data");
        dataProperty.Should().NotBeNull();
        var votesData = dataProperty.GetValue(data) as IEnumerable<object>;
        votesData.Should().HaveCount(votes.Count);
    }

}