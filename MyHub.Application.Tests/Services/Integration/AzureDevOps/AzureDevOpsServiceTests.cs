using MyHub.Application.Services.Integration.AzureDevOps;
using System.Text;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using MyHub.Domain.Integration.AzureDevOps.AzureWorkItems.Interfaces;

namespace MyHub.Application.Tests.Services.Integration.AzureDevOps
{
    public class AzureDevOpsServiceTests : ApplicationTestBase
	{
		private readonly IAzureDevOpsService _azureDevOpsService;
		private readonly Mock<DelegatingHandler> _httpClientHandler = new();

		public AzureDevOpsServiceTests()
		{
			_azureDevOpsService = new AzureDevOpsService(new HttpClient(_httpClientHandler.Object)
			{
				BaseAddress = new Uri("https://localhost/Test")
			});
		}

		[Fact]
		public async Task GetWorkItems_HasNoWorkItemsQuery_ReturnsEmpty()
		{
			//Arrange
			_httpClientHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.Returns(() => Task.FromResult(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK
				}));

			//Act
			var sendEmailException = await _azureDevOpsService.GetWorkItems();

			//Assert
			Assert.Empty(sendEmailException.WorkItems);
		}

		[Fact]
		public async Task GetWorkItems_HasNoWorkItemsQueryEmptyContent_ReturnsEmpty()
		{
			//Arrange
			_httpClientHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.Returns(() => Task.FromResult(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(JsonSerializer.Serialize(new { }), Encoding.UTF8, "application/json")
				}));

			//Act
			var sendEmailException = await _azureDevOpsService.GetWorkItems();

			//Assert
			Assert.Empty(sendEmailException.WorkItems);
		}

		[Fact]
		public async Task GetWorkItems_HasWorkItemsQueryHasNoWorkItems_ReturnsEmpty()
		{
			//Arrange
			_httpClientHandler.Protected().SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.Returns(() => Task.FromResult(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(JsonSerializer.Serialize(new { queryType = "testQueryType" }), Encoding.UTF8, "application/json")
				}))
				.Returns(() => Task.FromResult(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK
				}));

			//Act
			var sendEmailException = await _azureDevOpsService.GetWorkItems();

			//Assert
			Assert.Empty(sendEmailException.WorkItems);
		}

		[Fact]
		public async Task GetWorkItems_HasWorkItemsQueryEmptyResultsContent_ReturnsEmpty()
		{
			//Arrange
			_httpClientHandler.Protected().SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.Returns(() => Task.FromResult(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(JsonSerializer.Serialize(new { queryType = "testQueryType" }), Encoding.UTF8, "application/json")
				}))
				.Returns(() => Task.FromResult(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(JsonSerializer.Serialize(new { }), Encoding.UTF8, "application/json")
				}));

			//Act
			var sendEmailException = await _azureDevOpsService.GetWorkItems();

			//Assert
			Assert.Empty(sendEmailException.WorkItems);
		}

		[Fact]
		public async Task GetWorkItems_HasItems_ReturnsItems()
		{
			//Arrange			
			_httpClientHandler.Protected().SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.Returns(() => Task.FromResult(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(JsonSerializer.Serialize(new { queryType = "testQueryType", workItems = new[] { new { id = 1 } } }), Encoding.UTF8, "application/json")
				}))
				.Returns(() => Task.FromResult(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(JsonSerializer.Serialize(new { count = 1, value = new[] { new { id = 1 } } }), Encoding.UTF8, "application/json")
				}));

			//Act
			var sendEmailException = await _azureDevOpsService.GetWorkItems();

			//Assert
			Assert.Equal(1, sendEmailException.Count);
		}
	}
}
