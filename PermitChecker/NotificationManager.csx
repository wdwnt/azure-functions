#load "Sfwmd.csx"
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

    public async Task SendNotifications(Sfwmd result)
    {
        await SendSlackNotification(result);
    }

    private async Task SendSlackNotification(Sfwmd result)
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
            text = FormatMessageBody(result),
            icon_emoji = ":potable_water:"
        });

        var response = restClient.Execute(webhookRequest);
        Console.WriteLine(response.Content);
    }

    private static string FormatMessageBody(Sfwmd result)
    {
        return $"*SFWMD Update*\nApplication Number: {result.ApplicationNum}\nProject Name: {result.ProjectName}";
    }
}
