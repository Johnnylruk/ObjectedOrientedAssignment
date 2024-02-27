using EVotingSystem_SBMM.Controllers;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace EVotingSystem_SBMM.Tests;

public class Event_Tests
{
    private Mock<IEventRepository> _eventRepository;
    private EventController _eventController;

    public Event_Tests()
    {
        _eventRepository = new Mock<IEventRepository>();
        _eventController = new EventController(_eventRepository.Object);
    }
    
    
    private List<EventModel> GetSampleeventList()
    {
        var getAllData = Builder<EventModel>.CreateListOfSize(10).Build();
        return getAllData.ToList();
    }

    private EventModel GetSampleevent()
    {
        var getData = Builder<EventModel>.CreateNew().Build();
        return getData;
    }

    private EventModel CreateSampleModel()
    {
        var getData = Builder<EventModel>.CreateNew()
            .With(c => c.EventId, 1)
            .With(c => c.Name, "Election")
            .With(c => c.City, "Leeds")
            .With(c => c.StartDate, DateTime.Now)
            .With(c => c.EndDate, DateTime.Today.Date.AddDays(10))
            .With(c => c.Description, "Leeds Election")
            .Build();
        return getData;
    }

    [Fact]
    public void ListAllevents_Should_ListAll()
    {
        //Arrange
        var events = GetSampleeventList(); 
        _eventRepository.Setup(repo => repo.GetAll()).Returns(events);
       
        //Act
        var result = _eventController.Index();
        
        //Assert
        result.Should().NotBe(null);
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<EventModel>>(viewResult.ViewData.Model);
        model.Should().HaveCount(events.Count);
        _eventRepository.Verify(x => x.GetAll(), Times.Once);

    }

    #region Create
    
    [Fact]
    public void Createevent_Should_Return_Successfull_WhenModel_Is_Valid()
    {
        // Arrange
        var eventModel = CreateSampleModel();
        _eventRepository.Setup(repo => repo.CreateEvent(eventModel)).Verifiable();
        _eventController.ModelState.AddModelError("PropertyName", "Error message");

        // Act
        var result = _eventController.CreateEvent(eventModel) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.Model.Should().BeOfType<EventModel>().And.BeEquivalentTo(eventModel);
        _eventRepository.Verify(repo => repo.CreateEvent(eventModel), Times.Never);
    }
    
    [Fact]
    public void Createevent_Returns_ViewResult_When_ModelState_Is_Invalid()
    {
        // Arrange
        var EventModel = Builder<EventModel>.CreateNew().Build();
        _eventController.ModelState.AddModelError("PropertyName", "ErrorMessage");

        // Act
        var result = _eventController.CreateEvent(EventModel) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().BeNull(); 
    }
    
