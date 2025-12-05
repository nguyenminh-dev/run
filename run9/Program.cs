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
                // Serialize expression bằng Serialize.Linq
                var jsonExpr = _serializer.SerializeText(lambdaProp);

                // Parse lại JSON để ghi trực tiếp vào object
                using var exprDoc = System.Text.Json.JsonDocument.Parse(jsonExpr);
                var exprElement = exprDoc.RootElement;

                using var stream = new System.IO.MemoryStream();
                using (var writer = new System.Text.Json.Utf8JsonWriter(stream, new System.Text.Json.JsonWriterOptions { Indented = false }))
                {
                    writer.WriteStartObject();

                    foreach (var prop in type.GetProperties())
                    {
                        if (prop == exprProp) continue;

                        var value = prop.GetValue(request);
                        writer.WritePropertyName(prop.Name);
                        if (value == null)
                        {
                            writer.WriteNullValue();
                        }
                        else
                        {
                            System.Text.Json.JsonSerializer.Serialize(writer, value, prop.PropertyType);
                        }
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
    private const string BaseAddress = "http://localhost:21001";
    private const string Token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkM4RkFBMTA2OERFNjA4OEE1Mjk3OEQ1OUJBQUZGRjMzIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3NjQ3MzE0ODUsImV4cCI6MTc2NTMzNjI4NSwiaXNzIjoiaHR0cHM6Ly9hY2NvdW50LnB1YmxpYy5ya2UuYXBwLmRldi50bXRjby5vcmciLCJhdWQiOlsiQWNjb3VudEFwcCIsIkxveWFsdHlBcHAiLCJUUG9zQXBwIl0sImNsaWVudF9pZCI6IkxveWFsdHlTZXJ2ZXJDbGllbnQiLCJhcHAiOiJMb3lhbHR5QXBwIiwiY3VycmVudF9hcHAiOiIiLCJpYXQiOjE3NjQ3MzE0ODUsInNjb3BlIjpbIkFjY291bnRBcHAiLCJMb3lhbHR5QXBwIiwiVFBvc0FwcCJdfQ.FU8i_b9lNMg33_xRM4R9HR5_8T0DBCXKwnYYdLxNuQXrBAaIqEf6ACIPPbHSDF3ydtqcMalbiwIlAZ2zTsGMoYxpDrdCJYZY0EMEMiS_S6rmB9yJ3iHEm_u42pOtBiLiuF7ijGWdA2zB8CmK6N7j7HaOWB00wnwLB3KrKuj84NhP3zG_Y05skhoeC_3pfVrDWTny3yoUVx6Knvfar-r2j3TxKCrg_Gvb7gcXB2-kM4VNNaSUyRLI3ivAMY3uCJYUEg05kKxFvPe27ZN-7EzosxQk5ImZ-PnHvHllgq5oqfnfaJ8fmTaAy6_Rpz5RBK_GmnSP5KCfYNKlKUVlie0T3Q";
    private const string TenantId = "888906634625024";

    //private const string BaseAddress = "https://gateway.dev-v3.wionpos.public.rke.app.dev.tmtco.org";
    //private const string Token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkM4RkFBMTA2OERFNjA4OEE1Mjk3OEQ1OUJBQUZGRjMzIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3NjQyOTcwMzQsImV4cCI6MTc2Njg4OTAzNCwiaXNzIjoiaHR0cHM6Ly9hY2NvdW50LnB1YmxpYy5ya2UuYXBwLmRldi50bXRjby5vcmciLCJhdWQiOlsiQWNjb3VudEFwcCIsIkxveWFsdHlBcHAiLCJUaWNUaWNBcHAiLCJUUG9zQXBwIl0sImNsaWVudF9pZCI6IkxveWFsdHlVc2VyQ2xpZW50IiwiYXBwIjoiTG95YWx0eUFwcCIsImN1cnJlbnRfYXBwIjoiIiwidGVuYW50aWQiOiI4NjQ5MzQ2NTM1OTE1NTIiLCJyb2xlIjoiODY0OTM0NjYyNzAxMDU3Iiwic3ViIjoiODQyNTMyOTEzMjE3NTM2IiwiYXV0aF90aW1lIjoxNzY0Mjk3MDM0LCJpZHAiOiJsb2NhbCIsImVtYWlsIjoiODQyNTMyOTEzMjE3NTM2QHRtdC5jb20iLCJwaG9uZV9udW1iZXIiOiIrODQzNjI1MzA3MDAiLCJwaG9uZV9udW1iZXJfdmVyaWZpZWQiOiJUcnVlIiwiaWF0IjoxNzY0Mjk3MDM0LCJzY29wZSI6WyJBY2NvdW50QXBwIiwiTG95YWx0eUFwcCIsIlRpY1RpY0FwcCIsIlRQb3NBcHAiLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsicHdkIl19.Qf6SPgJJ_N0TbAeoocCN--tupeJ_FzxPGNT2s1qYAX7lvn8tlu5GJHWPX4PgMVjiLT5LgqYGP00HK9Ya_5MA0fQixHGz6qJb1tUdBBexEZxf5GiRWLLNiC-jq0SKxxuw3U5jzo_tozSoNDXiDRYVYrltKW0xM6ByoR9rid0QqkFoJ-zJ_jSd-uSSwLXLEa7zvqcN6D4lHOSbQnXDU7q4U8ZCSucLCjoS1DL-EED9Ja08wOAoouBwOI0lQ9cudx1VL4ni_9hd1ekgWuHD26LWdHy6TgRLNZ5ZPyKDUPlfd0f8XRmsv4el4G3ZXKWwh7lt2syRATxvEonGjI799OOUeg";
    //private const string TenantId = "864934653591552";

    private static readonly GrpcChannelPool _pool = new();

    public static async Task Main()
    {
        for (int i = 0; i < 10; i++)
        {
            var id = Guid.NewGuid();
            Console.WriteLine(id);
        }
        //await TestStoreByPredicate();
        await TestProductByPredicate();
    }

    private static Metadata CreateHeaders(string targetService)
    {
        return new Metadata
        {
            { "authorization", $"bearer {Token}" },
            { "x-target-service", targetService }, // ví dụ: "Customer" / "Authentication" / "Product"
            { "tenant", TenantId }
        };
    }

    public static T DeserializeRequest<T>(ByteString bytes)
    {
        var json = bytes.ToStringUtf8();

        var requestType = typeof(T);
        var exprProps = requestType.GetProperties()
            .Where(p => typeof(System.Linq.Expressions.LambdaExpression).IsAssignableFrom(p.PropertyType))
            .ToList();

        T typedRequest;

        if (exprProps.Count == 0)
        {
            typedRequest = (T)System.Text.Json.JsonSerializer.Deserialize(json, requestType);
        }
        else
        {
            typedRequest = (T)ExpressionAwareDeserialize(json, requestType, exprProps);
        }

        return typedRequest;
    }

    public static object? ExpressionAwareDeserialize(
            string json,
            Type requestType,
            List<System.Reflection.PropertyInfo> exprProps)
    {
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        var root = doc.RootElement;

        var instance = Activator.CreateInstance(requestType);
        var exprSerializer = new Serialize.Linq.Serializers.ExpressionSerializer(
            new Serialize.Linq.Serializers.JsonSerializer());

        foreach (var prop in requestType.GetProperties())
        {
            if (!root.TryGetProperty(prop.Name, out var element) || element.ValueKind == System.Text.Json.JsonValueKind.Null)
                continue;

            try
            {
                if (typeof(System.Linq.Expressions.LambdaExpression).IsAssignableFrom(prop.PropertyType))
                {
                    var exprJson = element.GetRawText();
                    var expr = exprSerializer.DeserializeText(exprJson) as System.Linq.Expressions.LambdaExpression;
                    prop.SetValue(instance, expr);
                }
                else
                {
                    var val = System.Text.Json.JsonSerializer.Deserialize(element.GetRawText(), prop.PropertyType);
                    prop.SetValue(instance, val);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        return instance;
    }

    private static async Task CallGenericAsync<TRequest>(
        TRequest dto,
        string messageType,
        string targetService)
    {
        //var base64Client = "eyJQcmVkaWNhdGVzIjp7Il9fdHlwZSI6IkxhbWJkYUV4cHJlc3Npb25Ob2RlOiNTZXJpYWxpemUuTGlucS5Ob2RlcyIsIk5vZGVUeXBlIjoxOCwiVHlwZSI6eyJHZW5lcmljQXJndW1lbnRzIjpbeyJOYW1lIjoiVE1ULkV4dGVybmFsLkdycGMuUHJvZHVjdCJ9LHsiTmFtZSI6IlN5c3RlbS5Cb29sZWFuIn1dLCJOYW1lIjoiU3lzdGVtLkZ1bmNcdTAwNjAyIn0sIkJvZHkiOnsiX190eXBlIjoiQmluYXJ5RXhwcmVzc2lvbk5vZGU6I1NlcmlhbGl6ZS5MaW5xLk5vZGVzIiwiTm9kZVR5cGUiOjEzLCJUeXBlIjp7Ik5hbWUiOiJTeXN0ZW0uQm9vbGVhbiJ9LCJMZWZ0Ijp7Il9fdHlwZSI6Ik1lbWJlckV4cHJlc3Npb25Ob2RlOiNTZXJpYWxpemUuTGlucS5Ob2RlcyIsIk5vZGVUeXBlIjoyMywiVHlwZSI6eyJOYW1lIjoiU3lzdGVtLkludDY0In0sIkV4cHJlc3Npb24iOnsiX190eXBlIjoiUGFyYW1ldGVyRXhwcmVzc2lvbk5vZGU6I1NlcmlhbGl6ZS5MaW5xLk5vZGVzIiwiTm9kZVR5cGUiOjM4LCJUeXBlIjp7Ik5hbWUiOiJUTVQuRXh0ZXJuYWwuR3JwYy5Qcm9kdWN0In0sIk5hbWUiOiJwciJ9LCJNZW1iZXIiOnsiRGVjbGFyaW5nVHlwZSI6eyJOYW1lIjoiVE1ULkV4dGVybmFsLkdycGMuUHJvZHVjdCJ9LCJTaWduYXR1cmUiOiJJbnQ2NCBJZCJ9fSwiTWV0aG9kIjp7fSwiUmlnaHQiOnsiX190eXBlIjoiQ29uc3RhbnRFeHByZXNzaW9uTm9kZTojU2VyaWFsaXplLkxpbnEuTm9kZXMiLCJOb2RlVHlwZSI6OSwiVHlwZSI6eyJOYW1lIjoiU3lzdGVtLkludDY0In0sIlZhbHVlIjo4NDI1MzM0OTE4MzQ4ODB9fSwiUGFyYW1ldGVycyI6W3siX190eXBlIjoiUGFyYW1ldGVyRXhwcmVzc2lvbk5vZGU6I1NlcmlhbGl6ZS5MaW5xLk5vZGVzIiwiTm9kZVR5cGUiOjM4LCJUeXBlIjp7Ik5hbWUiOiJUTVQuRXh0ZXJuYWwuR3JwYy5Qcm9kdWN0In0sIk5hbWUiOiJwciJ9XX19";
        //var bytesClient = Convert.FromBase64String(base64Client);
        //var dataClient = ByteString.CopyFrom(bytesClient);

        //var dtoclient = DeserializeRequest<TRequest>(dataClient);
        //var dtoServer = DeserializeRequest<TRequest>(data);

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
            p => p.Id == 8889075861422081;

        var dto = new ProductGrpcRequestDto
        {
            Predicates = predicate
        };

        return CallGenericAsync(dto, nameof(ProductGrpcRequestDto), "Product");
    }
}
