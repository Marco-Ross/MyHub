using AutoMapper;
using MyHub.Domain.Gallery;
using MyHub.Domain.Gallery.GalleryDto;

namespace MyHub.Domain.DtoMappingProfiles.GalleryProfile
{
	public class IsImageLikedWithCommentsResolver : IValueResolver<GalleryImage, GalleryImageWithCommentsDto, bool>
	{
		public bool Resolve(GalleryImage source, GalleryImageWithCommentsDto destination, bool destMember, ResolutionContext context)
		{
			return source.LikedGalleryUsers.Any(x => x.Id == context.Items[GalleryContextOptions.UserId]?.ToString());
		}
	}
}
