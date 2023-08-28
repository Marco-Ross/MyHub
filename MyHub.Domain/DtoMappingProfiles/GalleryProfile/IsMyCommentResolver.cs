using AutoMapper;
using MyHub.Domain.Gallery.GalleryDto;
using MyHub.Domain.Gallery;

namespace MyHub.Domain.DtoMappingProfiles.GalleryProfile
{
	public class IsMyCommentResolver : IValueResolver<GalleryImageComment, GalleryImageCommentDto, bool>
	{
		public bool Resolve(GalleryImageComment source, GalleryImageCommentDto destination, bool destMember, ResolutionContext context)
		{
			return source.UserId == context.Items[GalleryContextOptions.UserId]?.ToString();
		}
	}
}
