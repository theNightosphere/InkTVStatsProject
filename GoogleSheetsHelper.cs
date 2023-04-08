using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace InkTVStats
{
    public class GoogleSheetsHelper
    {
        public SheetsService Service { get; set; }
        const string APPLICATION_NAME = "InkTVStats";
        static readonly string[] scopes = { SheetsService.Scope.Spreadsheets };

        public GoogleSheetsHelper()
        {
            InitializeService();
        }

        private void InitializeService()
        {
            GoogleCredential credentials = GetCredentialsFromFile();
            Service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = APPLICATION_NAME,
            });
        }

        private GoogleCredential GetCredentialsFromFile()
        {
            GoogleCredential credential;
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
            }

            return credential;
        }
    }
}
