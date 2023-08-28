using AutoMapper;
using MyHub.Domain.Gallery.GalleryDto;
using MyHub.Domain.Gallery;

namespace MyHub.Domain.DtoMappingProfiles.GalleryProfile
{
	public class LikedUsersResolver : IValueResolver<GalleryImage, GalleryImageDto, List<LikedUsersDto>>
	{
		public List<LikedUsersDto> Resolve(GalleryImage source, GalleryImageDto destination, List<LikedUsersDto> destMember, ResolutionContext context)
		{
			var likedUsersDto = source.LikedGalleryUsers.Select(x => new LikedUsersDto { Id = x.Id, Username = x.Username }).ToList();

			return ChangeCurrentUserIfLikes(context, likedUsersDto);
		}

		private static List<LikedUsersDto> ChangeCurrentUserIfLikes(ResolutionContext context, List<LikedUsersDto> likedUsersDto)
		{
			var likedUser = likedUsersDto.Find(x => x.Id == context.Items[GalleryContextOptions.UserId].ToString());

			if (likedUser is null)
				return likedUsersDto;

			likedUser.Username = "You";

			return likedUsersDto.ToList();
		}
	}
}
