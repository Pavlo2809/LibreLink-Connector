using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using LibreLinkConnector.Models;
using Newtonsoft.Json;

namespace LibreLinkConnector.Services
{
    public class LibreLinkApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private string? _authToken;
        private string? _accountId;
        private string _baseUrl;

        public LibreLinkApiClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            // Don't set BaseAddress - we'll use full URLs instead
            _baseUrl = GetBaseUrl();

            // Set required headers as per LibreLink API documentation
            _httpClient.DefaultRequestHeaders.Add("product", "llu.android");
            _httpClient.DefaultRequestHeaders.Add("version", "4.16.0");
            _httpClient.DefaultRequestHeaders.Add("cache-control", "no-cache");
            _httpClient.DefaultRequestHeaders.Add("connection", "keep-alive");
            _httpClient.DefaultRequestHeaders.Add("timezone", TimeZoneInfo.Local.Id);
            // Don't add accept-encoding manually - let HttpClient handle it with AutomaticDecompression
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("LibreLinkConnector/1.0");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US");
            
            // Note: content-type header is added per-request for POST endpoints
            
            // Clear log on startup
            ApiLogger.ClearLog();
        }

        private string GetBaseUrl()
        {
            return AppSettings.UseEuropeanServer
                ? "https://api-eu.libreview.io"
                : "https://api.libreview.io";
        }

        public void UpdateBaseAddress()
        {
            _baseUrl = GetBaseUrl();
        }

        private string GetRegionalUrl(string region)
        {
            // Map region codes to their respective URLs
            return region.ToLower() switch
            {
                "eu" => "https://api-eu.libreview.io",
                "eu2" => "https://api-eu2.libreview.io",
                "us" => "https://api-us.libreview.io",
                "ap" => "https://api-ap.libreview.io",
                "au" => "https://api-au.libreview.io",
                "de" => "https://api-de.libreview.io",
                "fr" => "https://api-fr.libreview.io",
                _ => $"https://api-{region.ToLower()}.libreview.io"
            };
        }

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Email = email,
                    Password = password
                };

                var json = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var loginUrl = $"{_baseUrl}/llu/auth/login";
                
                // Log the request
                var requestHeaders = _httpClient.DefaultRequestHeaders
                    .SelectMany(h => h.Value.Select(v => $"{h.Key}: {v}"))
                    .Concat(content.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"))
                    .ToList();
                ApiLogger.LogRequest("POST", loginUrl, requestHeaders, json);
                
                var response = await _httpClient.PostAsync(loginUrl, content);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                // Log the response
                var responseHeaders = response.Headers
                    .SelectMany(h => h.Value.Select(v => $"{h.Key}: {v}"))
                    .Concat(response.Content.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"))
                    .ToList();
                ApiLogger.LogResponse((int)response.StatusCode, response.ReasonPhrase ?? "", responseHeaders, responseContent);
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"API returned {(int)response.StatusCode} ({response.StatusCode}).");
                }

                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

                if (loginResponse == null)
                {
                    throw new Exception("Failed to parse login response.");
                }

                if (loginResponse.Status != 0)
                {
                    throw new Exception($"Login failed. API returned status: {loginResponse.Status}");
                }

                // Check if we need to redirect to a different regional server
                if (loginResponse.Data?.Redirect == true && !string.IsNullOrEmpty(loginResponse.Data.Region))
                {
                    // Get the regional server URL and retry login
                    var regionalUrl = GetRegionalUrl(loginResponse.Data.Region);
                    var fullLoginUrl = $"{regionalUrl}/llu/auth/login";

                    // Retry login on the correct regional server using full URL
                    var retryContent = new StringContent(json, Encoding.UTF8, "application/json");
                    
                    // Log the retry request
                    var retryRequestHeaders = _httpClient.DefaultRequestHeaders
                        .SelectMany(h => h.Value.Select(v => $"{h.Key}: {v}"))
                        .Concat(retryContent.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"))
                        .ToList();
                    ApiLogger.LogRequest("POST", fullLoginUrl, retryRequestHeaders, json);
                    
                    var retryResponse = await _httpClient.PostAsync(fullLoginUrl, retryContent);
                    var retryResponseContent = await retryResponse.Content.ReadAsStringAsync();

                    // Log the retry response
                    var retryResponseHeaders = retryResponse.Headers
                        .SelectMany(h => h.Value.Select(v => $"{h.Key}: {v}"))
                        .Concat(retryResponse.Content.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"))
                        .ToList();
                    ApiLogger.LogResponse((int)retryResponse.StatusCode, retryResponse.ReasonPhrase ?? "", retryResponseHeaders, retryResponseContent);

                    if (!retryResponse.IsSuccessStatusCode)
                    {
                        throw new Exception($"Regional server ({loginResponse.Data.Region}) returned {(int)retryResponse.StatusCode} ({retryResponse.StatusCode}).");
                    }

                    loginResponse = JsonConvert.DeserializeObject<LoginResponse>(retryResponseContent);
                    
                    if (loginResponse == null || loginResponse.Status != 0)
                    {
                        throw new Exception($"Login failed on regional server. Status: {loginResponse?.Status}");
                    }

                    // Update base URL for future requests
                    _baseUrl = regionalUrl;
                }

                // Extract and set auth token and account ID
                var token = loginResponse.Data?.AuthTicket?.Token;
                var userId = loginResponse.Data?.User?.Id;
                
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception($"Login response did not contain authentication token. The API may have changed.");
                }

                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception($"Login response did not contain user ID. The API may have changed.");
                }

                _authToken = token;
                _accountId = ComputeSha256Hash(userId);
                SetAuthorizationHeader();

                // Debug: Verify auth header was set
                if (_httpClient.DefaultRequestHeaders.Authorization == null)
                {
                    throw new Exception($"Failed to set Authorization header after login!");
                }

                return loginResponse;
            }
            catch (HttpRequestException ex)
            {
                ApiLogger.LogError($"Network error during login", ex);
                throw new Exception($"Network error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                ApiLogger.LogError($"Login failed", ex);
                if (ex.Message.Contains("API returned"))
                {
                    throw;
                }
                throw new Exception($"Login failed: {ex.Message}", ex);
            }
        }

        public async Task<ConnectionsResponse?> GetConnectionsAsync()
        {
            try
            {
                EnsureAuthenticated();

                var url = $"{_baseUrl}/llu/connections";
                
                // Create request manually to ensure all headers are included
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                
                // Copy default headers (don't add Content-Type for GET requests)
                foreach (var header in _httpClient.DefaultRequestHeaders)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
                
                // Add Account-Id header for authenticated requests (SHA256 hash of user ID)
                if (!string.IsNullOrEmpty(_accountId))
                {
                    request.Headers.TryAddWithoutValidation("Account-Id", _accountId);
                }
                
                // Debug: Verify auth header is set
                if (_httpClient.DefaultRequestHeaders.Authorization == null)
                {
                    throw new Exception($"Authorization header is not set! Token exists: {!string.IsNullOrEmpty(_authToken)}");
                }

                // Log the request
                var requestHeaders = request.Headers
                    .SelectMany(h => h.Value.Select(v => $"{h.Key}: {v}"))
                    .ToList();
                ApiLogger.LogRequest("GET", url, requestHeaders, string.Empty);

                var response = await _httpClient.SendAsync(request);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                // Log the response
                var responseHeaders = response.Headers
                    .SelectMany(h => h.Value.Select(v => $"{h.Key}: {v}"))
                    .Concat(response.Content.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"))
                    .ToList();
                ApiLogger.LogResponse((int)response.StatusCode, response.ReasonPhrase ?? "", responseHeaders, responseContent);
                
                if (!response.IsSuccessStatusCode)
                {
                    var allHeaders = string.Join(", ", 
                        request.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}"));
                    throw new Exception($"API returned {(int)response.StatusCode} ({response.StatusCode}). URL: {url}. Headers: {allHeaders}. Response: {responseContent}");
                }

                return JsonConvert.DeserializeObject<ConnectionsResponse>(responseContent);
            }
            catch (Exception ex)
            {
                ApiLogger.LogError("Failed to get connections", ex);
                throw new Exception($"Failed to get connections: {ex.Message}", ex);
            }
        }

        public async Task<GraphResponse?> GetGraphDataAsync(string patientId)
        {
            try
            {
                EnsureAuthenticated();

                var url = $"{_baseUrl}/llu/connections/{patientId}/graph";
                
                // Create request manually to ensure all headers are included
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                
                // Copy default headers (don't add Content-Type for GET requests)
                foreach (var header in _httpClient.DefaultRequestHeaders)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
                
                // Add Account-Id header for authenticated requests (SHA256 hash of user ID)
                if (!string.IsNullOrEmpty(_accountId))
                {
                    request.Headers.TryAddWithoutValidation("Account-Id", _accountId);
                }
                
                // Log the request
                var requestHeaders = request.Headers
                    .SelectMany(h => h.Value.Select(v => $"{h.Key}: {v}"))
                    .ToList();
                ApiLogger.LogRequest("GET", url, requestHeaders, string.Empty);
                
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                // Log the response
                var responseHeaders = response.Headers
                    .SelectMany(h => h.Value.Select(v => $"{h.Key}: {v}"))
                    .Concat(response.Content.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"))
                    .ToList();
                ApiLogger.LogResponse((int)response.StatusCode, response.ReasonPhrase ?? "", responseHeaders, responseContent);
                
                response.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<GraphResponse>(responseContent);
            }
            catch (Exception ex)
            {
                ApiLogger.LogError("Failed to get graph data", ex);
                throw new Exception($"Failed to get graph data: {ex.Message}", ex);
            }
        }

        public void SetAuthToken(string token)
        {
            _authToken = token;
            SetAuthorizationHeader();
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(_authToken);

        private void SetAuthorizationHeader()
        {
            if (!string.IsNullOrEmpty(_authToken))
            {
                // Remove existing authorization header if present
                _httpClient.DefaultRequestHeaders.Authorization = null;
                
                // Set new authorization header
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", _authToken);
            }
        }

        private void EnsureAuthenticated()
        {
            if (!IsAuthenticated)
            {
                throw new InvalidOperationException("Not authenticated. Please login first.");
            }
        }

        private static string ComputeSha256Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
