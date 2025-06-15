using Nest;
using OrderService.Models;

namespace OrderService.Services
{
    public class LogService
    {
        private readonly ElasticClient _client;

        public LogService()
        {
            var settings = new ConnectionSettings(new Uri("https://localhost:9200"))
                .ServerCertificateValidationCallback((o, cert, chain, errors) => true)
                .BasicAuthentication("elastic", "H328TNOuz9CVa+r_UKV3")
                .DefaultIndex("applogs");

            _client = new ElasticClient(settings);
        }

        public async Task LogAsync(LogModel log)
        {
            await _client.IndexDocumentAsync(log);
        }
    }
}
