#load "..\Shared\AirtableHandler.csx"
#load "DisneyAppJson.csx"
#load "..\Shared\NotificationManager.csx"

using AirtableApiClient;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

private static TraceWriter _logger;
private static AirtableHandler _airtableHandler;
private static NotificationManager _notificationManager;

public static async void Run(TimerInfo myTimer, TraceWriter log)
{
    var baseKey = Utility.GetEnvironmentVariable("Airtable_MinnieVanBaseKey");
    var tableName = Utility.GetEnvironmentVariable("Airtable_MinnieVanTableName");
    _airtableHandler = new AirtableHandler(log, baseKey, tableName);
    _notificationManager = new NotificationManager(log);
    _logger = log;

    var resortList = new List<string>();
    var client = new RestClient("https://assets.adobedtm.com/b213090c5204bf94318f4ef0539a38b487d10368/scripts/satellite-546fba5c3765610015fd0200.json");
    var request = new RestRequest(Method.GET);
    var response = client.Execute(request);
    var responseObj = JsonConvert.DeserializeObject<RootObject>(response.Content);

    if (response.ResponseStatus != ResponseStatus.Completed) return;
    if (response.StatusCode == HttpStatusCode.OK)
    {
        var data = SearchForMinnieVanData(responseObj);
        foreach (Message message in data)
        {
            foreach (Trigger trigger in message.triggers.Where(a => a.key == "page.detailname" && a.matches == "co"))
            {
                foreach (string val in trigger.values)
                {
                    resortList.Add(val);
                }
            }
        }
    }

    var shouldSendMessage = false;

    foreach (var resort in resortList)
    {
        if (!(await _airtableHandler.RecordExists("ResortName", resort)))
        {
            shouldSendMessage = true;

            var fields = new Fields();
            fields.AddField("ResortName", resort);
            await _airtableHandler.AddRecordToTable(fields);
        }
    }

    if (shouldSendMessage)
    {
        var currentMinnieVanResorts = String.Join("\n", resortList.ToArray());
        var message = $"*Minnie Van List Update*\n{currentMinnieVanResorts}";
        await _notificationManager.SendSlackNotification(message, "minnievan");
    }
}

private static List<Message> SearchForMinnieVanData(RootObject root)
{
    return root.messages.Where(a => a.payload.html != null && a.payload.html.ToLower().Contains("minnie van")).ToList();
}
