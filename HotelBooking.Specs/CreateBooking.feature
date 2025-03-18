Feature: Hotel booking creating a hotel

    This feature is to check if the booking works as expected when different condition are given 

Scenario: check if there is a available room  
    Given is a hotel which have in total 2 rooms
    When a user books a room
    Then check if a room from 1 to 2 is available