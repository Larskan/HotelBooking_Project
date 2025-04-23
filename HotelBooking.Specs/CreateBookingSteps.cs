//using Reqnroll;
//using HotelBooking.Core;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;
////using TechTalk.SpecFlow;

//namespace HotelBooking.Specs
//{
//    [Binding]
//    public class CreateBookingSteps
//    {
//        #region Globals
//        private readonly ITestOutputHelper _outputHelper;
//        private IBookingManager bookingManager;
//        private Mock<IRepository<Booking>> mockBookingRepository;
//        private Mock<IRepository<Room>> mockRoomRepository;
//        private readonly ScenarioContext _scenarioContext;
//        private Booking booking;
//        private bool result;
//        private List<Room> rooms;
//        #endregion

//        public CreateBookingSteps(ScenarioContext scenarioContext, ITestOutputHelper outputHelper)
//        {
//            _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
//            _outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
//            mockBookingRepository = new Mock<IRepository<Booking>>();
//            mockRoomRepository = new Mock<IRepository<Room>>();
//            bookingManager = new BookingManager(mockBookingRepository.Object, mockRoomRepository.Object);
//        }

//        [Given(@"a hotel with (.*) available rooms")]
//        public void GivenAHotelWithAvailableRooms(int totalRooms)
//        {
//            rooms = new List<Room>();
//            for (int i = 1; i <= totalRooms; i++)
//            {
//                rooms.Add(new Room { Id = i });
//            }
//            mockRoomRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(rooms);
//        }

//        [Given(@"the booking start date is ""(.*)"" and the end date is ""(.*)""")]
//        public void GivenTheBookingDates(int startDateDays, int endDateDays)
//        {
//            DateTime startDate = DateTime.Today.AddDays(startDateDays);
//            DateTime endDate = DateTime.Today.AddDays(endDateDays);
//            booking = new Booking
//            {
//                StartDate = startDate,
//                EndDate = endDate,
//                IsActive = false
//            };
//        }

//        [When(@"a user books a room")]
//        public async Task WhenAUserBooksARoom()
//        {
//            try
//            {
//                result = await bookingManager.CreateBooking(booking);
//                // If no exception, store result as usual.
//                _scenarioContext["Exception"] = null;
//            }
//            catch (ArgumentException ex)
//            {
//                // Store the exception message for verification.
//                _scenarioContext["Exception"] = ex;
//                result = false;
//            }
//        }

//        [Then(@"the booking should be created successfully")]
//        public void ThenTheBookingShouldBeCreatedSuccessfully()
//        {
//            Assert.True(result);
//            Assert.True(booking.IsActive);
//        }

//        [Then(@"the booking should not be created")]
//        public void ThenTheBookingShouldNotBeCreated()
//        {
//            Assert.False(result);
//            // Optionally, also assert that booking remains inactive.
//            Assert.False(booking.IsActive);
//        }

//        [Then(@"the assigned room id should be between (.*) and (.*)")]
//        public void ThenTheAssignedRoomIdShouldBeBetween(int minRoomId, int maxRoomId)
//        {
//            Assert.InRange(booking.RoomId, minRoomId, maxRoomId);
//        }

//        [Then(@"an exception should be thrown indicating (.*)")]
//        public void ThenAnExceptionShouldBeThrownIndicating(string expectedMessageFragment)
//        {
//            var ex = _scenarioContext["Exception"] as ArgumentException;
//            Assert.NotNull(ex);
//            Assert.Contains(expectedMessageFragment, ex.Message);
//        }
//    }
//}
