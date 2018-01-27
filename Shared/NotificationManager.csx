#load "Utility.csx"

using RestSharp;

public class NotificationManager
{
    private TraceWriter _logger;
    private string _slackWebhookUrl;

    public NotificationManager(TraceWriter logger)
    {
        _logger = logger;
        _slackWebhookUrl = Utility.GetEnvironmentVariable("SlackWebhookUrl");
    }
    
    public async Task SendSlackNotification(string message, string iconEmoji)
    {
        var restClient = new RestClient
        {
            BaseUrl = new Uri("https://hooks.slack.com")
        };

        var webhookRequest = new RestRequest(Method.POST)
        {
            Resource = _slackWebhookUrl
        };

        webhookRequest.RequestFormat = DataFormat.Json;
        webhookRequest.AddBody(new {
            text = message,
            icon_emoji = $":{iconEmoji}:"
        });

        var response = restClient.Execute(webhookRequest);
        _logger.Info(response.Content);
    }
    
    public async Task SendSfwmdSlackNotification(string message)
    {
        await SendSlackNotification(message, "potable_water");
    }
}
