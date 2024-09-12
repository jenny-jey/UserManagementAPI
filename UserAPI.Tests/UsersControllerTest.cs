using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit;
using UserAPI.API;
using UserAPI.API.Controllers;
using UserAPI.API.Models;
using UserAPI.DataAccess;
using UserAPI.DataAccess.Repository;

namespace UserAPI.Tests
{
    [TestFixture]
    public class UsersControllerTest
    {

        private readonly Mock<IUserRepository> _mockRepo;
        private readonly Mock<ILogger<UsersController>> _mockLogger;
        private readonly UsersController _userController;


        public UsersControllerTest()
        {
            _mockRepo = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<UsersController>>();
            _userController = new UsersController(_mockRepo.Object, _mockLogger.Object);
        }

        private UserDto GetSampleUserDto()
        {
            return new UserDto
            {
                Id = 1,
                FirstName = "Jenifer",
                LastName = "Jeyakodi",
                EmailAddress = "jenijey@gmail.com",
                PhoneNumber = "1234567890",
            };
        }

        private List<UserDto> GetSampleUserDtoList()
        {
            return new List<UserDto>
            {
                new UserDto
            {
                Id = 1,
                FirstName = "Jenifer",
                LastName = "Jeyakodi",
                EmailAddress = "jenijey@gmail.com",
                PhoneNumber = "1234567890",
            },
                 new UserDto
            {
                Id = 2,
                FirstName = "Jeni",
                LastName = "Jeyakodi",
                EmailAddress = "jenij@gmail.com",
                PhoneNumber = "1234567890",
            }
            };

        }


        [Test]
        public async Task UpdateUser_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            //var user = new UserDto { Id = 1, FirstName = "Jeni", LastName = "Jeyakodi", EmailAddress = "jeni@gmail.com", PhoneNumber = "0123456789" };
            var userDto = GetSampleUserDto();

            // Act
            var result = await _userController.UpdateUser(2, userDto);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);

        }

        [Test]
        public async Task UpdateUser_ValidId_ReturnsNoContent()
        {
            // Arrange
            //var user = new UserDto { Id = 1, FirstName = "Jeni", LastName = "Jeyakodi", EmailAddress = "jenifer@gmail.com", PhoneNumber = "0123456789" };
            var userDto = GetSampleUserDto();
            _mockRepo.Setup(repo => repo.GetUserAsync(userDto.Id))
                       .ReturnsAsync(UserMapper.ToEntity(userDto));
            _mockRepo.Setup(repo => repo.UpdateUserAsync(It.IsAny<User>()))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _userController.UpdateUser(userDto.Id, userDto);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task CreateUser_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _userController.ModelState.AddModelError("Email", "Required");
            var userDto = new UserDto { FirstName = "Jeni", LastName = "Jeyakodi" };

            // Act
            var result = await _userController.CreateUser(userDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task CreateUser_ValidModel_ReturnsCreatedAtAction()
        {
            // Arrange
            //var user = new User { Id = 1, FirstName = "Jeni", LastName = "Jeyakodi", EmailAddress = "jenifer@gmail.com", PhoneNumber = "0123456789" };
            var userDto = GetSampleUserDto() as UserDto;
            //_mockRepo.Setup(repo => repo.AddUserAsync(User)).Returns(Task.CompletedTask);
            _mockRepo.Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
                       .Callback<User>(userEntity => userEntity.Id = userDto.Id)  // Set Id after creation
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _userController.CreateUser(userDto);

            // Assert
            //var createdAtActionResult = result as CreatedAtActionResult;
            //Assert.NotNull(createdAtActionResult);

            //Assert.That(createdAtActionResult.ActionName, Is.EqualTo(nameof(_userController.GetUserById)));
            //Assert.That(createdAtActionResult.RouteValues!["id"], Is.EqualTo(user.Id)); // route value id matches user id

            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdAtActionResult = result as CreatedAtActionResult;

            Assert.IsInstanceOf<UserDto>(createdAtActionResult.Value);
            var createdUser = createdAtActionResult.Value as UserDto;


            // Assert the properties of the created UserDto
            Assert.AreEqual(userDto.Id, createdUser.Id);
            Assert.AreEqual(nameof(_userController.GetUserById), createdAtActionResult.ActionName);
            Assert.AreEqual(userDto.Id, createdAtActionResult.RouteValues["id"]);
        }

        [Test]
        public async Task GetUser_UserExists_ReturnsOkResultWithUser()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, FirstName = "Jenifer", LastName = "Jeyakodi" };
            _mockRepo.Setup(repo => repo.GetUserAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userController.GetUserById(userId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");
            var returnedUser = okResult.Value as UserDto;
            Assert.IsNotNull(returnedUser, "Expected a User object");
            Assert.That(returnedUser.Id, Is.EqualTo(userId), "User ID does not match");
        }

        [Test]
        public async Task GetUser_UserDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = 1;
            _mockRepo.Setup(repo => repo.GetUserAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _userController.GetUserById(userId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task GetUsers_UsersExist_ReturnsOkResultWithUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, FirstName = "Jenifer", LastName = "Jeyakodi" },
                new User { Id = 2, FirstName = "Jeni", LastName = "Jeyakodi" }
            };
            _mockRepo.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _userController.GetUsers();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");
            var returnedUsers = okResult.Value as IEnumerable<UserDto>;
            Assert.IsNotNull(returnedUsers, "Expected a collection of User objects");
            Assert.That(returnedUsers.Count(), Is.EqualTo(2), "Number of users does not match");
        }

        [Test]
        public async Task GetUsers_NoUsersExist_ReturnsNotFoundResult()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(new List<User>());

            // Act
            var result = await _userController.GetUsers();

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result, "Expected NotFoundResult");
        }
    }
}