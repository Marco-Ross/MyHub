using AutoMapper;
using MyHub.Domain.Gallery.GalleryDto;
using MyHub.Domain.Gallery;

namespace MyHub.Domain.DtoMappingProfiles.GalleryProfile
{
	public class GalleryProfile : Profile
	{
		public GalleryProfile()
		{
			CreateMap<GalleryImage, GalleryImageDto>()
				.ForMember(x => x.LikedUsers, m => m.MapFrom<LikedUsersResolver>())
				.ForMember(x => x.IsLiked, m => m.MapFrom<IsImageLikedResolver>());

			CreateMap<GalleryImageComment, GalleryImageCommentDto>()
				.ForPath(x => x.Username, m => m.MapFrom(u => u.User.Username))
				.ForMember(x => x.IsMyComment, m => m.MapFrom<IsMyCommentResolver>());

			CreateMap<GalleryImage, GalleryImageWithCommentsDto>()
				.ForMember(x => x.LikedUsers, m => m.MapFrom<LikedUsersWithCommentsResolver>())
				.ForMember(x => x.IsLiked, m => m.MapFrom<IsImageLikedWithCommentsResolver>());
				
		}
	}
}
