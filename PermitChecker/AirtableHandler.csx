#load "..\Shared\Utility.csx"

using AirtableApiClient;

public class AirtableHandler
{
    private TraceWriter _logger;
    private string _airtableTableName;
    private AirtableBase _airtableBase;

    public AirtableHandler(TraceWriter logger)
    {
        _logger = logger;
        var airtableAppKey = Utility.GetEnvironmentVariable("Airtable_ApiKey");
        var airtableBaseKey = Utility.GetEnvironmentVariable("Airtable_BaseKey");
        _airtableTableName = Utility.GetEnvironmentVariable("Airtable_TableName");
        _airtableBase = new AirtableBase(airtableAppKey, airtableBaseKey);
    }

    public async Task<bool> RecordExists(Sfwmd result)
    {
        var response = await _airtableBase.ListRecords(
            _airtableTableName,
            filterByFormula: "{ApplicationNumber}=" + $"'{result.ApplicationNum}'"
        );

        if (response.Success) 
        {
            return response.Records.ToList().Count > 0;
        }
        else if (response.AirtableApiError is AirtableApiException)
        {
            _logger.Info($"Error: {response.AirtableApiError.ErrorMessage}");
            return false;
        }
        else
        {
            _logger.Info($"Failed response");
            return false;
        }
    }

    public async Task AddRecordToTable(Sfwmd result)
    {
        var fields = new Fields();
        fields.AddField("ApplicationNumber", result.ApplicationNum);
        fields.AddField("Received", result.ReceivedDate);
        fields.AddField("Approved", result.ApprovedDate);
        fields.AddField("ProjectName", result.ProjectName);
        fields.AddField("SourceId", result.PermitNumber);

        var response = await _airtableBase.CreateRecord(
            _airtableTableName, 
            fields);

        if (response.Success) 
        {
            _logger.Info($"Added record");
        }
        else if (response.AirtableApiError is AirtableApiException)
        {
            _logger.Info($"Error: {response.AirtableApiError.ErrorMessage}");
        }
        else
        {
            _logger.Info($"Failed response");
        }
    }
}
