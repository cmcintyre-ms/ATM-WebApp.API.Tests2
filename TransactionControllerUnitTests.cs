using ATM_WebApp.API.Controllers;
using ATM_WebApp.API.Models;
using ATM_WebApp.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ATM_WebApp.API.Tests2
{
    public class TransactionControllerUnitTests
    {
        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction
            {
                TransactionId = 1,
                Amount = 10,
                CustomerID = 1,
                TransactionType = "Retail",
                TransactionDate = DateTime.Now
            },
            new Transaction
            {
                TransactionId= 2,
                Amount = 20,
                CustomerID = 2,
                TransactionType = "ATM",
                TransactionDate= DateTime.Now
            }
        };

        Mock<ITransactionRepository> mockRepo = new Mock<ITransactionRepository>();

        #region GetAllTransactions Tests

        [Fact]
        public async void GetAllTransactions_Return_OkResult()
        {
            // Arrange
            mockRepo.Setup(t => t.GetAllTransactions()).ReturnsAsync(transactions);
            var controller = new TransactionsController(mockRepo.Object);

            // Act
            var result = await controller.GetTransactions();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void GetAllTransactions_Return_NoContentResult()
        {
            // Arrange
            mockRepo.Setup(t => t.GetAllTransactions()).ReturnsAsync((List<Transaction>)null);
            var controller = new TransactionsController(mockRepo.Object);

            // Act
            var result = await controller.GetTransactions();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void GetAllTransactions_MatchResult()
        {
            // Arrange
            mockRepo.Setup(t => t.GetAllTransactions()).ReturnsAsync(transactions);
            var controller = new TransactionsController(mockRepo.Object);

            // Act
            var result = await controller.GetTransactions();

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            var actualResult = okResult.Value as List<Transaction>;

            Assert.Equal(transactions[0].TransactionId, actualResult[0].TransactionId);
            Assert.Equal(transactions[1].TransactionId, actualResult[1].TransactionId);
        }

        #endregion

        #region GetTransactionById Tests

        [Fact]
        public async void GetTransactionById_Return_OkResult()
        {
            // Arrange
            int transactionId = transactions[0].TransactionId;
            mockRepo.Setup(t => t.GetTransactionById(It.IsAny<int>())).ReturnsAsync(
                (int i) => transactions.Where(x => x.TransactionId == i).Single());
            var controller = new TransactionsController(mockRepo.Object);

            // Act
            var result = await controller.GetTransaction(transactionId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void GetTransactionById_Return_NoContentResult()
        {
            // Arrange
            int transactionId = 9;
            mockRepo.Setup(t => t.GetTransactionById(It.IsAny<int>())).ReturnsAsync((Transaction)null);
            var controller = new TransactionsController(mockRepo.Object);

            // Act
            var result = await controller.GetTransaction(transactionId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        

        #endregion
    }
}
