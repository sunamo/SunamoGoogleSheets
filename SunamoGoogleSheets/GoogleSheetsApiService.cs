namespace SunamoGoogleSheets;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

/// <summary>
/// Wrapper around Google Sheets v4 API. Initialize once, then create spreadsheets.
/// OAuth token is cached per app name in the user's ApplicationData folder.
/// </summary>
public sealed class GoogleSheetsApiService(ILogger logger)
{
    private SheetsService? _sheetsService;
    private string _applicationName = "SunamoGoogleSheets";

    /// <summary>
    /// Initializes the service using an OAuth desktop client_secret JSON. Opens the system browser
    /// on first run for user consent; subsequent runs reuse the cached token.
    /// </summary>
    /// <param name="clientSecretsPath">Path to the client_secret*.json file (installed/desktop type).</param>
    /// <param name="applicationName">Name shown in Google API logs and used for the local token cache folder.</param>
    /// <param name="forceRefresh">If true, deletes the cached token and forces re-authorization.</param>
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
                    new[] { SheetsService.Scope.Spreadsheets },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credentialCachePath, true));
            }

            _sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });

            logger.LogInformation("Google Sheets API service initialized");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize Google Sheets API service");
            return false;
        }
    }

    /// <summary>
    /// Creates a new spreadsheet with the given title and writes the values starting at A1.
    /// Returns the spreadsheet's web URL, or null on failure.
    /// </summary>
    /// <param name="title">Spreadsheet title.</param>
    /// <param name="values">Rows of cell values; the first row is typically headers.</param>
    public async Task<string?> CreateSpreadsheetAsync(string title, IList<IList<object>> values)
    {
        if (_sheetsService == null)
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