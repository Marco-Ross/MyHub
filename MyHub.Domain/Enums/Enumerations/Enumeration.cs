using System.Reflection;

namespace MyHub.Domain.Enums.Enumerations
{
	public class Enumeration : IComparable
	{
		public string Name { get; private set; }

		public string Id { get; private set; }

		protected Enumeration(string id, string name) => (Id, Name) = (id, name);

		public override string ToString() => Name;

		public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
			typeof(T).GetFields(BindingFlags.Public |
								BindingFlags.Static |
								BindingFlags.DeclaredOnly)
					 .Select(f => f.GetValue(null))
					 .Cast<T>();

		public override bool Equals(object? obj)
		{
			if (obj is not Enumeration otherValue)
			{
				return false;
			}

			var typeMatches = GetType().Equals(obj.GetType());
			var valueMatches = Id.Equals(otherValue.Id);

			return typeMatches && valueMatches;
		}

		public int CompareTo(object? other)
		{
			if (other is null)
				return -1;

			return Id.CompareTo(((Enumeration)other).Id);
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}
	}
}
