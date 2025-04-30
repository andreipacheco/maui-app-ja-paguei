using System.Text.Json;
using System.Text;
using Pagamentos;

public class OneSignalService
{
    private readonly string _appId = Secrets.OneSignalAppId;
    private readonly string _apiKey = Secrets.OneSignalRestApiKey;
    private readonly HttpClient _httpClient;

    public OneSignalService()
    {
        _httpClient = new HttpClient();
    }

    public async Task EnviarNotificacaoAsync(string idPlayer)
    {
        try
        {
            // Corpo da solicitação para a API do OneSignal
            var notificationData = new
            {
                app_id = _appId,
                include_player_ids = new[] { idPlayer }, // Envia a notificação para o idPlayer específico
                headings = new { en = "Lembrete de Pagamento!" },
                contents = new { en = "Não se esqueça de verificar suas contas este mês!" },
                big_picture = "https://meu-balde-s3.s3.sa-east-1.amazonaws.com/AppJaPaguei/JaPaguei.png"
            };

            string json = JsonSerializer.Serialize(notificationData);

            // Configura a solicitação HTTP
            var request = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Basic {_apiKey}");

            // Envia a solicitação
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Notificação enviada com sucesso!");
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro ao enviar notificação: {error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao enviar notificação: {ex.Message}");
        }
    }
}
