using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.TestSupport;

using Xunit;

namespace MrCMS.Tests.Services
{
    public class UserManagementServiceTests : MrCMSTest
    {

        [Theory, AutoFakeItEasyData]
        public async Task UserService_AddUser_SavesAUserToSession
        ([Frozen] IGlobalRepository<User> repository, UserManagementService sut)
        {
            var user = new User();

            await sut.AddUser(user);

            A.CallTo(() => repository.Add(user, default)).MustHaveHappened();
        }

        [Theory, AutoFakeItEasyData]
        public async Task UserService_SaveUser_UpdatesAUser
        ([Frozen] IGlobalRepository<User> repository, UserManagementService sut)
        {
            var user = new User();

            await sut.SaveUser(user);

            A.CallTo(() => repository.Update(user, default)).MustHaveHappened();
        }

        [Theory, AutoFakeItEasyData]
        public async Task UserService_GetUser_ShouldReturnCorrectUser
        ([Frozen] IGlobalRepository<User> repository, UserManagementService sut)
        {
            var user = new User { FirstName = "Test", LastName = "User", Id = 123 };
            A.CallTo(() => repository.Query()).ReturnsAsAsyncQueryable(user);

            var loadedUser = await sut.GetUser(user.Id);

            loadedUser.Should().BeSameAs(user);
        }

        [Theory, AutoFakeItEasyData]
        public async Task UserService_GetUserDoesNotExist_ShouldReturnNull
        ([Frozen] IGlobalRepository<User> repository, UserManagementService sut)
        {
            A.CallTo(() => repository.Query()).ReturnsAsAsyncQueryable();

            var loadedUser = await sut.GetUser(-1);

            loadedUser.Should().BeNull();
        }

        [Theory, AutoFakeItEasyData]
        public async Task UserService_GetAllUsersPaged_ShouldReturnTheCollectionOfUsersPaged
        ([Frozen] IGlobalRepository<User> repository, UserManagementService sut)
        {
            var enumerable = Enumerable.Range(1, 15).Select(i => new User { FirstName = "Test " + i, LastName = "User" });
            A.CallTo(() => repository.Query()).ReturnsAsAsyncQueryable(enumerable.ToArray());

            var users = await sut.GetAllUsersPaged(1);

            users.Count.Should().Be(10);
            users.PageCount.Should().Be(2);
        }

        [Theory, AutoFakeItEasyData]
        public async Task UserService_DeleteUser_ShouldRemoveAUser
        ([Frozen] IGlobalRepository<User> repository, UserManagementService sut)
        {
            var user = new User { Id = 123 };
            A.CallTo(() => repository.Query()).ReturnsAsAsyncQueryable(user);

            await sut.DeleteUser(user.Id);

            A.CallTo(() => repository.Delete(user, default)).MustHaveHappened();
        }

        [Theory, AutoFakeItEasyData]
        public async Task UserService_IsUniqueEmail_ShouldReturnTrueIfThereAreNoOtherUsers
        ([Frozen] IGlobalRepository<User> repository, UserManagementService sut)
        {
            A.CallTo(() => repository.Query()).ReturnsAsAsyncQueryable();

            var result = await sut.IsUniqueEmail("test@example.com");

            result.Should().BeTrue();
        }

        [Theory, AutoFakeItEasyData]
        public async Task UserService_IsUniqueEmail_ShouldReturnFalseIfThereIsAnotherUserWithTheSameEmail
        ([Frozen] IGlobalRepository<User> repository, UserManagementService sut)
        {
            var user = new User { Email = "test@example.com" };
            A.CallTo(() => repository.Query()).ReturnsAsAsyncQueryable(user);

            var result = await sut.IsUniqueEmail("test@example.com");

            result.Should().BeFalse();
        }

        [Theory, AutoFakeItEasyData]
        public async Task UserService_IsUniqueEmail_ShouldReturnTrueIfTheIdPassedAlongWithTheSavedEmailIsThatOfTheSameUser
        ([Frozen] IGlobalRepository<User> repository, UserManagementService sut)
        {
            var user = new User { Email = "test@example.com", Id=123 };
            A.CallTo(() => repository.Query()).ReturnsAsAsyncQueryable(user);

            var result = await sut.IsUniqueEmail("test@example.com");

            result.Should().BeTrue();
        }
    }
}