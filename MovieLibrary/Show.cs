using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;


namespace MovieLibrary
{
    class Show
    {
        int id = 1;
        string title;
        int season;
        int episode;
        string writer;

        public Show()
        {
            id++;
            title = "Supernatural";
            season = 2;
            episode = 12;
            writer = "Kripke";

        }
        public Show(int _id, string _title, int _season, int _episode, string _writer)
        {


            id = _id;
            title = _title;
            season = _season;
            episode = _episode;
            writer = _writer;
            String file = $"{Environment.CurrentDirectory}/shows.csv";
            StreamWriter wr = new StreamWriter(file, true);
            wr.WriteLine($"{id},{title},{season},{episode}, {writer}");
            wr.Close();

        }
    }
}
