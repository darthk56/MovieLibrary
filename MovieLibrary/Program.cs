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


            do
            {
                Console.WriteLine("1. Search Movie");
                Console.WriteLine("2. Add Movie");
                Console.WriteLine("3. Delete Movie");
                Console.WriteLine("4. Edit Movie");
                Console.WriteLine("5. Quit");

                choice = Convert.ToInt16(Console.ReadLine());
                logger.LogInformation("You chose {Choice}", choice);

                if (choice == 1)
                {
                    Console.WriteLine("Please enter the movies title");

                    string Title = Console.ReadLine();

                    using (var db = new MovieContext())
                    {
                        var ReturnedMovie = db.Movies.Where(x => x.Title.ToLower().Contains(Title.ToLower())).FirstOrDefault();
                        if (ReturnedMovie != null)
                        {
                            Console.WriteLine(ReturnedMovie.Id + " " + ReturnedMovie.Title);
                        }
                        else
                        {
                            Console.WriteLine("Movie not found!");
                        }

                    }


                }
                if (choice == 2)
                {
                    Console.WriteLine("Please enter the movies title");
                    string Title = Console.ReadLine();
                    int exists = 0;
                    using (var db = new MovieContext())
                    {
                        var ReturnedMovie = db.Movies.Where(x => x.Title.ToLower() == Title.ToLower());
                        foreach (var m in ReturnedMovie)
                        {
                            if (m.Title == Title)
                            {
                                Console.WriteLine("This title already exists!");
                                exists++;

                            }
                        }
                        if (exists == 0)
                        {
                            Console.WriteLine("Enter genre:");
                            string genreName = Console.ReadLine();
                            var id = 0;
                            foreach (var m in db.Movies)
                            {
                                id++;
                            }
                            Console.Write("Enter a release date (e.g. 10/22/1987): ");
                            DateTime inputtedDate = DateTime.Parse(Console.ReadLine());
                            var movie = new Movie();
                            var genre = new Genre();
                            var genree = new MovieGenre();
                            genre.Name = genreName;
                            genree.Movie = movie;
                            genree.Genre = genre;
                            movie.Title = Title;
                            movie.ReleaseDate = inputtedDate;
                            db.Add(movie);
                            db.Add(genre);
                            db.Add(genree);


                            db.SaveChanges();
                        }
                    }
                }
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
                            if (id == m.Id)
                            {
                                removeID = m.Id;
                                Console.WriteLine("Removed movie!");
                                removed++;
                            }
                        }
                        db.Movies.Remove(db.Movies.Where(x => x.Id == removeID).FirstOrDefault());
                        db.SaveChanges();
                        if (removed == 0)
                        {
                            Console.WriteLine("Error removing movie!");
                        }
                    }


                }

                if (choice == 4)
                {

                    Console.WriteLine("Enter ID of movie to edit:");
                    int id = Convert.ToInt32(Console.ReadLine());

                    using (var db = new MovieContext())
                    {
                        var movieEdit = (Movie)db.Movies.Include(x => x.MovieGenres).ThenInclude(x => x.Genre).Where(x => x.Id == id).FirstOrDefault();
                        int exists = 0;
                        Console.WriteLine("Enter new title:");
                        string newTitle = Console.ReadLine();

                        var ReturnedMovie = db.Movies.Where(x => x.Title.ToLower() == newTitle.ToLower());
                        foreach (var m in ReturnedMovie)
                        {
                            if (m.Title == newTitle)
                            {
                                Console.WriteLine("This title already exists!");
                                exists++;

                            }
                        }
                        if (exists == 0)
                        {
                            Console.Write("Enter a new release date (e.g. 10/22/1987): ");
                            DateTime inputtedDate = DateTime.Parse(Console.ReadLine());
                            movieEdit.ReleaseDate = inputtedDate;
                            foreach (var mg in movieEdit.MovieGenres)
                            {
                                Console.WriteLine(mg.Genre.Name);
                            }
                            int loopVar1 = 0;
                            var genreList = db.Genres.Select(x => x.Name).ToList();

                            do
                            {
                                Console.WriteLine("Enter the genre name to edit");
                                string newGenre = Console.ReadLine();
                                int genreCount = 0;
                                foreach (var mg in movieEdit.MovieGenres)
                                {

                                    if (mg.Genre.Name == newGenre)
                                    {

                                        Console.WriteLine("Enter new name for genre");
                                        string newGenreName = Console.ReadLine();

                                        int loopVar2 = 0;
                                        do
                                        {
                                            if (genreList.Contains(newGenreName) == true)
                                            {
                                                var genreToUpdate = movieEdit.MovieGenres.FirstOrDefault(x => x.Genre.Name == newGenre);
                                                genreToUpdate.Genre.Name = newGenreName;
                                                loopVar2 = 1;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Sorry thats not a valid genre, try again");
                                            }
                                        } while (loopVar2 == 0);


                                        movieEdit.Title = newTitle;



                                        db.SaveChanges();

                                        loopVar1 = 1;
                                    }
                                    else
                                    {
                                        genreCount++;
                                    }
                                }
                                if (genreCount == movieEdit.MovieGenres.Count())
                                {
                                    Console.WriteLine("Sorry this movie doesnt have that genre, try again");
                                }

                            } while (loopVar1 == 0);


                        }




                    }

                }
                //if (choice == 6)
                //{
                //    Console.WriteLine("What title are you searching for?");
                //    string sTitle = Console.ReadLine().ToLower();
                //    //List<string> searched;
                //    int found = 0;
                //    foreach (string item in Titles)
                //    {

                //        if (item.ToLower().Contains(sTitle))
                //        {
                //            Console.WriteLine(item + " | MOVIE");
                //            found++;
                //        }
                //    }
                //    Console.WriteLine("Items found: " + found);





                //}

            }
            while (choice != 5);

            logger.LogInformation("Goodbye!");



        }




    }
}
