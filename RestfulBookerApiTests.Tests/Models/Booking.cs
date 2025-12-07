namespace RestfulBookerApiTests.Tests.Models
{
    public class Booking
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Totalprice { get; set; }
        public bool Depositpaid { get; set; }
        public BookingDates Bookingdates { get; set; }
        public string Additionalneeds { get; set; }
    }

    public class BookingDates
    {
        public string Checkin { get; set; }
        public string Checkout { get; set; }
    }

    public class BookingResponse
    {
        public int Bookingid { get; set; }
        public Booking Booking { get; set; }
    }

    public class BookingId
    {
        public int Bookingid { get; set; }
    }

    public class AuthToken
    {
        public string Token { get; set; }
    }

    public class AuthCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
