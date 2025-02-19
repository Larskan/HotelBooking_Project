using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace HotelBooking.UnitTests
{
    //public class ProjectTests
    //{
    //    private IBookingManager bookingManager;
    //    IRepository<Booking> bookingRepository;

    //    public ProjectTests()
    //    {
    //        DateTime start = DateTime.Today.AddDays(10);
    //        DateTime end = DateTime.Today.AddDays(20);
    //        bookingRepository = new FakeBookingRepository(start, end);
    //        IRepository<Room> roomRepository = new FakeRoomRepository();
    //        bookingManager = new BookingManager(bookingRepository, roomRepository);
    //    }

    //    [Theory]
    //    //Inline: Constants
    //    [InlineData(true)]
    //    public void test1()
    //    {
    //        //arrange
    //        //act
    //        //assert
    //    }

    //    [Theory]
    //    //Inline: Constants
    //    [MemberData(nameof(GetLocalData))]
    //    public void test1
    //    {
    //        //arrange
    //        //act
    //        //assert
    //    }

    //    //Iterate over MemberData instead of the Constants of Inline
    //    public static IEnumerable<object[]> GetLocalData()
    //    {
    //        var data = new List<object[]>
    //        {
    //            new object[] { }
    //        };
    //        return data;
    //    }

    //    //External data
    //    [Theory]
    //    //typeof: provide name of class that provides the test data, from class with IEnum
    //    [ClassData(typeof(Booking))]
    //    public void stuff()
    //    {
    //        //Arrange, act assert
    //    }
    //    //CustomDataAttribute, can have the test data in a file and grab it from there
    //    [Theory]
    //    [JsonData] 
    //    public void bla()
    //    {

    //    }
    //}
}
