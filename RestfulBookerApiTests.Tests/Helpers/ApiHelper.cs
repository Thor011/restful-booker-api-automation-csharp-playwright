using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Newtonsoft.Json;
using RestfulBookerApiTests.Tests.Config;
using RestfulBookerApiTests.Tests.Models;
using System.Net;

namespace RestfulBookerApiTests.Tests.Helpers
{
    public class ApiHelper : IDisposable
    {
        private IPlaywright? _playwright;
        private IAPIRequestContext? _requestContext;
        private string? _authToken;
        private bool _disposed = false;

        public ApiHelper()
        {
            InitializeAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _requestContext = await _playwright.APIRequest.NewContextAsync(new()
            {
                BaseURL = TestConfig.BaseUrl,
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            });
        }

        public async Task<PlaywrightResponse> GetAsync(string endpoint)
        {
            var response = await _requestContext!.GetAsync(endpoint);
            return new PlaywrightResponse(response);
        }

        public async Task<PlaywrightResponse> PostAsync(string endpoint, object body)
        {
            var jsonBody = JsonConvert.SerializeObject(body);
            var response = await _requestContext!.PostAsync(endpoint, new() { DataObject = body });
            return new PlaywrightResponse(response);
        }

        public async Task<PlaywrightResponse> PutAsync(string endpoint, object body, string? token = null)
        {
            var options = new APIRequestContextOptions { DataObject = body };
            
            if (!string.IsNullOrEmpty(token))
            {
                options.Headers = new Dictionary<string, string> { ["Cookie"] = $"token={token}" };
            }
            
            var response = await _requestContext!.PutAsync(endpoint, options);
            return new PlaywrightResponse(response);
        }

        public async Task<PlaywrightResponse> PutAsyncWithBasicAuth(string endpoint, object body)
        {
            var response = await _requestContext!.PutAsync(endpoint, new()
            {
                DataObject = body,
                Headers = new Dictionary<string, string>
                {
                    ["Authorization"] = "Basic YWRtaW46cGFzc3dvcmQxMjM="
                }
            });
            return new PlaywrightResponse(response);
        }

        public async Task<PlaywrightResponse> PatchAsync(string endpoint, object body, string? token = null)
        {
            var options = new APIRequestContextOptions { DataObject = body };
            
            if (!string.IsNullOrEmpty(token))
            {
                options.Headers = new Dictionary<string, string> { ["Cookie"] = $"token={token}" };
            }
            
            var response = await _requestContext!.PatchAsync(endpoint, options);
            return new PlaywrightResponse(response);
        }

        public async Task<PlaywrightResponse> DeleteAsync(string endpoint, string? token = null)
        {
            var options = new APIRequestContextOptions();
            
            if (!string.IsNullOrEmpty(token))
            {
                options.Headers = new Dictionary<string, string> { ["Cookie"] = $"token={token}" };
            }
            
            var response = await _requestContext!.DeleteAsync(endpoint, options);
            return new PlaywrightResponse(response);
        }

        public async Task<string?> CreateAuthToken(string? username = null, string? password = null)
        {
            var credentials = new AuthCredentials
            {
                Username = username ?? TestConfig.DefaultUsername,
                Password = password ?? TestConfig.DefaultPassword
            };

            var response = await _requestContext!.PostAsync("/auth", new() { DataObject = credentials });
            
            if (response.Ok)
            {
                var content = await response.TextAsync();
                var tokenResponse = JsonConvert.DeserializeObject<AuthToken>(content);
                _authToken = tokenResponse?.Token;
                return _authToken;
            }

            return null;
        }

        public async Task<BookingResponse?> CreateBooking(Booking booking)
        {
            var response = await _requestContext!.PostAsync("/booking", new() { DataObject = booking });
            
            if (response.Ok)
            {
                var content = await response.TextAsync();
                return JsonConvert.DeserializeObject<BookingResponse>(content);
            }

            return null;
        }

        public async Task<Booking?> GetBooking(int bookingId)
        {
            var response = await _requestContext!.GetAsync($"/booking/{bookingId}");
            
            if (response.Ok)
            {
                var content = await response.TextAsync();
                return JsonConvert.DeserializeObject<Booking>(content);
            }

            return null;
        }

        public async Task<PlaywrightResponse> GetWithHeader(string endpoint, string headerName, string headerValue)
        {
            var response = await _requestContext!.GetAsync(endpoint, new()
            {
                Headers = new Dictionary<string, string> { [headerName] = headerValue }
            });
            return new PlaywrightResponse(response);
        }

        public T? DeserializeResponse<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _requestContext?.DisposeAsync().GetAwaiter().GetResult();
                _playwright?.Dispose();
            }

            _disposed = true;
        }
    }

    // Wrapper class to make Playwright IAPIResponse compatible with existing tests
    public class PlaywrightResponse
    {
        private readonly IAPIResponse _response;

        public PlaywrightResponse(IAPIResponse response)
        {
            _response = response;
        }

        public HttpStatusCode StatusCode => (HttpStatusCode)_response.Status;
        public bool IsSuccessful => _response.Ok;
        public string? Content => _response.TextAsync().GetAwaiter().GetResult();
    }
}
