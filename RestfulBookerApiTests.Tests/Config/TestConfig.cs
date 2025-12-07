namespace RestfulBookerApiTests.Tests.Config
{
    public static class TestConfig
    {
        public static string BaseUrl => "https://restful-booker.herokuapp.com";
        public static string DefaultUsername => "admin";
        public static string DefaultPassword => "password123";
        public static int DefaultTimeoutMs => 30000;
        public static int MaxRetries => 3;
    }
}
