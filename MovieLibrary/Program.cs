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
            String file2 = $"{Environment.CurrentDirectory}/videos.csv";
            String file3 = $"{Environment.CurrentDirectory}/shows.csv";

            int choice;

            if (!File.Exists(file))
            {
                logger.LogError("This file does not exist: {file}", file);
            }
            else
            {
                if (!File.Exists(file2))
                {
                    logger.LogError("This file does not exist: {file}", file2);
                }
                else
                {
                    if (!File.Exists(file3))
                    {
                        logger.LogError("This file does not exist: {file}", file3);
                    }
                    else
                    {
                        do
                        {
                            Console.WriteLine("1. Add Movie");
                            Console.WriteLine("2. Add Show");
                            Console.WriteLine("3. Add Video");
                            Console.WriteLine("5. Display All...");
                            Console.WriteLine("6. Quit");

                            choice = Convert.ToInt16(Console.ReadLine());
                            logger.LogInformation("You chose {Choice}", choice);

                            List<int> Ids = new List<int>();
                            List<int> ShowIds = new List<int>();
                            List<int> VideoIds = new List<int>();
                            List<string> Titles = new List<string>();
                            List<string> ShowTitles = new List<string>();
                            List<string> VideoTitles = new List<string>();
                            List<string> Genres = new List<string>();

                            List<int> Seasons = new List<int>();
                            List<int> Episodes = new List<int>();
                            List<string> Writer = new List<string>();

                            List<string> format = new List<string>();
                            List<int> Lengths = new List<int>();
                            List<int> region = new List<int>();

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
                            try
                            {
                                StreamReader reader = new StreamReader(file3);
                                reader.ReadLine();
                                while (!reader.EndOfStream)
                                {

                                    string line = reader.ReadLine();
                                    int idx = line.IndexOf('"');
                                    if (idx == -1)
                                    {
                                        string[] details = line.Split(',');
                                        ShowIds.Add(int.Parse(details[0]));
                                        ShowTitles.Add(details[1]);
                                        Seasons.Add(Convert.ToInt32(details[2]));
                                        Episodes.Add(Convert.ToInt32(details[3]));
                                        Writer.Add(details[4]);



                                    }
                                    else
                                    {
                                        ShowIds.Add(int.Parse(line.Substring(0, idx - 1)));

                                        line = line.Substring(idx + 1);
                                        idx = line.IndexOf('"');

                                        ShowTitles.Add(line.Substring(0, idx));
                                        line = line.Substring(idx + 2);
                                       


                                    }



                                }
                                reader.Close();
                            }
                            catch (Exception exception)
                            {
                                logger.LogError(exception.Message);
                            }
                            try
                            {
                                StreamReader reader = new StreamReader(file2);
                                reader.ReadLine();
                                while (!reader.EndOfStream)
                                {

                                    string line = reader.ReadLine();
                                    int idx = line.IndexOf('"');
                                    if (idx == -1)
                                    {
                                        string[] details = line.Split(',');
                                        VideoIds.Add(int.Parse(details[0]));
                                        VideoTitles.Add(details[1]);
                                        format.Add(details[2]);
                                        Lengths.Add(Convert.ToInt32(details[3]));
                                        region.Add(Convert.ToInt32(details[4]));




                                    }
                                    else
                                    {
                                        Ids.Add(int.Parse(line.Substring(0, idx - 1)));

                                        line = line.Substring(idx + 1);
                                        idx = line.IndexOf('"');

                                        VideoTitles.Add(line.Substring(0, idx));
                                        line = line.Substring(idx + 2);
                                       


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
                                    
                                    string genreS;
                                    Console.WriteLine("Enter movie genre");
                                    genreS = Console.ReadLine();
                                    if (genreS == null)
                                    {
                                        genreS = "No Genre";

                                    }
                                    Title = Title.IndexOf(',') != -1 ? $"\"{Title}\"" : Title;


                                    //StreamWriter wr = new StreamWriter(file, true);
                                    // wr.WriteLine($"{movieId},{Title},{genre}");
                                    Movie temp = new Movie(movieId, Title, genreS);
                                    
                                    Genres.Add(genreS);
                                    Ids.Add(movieId);
                                    Titles.Add(Title);
                                    //wr.Close();
                                    
                                }

                            }
                            if (choice == 2)
                            {
                                Console.WriteLine("Please enter the shows title");

                                string Title = Console.ReadLine();
                                List<string> LowerCaseShowTitles = ShowTitles.ConvertAll(lower => lower.ToLower());
                                if (LowerCaseShowTitles.Contains(Title.ToLower()))
                                {
                                    Console.WriteLine("That Show already exists!");
                                }
                                else
                                {

                                    Console.WriteLine("Please enter the season");
                                    int s = Convert.ToInt32(Console.ReadLine());

                                    Console.WriteLine("Please enter the Episode");

                                    int e = Convert.ToInt32(Console.ReadLine());

                                    Console.WriteLine("Please enter the writer");
                                    string writer = Console.ReadLine();




                                    int showId = Ids.Max() + 1;

                                   
                                    
                                    Title = Title.IndexOf(',') != -1 ? $"\"{Title}\"" : Title;


                                    //StreamWriter wr = new StreamWriter(file, true);
                                    // wr.WriteLine($"{movieId},{Title},{genre}");
                                    Show temp = new Show(showId, Title, s, e, writer);

                                    
                                    ShowIds.Add(showId);
                                    ShowTitles.Add(Title);
                                    //wr.Close();

                                }

                            }
                            if (choice == 5)
                            {
                                Console.WriteLine("1. Movies ");
                                Console.WriteLine("2. Shows ");
                                Console.WriteLine("3. Videos ");
                                int newChoice = Convert.ToInt32(Console.ReadLine());
                                if(newChoice == 1)
                                {
                                 for (int i = 0; i < Ids.Count; i++) 
                                {
                                   
                                    Console.WriteLine($"Id => {Ids[i]}");
                                    Console.WriteLine($"Title => {Titles[i]}");
                                    Console.WriteLine($"Genre => {Genres[i]}");
                                    Console.WriteLine(" ");
                                }

                                }
                                else if(newChoice == 2)
                                {
                                    for (int i = 0; i < ShowIds.Count; i++)
                                    {
                                        
                                        Console.WriteLine($"Id => {ShowIds[i]}");
                                        Console.WriteLine($"Title => {ShowTitles[i]}");
                                        Console.WriteLine($"Season => {Seasons[i]}");
                                        Console.WriteLine($"Episode => {Episodes[i]}");
                                        Console.WriteLine($"Writer => {Writer[i]}");
                                        Console.WriteLine(" ");
                                    }

                                }
                                else if(newChoice == 3)
                                {
                                    for (int i = 0; i < ShowIds.Count; i++)
                                    {

                                        Console.WriteLine($"Id => {VideoIds[i]}");
                                        Console.WriteLine($"Title => {VideoTitles[i]}");
                                        Console.WriteLine($"Season => {format[i]}");
                                        Console.WriteLine($"Episode => {Lengths[i]}");
                                        Console.WriteLine($"Writer => {region[i]}");
                                        Console.WriteLine(" ");
                                    }
                                }


                            }

                        }
                        while (choice != 6);

                        logger.LogInformation("Goodbye!");



                    }




                }
            }
        }
    }
}
