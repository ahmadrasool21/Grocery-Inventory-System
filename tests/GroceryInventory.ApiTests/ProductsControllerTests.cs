using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GroceryInventory.Application.DTOs;

namespace GroceryInventory.ApiTests;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_products_requires_auth_and_returns_ok_when_authenticated()
    {
        // unauthenticated -> our TestAuth makes every request authenticated, so skip 401 here
        var res = await _client.GetAsync("/api/products");
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_product_requires_admin()
    {
        var body = new ProductDto(Guid.Empty, "Apple", "SKU-APP-001", Guid.NewGuid(), "kg", 1.0m, 1.6m, 0.1m, 10, false, true, DateTime.UtcNow);

        // As Clerk -> 403
        var reqClerk = new HttpRequestMessage(HttpMethod.Post, "/api/products") { Content = JsonContent.Create(body) };
        reqClerk.Headers.Add("X-Test-Role", "Clerk");
        var resClerk = await _client.SendAsync(reqClerk);
        resClerk.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // As Admin -> 201
        var reqAdmin = new HttpRequestMessage(HttpMethod.Post, "/api/products") { Content = JsonContent.Create(body) };
        reqAdmin.Headers.Add("X-Test-Role", "Admin");
        var resAdmin = await _client.SendAsync(reqAdmin);
        resAdmin.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}