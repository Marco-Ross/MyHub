namespace MyHub.Domain.Gallery.GalleryDto
{
	public class GalleryImageCommentDto
	{
		public string Id { get; set; } = string.Empty;
		public string UserId { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string Comment { get; set; } = string.Empty;
		public DateTime CommentDate { get; set; }
		public bool IsMyComment { get; set; }
	}
}
