using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphHandlers;

public class RetryHandler : DelegatingHandler
{
    private readonly ILogger<RetryHandler> _logger;
    private readonly int _maxRetries;

    public RetryHandler(ILogger<RetryHandler> logger, int maxRetries)
    {
        _logger = logger;
        _maxRetries = maxRetries;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = null!;
        int retryCount = 0;

        while (retryCount < _maxRetries)
        {
            try
            {
                response = await base.SendAsync(httpRequest, cancellationToken);
                response.EnsureSuccessStatusCode();
                if (!response.IsSuccessStatusCode)
                {
                    throw new WebException($"{response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }

                return response;
            }
            catch (WebException webEx)
            {
                _logger.LogError(webEx.Message);
                if (int.TryParse(webEx.Status.ToString(), out int statusCode))
                {
                    if (statusCode == 503)
                    {
                        _logger.LogInformation("Retrying request");
                        int timeToWait = 5; // Default wait time
                        if (response.Headers.TryGetValues("retry-after", out IEnumerable<string> retryAfter))
                        {
                            timeToWait = int.Parse(retryAfter.FirstOrDefault() ?? "5");
                        }
                        int delay = timeToWait * (int)Math.Pow(2, retryCount);
                        await Task.Delay(delay * 1000, cancellationToken);
                        retryCount++;
                    }
                    else
                    {
                        throw new WebException(webEx.Message);
                    }
                }
            }
        }

        throw new WebException("Max retries exceeded.");
    }
}
