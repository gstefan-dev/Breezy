using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Breezy.Application.Users.Commands;
using Breezy.Application.Users.DTOs;
using Breezy.Domain.Users.ValueObjects;
using FluentAssertions;

namespace Breezy.Tests.Integration.Controllers;

public class UsersControllerIntegrationTests : IClassFixture<BreezyWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersControllerIntegrationTests(BreezyWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_Returns201()
    {
        var requestBody = new { UserEmail = "test@gmail.com", UserName = "Test", Password = "123456789" };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/users/register", content);

        var responseBody = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = JsonSerializer.Deserialize<UserResponse>(responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        result.Email.Should().Be("test@gmail.com");
    }

    [Fact]
    public async Task Register_WithExistingEmail_Returns400()
    {
        var requestBody = new { UserEmail = "existing@gmail.com", UserName = "Test", Password = "123456789" };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/users/register", content);

        var responseBody = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = JsonSerializer.Deserialize<UserResponse>(responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        result.Email.Should().Be("existing@gmail.com");

        var requestBody2 = new { UserEmail = "existing@gmail.com", UserName = "Test", Password = "123456789" };

        var json2 = JsonSerializer.Serialize(requestBody2);
        var content2 = new StringContent(json2, Encoding.UTF8, "application/json");

        var response2 = await _client.PostAsync("/api/users/register", content2);

        var responseBody2 = await response.Content.ReadAsStringAsync();
        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result2 = JsonSerializer.Deserialize<UserResponse>(responseBody2,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        result.Email.Should().Be("existing@gmail.com");
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200()
    {
        var uniqueEmail = $"logintest_{Guid.NewGuid()}@gmail.com";
        var registerBody = new { UserEmail = uniqueEmail, UserName = "LoginTest", Password = "123456789" };
        var registerJson = JsonSerializer.Serialize(registerBody);
        var registerContent = new StringContent(registerJson, Encoding.UTF8, "application/json");

        await _client.PostAsync("/api/users/register", registerContent);

        var loginBody = new { Email = uniqueEmail, Password = "123456789" };
        var loginJson = JsonSerializer.Serialize(loginBody);
        var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/users/login", loginContent);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithWrongPassword_Return400()
    {
        var uniqueEmail = $"logintest_{Guid.NewGuid()}@gmail.com";
        var registerBody = new { UserEmail = uniqueEmail, UserName = "LoginTest", Password = "123456789" };
        var registerJson = JsonSerializer.Serialize(registerBody);
        var registerContent = new StringContent(registerJson, Encoding.UTF8, "application/json");

        await _client.PostAsync("/api/users/register", registerContent);

        var loginBody = new { Email = uniqueEmail, Password = "wrongpass123" };
        var loginJson = JsonSerializer.Serialize(loginBody);
        var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/users/login", loginContent);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetUser_WithoutToken_Returns401()
    {
        var randomId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/users/{randomId}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUser_WithValidToken_Returns200()
    {
        var uniqueEmail = $"logintest_{Guid.NewGuid()}@gmail.com";
        var registerBody = new { UserEmail = uniqueEmail, UserName = "LoginTest", Password = "123456789" };
        var registerJson = JsonSerializer.Serialize(registerBody);
        var registerContent = new StringContent(registerJson, Encoding.UTF8, "application/json");

        var registerResponse = await _client.PostAsync("/api/users/register", registerContent);
        var registerBody2 = await registerResponse.Content.ReadAsStringAsync();
        var registeredUser = JsonSerializer.Deserialize<UserResponse>(registerBody2,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var loginBody = new { Email = uniqueEmail, Password = "123456789" };
        var loginJson = JsonSerializer.Serialize(loginBody);
        var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/users/login", loginContent);
        var token = await response.Content.ReadAsStringAsync();
        token = token.Trim('"');

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var getResponse = await _client.GetAsync($"/api/users/{registeredUser.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}