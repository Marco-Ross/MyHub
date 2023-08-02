namespace MyHub.Domain.Titbits.Interfaces
{
	public interface ITitbitsService
	{
		Titbit? AddNewTitbit(string userId, Titbit titbit);
		List<TitbitCategory> AddNewTitbitCategories(List<TitbitCategory> categoryDescriptions);
		IQueryable<Titbit> GetTitbits(string createdUserId, string currentUserId);
		IQueryable<TitbitCategory> GetTitbitCategories();
		List<TitbitCategory> UpdateTitbitCategories(List<TitbitCategory> titbitCategories);
		void DeleteTitbitCategories(List<TitbitCategory> categories);
		bool LikeTitbit(string userId, string titbitId);
		bool UnlikeTitbit(string userId, string titbitId);
		bool DeleteTitbit(string titbitId);
		Titbit? UpdateTitbit(string userId, Titbit titbit);
	}
}
