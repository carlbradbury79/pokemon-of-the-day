using Amazon.Lambda.CloudWatchEvents;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PokemonScheduler;

public class Function
{
    private static readonly Random _random = new Random();
    private readonly HttpClient _http;
    private readonly IAmazonS3 _s3;

    public Function()
    {
        _http = new HttpClient();
        _s3 = new AmazonS3Client();
    }

    public async Task FunctionHandler(CloudWatchEvent<object> evt, ILambdaContext ctx)
    {
        try
        {
            var id = _random.Next(1, 1026);

            var response = await _http.GetStringAsync($"https://pokeapi.co/api/v2/pokemon/{id}");

            var key = $"{DateTime.UtcNow:yyyy-MM-dd}.json";
            var bucketName = Environment.GetEnvironmentVariable("BUCKET_NAME");

            if (string.IsNullOrEmpty(bucketName))
            {
                ctx.Logger.LogError("BUCKET_NAME environment variable is not set.");
                return;
            }

            await _s3.PutObjectAsync(new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                ContentBody = response,
                ContentType = "application/json"
            });

            ctx.Logger.LogInformation($"Saved Pokémon {id} as {key}");
        }
        catch (Exception e)
        {
            ctx.Logger.LogError($"An error occurred: {e.Message}");

            throw;
        }
    }
}
