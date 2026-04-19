using Backend.DTOs.Chat;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatBotService _chatBot;

    public ChatController(IChatBotService chatBot)
    {
        _chatBot = chatBot;
    }

    /// <summary>Trimite conversația către OpenAI și returnează răspunsul asistentului.</summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ChatConversationRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var reply = await _chatBot.CompleteAsync(request.Messages, cancellationToken);
            return Ok(reply);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(503, new { error = ex.Message });
        }
    }
}
