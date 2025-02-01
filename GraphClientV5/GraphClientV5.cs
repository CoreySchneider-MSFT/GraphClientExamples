
using Azure.Core;
using GraphHandlers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using Microsoft.Graph;
using Microsoft.Graph.Users.Item.Messages.Item.Attachments.CreateUploadSession;
using Polly.Extensions.Http;
using Polly;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Microsoft.Kiota.Abstractions;

namespace GraphClientV5;

public class GraphClientV5
{
	private readonly GraphServiceClient client;
	private readonly ILogger<GraphClientV5> logger;
	private readonly Random attachmentSeed = new Random();

	/// <summary>
	/// Contructor for Graph service client v5. Initializes the GraphServiceClient object using GraphSDK Microsoft.Graph v5.x
	/// <br></br>
	/// <see cref="Microsoft.Graph.GraphServiceClient"/>
	/// </summary>
	/// <param name="tokenCredential"></param>
	public GraphClientV5(TokenCredential tokenCredential)
	{
		var authProvider = new Microsoft.Graph.Authentication.AzureIdentityAuthenticationProvider(tokenCredential, new[] { "https://graph.microsoft.com/.default" });
		// get the default list of handlers and add the logging handler to the list
		var handlers = GraphClientFactory.CreateDefaultHandlers();
		using ILoggerFactory factory = LoggerFactory.Create(builder => { });
		ILogger logHandler = factory.CreateLogger<GraphHandlers.LoggingHandler>();
		logger = factory.CreateLogger<GraphClientV5>();

		// add custom log handler to log request headers.
		handlers.Add(new LoggingHandler((ILogger<GraphHandlers.LoggingHandler>)logHandler));

		//handlers.Add(new RetryHandler(retrylogHandler));
		handlers.Add(new PolicyHttpMessageHandler(
			HttpPolicyExtensions
			.HandleTransientHttpError()
			.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
			.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
			.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.BadGateway)
			.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
			.WaitAndRetryAsync(8, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
		));
		// initialize httpClient
		var httpClient = GraphClientFactory.Create(handlers);
		//initialize the GraphServiceClient
		client = new GraphServiceClient(httpClient: httpClient, tokenCredential: tokenCredential, new[] { "https://graph.microsoft.com/.default" });
	}
	static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
	{
		return HttpPolicyExtensions
			.HandleTransientHttpError()
			.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
			.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
			.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.BadGateway)
			.WaitAndRetryAsync(8, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
	}

	
}