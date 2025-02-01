using Azure.Core;
using Microsoft.Graph;
using Polly;
using Polly.Extensions.Http;

namespace GraphClientV3;

public class GraphClientV3
{
	public GraphClientV3(IAuthenticationProvider authenticationProvider)
	{


		var handlers = GraphClientFactory.CreateDefaultHandlers();
		handlers.Add(new PolicyHttpMessageHandler(GetRetryPolicy()));
		var httpClient = GraphClientFactory.Create(handlers);
		client = new GraphServiceClient(authenticationProvider, httpClient);
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