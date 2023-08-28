using AutoMapper;
using MyHub.Domain.Titbits.TitbitsDto;
using MyHub.Domain.Titbits;

namespace MyHub.Domain.DtoMappingProfiles.TitbitsProfile
{
	public class TitbitsProfile : Profile
	{
		public TitbitsProfile()
		{
			CreateMap<TitbitLink, TitbitLinkDto>();
			CreateMap<TitbitLinkDto, TitbitLink>();
			CreateMap<AddTitbitDto, Titbit>();
			CreateMap<UpdateTitbitDto, Titbit>();

			CreateMap<Titbit, TitbitDto>()
				.ForMember(x => x.IsLiked, opt => opt.MapFrom<IsTitibitLikedResolver>());

			CreateMap<TitbitCategory, TitbitCategoryDto>();
			CreateMap<TitbitCategoryDto, TitbitCategory>();
		}
	}
}
