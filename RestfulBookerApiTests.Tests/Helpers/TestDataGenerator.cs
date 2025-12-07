using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestfulBookerApiTests.Tests.Models;

namespace RestfulBookerApiTests.Tests.Helpers
{
    public static class TestDataGenerator
    {
        private static readonly Random _random = new Random();
        private static readonly string[] _firstNames = { "John", "Jane", "Bob", "Alice", "Charlie", "Diana", "Eve", "Frank" };
        private static readonly string[] _lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis" };
        private static readonly string[] _additionalNeeds = { "Breakfast", "Lunch", "Dinner", "Late checkout", "Early checkin", "None" };

        public static Booking GenerateValidBooking()
        {
            var checkin = DateTime.Now.AddDays(1);
            var checkout = checkin.AddDays(_random.Next(1, 10));

            return new Booking
            {
                Firstname = _firstNames[_random.Next(_firstNames.Length)],
                Lastname = _lastNames[_random.Next(_lastNames.Length)],
                Totalprice = _random.Next(50, 1000),
                Depositpaid = _random.Next(2) == 1,
                Bookingdates = new BookingDates
                {
                    Checkin = checkin.ToString("yyyy-MM-dd"),
                    Checkout = checkout.ToString("yyyy-MM-dd")
                },
                Additionalneeds = _additionalNeeds[_random.Next(_additionalNeeds.Length)]
            };
        }

        public static Booking GenerateBookingWithMissingField()
        {
            var booking = GenerateValidBooking();
            booking.Firstname = null; // Missing required field
            return booking;
        }

        public static Booking GenerateBookingWithXss()
        {
            var booking = GenerateValidBooking();
            booking.Firstname = "<script>alert(\"XSS\")</script>";
            booking.Additionalneeds = "<img src=x onerror=alert('XSS')>";
            return booking;
        }

        public static Booking GenerateBookingWithSqlInjection()
        {
            var booking = GenerateValidBooking();
            booking.Firstname = "' OR '1'='1";
            booking.Lastname = "'; DROP TABLE bookings; --";
            return booking;
        }

        public static Booking GenerateBookingWithCommandInjection()
        {
            var booking = GenerateValidBooking();
            booking.Firstname = "; ls -la";
            booking.Lastname = "$(whoami)";
            booking.Additionalneeds = "| cat /etc/passwd";
            return booking;
        }

        public static Booking GenerateBookingWithNegativePrice()
        {
            var booking = GenerateValidBooking();
            booking.Totalprice = -999999;
            return booking;
        }

        public static Booking GenerateBookingWithLargePayload()
        {
            var booking = GenerateValidBooking();
            var largeString = new StringBuilder();
            for (int i = 0; i < 100000; i++) // ~1MB of text
            {
                largeString.Append("A");
            }
            booking.Additionalneeds = largeString.ToString();
            return booking;
        }

        public static Booking GenerateBookingWithUnicode()
        {
            return new Booking
            {
                Firstname = "🔥👍😀",
                Lastname = "中文测试",
                Totalprice = 100,
                Depositpaid = true,
                Bookingdates = new BookingDates
                {
                    Checkin = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                    Checkout = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd")
                },
                Additionalneeds = "Unicode test テスト"
            };
        }

        public static Booking GenerateBookingWithSpecialCharacters()
        {
            var booking = GenerateValidBooking();
            booking.Firstname = "John@#$%";
            booking.Lastname = "Doe!&*()";
            booking.Additionalneeds = "Special chars: <>?{}[]|\\";
            return booking;
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
