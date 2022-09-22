using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;

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

            String file = $"{Environment.CurrentDirectory}/movies.csv";


            int choice;

            if (!File.Exists(file))
            {
                logger.LogError("This file does not exist: {file}", file);
            }
            else
            {
                do
                {
                    Console.WriteLine("1. Add Movie");
                    Console.WriteLine("2. Display All Movies");
                    Console.WriteLine("3. Quit");

                    choice = Convert.ToInt16(Console.ReadLine());
                    logger.LogInformation("You chose {Choice}", choice);

                    List<int> Ids = new List<int>();

                    List<string> Titles = new List<string>();

                    List<string> Genres = new List<string>();


                    try
                    {
                        StreamReader reader = new StreamReader(file);
                        reader.ReadLine();
                        while (!reader.EndOfStream)
                        {

                            string line = reader.ReadLine();
                            int idx = line.IndexOf('"');
                            if (idx == -1)
                            {
                                string[] details = line.Split(',');
                                Ids.Add(int.Parse(details[0]));
                                Titles.Add(details[1]);
                                Genres.Add(details[2].Replace("|", ", "));




                            }
                            else
                            {
                                Ids.Add(int.Parse(line.Substring(0, idx - 1)));

                                line = line.Substring(idx + 1);
                                idx = line.IndexOf('"');

                                Titles.Add(line.Substring(0, idx));
                                line = line.Substring(idx + 2);
                                Genres.Add(line.Replace("|", ", "));

                                
                            }



                        }
                        reader.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.LogError(exception.Message);
                    }
                        if (choice == 1)
                    {
                        Console.WriteLine("Please enter the movies title");

                        string Title = Console.ReadLine();
                        List<string> LowerCaseMovieTitles = Titles.ConvertAll(lower => lower.ToLower());

                        if (LowerCaseMovieTitles.Contains(Title.ToLower()))
                        {
                            Console.WriteLine("That movie already exists!");
                        }
                        else
                        {
                            int movieId = Ids.Max() + 1;
                            string genre;
                            Console.WriteLine("Enter movie genre");
                            genre = Console.ReadLine();
                            if(genre == null)
                            {
                                genre = "(No genre)";

                            }
                            Title = Title.IndexOf(',') != -1 ? $"\"{Title}\"" : Title;

                            StreamWriter wr = new StreamWriter(file, true);
                            wr.WriteLine($"{movieId},{Title},{genre}");
                           
                            Genres.Add(genre);
                            Ids.Add(movieId);
                            Titles.Add(Title);
                            wr.Close();

                        }

                    }
                        if(choice ==2)
                    {

                        for (int i = 0; i < Ids.Count; i++)
                        {
                            // display movie details
                            Console.WriteLine($"Id => {Ids[i]}");
                            Console.WriteLine($"Title => {Titles[i]}");
                            Console.WriteLine($"Genre => {Genres[i]}");
                            Console.WriteLine(" ");
                        }



                    }
                        
                }
                while(choice != 3);

                logger.LogInformation("Goodbye!");



            }




        }
    }
}
