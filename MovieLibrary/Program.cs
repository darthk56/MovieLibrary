using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;
using Microsoft.EntityFrameworkCore;
using MovieLibraryEntities.Context;
using MovieLibraryEntities.Models;

namespace MovieLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection
                .AddLogging(x => x.AddConsole())
                .BuildServiceProvider();
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            logger.LogInformation("Ready!");
            int choice;

            //do while loop executing everything while quit is not selected
            do
            {
                Console.WriteLine("1. Search Movie");
                Console.WriteLine("2. Add Movie");
                Console.WriteLine("3. Delete Movie");
                Console.WriteLine("4. Edit Movie");
                Console.WriteLine("5. Add User");
                Console.WriteLine("6. Rate Movie");
                Console.WriteLine("7. Quit");

                choice = Convert.ToInt16(Console.ReadLine());
                logger.LogInformation("You chose {Choice}", choice);
                //chocie 1: search for movie
                if (choice == 1)
                {
                    Console.WriteLine("Please enter the movies title");

                    string Title = Console.ReadLine();

                    using (var db = new MovieContext())
                    {
                        //getting movie title
                        var ReturnedMovie = db.Movies.Include(x => x.MovieGenres).ThenInclude(x => x.Genre).Where(x => x.Title.ToLower().Contains(Title.ToLower())).FirstOrDefault();
                        if (ReturnedMovie != null)
                        {
                            //if found displays movie and details
                            var genreList = db.MovieGenres.Where(x => x.Movie == ReturnedMovie).ToList();
                            Console.WriteLine("ID    Movie Title");
                            Console.WriteLine(ReturnedMovie.Id + " " + ReturnedMovie.Title);
                            Console.WriteLine("Genres:");
                            Console.WriteLine(genreList.Count());
                            foreach(var g in genreList)
                            {
                                Console.WriteLine(g.Genre.Name);
                            }
                        }
                        else
                        {
                            //alerts if nothing was returned from the search
                            Console.WriteLine("Movie not found!");
                        }

                    }


                }
                //choice 2: Add a movie
                if (choice == 2)
                {   //gets movie title
                    Console.WriteLine("Please enter the movies title");
                    string Title = Console.ReadLine();
                    int exists = 0;
                    using (var db = new MovieContext())
                    {
                        var ReturnedMovie = db.Movies.Where(x => x.Title.ToLower() == Title.ToLower());
                        //get potential list of movies with same name
                        foreach (var m in ReturnedMovie)
                        {
                            if (m.Title == Title)
                            {
                                Console.WriteLine("This title already exists!");
                                exists++;
                                //add to exists to skip over other prompts
                            }
                        }
                        //if none exist then continue
                        if (exists == 0)
                        {
                            //display list of genres to select from
                            var genreList = db.Genres.Select(x => x.Name).ToList();
                            foreach (var g in genreList)
                            {
                                Console.WriteLine(g);
                            }
                            Console.WriteLine("Enter genre:");
                            string genreName = Console.ReadLine();  
                            Console.Write("Enter a release date (e.g. 10/22/1987): ");
                            DateTime inputtedDate = DateTime.Parse(Console.ReadLine());
                            //get genre and release dates
                            var movie = new Movie();
                            var genre = new Genre();
                            var genree = new MovieGenre();
                            //create new objects for movie
                            genre.Name = genreName;
                            genree.Movie = movie;
                            genree.Genre = genre;
                            movie.Title = Title;
                            movie.ReleaseDate = inputtedDate;
                            //assign new values
                            db.Add(movie);
                            db.Add(genre);
                            db.Add(genree);
                            //add new objects to database

                            db.SaveChanges();
                        }
                    }
                }
                //Choice 3: Delete a movie
                if (choice == 3)
                {

                    Console.WriteLine("Enter ID number of movie to delete:");
                    int id = Convert.ToInt32(Console.ReadLine());
                    using (var db = new MovieContext())
                    {
                        int removed = 0;
                        long removeID = -1;
                        foreach (var m in db.Movies)
                        {
                            //searching all movies for matching ID
                            if (id == m.Id)
                            {
                                removeID = m.Id;
                                Console.WriteLine("Removed movie!");
                                removed++;
                                //confirm correct ID
                            }
                        }
                        db.Movies.Remove(db.Movies.Where(x => x.Id == removeID).FirstOrDefault());
                        //get movie from correct ID and remove it
                        db.SaveChanges();
                        if (removed == 0 || removed == null)
                        {
                            Console.WriteLine("Error removing movie!");
                        }
                    }


                }
                //Choice 4: Edit a movie
                if (choice == 4)
                {

                    Console.WriteLine("Enter ID of movie to edit:");
                    int id = Convert.ToInt32(Console.ReadLine());

                    using (var db = new MovieContext())
                    {
                        //find movie we want to edit from users inputted ID
                        var movieEdit = (Movie)db.Movies.Include(x => x.MovieGenres).ThenInclude(x => x.Genre).Where(x => x.Id == id).FirstOrDefault();
                        int exists = 0;
                        Console.WriteLine("Enter new title:");
                        string newTitle = Console.ReadLine();
                        //checking to see if the movie already exists (cant have 2 of the same movie)
                        var ReturnedMovie = db.Movies.Where(x => x.Title.ToLower() == newTitle.ToLower());
                        foreach (var m in ReturnedMovie)
                        {
                            if (m.Title == newTitle)
                            {
                                Console.WriteLine("This title already exists!");
                                exists++;

                            }
                        }
                        //if it doesnt exist continue
                        if (exists == 0)
                        {
                            Console.Write("Enter a new release date (e.g. 10/22/1987): ");
                            DateTime inputtedDate = DateTime.Parse(Console.ReadLine());
                            movieEdit.ReleaseDate = inputtedDate;
                            
                            int loopVar1 = 0;
                            //creating a list of all available genres to select
                            var genreList = db.Genres.Select(x => x.Name).ToList();
                            Console.WriteLine("List of available genres:");

                          
                            //displaying list of available genres
                            foreach (var g in genreList)
                            {
                                Console.WriteLine(g);
                            }
                            do
                            {
                                //list movies current genres
                                Console.WriteLine("---Movies Current Genres---");
                                foreach (var mg in movieEdit.MovieGenres)
                                {
                                    Console.WriteLine(mg.Genre.Name);
                                }
                                Console.WriteLine("Enter the genre name to edit");
                                string newGenre = Console.ReadLine();
                                int genreCount = 0;
                                //foreach loop checking to see if selected genre to edit actually exists in the movie
                                foreach (var mg in movieEdit.MovieGenres)
                                {

                                    if (mg.Genre.Name.ToLower() == newGenre.ToLower())
                                    {

                                        Console.WriteLine("Enter new name for genre");
                                        string newGenreName = Console.ReadLine();

                                        int loopVar2 = 0;
                                        do
                                        {
                                            //checking if new genre is a valid genre from main list
                                            if (genreList.Contains(newGenreName) == true)
                                            {
                                                //finds the genre user wants to change
                                                var genreToUpdate = movieEdit.MovieGenres.FirstOrDefault(x => x.Genre.Name == newGenre);
                                                //changes it
                                                genreToUpdate.Genre.Name = newGenreName;
                                                //stops the loop
                                                loopVar2 = 1;
                                            }
                                            else
                                            {
                                                //continues loop if genre is not in the main list
                                                Console.WriteLine("Sorry thats not a valid genre, try again");
                                            }
                                        } while (loopVar2 == 0);


                                        movieEdit.Title = newTitle;
                                        //give it the new title


                                        db.SaveChanges();

                                        loopVar1 = 1;
                                    }
                                    else
                                    {
                                        //adding to count to make sure it loops through all genres no more no less
                                        genreCount++;
                                    }
                                }
                                //if the genreCount is equal to the amount of moviegenres then we see no genres under the selected name were assosicated with the movie
                                if (genreCount == movieEdit.MovieGenres.Count())
                                {
                                    //alert that the movie does not have that genre associated with it
                                    Console.WriteLine("Sorry this movie doesnt have that genre, try again");
                                }

                            } while (loopVar1 == 0);


                        }




                    }

                }
                //Choice 5: Add a user
                if (choice == 5)
                {
                    using (var db = new MovieContext())
                    {

                        int userAge = 0;
                        do
                        {
                            //asking for age looping until a valid age is entered
                            Console.WriteLine("Please enter the age:");
                            userAge = Convert.ToInt16(Console.ReadLine());


                        } while (userAge == null || userAge == 0); ;
                        Console.WriteLine("Please enter the gender letter (M/F):");
                        string userGender = Console.ReadLine();
                        string userZip;
                        do
                        {
                            //asking for zipcode looping until valid
                            Console.WriteLine("Please enter the zip code");
                            userZip = Console.ReadLine();
                        } while (userZip == null || userZip.ToString().Length != 5);
                        //markers for occupations list
                        Console.WriteLine("Occupations:");
                        Console.WriteLine("ID  Title");
                        //loop over occupations and list the id and name
                        foreach (var o in db.Occupations)
                        {

                            Console.WriteLine(o.Id + " " + o.Name);
                        }
                        int occId;
                        do
                        {
                            //ask user to select an occupation by id form the list
                            Console.WriteLine("Please select an occupation by ID");
                            occId = Convert.ToInt16(Console.ReadLine());
                        } while (occId == null || occId > 21 || occId < 1);
                        //gets the correct occupation
                        var occ = db.Occupations.Where(o => o.Id == occId).FirstOrDefault();
                        var newUser = new User();
                        //creates the new user object
                        Console.WriteLine("Creating user using data: \n Occupation: " + occ.Name + "\n Age: " + userAge + "\n Gender: " + userGender + "\n Zipcode: " + userZip);
                        //logs the information about new user
                        newUser.Occupation = occ;
                        newUser.Age = userAge;
                        newUser.Gender = userGender;
                        newUser.ZipCode = userZip;
                        //adds values to the new user
                        db.Add(newUser);
                        //adds to database
                        db.SaveChanges();
                        Console.WriteLine("Success!");
                        Console.WriteLine("Your User ID is: " + db.Users.OrderBy(x => x.Id).LastOrDefault().Id);
                       
                    }


                }
                //Choice 6: Rate a movie
                if (choice == 6)
                {
                    int choiceId;





                    using (var db = new MovieContext())
                    {
                        do
                        {
                            //asks user to enter their id
                            Console.WriteLine("Please enter your User ID");
                            choiceId = Convert.ToInt16(Console.ReadLine());
                            if (choiceId == 0 || choiceId == null || choiceId > db.Users.Count())
                            {
                                Console.WriteLine("Not a valid ID try again");
                            }
                        } while (choiceId == null);
                        //selected user by ID
                        var user = db.Users.Where(o => o.Id == choiceId).FirstOrDefault();

                        int loopVar = 0;
                        do
                        {
                            Console.WriteLine("Please enter the movies ID");

                            int movieId = Convert.ToInt32(Console.ReadLine());
                            //finds movie by the ID
                            var movie = db.Movies.Where(x => x.Id == movieId).FirstOrDefault();
                            if (movie == null)
                            {
                                Console.WriteLine("Movie not found try again");
                            }
                            else
                            {
                                loopVar++;
                                var rated = new UserMovie();
                                int rating;
                                do
                                {
                                    //asks user to rate the movie on scale of 1-5 and assigns to a variable
                                    Console.WriteLine("Please rate the selected movie out of 5 (1 being lowest 5 being most)");
                                    rating = Convert.ToInt16(Console.ReadLine());
                                    if (rating < 1 || rating > 5 || rating == null)
                                    {
                                        Console.WriteLine("Sorry thats an invalid rating, try again!");
                                    }
                                } while (rating < 0 || rating > 5 || rating == null);
                                var date = new DateTime();
                                date = DateTime.Now;
                                //creates datetime object and gets current date/time
                                rated.RatedAt = date;
                                rated.Rating = rating;
                                rated.Movie = movie;
                                rated.User = user;
                                //Assigns values from variables to new rating and adds objects to the "UserMovie" lists
                                Console.WriteLine("User ID of " + user.Id + " is rating the movie " + movie.Title + " with a rating of " + rating + " stars");
                                //logs information about the rated movie
                                db.Add(rated);
                                //adds rating to database
                                db.SaveChanges();

                            }


                        } while (loopVar == 0);




                    }



                }

            }
            //exits if choice is 7
            while (choice != 7);

            logger.LogInformation("Goodbye!");



        }




    }
}
