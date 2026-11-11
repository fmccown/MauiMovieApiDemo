using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MauiMovieApiDemo
{
    public partial class MainPage : ContentPage
    {
        const string ApiKey = "YOUR_API_KEY";

        public ObservableCollection<Movie> MovieList { get; set; } = [];

        // Necessary to convert "25" string in "Age":"25" into an int automatically
        private readonly JsonSerializerOptions jsonOptions = new()
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        public MainPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        public async Task<Movie?> GetMovieAsync(string imdbId)
        {
            Movie? movie = null;

            if (imdbId.StartsWith("tt"))
            {
                const string baseUrl = "http://www.omdbapi.com/";
                Uri requestUrl = new($"{baseUrl}?i={imdbId}&apiKey={ApiKey}");

                using var httpClient = new HttpClient();
                try
                {
                    var json = await httpClient.GetStringAsync(requestUrl);
                    movie = JsonSerializer.Deserialize<Movie>(json, jsonOptions);
                }
                catch (HttpRequestException ex)
                {
                    await DisplayAlertAsync("HttpClient error", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlertAsync("Invalid ID",
                    "Please supply an IMDb ID, which must start with \"tt\".", "OK");
            }

            return movie;
        }

        private async void fetchButton_Clicked(object sender, EventArgs e)
        {
            Movie? movie = await GetMovieAsync(imdbIdTextBox.Text);
            if (movie != null && movie.Title != "")
            {
                MovieList.Add(movie);
            }
        }
    }
}
