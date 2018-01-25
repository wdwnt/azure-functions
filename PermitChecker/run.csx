#load "AirtableHandler.csx"
#load "NotificationManager.csx"
#load "RecordRetriever.csx"
#load "Sfwmd.csx"
#load "SfwmdMap.csx"

private static TraceWriter _logger;
private static AirtableHandler _airtableHandler;
private static NotificationManager _notificationManager;

public static async Task Run(TimerInfo myTimer, TraceWriter log)
{
    _airtableHandler = new AirtableHandler(log);
    _notificationManager = new NotificationManager(log);

    _logger = log;
    _logger.Info($"Start time: {DateTime.Now}");

    var results = RecordRetriever.GetSfwmdResults(DateTime.Now.AddDays(-22), DateTime.Now).ToList();

    foreach (var result in results)
    {
        if (!(await _airtableHandler.RecordExists(result)))
        {
            _logger.Info($"New result found: {result.ApplicationNum}");
            await _airtableHandler.AddRecordToTable(result);
            await _notificationManager.SendNotifications(result);
        }
    }
}
