using Microsoft.EntityFrameworkCore;
using MyHub.Domain.Titbits;
using MyHub.Domain.Titbits.Interfaces;
using MyHub.Domain.Users.Interfaces;
using MyHub.Infrastructure.Repository.EntityFramework;

namespace MyHub.Application.Services.Titbits
{
	public class TitbitsService : ITitbitsService
	{
		private readonly IUsersService _usersService;
		private readonly ApplicationDbContext _applicationDbContext;

		public TitbitsService(IUsersService usersService, ApplicationDbContext applicationDbContext)
		{
			_usersService = usersService;
			_applicationDbContext = applicationDbContext;
		}

		public Titbit? AddNewTitbit(string userId, Titbit titbit)
		{
			var user = _usersService.GetUserById(userId);

			if (user is null)
				return null;


			titbit.Id = Guid.NewGuid().ToString();
			titbit.UserCreated = user;
			titbit.DateUploaded = DateTime.Now;

			if (titbit.TitbitLinks is not null)
			{
				foreach (var link in titbit.TitbitLinks)
					link.Id = Guid.NewGuid().ToString();
			}

			var addedTitbit = _applicationDbContext.Titbits.Add(titbit);

			_applicationDbContext.SaveChanges();

			return _applicationDbContext.Titbits.Include(x => x.TitbitLinks).Include(x => x.TitbitCategory).Single(x => x.Id == titbit.Id);
		}

		public List<TitbitCategory> AddNewTitbitCategories(List<TitbitCategory> categories)
		{
			var updatedDescriptions = new List<TitbitCategory>();

			categories.ForEach(category =>
			{
				var newCategory = new TitbitCategory
				{
					Id = Guid.NewGuid().ToString(),
					Description = category.Description,
					DateUploaded = DateTime.Now
				};

				var addedCategory = _applicationDbContext.TitbitCategories.Add(newCategory);
				updatedDescriptions.Add(addedCategory.Entity);
			});

			_applicationDbContext.SaveChanges();

			return updatedDescriptions;
		}

		public List<TitbitCategory> UpdateTitbitCategories(List<TitbitCategory> categories)
		{
			categories.ForEach(category =>
			{
				var loadedCategory = _applicationDbContext.TitbitCategories.Where(x => x.Id == category.Id).Single();

				loadedCategory.Description = category.Description;
			});

			_applicationDbContext.SaveChanges();

			return categories;
		}

		public void DeleteTitbitCategories(List<TitbitCategory> categories)
		{
			categories.ForEach(category =>
			{
				var categoryToRemove = _applicationDbContext.TitbitCategories.Single(x => x.Id == category.Id);
				var loadedCategory = _applicationDbContext.TitbitCategories.Remove(categoryToRemove);
			});

			_applicationDbContext.SaveChanges();
		}

		public IQueryable<Titbit> GetTitbits(string createdUserId, string currentUserId)
		{
			return _applicationDbContext.Titbits.Where(x => x.UserCreatedId == createdUserId).Include(x => x.TitbitLinks).Include(x => x.TitbitCategory)
				.Include(x => x.LikedTitbitUsers.Where(x => x.Id == currentUserId)).OrderByDescending(x => x.DateUploaded);
		}

		public IQueryable<TitbitCategory> GetTitbitCategories()
		{
			return _applicationDbContext.TitbitCategories.OrderBy(x => x.DateUploaded);
		}

		public bool LikeTitbit(string currentUserId, string titbitId)
		{
			try
			{
				var currentUser = _usersService.GetUserById(currentUserId);

				if (currentUser is null)
					return false;

				var titbit = _applicationDbContext.Titbits.Include(x => x.LikedTitbitUsers).SingleOrDefault(x => x.Id == titbitId);

				if (titbit is null) return false;

				titbit.LikedTitbitUsers.Add(currentUser);

				_applicationDbContext.SaveChanges();

				return true;
			}
			catch (DbUpdateException)
			{
				return false;
			}
		}

		public bool UnlikeTitbit(string currentUserId, string titbitId)
		{
			try
			{
				var currentUser = _usersService.GetUserById(currentUserId);

				if (currentUser is null)
					return false;

				var titbit = _applicationDbContext.Titbits.Include(x => x.LikedTitbitUsers).SingleOrDefault(x => x.Id == titbitId);

				if (titbit is null) return false;

				titbit.LikedTitbitUsers.Remove(currentUser);

				_applicationDbContext.SaveChanges();

				return true;
			}
			catch (DbUpdateException)
			{
				return false;
			}
		}

		public bool DeleteTitbit(string titbitId)
		{
			var titbit = _applicationDbContext.Titbits.SingleOrDefault(x => x.Id == titbitId);

			if (titbit is null)
				return false;

			_applicationDbContext.Titbits.Remove(titbit);

			_applicationDbContext.SaveChanges();

			return true;
		}

		public Titbit? UpdateTitbit(string userId, Titbit titbit)
		{
			var user = _usersService.GetUserById(userId);

			if (user is null)
				return null;

			var existingTitbit = _applicationDbContext.Titbits.Include(x => x.TitbitLinks).Single(x => x.Id == titbit.Id);

			existingTitbit.Title = titbit.Title;
			existingTitbit.Description = titbit.Description;
			existingTitbit.CategoryId = titbit.CategoryId;
			existingTitbit.UserUpdated = user;
			existingTitbit.DateUpdated = DateTime.Now;
			existingTitbit.TitbitLinks = titbit.TitbitLinks;

			foreach (var link in existingTitbit.TitbitLinks)
			{
				if(string.IsNullOrWhiteSpace(link.Id))
					link.Id = Guid.NewGuid().ToString();
			}

			_applicationDbContext.SaveChanges();

			return _applicationDbContext.Titbits.Include(x => x.TitbitLinks).Include(x => x.TitbitCategory).Single(x => x.Id == titbit.Id);
		}
	}
}
