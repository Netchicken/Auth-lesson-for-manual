//https://code-maze.com/aspnet-core-integration-testing/ 
//https://github.com/CodeMazeBlog/testing-aspnetcore-mvc/blob/integration-testing-mvc/EmployeesApp/EmployeesApp.IntegrationTests/EmployeesControllerIntegrationTests.cs
namespace APIIntegrationTests
{
    //The first thing we have to do is to implement the previously created TestingWebAppFactory class:
    public class MoviesControllerInteqrationTest : IClassFixture<TestingWebAppFactory<Program>>
    {
        private readonly HttpClient _client;
        //passing in the class using Injection and across to _client in the constructor
        public MoviesControllerInteqrationTest(TestingWebAppFactory<Program> factory)
            => _client = factory.CreateClient();

        // GET: api/Movies
        [Fact]
        public async Task IndexReturnsMovies()
        {
            var response = await _client.GetAsync("/Movies");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("MAN CALLED OTTO", responseString);
        }

        // GET: api/Casts
        [Fact]
        public async Task IndexReturnsCast()
        {
            var response = await _client.GetAsync("/Casts");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Sigourney", responseString);
        }
    }
}