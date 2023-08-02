using AutoMapper;
using MyHub.Domain.DtoMappingProfiles.Gallery;
using MyHub.Domain.Titbits.TitbitsDto;
using MyHub.Domain.Titbits;

namespace MyHub.Domain.DtoMappingProfiles.Titbits
{
	public class IsTitibitLikedResolver : IValueResolver<Titbit, TitbitDto, bool>
	{
		public bool Resolve(Titbit source, TitbitDto destination, bool destMember, ResolutionContext context)
		{
			return source.LikedTitbitUsers.Any(x => x.Id == context.Items[GalleryContextOptions.UserId]?.ToString());
		}
	}
}
