using System;

namespace BlogApp.BL.Dtos.Common
{
    public record CursorPagedRequestDto
    {
        public int Size { get; set; } = 10;
        public DateTime? Cursor { get; set; }
    }
}
