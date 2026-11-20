namespace SunamoGoogleSheets;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

/// <summary>
/// Represents the drive sheet info type.
/// </summary>
public sealed class DriveSheetInfo
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public string Id { get; set; } = "";
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = "";
    /// <summary>
    /// Gets or sets the modified time.
    /// </summary>
    public DateTime? ModifiedTime { get; set; }
    /// <summary>
    /// Gets or sets the web view link.
    /// </summary>
    public string WebViewLink { get; set; } = "";
}

/// <summary>
/// Represents the google sheets api service type.
/// </summary>
public sealed class GoogleSheetsApiService(ILogger logger)
{
    private SheetsService? _sheetsService;
    private DriveService? _driveService;
    private string _applicationName = "SunamoGoogleSheets";

    /// <summary>
    /// Initialize async.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <returns>The initialize async result.</returns>
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

    /// <summary>
    /// Write values to existing async.
    /// </summary>
    /// <param name="initialized"">The initialized.</param>
    /// <returns>The write values to existing async result.</returns>
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

    /// <summary>
    /// Replaces all conditional formatting on the first sheet with green highlighting of numeric cells
    /// (columns B onward) that are >= the median of their own row, for the given zero-based row indexes.
    /// </summary>
    public async Task<bool> HighlightAtOrAboveRowMedianAsync(string spreadsheetId, IEnumerable<int> rowIndexes)
    {
        if (_sheetsService is null)
        {
            logger.LogError("Sheets service not initialized. Call InitializeAsync first.");
            return false;
        }
        try
        {
            var spreadsheet = await _sheetsService.Spreadsheets.Get(spreadsheetId).ExecuteAsync();
            var sheet = spreadsheet.Sheets[0];
            var sheetId = sheet.Properties.SheetId;
            var columnCount = sheet.Properties.GridProperties?.ColumnCount ?? 26;
            var lastColumnLetter = ColumnLetter(columnCount);

            var requests = new List<Request>();
            var existingRules = sheet.ConditionalFormats?.Count ?? 0;
            for (var i = 0; i < existingRules; i++)
            {
                requests.Add(new Request { DeleteConditionalFormatRule = new DeleteConditionalFormatRuleRequest { SheetId = sheetId, Index = 0 } });
            }

            foreach (var rowIndex in rowIndexes)
            {
                var row = rowIndex + 1;
                var formula = $"=AND(ISNUMBER(B{row}),B{row}>=MEDIAN($B{row}:${lastColumnLetter}{row}))";
                requests.Add(new Request
                {
                    AddConditionalFormatRule = new AddConditionalFormatRuleRequest
                    {
                        Index = 0,
                        Rule = new ConditionalFormatRule
                        {
                            Ranges = new List<GridRange>
                            {
                                new GridRange { SheetId = sheetId, StartRowIndex = rowIndex, EndRowIndex = rowIndex + 1, StartColumnIndex = 1, EndColumnIndex = columnCount }
                            },
                            BooleanRule = new BooleanRule
                            {
                                Condition = new BooleanCondition
                                {
                                    Type = "CUSTOM_FORMULA",
                                    Values = new List<ConditionValue> { new ConditionValue { UserEnteredValue = formula } }
                                },
                                Format = new CellFormat { BackgroundColor = new Color { Red = 0.714f, Green = 0.843f, Blue = 0.659f } }
                            }
                        }
                    }
                });
            }

            if (requests.Count == 0) return true;
            await _sheetsService.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest { Requests = requests }, spreadsheetId).ExecuteAsync();
            logger.LogInformation("Applied median highlighting to spreadsheet {Id}", spreadsheetId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply median highlighting to spreadsheet {Id}", spreadsheetId);
            return false;
        }
    }

    private static string ColumnLetter(int columnNumber)
    {
        var letters = "";
        while (columnNumber > 0)
        {
            var remainder = (columnNumber - 1) % 26;
            letters = (char)('A' + remainder) + letters;
            columnNumber = (columnNumber - 1) / 26;
        }
        return letters;
    }

    /// <summary>
    /// Read values async.
    /// </summary>
    /// <param name="columnNumber">The column number.</param>
    /// <returns>The read values async result.</returns>
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

    /// <summary>
    /// Create spreadsheet in folder async.
    /// </summary>
    /// <param name="spreadsheetId">The spreadsheet id.</param>
    /// <param name="range">The range.</param>
    /// <returns>The create spreadsheet in folder async result.</returns>
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

    /// <summary>
    /// List spreadsheets in folder async.
    /// </summary>
    /// <returns>The list spreadsheets in folder async result.</returns>
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

    /// <summary>
    /// Delete file async.
    /// </summary>
    /// <param name="{FolderId}"">The folder id.</param>
    /// <param name="result.Count">The result count.</param>
    /// <param name="folderId">The folder id.</param>
    /// <returns>The delete file async result.</returns>
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

    /// <summary>
    /// Create spreadsheet async.
    /// </summary>
    /// <param name="ex">The ex.</param>
    /// <returns>The create spreadsheet async result.</returns>
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
