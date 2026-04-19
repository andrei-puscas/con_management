using Backend.DTOs.Chat;

namespace Backend.Services;

public interface IChatBotService
{
    Task<ChatReplyDto> CompleteAsync(IReadOnlyList<ChatMessageDto> messages, CancellationToken cancellationToken = default);
}
