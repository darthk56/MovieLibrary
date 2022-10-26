using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;

namespace MovieLibrary
{

    
    class Movie
    {
        int id = 1;
        string title;
        string genre;

       
        public Movie(int _id, string _title, string _genre)
        {


            id = _id;
            title = _title;
            genre = _genre;
            String file = $"{Environment.CurrentDirectory}/movies.csv";
            StreamWriter wr = new StreamWriter(file, true);
            wr.WriteLine($"{id},{title},{genre}");
            wr.Close();

        }

        public string Display()
        {
            return id + "," + title + "," + genre;
        }
        
    }
}
