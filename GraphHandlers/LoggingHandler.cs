using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraphHandlers;

public class LoggingHandler : DelegatingHandler
{
	private readonly ILogger<LoggingHandler> _logger;

	public LoggingHandler(ILogger<LoggingHandler> logger)
	{
		_logger = logger;
	}

	/// <summary>
	/// Sends a HTTP request.
	/// </summary>
	/// <param name="httpRequest">The <see cref="HttpRequestMessage"/> to be sent.</param>
	/// <param name="cancellationToken">The <see cref="CancellationToken"/> for the request.</param>
	/// <returns></returns>
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken)
	{

		var jopt = new JsonSerializerOptions() { WriteIndented = true };

		// grab the outbound 'request'
		var sb = new StringBuilder();
		sb.AppendLine($"============[ Out-bound Request from App ]===============");
		sb.AppendLine($"{System.Text.Json.JsonSerializer.Serialize(httpRequest,jopt)}");
		sb.AppendLine($"============[ END Out-bound Request from App ]===============");
		_logger.LogInformation(sb.ToString());





		sb.Clear();
		// Always call base.SendAsync so that the request is forwarded through the pipeline.
		HttpResponseMessage response = await base.SendAsync(httpRequest, cancellationToken);

		// capture response from Microsoft Graph
		sb.AppendLine($"============[ In-bound Response from Graph ]===============");
		sb.AppendLine($"{System.Text.Json.JsonSerializer.Serialize(response,jopt)}");
		sb.AppendLine($"============[ END In-bound Response from Graph ]===============");
		if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
		{
			_logger.LogError(sb.ToString());
		}
		else
		{

			_logger.LogInformation(sb.ToString());
		}
		return response;
	}
}