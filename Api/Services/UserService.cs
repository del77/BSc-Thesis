using System;
using System.Threading.Tasks;
using Api.Entities;
using Api.Repositories;
using ApplicationException = Api.Framework.ApplicationException;

namespace Api.Services
{
    public interface IUserService
    {
        Task<User> Register(string username, string password);
        Task<User> Login(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly IUsersRepository _usersRepository;

        private const string PasswordRequiredCode = "PasswordRequired";
        private const string UsernameTakenCode = "UsernameTaken";

        public UserService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<User> Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ApplicationException("Password is required", PasswordRequiredCode);

            if (await _usersRepository.ExistsUserByUsername(username))
                throw new ApplicationException("Provided username is taken.", UsernameTakenCode);

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            var user = new User(username, passwordHash, passwordSalt);

            await _usersRepository.CreateUserAsync(user);

            return user;
        }

        public async Task<User> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = await _usersRepository.GetUserByUsernameAsync(username);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }


        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Password cannot be empty.");

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password) || storedHash.Length != 64 || storedSalt.Length != 128)
                return false;

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}