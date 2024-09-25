using API_Client.Controllers;
using API_Client.Models;
using API_Client.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API_Client.Context;
using Newtonsoft.Json;
using System.Diagnostics;
using Xunit.Abstractions;
using System.Text.Json;

public class ClientsControllerTests
{
    private ClientsController _controller;
    private Mock<ClientService> _mockClientService;
    private ClientsContext _context;
    private readonly ITestOutputHelper _output;


    public ClientsControllerTests(ITestOutputHelper output)
    {
        _output = output;
        // Setup InMemory Database
        var options = new DbContextOptionsBuilder<ClientsContext>()
            .UseInMemoryDatabase(databaseName: "TestDB")
            .Options;
        _context = new ClientsContext(options);
        _mockClientService = new Mock<ClientService>(null);  // Mock ClientService
        _controller = new ClientsController(_context, _mockClientService.Object);
    }

    // Test for GET api/customers
    [Fact]
    public async Task GetCustomers_ReturnsListOfClients()
    {
        // Arrange
        _context.Customers.RemoveRange(_context.Customers);
        var clients = new List<Clients>
        {
            new Clients { Id = 8, Name = "Client 8", Email = "client1@example.com", Phone = "123456789", CommandeIds = [1,2] },
            new Clients { Id = 9, Name = "Client 9", Email = "client2@example.com", Phone = "987654321", CommandeIds = [3,4] }
        };
        await _context.Customers.AddRangeAsync(clients);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetCustomers();
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
       
        var returnClients = Assert.IsType<List<Clients>>(okResult.Value);
        _output.WriteLine(JsonConvert.SerializeObject(returnClients));
        Assert.Equal(2, returnClients.Count);
    }

    // Test for GET api/customers/{id}
    [Fact]
    public async Task GetCustomer_ReturnsClientById()
    {
        // Arrange
        var client = new Clients { Id = 1, Name = "Client 1", Email = "client1@example.com", Phone = "123456789", CommandeIds = new List<int> { 1, 2 } };
        await _context.Customers.AddAsync(client);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetCustomer(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnClient = Assert.IsType<Clients>(okResult.Value);
        Assert.Equal(1, returnClient.Id);
    }

    [Fact]
    public async Task GetCustomer_ReturnsNotFound_WhenClientDoesNotExist()
    {
        // Act
        var result = await _controller.GetCustomer(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var jsonResponse = JsonConvert.SerializeObject(notFoundResult.Value);
        dynamic response = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

        Assert.Equal("Client non trouvé.", (string)response.message);
    }

    // Test for POST api/customers
    [Fact]
    public async Task CreateCustomer_ReturnsCreatedClient()
    {
        // Arrange
        var newClient = new Clients { Name = "New Client", Email = "newclient@example.com", Phone = "111111111", CommandeIds = [8,9] };

        // Act
        var result = await _controller.PostCustomer(newClient);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdClient = Assert.IsType<Clients>(createdResult.Value);
        Assert.Equal("New Client", createdClient.Name);
    }

    // Test for PUT api/customers/{id}
    [Fact]
    public async Task UpdateCustomer_ReturnsNoContent_WhenClientIsUpdated()
    {
        // Arrange
        var existingClient = new Clients { Id = 58, Name = "Existing Client", Email = "client@example.com", Phone = "123456789",CommandeIds = [89,88] };
        await _context.Customers.AddAsync(existingClient);
        await _context.SaveChangesAsync();
        _context.Entry(existingClient).State = EntityState.Detached;
        var updatedClient = new Clients { Id = 58, Name = "Updated Client", Email = "updatedclient@example.com", Phone = "987654321", CommandeIds = [89, 90] };

        // Act
        var result = await _controller.PutCustomer(58, updatedClient);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result);
        var jsonResponse = JsonConvert.SerializeObject(okObjectResult.Value);
        dynamic response = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

        Assert.Equal("Client mis à jour avec succès.", (string)response.message);
    }

    [Fact]
    public async Task UpdateCustomer_ReturnsNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        var updatedClient = new Clients { Id = 999, Name = "Non Existent Client", Email = "noone@example.com", Phone = "000000000", CommandeIds = [1,2,3] };

        // Act
        var result = await _controller.PutCustomer(999, updatedClient);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var jsonResponse = JsonConvert.SerializeObject(notFoundResult.Value);
        dynamic response = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

        Assert.Equal("Client non trouvé.", (string)response.message);
    }

    // Test for DELETE api/customers/{id}
    [Fact]
    public async Task DeleteCustomer_ReturnsNoContent_WhenClientIsDeleted()
    {
        // Arrange
        var client = new Clients { Id = 25, Name = "Client to delete", Email = "delete@example.com", Phone = "123456789", CommandeIds = [10,11] };
        await _context.Customers.AddAsync(client);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteCustomer(1);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result);
        var jsonResponse = JsonConvert.SerializeObject(okObjectResult.Value);
        dynamic response = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

        Assert.Equal("Client supprimé avec succès.", (string)response.message);
    }

    [Fact]
    public async Task DeleteCustomer_ReturnsNotFound_WhenClientDoesNotExist()
    {
        // Act
        var result = await _controller.DeleteCustomer(999);
        
        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var jsonResponse = JsonConvert.SerializeObject(notFoundResult.Value);
        dynamic response = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

        Assert.Equal("Client non trouvé.", (string)response.message);
    }
}
