using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;
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
                            string genre = Console.ReadLine();
                            var id = 0;
                            foreach (var m in db.Movies)
                            {
                                id++;
                            }
                            Console.Write("Enter a release date (e.g. 10/22/1987): ");
                            DateTime inputtedDate = DateTime.Parse(Console.ReadLine());
                            var movie = new Movie();
                            var genree = new MovieGenre();
                            genree.Movie = movie;
                            genree.Id = (int)movie.Id;
                            movie.Title = Title;
                            movie.ReleaseDate = inputtedDate;
                            db.Add(genree);
                            db.Add(movie);
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
                        db.Movies.Remove((Movie)db.Movies.Where(x => x.Id == removeID).FirstOrDefault());
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
                        var movieEdit = (Movie)db.Movies.Where(x => x.Id == id).FirstOrDefault();
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
                            Console.WriteLine("Enter new genre:");
                            string newGenre = Console.ReadLine();
                            movieEdit.Title = newTitle;
                            MovieGenre genree = new MovieGenre();
                            genree.Id = (int)movieEdit.Id;
                            genree.Movie = movieEdit;
                            db.Add(genree);
                            db.SaveChanges();
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
