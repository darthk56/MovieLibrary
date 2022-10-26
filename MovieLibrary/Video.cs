using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;


namespace MovieLibrary
{
    class Video
    {

        int id = 1;
        string title;
        string format;
        int length;
        int regions;

        public Video()
        {
            id++;
            title = "Toy Story";
            format = "VHS, DVD, BluRay";
            length = 100;
            regions = 0;

        }
        public Video(int _id, string _title, string _format, int _length, int _regions)
        {
            id = _id;
            title = _title;
            format = _format;
            length = _length;
            regions = _regions;
           
            String file = $"{Environment.CurrentDirectory}/videos.csv";
            StreamWriter wr = new StreamWriter(file, true);
            wr.WriteLine($"{id},{title},{format},{length}, {regions}");
            wr.Close();
        }
    }
}
