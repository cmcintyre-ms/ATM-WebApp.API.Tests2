using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ATM_WebApp.API.Models;
using ATM_WebApp.API.Repositories.Interfaces;
using ATM_WebApp.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace ATM_WebApp.API.Tests2
{
    public class CustomerControllerUnitTests
    {
        List<Customer> customers = new List<Customer>
        {
            new Customer
            {
                CustomerId = 1,
                FirstName = "Cheryl",
                LastName = "McIntyre",
                Address1 = "1 Example Way",
                City = "Belfast",
                Postcode = "BT1 1AA",
                PhoneNum = "02890236521"
            },
            new Customer
            {
                CustomerId = 2,
                FirstName = "Clark",
                LastName = "Kent",
                Address1 = "2 Another Street",
                City = "Smallville",
                Postcode = "56523",
                PhoneNum = "555365985"
            }
        };

        Mock<ICustomerRepository> mockRepo = new Mock<ICustomerRepository>();

        #region GetAllCustomer Tests

        [Fact]
        public async void GetAllCustomer_Return_OkResult()
        {
            // Arrange
            mockRepo.Setup(c => c.GetCustomers()).ReturnsAsync(customers);
            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = await controller.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void GetAllCustomers_Returns_NoContentResult()
        {
            // Arrange
            mockRepo.Setup(c => c.GetCustomers()).ReturnsAsync((List<Customer>)null);
            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = await controller.GetAll();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void GetAllCustomers_MatchResults()
        {
            // Arrange
            mockRepo.Setup(c => c.GetCustomers()).ReturnsAsync(customers);
            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = await controller.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            var actualResult = okResult.Value as List<Customer>;

            Assert.Equal(customers[0].CustomerId, actualResult[0].CustomerId);
            Assert.Equal(customers[1].FirstName, actualResult[1].FirstName);
        }

        #endregion

        #region GetCustomerById Tests

        [Fact]
        public async void GetCustomerById_Returns_OkResult()
        {
            // Arrange
            int customerId = customers[0].CustomerId;
            mockRepo.Setup(c => c.GetCustomerById(It.IsAny<int>()))
                .ReturnsAsync((int i) => customers.Where(x => x.CustomerId == i).Single());
            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = await controller.GetCustomer(customerId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void GetCustomerById_Returns_NotFoundResult()
        {
            // Arrange
            int customerId = 9;
            mockRepo.Setup(c => c.GetCustomerById(It.IsAny<int>()))
                .ReturnsAsync((Customer)null);
            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = await controller.GetCustomer(customerId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void GetCustomerById_Return_BadRequestResult()
        {
            // Arrange
            int customerId = 0;
            mockRepo.Setup(c => c.GetCustomerById(It.IsAny<int>()))
                .ReturnsAsync((int i) => customers.Where(x => x.CustomerId == i).Single());
            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = await controller.GetCustomer(customerId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void GetCustomerById_MatchResult()
        {
            // Arrange
            int customerId = customers[0].CustomerId;
            mockRepo.Setup(c => c.GetCustomerById(It.IsAny<int>()))
                .ReturnsAsync((int i) => customers.Where(x => x.CustomerId == i).Single());
            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = await controller.GetCustomer(customerId);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            var actualResult = okResult.Value as Customer;

            Assert.Equal(customers[0].FirstName, actualResult.FirstName);
            Assert.Equal(customers[0].LastName, actualResult.LastName);
        }

        #endregion

        #region UpdateCustomer Tests

        [Fact]
        public async void UpdateCustomer_Returns_OkResult()
        {
            // Arrange
            Customer testCustomer = new Customer
            {
                CustomerId = customers[1].CustomerId,
                FirstName = "Super",
                LastName = "Man",
                Address1 = customers[1].Address1,
                City = customers[1].City,
                Postcode = customers[1].Postcode,
                PhoneNum = customers[1].PhoneNum
            };
            mockRepo.Setup(c => c.UpdateCustomer(It.IsAny<int>(), It.IsAny<Customer>()));
            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = await controller.PutCustomer(testCustomer.CustomerId, testCustomer);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        #endregion
    }
}
