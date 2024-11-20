using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;
using System.Threading.Tasks;

namespace UserAuthApi.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMongoClient _mongoClient;

        public RequestLoggingMiddleware(RequestDelegate next, IMongoClient mongoClient)
        {
            _next = next;
            _mongoClient = mongoClient;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var database = _mongoClient.GetDatabase("logs");
            var collection = database.GetCollection<BsonDocument>("requests");

            var log = new BsonDocument
            {
                { "method", context.Request.Method },
                { "path", context.Request.Path.ToString() },
                { "timestamp", DateTime.UtcNow }
            };

            context.Request.EnableBuffering();
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            log.Add("request_body", requestBody);

            await collection.InsertOneAsync(log);
            await _next(context);
        }
    }
}
