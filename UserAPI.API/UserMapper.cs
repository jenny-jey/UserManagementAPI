using UserAPI.API.Models;
using UserAPI.DataAccess;

namespace UserAPI.API
{
    public static  class UserMapper
    {
        public static UserDto ToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                PhoneNumber = user.PhoneNumber
            };
        }

        public static User ToEntity(UserDto userDto)
        {
            return new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                EmailAddress = userDto.EmailAddress,
                PhoneNumber = userDto.PhoneNumber
            };
        }
    }
}
