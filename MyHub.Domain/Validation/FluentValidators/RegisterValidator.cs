using MyHub.Domain.Users;

namespace MyHub.Domain.Validation.FluentValidators
{
    public class UserRegisterValidator
    {
        public AccessingUser AccessingUser;
        public UserRegisterValidator(AccessingUser accessingUser) => AccessingUser = accessingUser;
    }
}
