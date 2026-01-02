namespace BlogApp.BL.Dtos.MessageDtos
{
    public record MessageCreateDto
    {
        public string ReceiverId { get; set; }
        public string Content { get; set; }
    }
}
