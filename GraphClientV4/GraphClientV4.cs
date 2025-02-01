using Azure.Core;
using Azure.Identity;
using GraphClientSettings;
using GraphHandlers;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client.Advanced;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensibility;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;
using static System.Formats.Asn1.AsnWriter;

namespace GraphClientV4;

public class GraphClientV4
{
	private readonly GraphServiceClient client;
	private readonly ILogger<GraphClientV4> logger;
	public GraphClientV4(TokenCredential tokenCredential)
	{

		// Create an authentication context.

		using ILoggerFactory factory = LoggerFactory.Create(builder => { });
		logger = factory.CreateLogger<GraphClientV4>();
		ILogger<LoggingHandler> logHandler = new LoggerFactory().CreateLogger<LoggingHandler>();
		var handlers = GraphClientFactory.CreateDefaultHandlers(null);
		handlers.Add(new LoggingHandler(logHandler));

		var httpClient = GraphClientFactory.Create(handlers);

		var httpProvider = new SimpleHttpProvider(httpClient);
		client = new GraphServiceClient(tokenCredential, new[] {"https://graph.microsoft.com/.default"}, httpProvider);
		//client = new GraphServiceClient(httpClient: httpClient, tokenCredential: tokenCredential, new[] { "https://graph.microsoft.com/.default" });

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
	public async Task<object> SendEmail()
	{
		var msg = GetMessageTemplate();

		var a = await client.Users[msg!.Sender!.EmailAddress!.Address!].Messages.Request().GetAsync();

		return a!.FirstOrDefault()!;
	}


	private Message GetMessageTemplate(int count = 0)
	{
		return new Message()
		{
			Subject = $"Multi-Attachment Test# {count}",
			ToRecipients = new List<Recipient>() {
				new Recipient()
				{
					EmailAddress= new EmailAddress() {Address = "leeloo@bullhornblog.com"}
				}
			},
			Sender = new Recipient() { EmailAddress = new EmailAddress() { Address = "rambler@bullhornblog.com" } },
			Body = new ItemBody()
			{
				ContentType = BodyType.Html,
				Content = $"Greetings </br> This is a test email to reproduce a 404 error on multi-attachment upload."

			},
			IsDraft = true,
			HasAttachments = false,
		};
	}
}