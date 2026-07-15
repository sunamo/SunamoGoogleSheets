namespace SunamoGoogleSheets;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

public sealed class DriveSheetInfo
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTime? ModifiedTime { get; set; }
    public string WebViewLink { get; set; } = "";
}

public sealed class GoogleSheetsApiService(ILogger logger)
{
    private SheetsService? _sheetsService;
    private DriveService? _driveService;
    private string _applicationName = "SunamoGoogleSheets";

    public async Task<bool> InitializeAsync(string clientSecretsPath, string applicationName, bool forceRefresh = false)
    {
        try
        {
            _applicationName = applicationName;
            logger.LogInformation("Initializing Google Sheets API service for app '{App}'", applicationName);

            if (!File.Exists(clientSecretsPath))
            {
                logger.LogError("Client secrets file not found at: {Path}", clientSecretsPath);
                return false;
            }

            var credentialCachePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                applicationName,
                "GoogleSheetsAPI"
            );

            if (forceRefresh && Directory.Exists(credentialCachePath))
            {
                try
                {
                    Directory.Delete(credentialCachePath, true);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to clear OAuth cache");
                }
            }

            UserCredential credential;
            using (var stream = new FileStream(clientSecretsPath, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    new[] { SheetsService.Scope.Spreadsheets, DriveService.ScopeConstants.DriveFile },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credentialCachePath, true));
            }

            _sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });
            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });

            logger.LogInformation("Google Sheets + Drive API services initialized");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize Google Sheets API service");
            return false;
        }
    }

    public async Task<bool> WriteValuesToExistingAsync(string spreadsheetId, IList<IList<object>> values, string range = "Sheet1!A1:Z1000")
    {
        if (_sheetsService is null)
        {
            logger.LogError("Sheets service not initialized. Call InitializeAsync first.");
            return false;
        }
        try
        {
            var clearReq = _sheetsService.Spreadsheets.Values.Clear(new Google.Apis.Sheets.v4.Data.ClearValuesRequest(), spreadsheetId, range);
            await clearReq.ExecuteAsync();

            var anchor = range.Contains('!') ? range.Split('!')[0] + "!A1" : "A1";
            var valueRange = new Google.Apis.Sheets.v4.Data.ValueRange { Values = values };
            var updateReq = _sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, anchor);
            updateReq.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            await updateReq.ExecuteAsync();
            logger.LogInformation("Wrote {RowCount} rows to existing spreadsheet {Id}", values.Count, spreadsheetId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to write values to existing spreadsheet {Id}", spreadsheetId);
            return false;
        }
    }

    public async Task<IList<IList<object>>?> ReadValuesAsync(string spreadsheetId, string range)
    {
        if (_sheetsService is null)
        {
            logger.LogError("Sheets service not initialized. Call InitializeAsync first.");
            return null;
        }
        try
        {
            var resp = await _sheetsService.Spreadsheets.Values.Get(spreadsheetId, range).ExecuteAsync();
            return resp.Values;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to read values from spreadsheet {Id} range {Range}", spreadsheetId, range);
            return null;
        }
    }

    public async Task<(string? Id, string? Url)> CreateSpreadsheetInFolderAsync(string folderId, string title, IList<IList<object>> values)
    {
        if (_driveService is null || _sheetsService is null)
        {
            logger.LogError("Drive/Sheets service not initialized. Call InitializeAsync first.");
            return (null, null);
        }
        try
        {
            var fileMeta = new Google.Apis.Drive.v3.Data.File
            {
                Name = title,
                MimeType = "application/vnd.google-apps.spreadsheet",
                Parents = new List<string> { folderId },
            };
            var createReq = _driveService.Files.Create(fileMeta);
            createReq.Fields = "id, webViewLink";
            var created = await createReq.ExecuteAsync();
            var spreadsheetId = created.Id;
            var url = created.WebViewLink ?? $"https://docs.google.com/spreadsheets/d/{spreadsheetId}";
            logger.LogInformation("Created spreadsheet '{Title}' (id={Id}) in folder {FolderId}", title, spreadsheetId, folderId);

            if (values is { Count: > 0 })
            {
                var valueRange = new ValueRange { Values = values };
                var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, "A1");
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                await updateRequest.ExecuteAsync();
                logger.LogInformation("Wrote {RowCount} rows to spreadsheet '{Title}'", values.Count, title);
            }
            return (spreadsheetId, url);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create spreadsheet '{Title}' in folder {FolderId}", title, folderId);
            return (null, null);
        }
    }

    public async Task<List<DriveSheetInfo>?> ListSpreadsheetsInFolderAsync(string folderId)
    {
        if (_driveService is null)
        {
            logger.LogError("Drive service not initialized. Call InitializeAsync first.");
            return null;
        }
        try
        {
            var listReq = _driveService.Files.List();
            listReq.Q = $"'{folderId}' in parents and mimeType='application/vnd.google-apps.spreadsheet' and trashed=false";
            listReq.Fields = "files(id, name, modifiedTime, webViewLink)";
            listReq.OrderBy = "modifiedTime desc";
            listReq.PageSize = 100;
            var resp = await listReq.ExecuteAsync();
            var result = new List<DriveSheetInfo>();
            if (resp.Files is not null)
            {
                foreach (var f in resp.Files)
                {
                    result.Add(new DriveSheetInfo
                    {
                        Id = f.Id ?? "",
                        Name = f.Name ?? "",
                        ModifiedTime = f.ModifiedTimeDateTimeOffset?.UtcDateTime,
                        WebViewLink = f.WebViewLink ?? "",
                    });
                }
            }
            logger.LogInformation("Listed {Count} spreadsheets in folder {FolderId}", result.Count, folderId);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to list spreadsheets in folder {FolderId}", folderId);
            return null;
        }
    }

    public async Task<bool> DeleteFileAsync(string fileId)
    {
        if (_driveService is null)
        {
            logger.LogError("Drive service not initialized. Call InitializeAsync first.");
            return false;
        }
        try
        {
            await _driveService.Files.Delete(fileId).ExecuteAsync();
            return true;
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete Drive file {Id}", fileId);
            return false;
        }
    }

    public async Task<string?> CreateSpreadsheetAsync(string title, IList<IList<object>> values)
    {
        if (_sheetsService is null)
        {
            logger.LogError("Sheets service not initialized. Call InitializeAsync first.");
            return null;
        }

        try
        {
            var spreadsheet = new Spreadsheet
            {
                Properties = new SpreadsheetProperties { Title = title }
            };

            var createRequest = _sheetsService.Spreadsheets.Create(spreadsheet);
            var created = await createRequest.ExecuteAsync();
            var spreadsheetId = created.SpreadsheetId;
            logger.LogInformation("Created spreadsheet '{Title}' (id={Id})", title, spreadsheetId);

            if (values is { Count: > 0 })
            {
                var valueRange = new ValueRange { Values = values };
                var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, "A1");
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                await updateRequest.ExecuteAsync();
                logger.LogInformation("Wrote {RowCount} rows to spreadsheet '{Title}'", values.Count, title);
            }

            return $"https://docs.google.com/spreadsheets/d/{spreadsheetId}";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create spreadsheet '{Title}'", title);
            return null;
        }
    }
}
