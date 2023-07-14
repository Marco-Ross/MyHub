namespace MyHub.Domain.Gallery.GalleryDto
{
	public class UserGalleryDto
	{
		public string Id { get; set; } = string.Empty;
		public string UserCreatedId { get; set; } = string.Empty;
		public int LikesCount { get; set; }
		public int CommentsCount { get; set; }
		public bool IsLiked { get; set; }
		public List<LikedUsersDto> LikedUsers { get; set; } = new List<LikedUsersDto>();
		//public List<GalleryImageCommentDto> GalleryImageComments { get; set; } = new List<GalleryImageCommentDto>();
		public string Caption { get; set; } = string.Empty;
		public DateTime DateUploaded { get; set; }
	}
}
