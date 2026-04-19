using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Chat;

public class ChatConversationRequestDto
{
    [Required]
    [MinLength(1)]
    public List<ChatMessageDto> Messages { get; set; } = new();
}