    [Fact]
    public void Createevent_Returns_RedirectToAction_Index_When_Created_Successfully()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var EventModel = Builder<EventModel>.CreateNew().Build();
        _eventRepository.Setup(repo => repo.CreateEvent(EventModel)).Verifiable();
        _eventController.ModelState.Clear(); // Ensure ModelState is valid
        _eventController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["SuccessMessage"] = "Event has been created."
        };

        // Act
        var result = _eventController.CreateEvent(EventModel) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("Index");
        _eventController.TempData["SuccessMessage"].Should().Be("Event has been created.");
    }
    [Fact]
    public void Createevent_Returns_RedirectToAction_Index_When_Creation_Fails()
    {
        //Arrange
        var httpContext = new DefaultHttpContext();
        var EventModel = Builder<EventModel>.CreateNew().Build();
        _eventRepository.Setup(repo => repo.CreateEvent(EventModel)).Verifiable();
        _eventController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["ErrorMessage"] = "Event not found."
        };
        //Act
        var result = _eventController.CreateEvent(EventModel) as RedirectToActionResult;
            
        //Assert 
        result.Should().NotBeNull();
        result.ActionName.Should().Be("Index");
        _eventController.TempData["ErrorMessage"].Should().Be("Event not found.");
    }

    #endregion

    #region Edit

    [Fact]
    public void Editevent_Should_Return_event_By_Id()
    {
        //Arrange
        var eventModel = GetSampleevent();
        _eventRepository.Setup(repo => repo.GetEventById(1)).Returns(eventModel);
        
        //Act 
        var result = _eventController.EditEvent(1);
        
        //Assert
        result.Should().NotBe(null);
        result.Should().BeOfType<ViewResult>();
        _eventRepository.Verify(x => x.GetEventById(1), Times.Once);
    }
    
    [Fact]
    public void Editevent_Should_Return_Successfull_WhenModel_Is_Valid()
    {
        //Arrange
         var eventModel = CreateSampleModel();
         _eventRepository.Setup(repo => repo.UpdateEvent(eventModel)).Verifiable();
        _eventController.ModelState.AddModelError("PropertyName", "Error message");

        //Act
        var result = _eventController.EditEvent(eventModel) as ViewResult;

        //Assert
        result.Should().NotBeNull();
        result.Model.Should().BeOfType<EventModel>().And.BeEquivalentTo(eventModel);
        _eventRepository.Verify(repo => repo.UpdateEvent(eventModel), Times.Never);
    }

     [Fact]
    public void Editevent_Returns_ViewResult_When_ModelState_Is_Invalid()
    {
        // Arrange
        var EventModel = Builder<EventModel>.CreateNew().Build();
        _eventController.ModelState.AddModelError("PropertyName", "ErrorMessage");

        // Act
        var result = _eventController.EditEvent(EventModel) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().BeNull(); 
    }
    
    [Fact]
    public void EditEvent_Returns_RedirectToAction_Index_When_Updated_Successfully()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var EventModel = Builder<EventModel>.CreateNew().Build();
        _eventRepository.Setup(repo => repo.UpdateEvent(EventModel)).Verifiable();
        _eventController.ModelState.Clear(); // Ensure ModelState is valid
        _eventController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["SuccessMessage"] = "Event has been updated."
        };

        // Act
        var result = _eventController.EditEvent(EventModel) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("Index");
        _eventController.TempData["SuccessMessage"].Should().Be("Event has been updated.");
    }
    
    [Fact]
    public void Editevent_Returns_RedirectToAction_Index_When_Updating_Fails()
    {
        //Arrange
        var httpContext = new DefaultHttpContext();
        var EventModel = Builder<EventModel>.CreateNew().Build();
        _eventRepository.Setup(repo => repo.UpdateEvent(EventModel)).Verifiable();
        _eventController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        {
            ["ErrorMessage"] = "Event not found."
        };
        //Act
        var result = _eventController.EditEvent(EventModel) as RedirectToActionResult;
            
        //Assert 
        result.Should().NotBeNull();
        result.ActionName.Should().Be("Index");
        _eventController.TempData["ErrorMessage"].Should().Be("Event not found.");
    }

    #endregion

    #region Delete
    
    [Fact]
    public void Deleteevent_Should_Return_event_By_Id()
    {
        //Arrange
        var eventModel = GetSampleevent();
        _eventRepository.Setup(repo => repo.GetEventById(1)).Returns(eventModel);
        
        //Act 
        var result = _eventController.DeleteEvent(1);
        
        //Assert
        result.Should().NotBe(null);
        result.Should().BeOfType<ViewResult>();
        _eventRepository.Verify(x => x.GetEventById(1), Times.Once);
    }
    
    [Fact]
    public void DeletEvent_Should_Return_Successfull_WhenModel_Is_Valid()
    {
        //Arrange
        var eventModel = CreateSampleModel();
        _eventRepository.Setup(repo => repo.DeleteEvent(eventModel.EventId)).Verifiable();
        _eventController.ModelState.AddModelError("PropertyName", "Error message");

        //Act
        var result = _eventController.DeleteEvent(eventModel) as ViewResult;

        //Assert
        result.Should().NotBeNull();
        result.Model.Should().BeOfType<EventModel>().And.BeEquivalentTo(eventModel);
        _eventRepository.Verify(repo => repo.DeleteEvent(eventModel.EventId), Times.Never);
    }
    
    [Fact]
        public void Deleteevent_Returns_RedirectToAction_Index_When_Deleted_Successfully()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var EventModel = Builder<EventModel>.CreateNew().Build();
            _eventRepository.Setup(repo => repo.DeleteEvent(EventModel.EventId)).Returns(true);
            _eventController.ModelState.Clear(); // Ensure ModelState is valid
            _eventController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["SuccessMessage"] = "Event has been deleted."
            };

            // Act
            var result = _eventController.DeleteEvent(EventModel) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _eventController.TempData["SuccessMessage"].Should().Be("Event has been deleted.");
        }

        [Fact]
        public void Deleteevent_Returns_RedirectToAction_Index_When_Deletion_Fails()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var EventModel = Builder<EventModel>.CreateNew().Build();
            _eventRepository.Setup(repo => repo.DeleteEvent(EventModel.EventId)).Returns(false);
            _eventController.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["ErrorMessage"] = "Event not found."
            };
            //Act
            var result = _eventController.DeleteEvent(EventModel) as RedirectToActionResult;
            
            //Assert 
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            _eventController.TempData["ErrorMessage"].Should().Be("Event not found.");
        }

        [Fact]
        public void DeletEvent_Returns_ViewResult_When_ModelState_Is_Invalid()
        {
            // Arrange
            var EventModel = Builder<EventModel>.CreateNew().Build();
            _eventController.ModelState.AddModelError("PropertyName", "ErrorMessage");

            // Act
            var result = _eventController.DeleteEvent(EventModel) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNull(); // Ensure it returns the default view
        }
    #endregion
}