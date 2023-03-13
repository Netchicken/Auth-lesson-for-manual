using dsd03Razor2020Assessment.Models;

using RolesForAssessment.Data;

using System.Text.Json;

namespace APIIntegrationTests.TestData
{

    internal static class AddTestData
    {
        //import json array and add to database context
        public static void AddMovieData(ApplicationDbContext context)
        {
            var jsonString = File.ReadAllText("TestData/testMovieData.json");

            //need to stop it being case sensitive the model is capital case and the json is not
            var options = new JsonSerializerOptions
            {//stop changing the case from uppercase to lower case for the first letter of the Key
                PropertyNameCaseInsensitive = true
            };

            var list = JsonSerializer.Deserialize<Movie[]>(jsonString, options);
            {
                foreach (var item in list)
                {
                    context.Movie.Add(item);
                }
                //save to the in memory database
                context.SaveChanges();
            }

        }


        public static void AddSingleMovieData(ApplicationDbContext context)
        {
            Movie movie = new Movie();

            movie.Id = Guid.Parse("b3721c65-7d23-4302-53f4-08daf769a5fb");
            movie.Title = "A MAN CALLED OTTO";
            movie.ReleaseDate = DateTime.Parse("2023 - 01 - 16T00: 00:00");
            movie.Overview = "Based on the comical and moving New York Times bestseller, A Man Called Otto tells the story of Otto Anderson (Tom Hanks), a grumpy widower whose only joy comes from criticizing and judging his exasperated neighbors. When a lively young family moves in next door, he meets his match in quick-witted and very pregnant Marisol, leading to an unexpected friendship that will turn his world upside-down.";
            movie.Genre = "Sci-Fi, Adventure, Action, Fantasy";
            movie.Price = 123;

            context.Movie.Add(movie);
            //save to the in memory database
            context.SaveChanges();
        }




        public static void AddCastData(ApplicationDbContext context)
        {
            var jsonString = File.ReadAllText("TestData/testCastData.json");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var list = JsonSerializer.Deserialize<Cast[]>(jsonString, options);
            {
                foreach (var item in list)
                {
                    context.Cast.Add(item);
                }
                context.SaveChanges();
            }

        }


    }
}