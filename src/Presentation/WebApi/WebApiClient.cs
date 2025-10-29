using RestSharp;

namespace Presentation.WebApi
{
    public interface IWebApiClient
    {
        Task ImportLinesAsync(List<string> dataLines, string fileName, string? userId);
    }

    public class WebApiClient : IWebApiClient, IDisposable
    {
        private readonly RestClient _client;

        public WebApiClient()
        {
            _client = new RestClient(new RestClientOptions("https://localhost:44325/"));
        }

        public async Task ImportLinesAsync(List<string> dataLines, string fileName, string? userId)
        {
            var request = new RestRequest("api/Transactions/import", Method.Post);
            request.AddHeader("content-type", "application/json");
            request.AddQueryParameter("fileName", fileName);
            request.AddQueryParameter("userId", userId);
            request.AddBody(dataLines.ToArray());

            var response = await _client.ExecuteAsync(request);
            if (!response.IsSuccessful)
            {
                throw new Exception($"Error importing lines: {response.Content}");
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}