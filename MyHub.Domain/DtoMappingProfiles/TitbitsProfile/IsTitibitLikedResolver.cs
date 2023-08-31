using AutoMapper;
using MyHub.Domain.Titbits.TitbitsDto;
using MyHub.Domain.Titbits;
using MyHub.Domain.DtoMappingProfiles.GalleryProfile;

namespace MyHub.Domain.DtoMappingProfiles.TitbitsProfile
{
	public class IsTitibitLikedResolver : IValueResolver<Titbit, TitbitDto, bool>
	{
		public bool Resolve(Titbit source, TitbitDto destination, bool destMember, ResolutionContext context)
		{
			return source.LikedTitbitUsers.Any(x => x.Id == context.Items[GalleryContextOptions.UserId]?.ToString());
		}
	}
}
