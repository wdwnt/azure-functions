#load "..\Shared\AirtableHandler.csx"
#load "..\Shared\NotificationManager.csx"
#load "RecordRetriever.csx"
#load "Sfwmd.csx"
#load "SfwmdMap.csx"
#load "..\Shared\Utility.csx"

using AirtableApiClient;

private static TraceWriter _logger;
private static AirtableHandler _airtableHandler;
private static NotificationManager _notificationManager;

public static async Task Run(TimerInfo myTimer, TraceWriter log)
{
    var baseKey = Utility.GetEnvironmentVariable("Airtable_BaseKey");
    var tableName = Utility.GetEnvironmentVariable("Airtable_TableName");
    _airtableHandler = new AirtableHandler(log, baseKey, tableName);
    _notificationManager = new NotificationManager(log);

    _logger = log;
    _logger.Info($"Start time: {DateTime.Now}");

    var results = RecordRetriever.GetSfwmdResults(DateTime.Now.AddDays(-5), DateTime.Now).ToList();
    _logger.Info($"Results Found: {results.Count}");

    foreach (var result in results)
    {
        if (!(await _airtableHandler.RecordExists("ApplicationNumber", result.ApplicationNum)))
        {
            _logger.Info($"New result found: {result.ApplicationNum}");
        
            var fields = new Fields();
            fields.AddField("ApplicationNumber", result.ApplicationNum);
            fields.AddField("Received", result.ReceivedDate);
            fields.AddField("Approved", result.ApprovedDate);
            fields.AddField("ProjectName", result.ProjectName);
            fields.AddField("SourceId", result.PermitNumber);
            await _airtableHandler.AddRecordToTable(fields);

            var message = $"*SFWMD Update*\nApplication Number: {result.ApplicationNum}\nProject Name: {result.ProjectName}\n<http://apps.sfwmd.gov/WAB/ePermittingWebApp/index.html?mobileBreakPoint=300&slayer=0&exprnum=0&esearch={result.ApplicationNum}|Details>";
            await _notificationManager.SendSlackNotification(message, "potable_water");
        }
    }
}
