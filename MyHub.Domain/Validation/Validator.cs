namespace MyHub.Domain.Validation
{
	public class Validator<T> where T : new()
	{
		public Validator() { }

		public Validator(T response) 
		{
			ResponseValue = response;
		}

		public List<string> Errors { get; set; } = new List<string>();
		public string ErrorsString => string.Join(", ", Errors);
		public T ResponseValue { get; set; } = new T();
		public bool IsValid { get; set; } = true;
		public bool IsInvalid { get; set; } = false;

		public Validator<T> AddError(string error)
		{
			IsValid = false;
			IsInvalid = true;
			Errors.Add(error);
			return this;
		}
	}
	
	public class Validator
	{
		public List<string> Errors { get; set; } = new List<string>();
		public string ErrorsString => string.Join(", ", Errors);
		public bool IsValid { get; set; } = true;
		public bool IsInvalid { get; set; } = false;

		public Validator AddError(string error)
		{
			IsValid = false;
			IsInvalid = true;
			Errors.Add(error);
			return this;
		}
	}
}
