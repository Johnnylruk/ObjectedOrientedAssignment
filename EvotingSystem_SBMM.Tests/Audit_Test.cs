using EVotingSystem_SBMM.Controllers;
using EVotingSystem_SBMM.Data;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EVotingSystem_SBMM.Tests;

public class Audit_Test
{
    private EVotingSystemDB _evotingSystemDB;
    private Mock<IEventRepository> _eventRepository;
    private AuditController _auditController;

    public Audit_Test()
    {
        // Create an instance of DbContextOptions for in-memory database
        var options = new DbContextOptionsBuilder<EVotingSystemDB>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        // Create an instance of EVotingSystemDB using the DbContextOptions
        _evotingSystemDB = new EVotingSystemDB(options);
        
        _eventRepository = new Mock<IEventRepository>();
        _auditController = new AuditController(_evotingSystemDB, _eventRepository.Object);
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
    
}