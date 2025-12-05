using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using TMT.External.Grpc;

namespace testapp.Controllers
{
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        //private const string BaseAddress = "https://gateway.dev-v3.wionpos.public.rke.app.dev.tmtco.org/";
        //private const string Token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkM4RkFBMTA2OERFNjA4OEE1Mjk3OEQ1OUJBQUZGRjMzIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3NjQyOTcwMzQsImV4cCI6MTc2Njg4OTAzNCwiaXNzIjoiaHR0cHM6Ly9hY2NvdW50LnB1YmxpYy5ya2UuYXBwLmRldi50bXRjby5vcmciLCJhdWQiOlsiQWNjb3VudEFwcCIsIkxveWFsdHlBcHAiLCJUaWNUaWNBcHAiLCJUUG9zQXBwIl0sImNsaWVudF9pZCI6IkxveWFsdHlVc2VyQ2xpZW50IiwiYXBwIjoiTG95YWx0eUFwcCIsImN1cnJlbnRfYXBwIjoiIiwidGVuYW50aWQiOiI4NjQ5MzQ2NTM1OTE1NTIiLCJyb2xlIjoiODY0OTM0NjYyNzAxMDU3Iiwic3ViIjoiODQyNTMyOTEzMjE3NTM2IiwiYXV0aF90aW1lIjoxNzY0Mjk3MDM0LCJpZHAiOiJsb2NhbCIsImVtYWlsIjoiODQyNTMyOTEzMjE3NTM2QHRtdC5jb20iLCJwaG9uZV9udW1iZXIiOiIrODQzNjI1MzA3MDAiLCJwaG9uZV9udW1iZXJfdmVyaWZpZWQiOiJUcnVlIiwiaWF0IjoxNzY0Mjk3MDM0LCJzY29wZSI6WyJBY2NvdW50QXBwIiwiTG95YWx0eUFwcCIsIlRpY1RpY0FwcCIsIlRQb3NBcHAiLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsicHdkIl19.Qf6SPgJJ_N0TbAeoocCN--tupeJ_FzxPGNT2s1qYAX7lvn8tlu5GJHWPX4PgMVjiLT5LgqYGP00HK9Ya_5MA0fQixHGz6qJb1tUdBBexEZxf5GiRWLLNiC-jq0SKxxuw3U5jzo_tozSoNDXiDRYVYrltKW0xM6ByoR9rid0QqkFoJ-zJ_jSd-uSSwLXLEa7zvqcN6D4lHOSbQnXDU7q4U8ZCSucLCjoS1DL-EED9Ja08wOAoouBwOI0lQ9cudx1VL4ni_9hd1ekgWuHD26LWdHy6TgRLNZ5ZPyKDUPlfd0f8XRmsv4el4G3ZXKWwh7lt2syRATxvEonGjI799OOUeg";
        //private const string TenantId = "864934653591552";

        private const string BaseAddress = "http://localhost:21001";
        private const string Token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkM4RkFBMTA2OERFNjA4OEE1Mjk3OEQ1OUJBQUZGRjMzIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3NjQyOTcwMzQsImV4cCI6MTc2Njg4OTAzNCwiaXNzIjoiaHR0cHM6Ly9hY2NvdW50LnB1YmxpYy5ya2UuYXBwLmRldi50bXRjby5vcmciLCJhdWQiOlsiQWNjb3VudEFwcCIsIkxveWFsdHlBcHAiLCJUaWNUaWNBcHAiLCJUUG9zQXBwIl0sImNsaWVudF9pZCI6IkxveWFsdHlVc2VyQ2xpZW50IiwiYXBwIjoiTG95YWx0eUFwcCIsImN1cnJlbnRfYXBwIjoiIiwidGVuYW50aWQiOiI4NjQ5MzQ2NTM1OTE1NTIiLCJyb2xlIjoiODY0OTM0NjYyNzAxMDU3Iiwic3ViIjoiODQyNTMyOTEzMjE3NTM2IiwiYXV0aF90aW1lIjoxNzY0Mjk3MDM0LCJpZHAiOiJsb2NhbCIsImVtYWlsIjoiODQyNTMyOTEzMjE3NTM2QHRtdC5jb20iLCJwaG9uZV9udW1iZXIiOiIrODQzNjI1MzA3MDAiLCJwaG9uZV9udW1iZXJfdmVyaWZpZWQiOiJUcnVlIiwiaWF0IjoxNzY0Mjk3MDM0LCJzY29wZSI6WyJBY2NvdW50QXBwIiwiTG95YWx0eUFwcCIsIlRpY1RpY0FwcCIsIlRQb3NBcHAiLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsicHdkIl19.Qf6SPgJJ_N0TbAeoocCN--tupeJ_FzxPGNT2s1qYAX7lvn8tlu5GJHWPX4PgMVjiLT5LgqYGP00HK9Ya_5MA0fQixHGz6qJb1tUdBBexEZxf5GiRWLLNiC-jq0SKxxuw3U5jzo_tozSoNDXiDRYVYrltKW0xM6ByoR9rid0QqkFoJ-zJ_jSd-uSSwLXLEa7zvqcN6D4lHOSbQnXDU7q4U8ZCSucLCjoS1DL-EED9Ja08wOAoouBwOI0lQ9cudx1VL4ni_9hd1ekgWuHD26LWdHy6TgRLNZ5ZPyKDUPlfd0f8XRmsv4el4G3ZXKWwh7lt2syRATxvEonGjI799OOUeg";
        private const string TenantId = "864934653591552";

