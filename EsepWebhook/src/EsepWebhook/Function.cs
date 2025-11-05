using System.Net.Cache;
using System.Text;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    
    public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
    {
        context.Logger.LogInformation($"FunctionHandler received: {input}");

        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.Body);
        
        context.Logger.LogInformation($"{json}");

        string payload = $"{{'text':'Issue Created: {json.issue.html_url}'}}";

        var client = new HttpClient();
        var webRequest = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
    
        var response = client.Send(webRequest);
        using var reader = new StreamReader(response.Content.ReadAsStream());
        string readerBody = reader.ReadToEnd();
            
        return new APIGatewayProxyResponse {
            StatusCode = (int)response.StatusCode,
            Body = readerBody,
            Headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json"}
            },
            IsBase64Encoded = false
        };
    }

    
    /*
    public APIGatewayProxyResponse FunctionHandler(object input, ILambdaContext context)
    {
        context.Logger.LogInformation($"FunctionHandler received: {input}");

        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());



        // Testing from postman, you can use this code to test the function
        
        context.Logger.LogInformation($"Body: {json.body}");
        dynamic body = JsonConvert.DeserializeObject<dynamic>(json.body.ToString());
        context.Logger.LogInformation($"Issue: {body.issue}");
        context.Logger.LogInformation($"Html: {body.issue.html_url}");
        string payload = $"{{'text':'Issue Created: {body.issue.html_url}'}}";
        

        context.Logger.LogInformation($"{json.body}");

        string payload = $"{{'text':'Issue Created: {json.issue.html_url}'}}";

        var client = new HttpClient();
        var webRequest = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        var response = client.Send(webRequest);
        using var reader = new StreamReader(response.Content.ReadAsStream());
        string readerBody = reader.ReadToEnd();

        return new APIGatewayProxyResponse
        {
            StatusCode = (int)response.StatusCode,
            Body = readerBody,
            Headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json"}
            },
            IsBase64Encoded = false
        };
    }
    */
}