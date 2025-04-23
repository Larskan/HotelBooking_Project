using Reqnroll;
using HotelBooking.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HotelBooking.Specs
{
    [Binding]
    public class ExtraCreateBookingSteps
    {
        #region Globals
        private readonly ITestOutputHelper _outputHelper;
        private IBookingManager bookingManager;
        private Mock<IRepository<Booking>> mockBookingRepository;
        private Mock<IRepository<Room>> mockRoomRepository;
        private readonly ScenarioContext _scenarioContext;
        private Booking booking;
        private bool result;
        private List<Room> rooms;
        #endregion Globals

        public ExtraCreateBookingSteps(ScenarioContext scenarioContext, ITestOutputHelper outputHelper)
        {
            _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            _outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
            mockBookingRepository = new Mock<IRepository<Booking>>();
            mockRoomRepository = new Mock<IRepository<Room>>();
            bookingManager = new BookingManager(mockBookingRepository.Object, mockRoomRepository.Object);
        }

        //Valid Booking: A room is available and dates are valid.
        //No Available Room: All rooms are booked.
        //Invalid Start Date (Boundary): Start date is today or earlier
        //Booking earliest (Boundary): Booking at the earliest start date.
        //Invalid Date Range: Start date is after end date.
        //Zero-Day Booking (Edge Case): Start and end dates are the same.

        // Do not add dupes, if they repeat, they should only be here once.

        #region GIVEN
        [Given(@"a hotel with (.*) available rooms")]
        public void GivenAHotelWithAvailableRooms(int availableRooms)
        {
            rooms = new List<Room>();
            for (int i = 1; i <= availableRooms; i++)
            {
                rooms.Add(new Room { Id = i });
            }
            mockRoomRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(rooms);
        }

        [Given(@"the booking start date is ""(.*)"" and the end date is ""(.*)""")]
        public void GivenBookingDates(int startDateParam, int endDateParam)
        {
            DateTime startDate = DateTime.Today.AddDays(startDateParam);
            DateTime endDate = DateTime.Today.AddDays(endDateParam);
            booking = new Booking
            {
                StartDate = startDate,
                EndDate = endDate,
            };
        }
        #endregion GIVEN

        #region WHEN
        [When(@"a user books a room")]
        public async Task WhenRoomIsBooked()
            //Dont use try catch here
        {
            try
            {
                result = await bookingManager.CreateBooking(booking);
                // If no exception, store result as usual.
                _scenarioContext["Exception"] = null;
            }
            catch (ArgumentException ex)
            {
                // Store the exception message for verification.
                _scenarioContext["Exception"] = ex;
                result = false;
            }
        }
        #endregion WHEN

        #region THEN
        [Then(@"the booking should be created successfully")]
        public void ThenTheBookingIsCreatedSuccessfully()
        {
            Assert.True(result);
            Assert.True(booking.IsActive);
        }

        [Then(@"the booking should not be created")]
        public void ThenTheBookingIsNotCreated()
        {
            Assert.False(result);
            Assert.False(booking.IsActive);
        }

        [Then(@"the assigned room ID should be between (.*) and (.*)")]
        public void ThenTheAssignedRoomIDShouldBeBetweenAvailableRoomIDs(int minimumRoomID, int maximumRoomID)
        {
            Assert.InRange(booking.RoomId, minimumRoomID, maximumRoomID);
        }

        [Then(@"an exception should be thrown indicating (.*)")]
        public void ThenExceptionThrownIndicatingMessage(string expectedMessage)
        {
            var ex = _scenarioContext["Exception"] as ArgumentException;
            Assert.NotNull(ex);
            Assert.Contains(expectedMessage, ex.Message);
        }
        #endregion THEN
    }
}
