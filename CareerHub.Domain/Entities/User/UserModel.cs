using CareerHub.Domain.Entities.Common;
using CareerHub.Domain.Extensions;

namespace CareerHub.Domain.Entities.User
{
    public class UserModel : BaseEntity
    {
        public UserId Id { get; private set; } = new(Guid.NewGuid());
        public string Email { get; private set; } = string.Empty;
        public string NormalizedEmail { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public bool IsEmailVerified { get; private set; } = false;
        public DateTimeOffset DateJoined { get; private set; }
        public DateTimeOffset LastLogin { get; private set; }

        // Private constructor
        private UserModel()
        {
            DateJoined = DateTime.UtcNow.ToBangladeshTime();
            LastLogin = DateJoined;
        }

        // Factory method
        public static UserModel CreateUser(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }

            var newUser = new UserModel
            {
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
            };

            return newUser;
        }

        public void SetEmail(string email)
        {
            Email = email;
            NormalizedEmail = email.ToUpperInvariant();
        }

        public void SetPassword(string password)
        {
            Password = password;
        }

        public void MarkEmailAsVerified()
        {
            IsEmailVerified = true;
        }

        public void UpdateLastLogin()
        {
            LastLogin = DateTime.UtcNow.ToBangladeshTime();
        }
    }
}
