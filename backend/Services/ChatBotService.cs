using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Backend.DTOs.Chat;
using Microsoft.Extensions.Configuration;

namespace Backend.Services;

public class ChatBotService : IChatBotService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _configuration;

    public ChatBotService(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        _configuration = configuration;
    }

    public async Task<ChatReplyDto> CompleteAsync(IReadOnlyList<ChatMessageDto> messages, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Cheia OpenAI nu este configurată. Setează OpenAI:ApiKey în appsettings sau user secrets.");

        var model = _configuration["OpenAI:Model"] ?? "gpt-4o-mini";
        var systemPrompt = _configuration["OpenAI:SystemPrompt"]
            ?? "Ești un asistent util pentru aplicația ConManagement (management proiecte și echipe în construcții). Răspunde pe română, clar și concis.";

        var sanitized = messages
            .Where(m => m.Role is "user" or "assistant")
            .TakeLast(24)
            .Select(m => new { role = m.Role, content = Truncate(m.Content ?? "", 12000) })
            .ToList();

        if (sanitized.Count == 0)
            throw new ArgumentException("Trimite cel puțin un mesaj de la utilizator.");

        var payloadMessages = new List<object> { new { role = "system", content = systemPrompt } };
        foreach (var m in sanitized)
            payloadMessages.Add(m);

        var body = new { model, messages = payloadMessages };
        var json = JsonSerializer.Serialize(body);

        using var req = new HttpRequestMessage(HttpMethod.Post, "chat/completions");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey.Trim());
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var res = await _http.SendAsync(req, cancellationToken);
        var responseText = await res.Content.ReadAsStringAsync(cancellationToken);

        if (!res.IsSuccessStatusCode)
        {
            var err = TryParseOpenAiError(responseText);
            throw new InvalidOperationException(err ?? $"OpenAI a returnat {(int)res.StatusCode}: {responseText[..Math.Min(200, responseText.Length)]}");
        }

        using var doc = JsonDocument.Parse(responseText);
        var root = doc.RootElement;
        var content = root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
        return new ChatReplyDto { Content = content };
    }

    private static string Truncate(string s, int max)
    {
        if (s.Length <= max) return s;
        return s[..max];
    }

    private static string? TryParseOpenAiError(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("error", out var err) && err.TryGetProperty("message", out var msg))
                return msg.GetString();
        }
        catch
        {
            // ignore
        }

        return null;
    }
}
