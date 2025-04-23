using HotelBooking.Core;
using HotelBooking.Infrastructure;
using HotelBooking.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Xunit.Abstractions;
using Xunit;
using System.Threading.Tasks;
using System;

namespace HotelBooking.UnitTests
{
    // ***********  FINDAVAILABLEROOM  ***********
    // Has if (startDate <= DateTime.Today || startDate >= endDate) aka 2 Conditions = 2^2 = 4 tests.
    // Has foreach (var room in rooms), has no true/false, but can test no iteration, 1 iteration, 3 iterations = 3 tests.
    // Has if (activeBookingsForCurrentRoom.All(b => startDate < b.StartDate && endDate<b.StartDate || startDate> b.EndDate && endDate > b.EndDate))
    // Which is a condition with 2 conditions inside it, so we can test it with 2^2 = 4 tests.
    // In total we would need 4 + 3 + 4 = 11 tests for FindAvailableRoom if using MCC. 

    // If we use MC/DC instead of MCC, we just need to show that each condition independently affects the outcome of the decision
    // For Decision 1: We need to test that True False causes decision to be true & False True causes decision to be true & False False causes decision to be false = 3 Tests
    // For Decision 2: This is a control structure(foreach), it isnt MC/DC directly, but can test the two paths: 1st room free(return early), several iterations(3rd room free)
    // **Note for D2: Can skip this in the MC/DC testing..could add in the Path Testing I plan to add for fun - Check Path 3 and Path 4
    // For Decision 3: We need to show that each condition independently affects the decision, so 3 Tests.

    // In total we would need 3 + 0 + 3 = 6 tests for FindAvailableRoom if using MC/DC.

    // ***********  GETFULLYOCCUPIEDDATES  ***********
    // Decision 1: if (startDate > endDate) = 1 Condition. For MCC its 2 tests(true and false), for MC/DC its 1 test(show it affects outcome)
    // Decision 2: if (bookings.Any()) = 1 Condition. For MCC its 2 tests(true and false), for MC/DC its 1 test(show it affects outcome)
    // Decision 3: noOfBookings = Has a compound boolean condition: b.IsActive, d >= b.StartDate, d <= b.EndDate, so 3 conditions, which is 2^3= 8 tests for MCC and 3 Tests for MC/DC
    // Decision 4: if (noOfBookings.Count() >= noOfRooms) = 1 Condition. For MCC its 2 tests(true and false), for MC/DC its 1 test(show it affects outcome)
    // In total we would need 2 + 2 + 8 + 2 = 14 tests for GetFullyOccupiedDates if using MCC.
    // In total we would need 1 + 1 + 3 + 1 = 6 tests for GetFullyOccupiedDates if using MC/DC.

    // So we have decided to use MC/DC, since it is 12 tests instead of 25 tests
    public class WhiteBoxTests
    {
        #region Globals
        // Note: The occupied dates in the db initializer are: +4 to +18
        SqliteConnection connection;
        BookingManager bookingManager;
        private readonly ITestOutputHelper _outputHelper;
        #endregion Globals

        #region Constructor
        public WhiteBoxTests(ITestOutputHelper testoutputHelper)
        {
            _outputHelper = testoutputHelper;
            // Initialize SQLite native engine
            SQLitePCL.Batteries_V2.Init();

            connection = new SqliteConnection("DataSource=:memory:");

            // In-memory database only exists while the connection is open
            connection.Open();

            // Initialize test database
            var options = new DbContextOptionsBuilder<HotelBookingContext>().UseSqlite(connection).Options;
            var dbContext = new HotelBookingContext(options);
            IDbInitializer dbInitializer = new DbInitializer();
            dbInitializer.Initialize(dbContext);

            // Create repositories and BookingManager
            var bookingRepos = new BookingRepository(dbContext);
            var roomRepos = new RoomRepository(dbContext);
            bookingManager = new BookingManager(bookingRepos, roomRepos);

            Debug.WriteLine("WhiteBoxTests constructor called!");

        }
        #endregion Constructor


