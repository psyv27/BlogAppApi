namespace BlogApp.BL.Dtos.Common
{
    public record PageRequestDto
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
    }
}