        private readonly ILogger<WeatherForecastController> _logger;
        private static readonly GrpcChannelPool _pool = new();

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task Get()
        {
            await TestProductByPredicate();
        }

        public static Task TestProductByPredicate()
        {
            Expression<Func<Product, bool>> predicate =
                p => p.IsActive;

            var dto = new ProductGrpcRequestDto
            {
                Predicates = predicate
            };

            return CallGenericAsync(dto, nameof(ProductGrpcRequestDto), "Product");
        }

        private static async Task CallGenericAsync<TRequest>(
        TRequest dto,
        string messageType,
        string targetService)
        {
            var base64Client = "eyJQcmVkaWNhdGVzIjp7Il9fdHlwZSI6IkxhbWJkYUV4cHJlc3Npb25Ob2RlOiNTZXJpYWxpemUuTGlucS5Ob2RlcyIsIk5vZGVUeXBlIjoxOCwiVHlwZSI6eyJHZW5lcmljQXJndW1lbnRzIjpbeyJOYW1lIjoiVE1ULkV4dGVybmFsLkdycGMuUHJvZHVjdCJ9LHsiTmFtZSI6IlN5c3RlbS5Cb29sZWFuIn1dLCJOYW1lIjoiU3lzdGVtLkZ1bmNcdTAwNjAyIn0sIkJvZHkiOnsiX190eXBlIjoiQmluYXJ5RXhwcmVzc2lvbk5vZGU6I1NlcmlhbGl6ZS5MaW5xLk5vZGVzIiwiTm9kZVR5cGUiOjEzLCJUeXBlIjp7Ik5hbWUiOiJTeXN0ZW0uQm9vbGVhbiJ9LCJMZWZ0Ijp7Il9fdHlwZSI6Ik1lbWJlckV4cHJlc3Npb25Ob2RlOiNTZXJpYWxpemUuTGlucS5Ob2RlcyIsIk5vZGVUeXBlIjoyMywiVHlwZSI6eyJOYW1lIjoiU3lzdGVtLkludDY0In0sIkV4cHJlc3Npb24iOnsiX190eXBlIjoiUGFyYW1ldGVyRXhwcmVzc2lvbk5vZGU6I1NlcmlhbGl6ZS5MaW5xLk5vZGVzIiwiTm9kZVR5cGUiOjM4LCJUeXBlIjp7Ik5hbWUiOiJUTVQuRXh0ZXJuYWwuR3JwYy5Qcm9kdWN0In0sIk5hbWUiOiJwciJ9LCJNZW1iZXIiOnsiRGVjbGFyaW5nVHlwZSI6eyJOYW1lIjoiVE1ULkV4dGVybmFsLkdycGMuUHJvZHVjdCJ9LCJTaWduYXR1cmUiOiJJbnQ2NCBJZCJ9fSwiTWV0aG9kIjp7fSwiUmlnaHQiOnsiX190eXBlIjoiQ29uc3RhbnRFeHByZXNzaW9uTm9kZTojU2VyaWFsaXplLkxpbnEuTm9kZXMiLCJOb2RlVHlwZSI6OSwiVHlwZSI6eyJOYW1lIjoiU3lzdGVtLkludDY0In0sIlZhbHVlIjo4NDI1MzM0OTE4MzQ4ODB9fSwiUGFyYW1ldGVycyI6W3siX190eXBlIjoiUGFyYW1ldGVyRXhwcmVzc2lvbk5vZGU6I1NlcmlhbGl6ZS5MaW5xLk5vZGVzIiwiTm9kZVR5cGUiOjM4LCJUeXBlIjp7Ik5hbWUiOiJUTVQuRXh0ZXJuYWwuR3JwYy5Qcm9kdWN0In0sIk5hbWUiOiJwciJ9XX19";
            var bytesClient = Convert.FromBase64String(base64Client);
            var dataClient = ByteString.CopyFrom(bytesClient);

            var request = new GenericRequest
            {
                MessageType = messageType,
                Data = dataClient
            };

            var headers = CreateHeaders(targetService);
            var result = await _pool.ExecuteWithPolicyAsync(BaseAddress, async channel =>
            {
                var client = new GenericGrpcService.GenericGrpcServiceClient(channel);
                return await client.SendGenericMessageAsync(request, headers);
            });

            Console.WriteLine(result);

            if (result.Data != null && !result.Data.IsEmpty)
            {
                Console.WriteLine($"[{targetService}] [{messageType}] Data JSON:");
                Console.WriteLine(result.Data.ToStringUtf8());
            }

            Console.WriteLine(new string('-', 100));
        }

        private static Metadata CreateHeaders(string targetService)
        {
            return new Metadata
            {
                { "Authorization", $"Bearer {Token}" },
                { "x-target-service", targetService }, // ví dụ: "Customer" / "Authentication" / "Product"
                { "tenant", TenantId }
            };
        }
    }
}
