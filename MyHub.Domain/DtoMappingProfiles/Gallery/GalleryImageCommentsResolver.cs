using AutoMapper;
using MyHub.Domain.Gallery.GalleryDto;
using MyHub.Domain.Gallery;

namespace MyHub.Domain.DtoMappingProfiles.Gallery
{
	public class GalleryImageCommentsResolver : IValueResolver<GalleryImage, UserGalleryDto, List<GalleryImageCommentDto>>
	{
		public List<GalleryImageCommentDto> Resolve(GalleryImage source, UserGalleryDto destination, List<GalleryImageCommentDto> destMember, ResolutionContext context)
		{
			return source.GalleryImageComments.Select(x => new GalleryImageCommentDto
			{
				Id = x.Id,
				Comment = x.Comment,
				Username = x.User.Username,
				CommentDate = x.CommentDate
			}).ToList();
		}
	}
}
