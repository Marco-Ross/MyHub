namespace MyHub.Domain.Gallery.GalleryDto
{
	public class GalleryImageWithCommentsDto
	{
		public string Id { get; set; } = string.Empty;
		public bool IsLiked { get; set; }
		public int LikesCount { get; set; }
		public int CommentsCount { get; set; }
		public List<LikedUsersDto> LikedUsers { get; set; } = new List<LikedUsersDto>();
		public List<GalleryImageCommentDto> GalleryImageComments { get; set; } = new List<GalleryImageCommentDto>();
	}
}
