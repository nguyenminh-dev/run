using Gateway.API;
using Google.Protobuf;
using Grpc.Core;
using run;
using Serialize.Linq.Serializers;
using System.Linq.Expressions;
using TMT.External.Grpc;

public static class ExpressionHelper
{
    private static readonly ExpressionSerializer _serializer =
        new ExpressionSerializer(new JsonSerializer());

    public static byte[] SerializeRequest<TRequest>(TRequest request)
    {
        var type = typeof(TRequest);

        // Tìm property kiểu Expression<Func<...>>
        var exprProp = type.GetProperties()
            .FirstOrDefault(p => typeof(LambdaExpression).IsAssignableFrom(p.PropertyType));

        if (exprProp != null)
        {
            var lambdaProp = exprProp.GetValue(request) as LambdaExpression;
            if (lambdaProp != null)
            {
                // Serialize expression riêng bằng Serialize.Linq
                var jsonExpr = _serializer.SerializeText(lambdaProp);

                // Parse lại expression JSON để ghi trực tiếp
                using var exprDoc = System.Text.Json.JsonDocument.Parse(jsonExpr);
                var exprElement = exprDoc.RootElement;

                // Serialize các property khác + Expression thẳng JSON object
                using var stream = new System.IO.MemoryStream();
                using (var writer = new System.Text.Json.Utf8JsonWriter(stream))
                {
                    writer.WriteStartObject();

                    foreach (var prop in type.GetProperties())
                    {
                        if (prop == exprProp) continue;
                        var value = prop.GetValue(request);
                        writer.WritePropertyName(prop.Name);
                        System.Text.Json.JsonSerializer.Serialize(writer, value, prop.PropertyType);
                    }

                    writer.WritePropertyName(exprProp.Name);
                    exprElement.WriteTo(writer); // Ghi Expression nguyên bản

                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }
        // Không có Expression → serialize bình thường
        return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(request);
    }
}

public class Program
{
    //private const string BaseAddress = "http://localhost:21001";
    //private const string Token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkM4RkFBMTA2OERFNjA4OEE1Mjk3OEQ1OUJBQUZGRjMzIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3NjQxNDUyNDEsImV4cCI6MTc2NjczNzI0MSwiaXNzIjoiaHR0cHM6Ly9hY2NvdW50LnB1YmxpYy5ya2UuYXBwLmRldi50bXRjby5vcmciLCJhdWQiOlsiQWNjb3VudEFwcCIsIkxveWFsdHlBcHAiLCJUaWNUaWNBcHAiLCJUUG9zQXBwIl0sImNsaWVudF9pZCI6IkxveWFsdHlVc2VyQ2xpZW50IiwiYXBwIjoiTG95YWx0eUFwcCIsImN1cnJlbnRfYXBwIjoiVFBvc1VzZXJDbGllbnQiLCJ0ZW5hbnRpZCI6Ijg4Mzk4NDE1NTM0NDg5NiIsInJvbGUiOiI4ODM5ODQxNTkxNDU5ODUiLCJzdWIiOiIxMTU3Mzk3NzEzNzE1OCIsImF1dGhfdGltZSI6MTc2NDE0NTI0MSwiaWRwIjoibG9jYWwiLCJlbWFpbCI6Im1pbmhudkB0bXRjby5hc2lhIiwicGhvbmVfbnVtYmVyIjoiKzg0MzYyNTMwMzkzIiwicGhvbmVfbnVtYmVyX3ZlcmlmaWVkIjoiVHJ1ZSIsImlhdCI6MTc2NDE0NTI0MSwic2NvcGUiOlsiQWNjb3VudEFwcCIsIkxveWFsdHlBcHAiLCJUaWNUaWNBcHAiLCJUUG9zQXBwIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbInB3ZCJdfQ.tB-ujpFMvuNhrAiK0iKASeQKXl0YAwn42P0CLKUZATxuZvrLCMASHhHqsiVLgfPoSmX5wRcqXXOKU0zICmQxymIspVnpAxLUk1vSej0gmZ-1ZRdGQx8vKbSsb4WfJuQhkkO6ISp8PRARG8KTiUwgpIPBMimm9Y5KRY4J4W2KDGxnPphW_U9n0_A0pJN104_gkaVfWW7VRXCatAZ3DQt6HBzplo9oiQie0jyCeA9A0lT1h_g0JQgpSc8c-xGJnRoI4NtqN4LEPR-fNxqNDiISsB08DNQq3G6Wd0j2S3rMjesFu7akjMxWO2mnMTQVJ9-fzBq5jmkfeb1JfqrX5ywOxA";
    //private const string TenantId = "883984155344896";

    private const string BaseAddress = "https://gateway.dev-v3.wionpos.public.rke.app.dev.tmtco.org/";
    private const string Token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkM4RkFBMTA2OERFNjA4OEE1Mjk3OEQ1OUJBQUZGRjMzIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3NjQxNDU3MzAsImV4cCI6MTc2NjczNzczMCwiaXNzIjoiaHR0cHM6Ly9hY2NvdW50LnB1YmxpYy5ya2UuYXBwLmRldi50bXRjby5vcmciLCJhdWQiOlsiQWNjb3VudEFwcCIsIkxveWFsdHlBcHAiLCJUaWNUaWNBcHAiLCJUUG9zQXBwIl0sImNsaWVudF9pZCI6IkxveWFsdHlVc2VyQ2xpZW50IiwiYXBwIjoiTG95YWx0eUFwcCIsImN1cnJlbnRfYXBwIjoiVFBvc1VzZXJDbGllbnQiLCJ0ZW5hbnRpZCI6Ijg2NDkzNDY1MzU5MTU1MiIsInJvbGUiOiI4NjQ5MzQ2NjI3MDEwNTciLCJzdWIiOiI4NDI1MzI5MTMyMTc1MzYiLCJhdXRoX3RpbWUiOjE3NjE4NzQzNjYsImlkcCI6ImxvY2FsIiwiZW1haWwiOiI4NDI1MzI5MTMyMTc1MzZAdG10LmNvbSIsInBob25lX251bWJlciI6Iis4NDM2MjUzMDcwMCIsInBob25lX251bWJlcl92ZXJpZmllZCI6IlRydWUiLCJpYXQiOjE3NjE4NzQzNjYsInNjb3BlIjpbIkFjY291bnRBcHAiLCJMb3lhbHR5QXBwIiwiVGljVGljQXBwIiwiVFBvc0FwcCIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJwd2QiXX0.l5QhkWRO0Iy0m6kiuC-yxPmE5NL3fBRLKfG5XGPsH5vpmQN0Pm7vLLnfstS3keZ80PsuNNdG_nC5TG1xeXtaalQwo6t_vjSNp95e8rhzqjRp-YL3Exs4Pt5iDOgJhdtq8wgx8BQr7Yx9sMDX3WtwQtvtf7prv4iyyxLnRt6khwSLsmN7sTJUASWoFbLMIJ50k0wl7B5C8ddNGZFVIPTxf_7FIycM2LqeKNKM9l3xv3bXjAYTGVrbLghKOMYHJBjfM0OLeqyuDPDgUMYdBFVDeo881vVi_ZsUeHJ2mKyZLW3jMGRYUEX18nn_8GlWVPrQrH8_szwgwHnXMwpBprvsGg";
    private const string TenantId = "864934653591552";
    private static readonly GrpcChannelPool _pool = new();

    public static async Task Main()
    {
        await TestStoreByPredicate();
        //await TestProductByPredicate();
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

    private static async Task CallGenericAsync<TRequest>(
        TRequest dto,
        string messageType,
        string targetService)
    {
        var bytes = ExpressionHelper.SerializeRequest(dto);
        var data = ByteString.CopyFrom(bytes);

        var request = new GenericRequest
        {
            MessageType = messageType,
            Data = data
        };

        var headers = CreateHeaders(targetService);

        var result = await _pool.ExecuteWithPolicyAsync(BaseAddress, async channel =>
        {
            var client = new GenericGrpcService.GenericGrpcServiceClient(channel);
            var forwardHeaders = new Metadata();
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

    public static Task TestStoreByPredicate()
    {
        Expression<Func<Store, bool>> predicate =
            s => s.IsActive && s.Name.Contains("Minh") && s.Description.Contains("Quần áo");

        var dto = new StoreGrpcRequestDto
        {
            Predicates = predicate
        };

        // targetService tuỳ cấu hình gateway, ở đây ví dụ: "Authentication"
        return CallGenericAsync(dto, "StoreGrpcRequestDto", "Authentication");
    }

    // ---------- Product ----------
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
}
