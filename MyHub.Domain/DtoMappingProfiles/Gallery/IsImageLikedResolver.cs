using AutoMapper;
using MyHub.Domain.Gallery;
using MyHub.Domain.Gallery.GalleryDto;

namespace MyHub.Domain.DtoMappingProfiles.Gallery
{
	public class IsImageLikedResolver : IValueResolver<GalleryImage, UserGalleryDto, bool>
	{
		public bool Resolve(GalleryImage source, UserGalleryDto destination, bool destMember, ResolutionContext context)
		{
			return source.LikedGalleryUsers.Any(x => x.Id == context.Items[GalleryContextOptions.UserId]?.ToString());
		}
	}
}
