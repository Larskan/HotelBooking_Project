using System;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Xunit.Abstractions;


namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        #region Globals
        private readonly ITestOutputHelper _outputHelper;
        private IBookingManager bookingManager;
        private Mock<IRepository<Booking>> mockBookingRepository;
        private Mock<IRepository<Room>> mockRoomRepository;
        private static string _errorMessage = "The start date cannot be in the past or later than the end date.";
        #endregion Globals

        #region Constructor
        public BookingManagerTests(ITestOutputHelper testoutputHelper)
        {
            _outputHelper = testoutputHelper;
            Debug.WriteLine("BookingManagerTests constructor called!");
            var rooms = new List<Room>
            {
                new Room { Id=1, Description="A" },
                new Room { Id=2, Description="B" },
            };

            IEnumerable<Booking> bookings = new List<Booking>
            {
                new Booking { Id=1, StartDate=DateTime.Today.AddDays(1), EndDate=DateTime.Today.AddDays(5), IsActive=true, CustomerId=1, RoomId=1 },
                new Booking { Id=1, StartDate=DateTime.Today.AddDays(10), EndDate=DateTime.Today.AddDays(20), IsActive=true, CustomerId=1, RoomId=1 },
                new Booking { Id=2, StartDate=DateTime.Today.AddDays(10), EndDate=DateTime.Today.AddDays(20), IsActive=true, CustomerId=2, RoomId=2 },
            };


            // Create fake RoomRepository. 
            mockRoomRepository = new Mock<IRepository<Room>>();
            // Create fake BookingRepository. 
            mockBookingRepository = new Mock<IRepository<Booking>>();

            // Implement fake GetAll() method.
            mockRoomRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(rooms);
            mockBookingRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(bookings);
            

            //Implement booking manager
            bookingManager = new BookingManager(mockBookingRepository.Object, mockRoomRepository.Object);
        }
        #endregion Constructor

        #region MemberData Lars
        //This is MemberData CreateBooking_Failed_Creation and CreateBooking_Successful_Creation
        public static IEnumerable<Object[]> DataForCreatingBooking => new List<object[]>
        {
            //Successful booking: No conflicts, room is free
            //id, startDate, endDate, free:true, customerID, expectedRoomID
            new Object[]{1, DateTime.Today.AddDays(100), DateTime.Today.AddDays(105), true, 1, 1},
            new Object[]{1, DateTime.Today.AddDays(200), DateTime.Today.AddDays(205), true, 2, 1},

            //Failed booking: Simulate conflict with existing booking
            new Object[]{2, DateTime.Today.AddDays(4), DateTime.Today.AddDays(15), false, 2, 2},
        };
        #endregion MemberData Lars

        #region MemberData Niels
        //This is MemberData for GetFullyOccupiedDates_StartDateNotLargerThenEndDate_ThrowsArgumentException
        public static IEnumerable<object[]> GetOccupiedExceptionData()
        {
            var data = new List<object[]>
            {
                new object[] {DateTime.Today.AddDays(5),DateTime.Today, typeof(ArgumentException) },
                new object[] {DateTime.Today.AddDays(1),DateTime.Today, typeof(ArgumentException) }
            };

            return data;
        }
        //This is MemberData for GetFullyOccupiedDates_OccupiedRoomsBetweenStartDateAndEndDate_OccupiedRoomCount
        public static IEnumerable<object[]> GetOccupiedData()
        {
            var data = new List<object[]>
            {
                new object[] { DateTime.Today.AddDays(10), DateTime.Today.AddDays(20), 11 },
                new object[] { DateTime.Today.AddDays(5), DateTime.Today.AddDays(10), 1 },
                new object[] { DateTime.Today.AddDays(15), DateTime.Today.AddDays(25), 6 },
                new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(9), 0 }
            };

            return data;
        }
        #endregion MemberData Niels

        #region MemberData Jan
        public static IEnumerable<object[]> GetInvalidDatesData()
        {
            yield return new object[] { DateTime.Today, DateTime.Today.AddDays(1), _errorMessage };
            yield return new object[] { DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), _errorMessage };
            yield return new object[] { DateTime.Today.AddDays(2), DateTime.Today.AddDays(2), _errorMessage };
        }

        public static IEnumerable<object[]> GetAvailableRoomsData()
        {
            yield return new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(2) };
            yield return new object[] { DateTime.Today.AddDays(3), DateTime.Today.AddDays(4) };
        }

        public static IEnumerable<object[]> GetNotAvailableRoomsData()
        {
            yield return new object[] { DateTime.Today.AddDays(5), DateTime.Today.AddDays(15), -1 };
            yield return new object[] { DateTime.Today.AddDays(10), DateTime.Today.AddDays(20), -1 };
            yield return new object[] { DateTime.Today.AddDays(15), DateTime.Today.AddDays(25), -1 };
        }
        #endregion MemberData Jan

        #region Henriks Default
        //[Fact]
        //public async Task FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentExceptionHenrik()
        //{
        //    // Arrange
        //    DateTime date = DateTime.Today;

        //    // Act
        //    Task result() => bookingManager.FindAvailableRoomNew(date, date);

        //    // Assert
        //    await Assert.ThrowsAsync<ArgumentException>(result);
        //}

        //[Fact]
        //public async Task FindAvailableRoom_RoomAvailable_RoomIdNotMinusOneHenrik()
        //{
        //    // Arrange
        //    DateTime date = DateTime.Today.AddDays(1);
        //    // Act
        //    int roomId = await bookingManager.FindAvailableRoomNew(date, date);
        //    // Assert
        //    Assert.NotEqual(-1, roomId);
        //}

        //[Fact]
        //public async Task FindAvailableRoom_RoomAvailable_ReturnsAvailableRoomHenrik()
        //{
        //    // This test was added to satisfy the following test design
        //    // principle: "Tests should have strong assertions".

        //    // Arrange
        //    DateTime date = DateTime.Today.AddDays(1);

        //    // Act
        //    int roomId = await bookingManager.FindAvailableRoomNew(date, date);

        //    var bookingForReturnedRoomId = (await mockBookingRepository.Object.GetAllAsync()).
        //        Where(b => b.RoomId == roomId
        //                   && b.StartDate <= date
        //                   && b.EndDate >= date
        //                   && b.IsActive);

        //    // Assert
        //    Assert.Empty(bookingForReturnedRoomId);
        //}
        #endregion Henriks Default

        #region Lars
        //Why member? Can use objects directly for complex test cases.
        //Data is stored within the test file, so you have easy access to your data...though this can cause cluttering..but it does allow us to reuse data. 
        //Strong typing is good here, because it always knows the type of data it is dealing with, compared to Custom where we know the type was wrong when test fails.
        //FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne
        [Theory]
        [MemberData(nameof(DataForCreatingBooking))]
        public async Task CreateBooking_Creation_SuccessfulCreation(int id, DateTime start, DateTime end, bool free, int customer, int expectedRoom)
        {
            //This test is only launched for successful creation:
            if (!free){ return;}

            //Arrange
            //Booking instance from Memberdata
            var booking = new Booking{Id = id,StartDate = start,EndDate = end,IsActive = free,CustomerId = customer,RoomId = expectedRoom};

            //Act
            bool result = await bookingManager.CreateBooking(booking);
            _outputHelper.WriteLine($"Act Value: {result}");

            //Assert
            Assert.True(result);
            Assert.Equal(expectedRoom, booking.RoomId);
            _outputHelper.WriteLine($"expectedRoom Value: {expectedRoom}");
            _outputHelper.WriteLine($"booking.RoomID Value: {booking.RoomId}");
            mockBookingRepository.Verify(x => x.AddAsync(It.Is<Booking>(b => b.IsActive == true)), Times.Once);
        }

        [Theory]
        [MemberData(nameof(DataForCreatingBooking))]
        public async Task CreateBooking_Creation_FailedCreation(int id, DateTime start, DateTime end, bool free, int customer, int expectedRoom)
        {
            //Only launch this test incase of failure
            if (free){return;}

            //Arrange
            //Booking instance, actual input from CreateBooking method, origin of StartDate, EndDate etc.
            var booking = new Booking{Id = id,StartDate = start,EndDate = end,IsActive = false, CustomerId = customer,RoomId = expectedRoom};

            //Override global mock to create conflict so FindAvailable room returns -1
            List<Booking> existingBookings = new()
            {
                new Booking
                {
                    RoomId = expectedRoom, //ensures conflict to right room
                    IsActive = true, //ensures booking blocks the new one
                    StartDate = start.AddDays(-1),//ensure dates conflict with new booking
                    EndDate = end.AddDays(1)
                }
            };

            //Config of repo mocks, overrides the global
            mockBookingRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(existingBookings);
            mockRoomRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Room> { new Room { Id = expectedRoom } });
            _outputHelper.WriteLine($"mockBookRepo Value: {existingBookings.Count()}");
            _outputHelper.WriteLine($"MockRoomRepo Value: {expectedRoom}");

            //Act
            bool result = await bookingManager.CreateBooking(booking);
            _outputHelper.WriteLine($"Act Value: {result}");

            //Assert
            Assert.False(result, "Expected booking to fail");
            _outputHelper.WriteLine($"Result Value: {result}");
            //Verify that AddSync was never called because booking failed
            mockBookingRepository.Verify(x => x.AddAsync(It.IsAny<Booking>()), Times.Never);

        }

        //Good because its great for large datasets, since we keep the data in a seperate json file, we dont clutter the code, which boosts readability.
        //Of course this is not a large dataset, its more a proof of concept.
        //Can be hard to debug, since errors in dataset are NOT obvious. 
        //Has to make sure the json file is included, NOTE: Right click, open properties, Build action: content, copy to out: copy if newer
        [Theory]
        [CustomData("CreateBookingCustomData.json")]
        public async Task CreateBooking_Creation_WrongParametersRoomID(int id, DateTime start, DateTime end, bool free, int customer, int expectedRoom)
        {
            //Arrange
            var booking = new Booking{Id = id,StartDate = start,EndDate = end,CustomerId = customer,RoomId = 0 };

            //Setup book repo, no conflicts
            mockBookingRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Booking>());
            //Setup room repo, simulate room
            mockRoomRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Room> { new Room { Id = expectedRoom } });
            _outputHelper.WriteLine($"MockRoomRepo Value: {expectedRoom}");

            //Act
            bool result = await bookingManager.CreateBooking(booking);
            _outputHelper.WriteLine($"Act Value: {result}");

            //Assert
            Assert.Equal(expectedRoom >= 0, result);
            Assert.Equal(expectedRoom >= 0 ? expectedRoom : 0, booking.RoomId);
            //condition ? expression_iftrue : expression_iffalse
            mockBookingRepository.Verify(x => x.AddAsync(It.IsAny<Booking>()), expectedRoom >= 0 ? Times.Once() : Times.Never());
            _outputHelper.WriteLine($"Result Value: {result}");
            _outputHelper.WriteLine($"expectedRoom Value: {expectedRoom}");
            _outputHelper.WriteLine($"RoomID Value: {booking.RoomId}");

        }
        #endregion Lars

        #region Niels
        [Theory]
        [MemberData(nameof(GetOccupiedExceptionData))]
        public async Task GetFullyOccupiedDates_StartDateNotLargerThenEndDate_ThrowsArgumentException(DateTime startDate, DateTime endDate, Type expectedExceptionType)
        {
            // Arrange
            // Act
            var exception = await Record.ExceptionAsync(async () => await bookingManager.GetFullyOccupiedDates(startDate, endDate));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType(expectedExceptionType, exception);
        }

        [Theory]
        [MemberData(nameof(GetOccupiedData))]
        public async Task GetFullyOccupiedDates_OccupiedRoomsBetweenStartDateAndEndDate_OccupiedRoomCount(DateTime startDate, DateTime endDate, int expectedOccupiedDates)
        {
            // Arrange
            // Act
            var result = await bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Equal(expectedOccupiedDates, result.Count);
        }
        #endregion Niels

        #region Jan
        //Note: Dennis will update it
        [Theory]
        [MemberData(nameof(GetInvalidDatesData))]
        public async Task FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException(DateTime startDate, DateTime endDate, string expected)
        {
            // Act
            Task result() => bookingManager.FindAvailableRoom(startDate, endDate);


            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(result);
            Assert.Equal(expected, exception.Message);
        }

        [Theory]
        [MemberData(nameof(GetAvailableRoomsData))]
        public async Task FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne(DateTime startDate, DateTime endDate)
        {
            // Arrange

            // Act
            int roomId = await bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.NotEqual(-1, roomId);
        }

        [Theory]
        [MemberData(nameof(GetNotAvailableRoomsData))]
        public async Task FindAvailableRoom_RoomNotAvailable_RoomIdIsMinusOne(DateTime startDate, DateTime endDate, int expected)
        {
            // Arrange

            // Act    
            int roomId = await bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.Equal(expected, roomId);
        }

        [Theory]
        [MemberData(nameof(GetAvailableRoomsData))]
        public async Task FindAvailableRoom_RoomAvailable_ReturnsAvailableRoom(DateTime startDate, DateTime endDate)
        {
            // Arrange
            var bookings = await mockBookingRepository.Object.GetAllAsync();

            // Act
            int roomId = await bookingManager.FindAvailableRoom(startDate, endDate);

            var bookingForReturnedRoomId = bookings
                .Where(b => b.RoomId == roomId
                            && b.StartDate <= endDate
                            && b.EndDate >= startDate
                            && b.IsActive);

            // Assert
            Assert.Empty(bookingForReturnedRoomId);
        }
        #endregion Jan

        #region Dennis
        [Theory]
        [InlineData(-1, 5)] // Past date
        [InlineData(0, 5)] // Current date
        [InlineData(5, 3)] // End before start
        public async Task FindAvailableRoom_InvalidDates_ThrowsAnArgumentException(int startDay, int endDay)
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(startDay);
            var endDate = DateTime.Today.AddDays(endDay);
            var expectedMessage = "The start date cannot be in the past or later than the end date.";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                bookingManager.FindAvailableRoom(startDate, endDate));
            Assert.Equal(expectedMessage, exception.Message);
     
        }
        #endregion Dennis

    }
}
