#Feature: Hotel booking creation
#
#  This feature tests the booking creation functionality under various conditions.

  # ECT & BVT Table for Hotel Booking System

    # Test Case ID  |	Scenario Description                                              |	   Start Date       | 	End Date            |	Available Rooms |   Expected Outcome
    # TC01	        | Valid Booking: A room is available and dates are valid. 	        |       Tomorrow	    |     Tomorrow + 1 day	|        2	        |   Booking created; returns a room ID (True).
    # TC02	        | No Available Room: All rooms are booked.	                        |       Tomorrow	    |     Tomorrow + 1 day	|        0	        |   Booking not created; returns False.
    # TC03	        | Invalid Start Date (Boundary): Start date is today or earlier.	  |    Today or earlier |     Tomorrow	        |        2	        |   Exception thrown (The start date cannot be in the past or later than the end date).
    # TC04	        | Booking earliest (Boundary): Booking at the earliest start date.  |	      Tomorrow	    |     Tomorrow + 1 day  |        2	        |   Booking created; returns a room ID (True).
    # TC05	        | Invalid Date Range: Start date is after end date.	                |   Tomorrow + 2 days |	    Tomorrow + 1 day	|        2	        |   Exception thrown (The start date cannot be in the past or later than the end date).
    # TC06	        | Zero-Day Booking (Edge Case): Start and end dates are the same.	  |       Tomorrow	    |     Tomorrow	        |        2	        |   Exception thrown or booking rejected.
  
  
  # TC01: Valid Booking - A valid scenario where a room is available.
#  Scenario: Valid booking with available room
#    Given a hotel with 2 available rooms
#    And the booking start date is "1" and the end date is "2"
#    When a user books a room
#    Then the booking should be created successfully
#    And the assigned room id should be between 1 and 2
#
#  # TC02: No Available Room - Simulate scenario when no room is available.
#  Scenario: Booking fails when no rooms are available
#    Given a hotel with 0 available rooms
#    And the booking start date is "1" and the end date is "2"
#    When a user books a room
#    Then the booking should not be created
#
#  # TC03: Boundary - Earliest Acceptable Start Date
#  Scenario: Booking fails when Start date is today or earlier
#    Given a hotel with 2 available rooms
#    And the booking start date is "0" and the end date is "2"
#    When a user books a room
#    Then an exception should be thrown indicating The start date cannot be in the past or later than the end date.


