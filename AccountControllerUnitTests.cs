using ATM_WebApp.API.Controllers;
using ATM_WebApp.API.Models;
using ATM_WebApp.API.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ATM_WebApp.API.Tests2
{
    public class AccountControllerUnitTests
    {
        List<Account> accounts = new List<Account>
        {
            new Account
            {
                AccountId = 1,
                AccountName = "Current",
                CustomerID = 1,
                Balance = 1000,
                CardNum = "1111111111111111",
                PIN = "1111"
            },
            new Account
            {
                AccountId = 2,
                AccountName = "Savings",
                CustomerID = 2,
                Balance = 2000,
                CardNum = "2222222222222222",
                PIN = "2222"
            }
        };

        Mock<IAccountRepository> mockRepo = new Mock<IAccountRepository>();


        #region GetAllAccounts Tests
        [Fact]
        public async void GetAllAccounts_Returns_OkResult() // when account is found
        {
            // Arrange
            mockRepo.Setup(a => a.GetAllAccounts()).ReturnsAsync(accounts);
            var controller = new AccountsController(mockRepo.Object);

            // Act
            var result = await controller.GetAccounts();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

        }

        [Fact]
        public async void GetAllAccounts_Returns_NotContent_Result() // when account is not found
        {
            // Arrange
            mockRepo.Setup(a => a.GetAllAccounts()).ReturnsAsync((List<Account>)null);
            var controller = new AccountsController(mockRepo.Object);

            // Act
            var result = await controller.GetAccounts();

            // Assert
            //Assert.Null(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void GetAllAccounts_MatchResults() // returns all accounts etc
        {
            // Arrange
            mockRepo.Setup(a => a.GetAllAccounts()).ReturnsAsync(accounts);
            var controller = new AccountsController(mockRepo.Object);

            // Act
            var result = await controller.GetAccounts();

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            var testAccount = okResult.Value as List<Account>;

            // Best to sort lists using for loops

            // count number in lists
            Assert.Equal(accounts[0].AccountName, testAccount[0].AccountName);
            Assert.Equal(accounts[1].CardNum, testAccount[1].CardNum);


        }
        #endregion

        #region GetAccountById Tests

        [Fact]
        public async void GetAccountById_Return_OkResult()
        {
            // Arrange
            int accountId = accounts[0].AccountId;

            mockRepo.Setup(a => a.GetAccountById(
                accountId)).ReturnsAsync(
                accounts.SingleOrDefault(x => x.AccountId == accountId));

            var controller = new AccountsController(mockRepo.Object);

            // Act
            var result = await controller.GetAccount(accountId);

            // Assert
            Assert.IsType<OkObjectResult>(result);

        }

        [Fact]
        public async void GetAccountById_Return_BadRequestResult() // when no id is found
        {
            // Arrange
            int accountId = 0;

            //mockRepo.Setup(a => a.GetAccountById(
            //    It.IsAny<int>())).ReturnsAsync((int i) =>
            //    accounts.Where(x => x.AccountId == i).Single());

            mockRepo.Setup(a => a
                .GetAccountById(accountId))
                .ReturnsAsync(
                    accounts.SingleOrDefault(x => x.AccountId == accountId)
                 );

            var controller = new AccountsController(mockRepo.Object);

            // Act
            var result = await controller.GetAccount(accountId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        // test large exteme id number = 100 -  using [Theory]
        // Failure tests - null, -1, 0, 3 (just above), 100

        [Fact]
        public async void GetAccountById_Return_MatchResult()
        {
            // Arrange
            int accountId = accounts[0].AccountId;

            mockRepo.Setup(a => a.GetAccountById(accountId))
                .ReturnsAsync(
                    accounts.SingleOrDefault(x => x.AccountId == accountId)
                 //accounts.Where(a => a.AccountId == accountId)
                 // .Select(a => new Account { AccountId = a.AccountId })
                 // .Single()
                 );

            var controller = new AccountsController(mockRepo.Object);

            // Act
            var result = await controller.GetAccount(accountId);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            var actualResult = okResult.Value as Account;

            actualResult.Should().Be(accounts[0]);
        }

        #endregion

        #region AddAccount Tests

        [Fact]
        public async void AddAccount_Returns_OkResult()
        {
            // Arrange
            Account testAccount = new Account()
            {
                AccountId = 6,
                AccountName = "Current Account",
                CustomerID = 1,
                Balance = 30,
                CardNum = "6985236985214568",
                PIN = "6666"
            };

            mockRepo.Setup(a => a.AddAccount(It.IsAny<Account>())).ReturnsAsync(testAccount.AccountId);
            var controller = new AccountsController(mockRepo.Object);

            // Act
            var result = await controller.PostAccount(testAccount);

            // Assert
            Assert.IsType<OkObjectResult>(result);

        }

        [Fact]
        public async void AddAccount_Returns_NoContentResult()
        {
            // Arrange
            Account testAccount = new Account
            {
                AccountId = 0,
                AccountName = null,
                CustomerID = 0,
                Balance = 0,
                CardNum = null,
                PIN = null
            };

            mockRepo.Setup(a => a.AddAccount(It.IsAny<Account>())).ReturnsAsync(testAccount.AccountId);
            var controller = new AccountsController(mockRepo.Object);

            // Act
            var result = await controller.PostAccount(testAccount);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        #endregion

        #region UpdateAccount Tests

        [Fact]
        public async void UpdateAccount_Returns_OkResult()
        {
            // Arrange
            Account testAccount = new Account
            {
                AccountId = accounts[0].AccountId,
                AccountName = "ISA Account",
                CustomerID = accounts[0].CustomerID,
                Balance = accounts[0].Balance,
                CardNum = accounts[0].CardNum,
                PIN = accounts[0].PIN
            };
            mockRepo.Setup(a => a.UpdateAccount(It.IsAny<int>(), It.IsAny<Account>()));
            var controller = new AccountsController(mockRepo.Object);

            // Act
            var result = await controller.PutAccount(testAccount.AccountId, testAccount);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        #endregion

        #region DeleteAccount Tests

        [Fact]
        public async void DeleteAccount_Verfiy()
        {
            // Arrange
            var testAccount = accounts[0];
            mockRepo.Setup(a => a.DeleteAccount(It.IsAny<int>())).Verifiable();
            var controller = new AccountsController(mockRepo.Object);

            // Act
            var result = await controller.DeleteAccount(testAccount.AccountId);

            // Assert
            mockRepo.Verify();
        }



        #endregion
    }
}
