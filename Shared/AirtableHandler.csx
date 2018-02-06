using AirtableApiClient;

public class AirtableHandler
{
    private TraceWriter _logger;
    private string _airtableTableName;
    private AirtableBase _airtableBase;

    public AirtableHandler(TraceWriter logger, string baseKey, string tableName)
    {
        _logger = logger;
        var airtableAppKey = Utility.GetEnvironmentVariable("Airtable_ApiKey");
        _airtableTableName = tableName;
        _airtableBase = new AirtableBase(airtableAppKey, baseKey);
    }

    public async Task<bool> RecordExists(string property, string valueToMatch)
    {
        var response = await _airtableBase.ListRecords(
            _airtableTableName,
            filterByFormula: $"{property}='{valueToMatch}'"
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

    public async Task AddRecordToTable(Fields fields)
    {
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
