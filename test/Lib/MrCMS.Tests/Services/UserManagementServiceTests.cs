using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.TestSupport;
using NHibernate;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class UserManagementServiceTests : InMemoryDatabaseTest
    {
        private ISession _session;
        private UserManagementService _userManagementService;

        public UserManagementServiceTests()
        {
            _session = Session;
            _userManagementService = new UserManagementService(_session, EventContext);
        }

        [Fact]
        public async Task UserService_AddUser_SavesAUserToSession()
        {
            _session = A.Fake<ISession>();
            _userManagementService = new UserManagementService(_session, EventContext);
            var user = new User();

            await _userManagementService.AddUser(user);

            A.CallTo(() => _session.SaveAsync(user, CancellationToken.None)).MustHaveHappened();
        }

        [Fact]
        public async Task UserService_SaveUser_UpdatesAUser()
        {
            _session = A.Fake<ISession>();
            _userManagementService = new UserManagementService(_session, EventContext);
            var user = new User();

            await _userManagementService.SaveUser(user);

            A.CallTo(() => _session.UpdateAsync(user, CancellationToken.None)).MustHaveHappened();
        }

        [Fact]
        public async Task UserService_GetUser_ShouldReturnCorrectUser()
        {
            var user = new User { FirstName = "Test", LastName = "User" };
            await Session.TransactAsync(session => session.SaveOrUpdateAsync(user));

            var loadedUser = _userManagementService.GetUser(user.Id);

            loadedUser.Should().BeSameAs(user);
        }

        [Fact]
        public void UserService_GetUserDoesNotExist_ShouldReturnNull()
        {
            var loadedUser = _userManagementService.GetUser(-1);

            loadedUser.Should().BeNull();
        }

        [Fact]
        public async Task UserService_GetAllUsersPaged_ShouldReturnTheCollectionOfUsersPaged()
        {
            foreach (var i in Enumerable.Range(1, 15))
            {
                await Session.TransactAsync(
                    session => session.SaveOrUpdateAsync(new User { FirstName = "Test " + i, LastName = "User" }));
            }

            var users = await _userManagementService.GetAllUsersPaged(1);

            users.Count.Should().Be(10);
            users.PageCount.Should().Be(2);
        }

        [Fact]
        public async Task UserService_DeleteUser_ShouldRemoveAUser()
        {
            var user = new User();
            await Session.TransactAsync(session => session.SaveAsync(user));

            await _userManagementService.DeleteUser(user.Id);

            (await Session.QueryOver<User>().RowCountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task UserService_IsUniqueEmail_ShouldReturnTrueIfThereAreNoOtherUsers()
        {
            (await _userManagementService.IsUniqueEmail("test@example.com")).Should().BeTrue();
        }

        [Fact]
        public async Task UserService_IsUniqueEmail_ShouldReturnFalseIfThereIsAnotherUserWithTheSameEmail()
        {
            await Session.TransactAsync(session => session.SaveAsync(new User { Email = "test@example.com" }));
            (await _userManagementService.IsUniqueEmail("test@example.com")).Should().BeFalse();
        }

        [Fact]
        public async Task
            UserService_IsUniqueEmail_ShouldReturnTrueIfTheIdPassedAlongWithTheSavedEmailIsThatOfTheSameUser()
        {
            var user = new User { Email = "test@example.com" };
            await Session.TransactAsync(session => session.SaveAsync(user));
            (await _userManagementService.IsUniqueEmail("test@example.com", user.Id)).Should().BeTrue();
        }
    }
}