        #region Tests for FindAvailableRoom with MC/DC
        // Test 1: if (startDate <= DateTime.Today || startDate >= endDate) = True || False.
        [Fact]
        public async Task FindAvailableRoom_DecisionOneTrueFalse_ThrowsArgumentException()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(-1);
            DateTime endDate = DateTime.Today.AddDays(2);
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => bookingManager.FindAvailableRoom(startDate, endDate));
        }

        // Test 2: if (startDate <= DateTime.Today || startDate >= endDate) = False || True.
        [Fact]
        public async Task FindAvailableRoom_DecisionOneFalseTrue_ThrowsArgumentException()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(2);
            DateTime endDate = DateTime.Today.AddDays(2);
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => bookingManager.FindAvailableRoom(startDate, endDate));
        }
        // Test 3: if (startDate <= DateTime.Today || startDate >= endDate) = False || False.
        [Fact]
        public async Task FindAvailableRoom_DecisionOneFalseFalse_NoThrowsArgumentException()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(2);
            DateTime endDate = DateTime.Today.AddDays(3);
            // Act
            var result = await bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.True(result >= -1); // No exception should be thrown
        }

        // A = startDate < b.StartDate && endDate < b.StartDate - b = booking
        // A: If our bookings start and end date is before the booked start date - Our booked start date is +4
        // B: If our bookings start and end date is after the booked end date - Our booked end date is +18
        // B = startDate > b.EndDate && endDate > b.EndDate - b = booking
        // Test 4: True || False = True
        [Fact]
        public async Task FindAvailableRoom_DecisionThreeTrueFalse_ReturnRoomID()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(2);
            DateTime endDate = DateTime.Today.AddDays(3);

            // Act
            var roomId = await bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.NotEqual(-1, roomId);

        }
        // A = startDate < b.StartDate && endDate < b.StartDate
        // B = startDate > b.EndDate && endDate > b.EndDate
        // Test 5: False || True = True
        [Fact]
        public async Task FindAvailableRoom_DecisionThreeFalseTrue_ReturnRoomID()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(20);
            DateTime endDate = DateTime.Today.AddDays(22);

            // Act
            var roomId = await bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.NotEqual(-1, roomId);

        }
        // A = startDate < b.StartDate && endDate < b.StartDate
        // B = startDate > b.EndDate && endDate > b.EndDate
        // Test 6: False || False = False
        [Fact]
        public async Task FindAvailableRoom_DecisionThreeFalseFalse_ReturnRoomID()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(4);
            DateTime endDate = DateTime.Today.AddDays(18);
            // Act
            var roomId = await bookingManager.FindAvailableRoom(startDate, endDate);
            // Assert
            Assert.Equal(-1, roomId);

        }



        #endregion Tests for FindAvailableRoom with MC/DC

        #region Tests for GetFullyOccupiedDates with MC/DC
        // Test 1: if (startDate > endDate) = True
        [Fact]
        public async Task GetFullyOccupiedDates_DecisionOneFalse_ThrowsArgumentException()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(2);
            DateTime endDate = DateTime.Today.AddDays(1);
            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => bookingManager.GetFullyOccupiedDates(startDate, endDate));
        }

        // Test 2: if (bookings.Any()) with no bookings
        [Fact]
        public async Task GetFullyOccupiedDates_DecisionTwoFalse_ReturnsEmptyList()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(20);
            DateTime endDate = DateTime.Today.AddDays(22);
            // Act
            var result = await bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            Assert.Empty(result); // If there is no bookings, it cant return the fully occupied dates, since there is none.
        }

        // Test 3: Compound condition: b.IsActive affects the result - False, True, True
        [Fact]
        public async Task GetFullyOccupiedDates_DecisionThreeFalseTrueTrue_Ignored()
        {
            // Arrange
            var dbcontext = new HotelBookingContext(new DbContextOptionsBuilder<HotelBookingContext>().UseSqlite(connection).Options);
            // Clear existing data
            dbcontext.Booking.RemoveRange(dbcontext.Booking);
            dbcontext.SaveChanges();

            dbcontext.Booking.Add(new Booking
            {
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(3),
                IsActive = false,
                CustomerId = 1,
                RoomId = 1
            });
            dbcontext.SaveChanges();

            var manager = new BookingManager(new BookingRepository(dbcontext), new RoomRepository(dbcontext));
            
            // Act
            var result = await manager.GetFullyOccupiedDates(DateTime.Today.AddDays(1), DateTime.Today.AddDays(3));

            // Assert
            Assert.Empty(result); // Booking is ignored due to IsActive=false

        }
        // Test 4: Compound condition: d >= b.StartDate affects the result - True, False, True
        [Fact]
        public async Task GetFullyOccupiedDates_DecisionThreeTrueFalseTrue_OutsideRange()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(2); //Not allowed to be later than the booked start date of +4
            DateTime endDate = DateTime.Today.AddDays(3);

            // Act
            var result = await bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Empty(result); // The startDate are earlier than the booked dates, so it should return an empty list.

        }
        // Test 5: Compound condition: d <= b.EndDate affects the result - True, True, False
        [Fact]
        public async Task GetFullyOccupiedDates_DecisionThreeTrueTrueFalse_OutsideRange()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(19);
            DateTime endDate = DateTime.Today.AddDays(20);

            // Act
            var result = await bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Empty(result);
        }

        // Test 6: if (noOfBookings.Count() >= noOfRooms), if its true it will print as normal. If False, it will return an empty list. Needs 3 Rooms, 2 Bookings
        [Fact]
        public async Task GetFullyOccupiedDates_DecisionFourFalse_ReturnsEmptyList()
        {
            // Arrange
            var dbcontext = new HotelBookingContext(new DbContextOptionsBuilder<HotelBookingContext>().UseSqlite(connection).Options);

            // Clear existing data
            dbcontext.Booking.RemoveRange(dbcontext.Booking);
            dbcontext.SaveChanges();

            dbcontext.Booking.Add(new Booking
            {
                StartDate = DateTime.Today.AddDays(4),
                EndDate = DateTime.Today.AddDays(18),
                IsActive = true,
                CustomerId = 1,
                RoomId = 1
            });
            dbcontext.Booking.Add(new Booking
            {
                StartDate = DateTime.Today.AddDays(4),
                EndDate = DateTime.Today.AddDays(18),
                IsActive = true,
                CustomerId = 1,
                RoomId = 2
            });
            dbcontext.SaveChanges();

            var manager = new BookingManager(new BookingRepository(dbcontext), new RoomRepository(dbcontext));

            // Act
            var result = await manager.GetFullyOccupiedDates(DateTime.Today.AddDays(4), DateTime.Today.AddDays(18));

            // Assert
            Assert.Empty(result);

        }
        #endregion Tests for GetFullyOccupiedDates with MC/DC

        #region Tests for GetFullyOccupiedDates with MCC - Abandoned
        // Test 1: if (startDate > endDate) = True
        [Fact]
        public async Task GetFullyOccupiedDates_True_ThrowsArgumentException()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(3);
            DateTime endDate = DateTime.Today.AddDays(2);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => bookingManager.GetFullyOccupiedDates(startDate, endDate));
        }

        // Test 2: if (startDate > endDate) = False
        [Fact]
        public async Task GetFullyOccupiedDates_False_NoThrowsArgumentException()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(2);
            DateTime endDate = DateTime.Today.AddDays(3);

            // Act
            var result = await bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.NotNull(result); // No exception should be thrown
        }
        #endregion Tests for GetFullyOccupiedDates with MCC - Abandoned


        #region Other Tests for FindAvailableRoom for Path Testing - For fun

        // Path 2: No rooms available, returns -1
        [Fact]
        public async Task FindAvailableRoom_NoRoomsAvailable_ReturnsMinusOne()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(8);
            DateTime endDate = DateTime.Today.AddDays(10);
            // Act
            var roomId = await bookingManager.FindAvailableRoom(startDate, endDate);
            // Assert
            Assert.Equal(-1, roomId);
        }
        // Path 3: First room avaiable, early return
        [Fact]
        public async Task FindAvailableRoom_RoomFound_ReturnsRoomID()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today.AddDays(2);

            // Act
            var roomId = await bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.Equal(1, roomId);

        }
        // Path 4: Several rooms checked, available room is not first.
        [Fact]
        public async Task FindAvailableRoom_RoomFound_ReturnsRoomId_ForSecondRoom()
        {
            // Arrange
            var dbcontext = new HotelBookingContext(new DbContextOptionsBuilder<HotelBookingContext>().UseSqlite(connection).Options);
            // Clear existing data for booking
            dbcontext.Booking.RemoveRange(dbcontext.Booking);
            dbcontext.SaveChanges();


            // Add booking for Room 1 only, leaving 2 and 3 free.
            dbcontext.Booking.Add(new Booking
            {
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(4),
                IsActive = true,
                CustomerId = 1,
                RoomId = 1
            });
            dbcontext.SaveChanges();

            // Recreate BookingManager with updated context
            var manager = new BookingManager(new BookingRepository(dbcontext), new RoomRepository(dbcontext));

            // Act
            var roomId = await manager.FindAvailableRoom(DateTime.Today.AddDays(2), DateTime.Today.AddDays(4));

            // Assert
            Assert.Equal(2, roomId); // Room 2 is available, but Room 1 is booked
        }

        // Path 5: Edge Case: No Bookings
        // Path 6: Edge Case: No rooms available at all

        #endregion Other Tests for FindAvaiableRooms for Path Testing - For fun

        #region Other Tests for GetFullyOccupiedDates for Path Testing - For fun

        // Path 2: No bookings exist
        [Fact]
        public async Task GetFullyOccupiedDates_NoBookings_ReturnsEmptyList()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(19);
            DateTime endDate = DateTime.Today.AddDays(22);
            // Act
            var result = await bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            Assert.Empty(result);
        }
        // Path 3: Valid dates, bookings exist, but no day is fully booked
        // Path 4: Some days are fully booked
        // Path 5: Every date in range is fully booked
        #endregion Other Tests for GetFullyOccupiedDates for Path Testing - For fun


    }
}
