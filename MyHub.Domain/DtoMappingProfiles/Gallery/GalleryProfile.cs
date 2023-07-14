using AutoMapper;
using MyHub.Domain.Gallery.GalleryDto;
using MyHub.Domain.Gallery;

namespace MyHub.Domain.DtoMappingProfiles.Gallery
{
	public class GalleryProfile : Profile
	{
		public GalleryProfile()
		{
			CreateMap<GalleryImage, UserGalleryDto>()
				.ForMember(x => x.LikedUsers, m => m.MapFrom<LikedUsersResolver>())
				.ForMember(x => x.IsLiked, opt => opt.MapFrom<IsImageLikedResolver>());
				//.ForMember(x => x.GalleryImageComments, opt => opt.MapFrom<GalleryImageCommentsResolver>());

			CreateMap<GalleryImageComment, GalleryImageCommentDto>()
				.ForPath(x => x.Username, m => m.MapFrom(u => u.User.Username));

			CreateMap<GalleryImage, UserGalleryCommentsDto>()
				.ForMember(x => x.LikedUsers, m => m.MapFrom<UserGalleryCommentsResolver>());
		}
	}
}
