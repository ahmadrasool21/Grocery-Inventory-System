using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GroceryInventory.Application.DTOs;

namespace GroceryInventory.ApiTests;

public class StockControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    public StockControllerTests(CustomWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Purchase_requires_admin_and_validates_quantity()
    {
        var productId = Guid.NewGuid();

        // Bad quantity -> 400 ProblemDetails
        var bad = new HttpRequestMessage(HttpMethod.Post, "/api/stock/purchase")
        { Content = JsonContent.Create(new MoveStockRequest(productId, 0, 0.7m, null, null, null)) };
        bad.Headers.Add("X-Test-Role", "Admin");
        var badRes = await _client.SendAsync(bad);
        badRes.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        badRes.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        // Clerk -> 403
        var clerk = new HttpRequestMessage(HttpMethod.Post, "/api/stock/purchase")
        { Content = JsonContent.Create(new MoveStockRequest(productId, 5, 0.7m, null, null, null)) };
        clerk.Headers.Add("X-Test-Role", "Clerk");
        var clerkRes = await _client.SendAsync(clerk);
        clerkRes.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Sale_allows_clerk()
    {
        var productId = Guid.NewGuid();
        var sale = new HttpRequestMessage(HttpMethod.Post, "/api/stock/sale")
        { Content = JsonContent.Create(new MoveStockRequest(productId, 2, null, "sale", null, null)) };
        sale.Headers.Add("X-Test-Role", "Clerk");
        var res = await _client.SendAsync(sale);
        res.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Adjust_negative_without_reason_returns_problem()
    {
        var productId = Guid.NewGuid();
        var adjust = new HttpRequestMessage(HttpMethod.Post, "/api/stock/adjust")
        { Content = JsonContent.Create(new MoveStockRequest(productId, -1, null, null, null, null)) };
        adjust.Headers.Add("X-Test-Role", "Admin");
        var res = await _client.SendAsync(adjust);
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}