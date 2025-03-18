using Reqnroll;
using HotelBooking.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HotelBooking.Specs
{
    [Binding]
    public class CreateBookingSteps
    {
        #region Globals
        private readonly ITestOutputHelper _outputHelper;
        private IBookingManager bookingManager;
        private Mock<IRepository<Booking>> mockBookingRepository;
        private Mock<IRepository<Room>> mockRoomRepository;
        private readonly ScenarioContext _scenarioContext;
        private Booking booking;
        private bool result;

        public CreateBookingSteps(ScenarioContext scenarioContext, ITestOutputHelper outputHelper)
        {
            _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            _outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
            mockBookingRepository = new Mock<IRepository<Booking>>();
            mockRoomRepository = new Mock<IRepository<Room>>();
            bookingManager = new BookingManager(mockBookingRepository.Object, mockRoomRepository.Object);
        }
        #endregion

        [Given(@"is a hotel which have in total (.*) rooms")]
        public void GivenIsAHotelWhichHaveInTotalRooms(int totalRooms)
        {
            var rooms = new List<Room>();
            for (int i = 1; i <= totalRooms; i++)
            {
                rooms.Add(new Room { Id = i });
            }
            mockRoomRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(rooms);
        }

        [When(@"a user books a room")]
        public async Task WhenAUserBooksARoom()
        {
            booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(2),
                IsActive = false
            };
            result = await bookingManager.CreateBooking(booking);
        }

        [Then(@"check if a room from (.*) to (.*) is available")]
        public void ThenCheckIfARoomFromToIsAvailable(int startOffset, int endOffset)
        {
            DateTime startDate = DateTime.Today.AddDays(startOffset);
            DateTime endDate = DateTime.Today.AddDays(endOffset);

            Assert.True(result);
            Assert.Equal(startDate, booking.StartDate);
            Assert.Equal(endDate, booking.EndDate);
            Assert.True(booking.IsActive);
        }
    }
}