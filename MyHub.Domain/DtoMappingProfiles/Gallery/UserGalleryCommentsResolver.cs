﻿using AutoMapper;
using MyHub.Domain.Gallery.GalleryDto;
using MyHub.Domain.Gallery;

namespace MyHub.Domain.DtoMappingProfiles.Gallery
{
	public class UserGalleryCommentsResolver : IValueResolver<GalleryImage, UserGalleryCommentsDto, List<LikedUsersDto>>
	{
		public List<LikedUsersDto> Resolve(GalleryImage source, UserGalleryCommentsDto destination, List<LikedUsersDto> destMember, ResolutionContext context)
		{
			var likedUsersDto = source.LikedUsers.Select(x => new LikedUsersDto { Id = x.Id, Username = x.Username }).ToList();

			return ChangeCurrentUserIfLikes(context, likedUsersDto);
		}

		private static List<LikedUsersDto> ChangeCurrentUserIfLikes(ResolutionContext context, List<LikedUsersDto> likedUsersDto)
		{
			var likedUser = likedUsersDto.Find(x => x.Id == context.Items[GalleryContextOptions.UserId].ToString());

			if (likedUser is null)
				return likedUsersDto;

			likedUsersDto.Remove(likedUser);

			likedUser.Username = "You";
			likedUsersDto.Insert(0, likedUser);

			return likedUsersDto.Take(2).ToList();
		}
	}
}
