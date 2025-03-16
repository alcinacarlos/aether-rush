using Godot;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public partial class GameManager : Node
{
    [Export]
    public int Money = 0;

    public static GameManager Instance;

    public string PlayerName { get; private set; } = "Unknown"; // Nombre del jugador

    private readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();

    private const string UrlApi = "https://apigodot-mongodb.onrender.com/api/Player";

    private async Task SendAPIData(string playerName, int money)
    {
        var data = new { Id="test", Name = PlayerName, MaxScore = money };

        var json = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{UrlApi}", json);

        if (response.IsSuccessStatusCode)
		{
			GD.Print("Datos guardados en la API");
		}
		else
		{
			GD.Print("Error al guardar datos en la API");
		}
    }

    public void GameWon()
    {
        SendAPIData(PlayerName, Money);
        PlayerName = "Unknown";
        reset_money();
    }

    private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        string response = Encoding.UTF8.GetString(body);
        GD.Print($"Respuesta del servidor ({responseCode}): {response}");
    }
    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
    }

    public void SetPlayerName(string name)
    {
        PlayerName = name;
        GD.Print("Nombre del jugador guardado: " + PlayerName); // Debug
    }
    public void add_money(int money)
    {
        Money += money;
    }
    public void reset_money()
    {
        Money = 0;
    }
}
