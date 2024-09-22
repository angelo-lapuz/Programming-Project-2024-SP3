using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using PeakHub.Models;


namespace PeakHub.Utilities
{
    public class Loader
    {

        public  List<Peak> getAll()
        {
            using var reader = new StreamReader("wwwroot/cleaned_peaks.csv");
            using var csv = new CsvHelper.CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });

            var tasks = new List<Peak>();
            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                var task = new Peak
                {
                    Name = csv.GetField<string>("Peak"),
                    Details = $"Elevation: {csv.GetField<string>("Elevation")}, Ascents: {csv.GetField<string>("Ascents")}",
                    Coords = csv.GetField<string>("Latitude/Longitude"),
                    Difficulty = csv.GetField<string>("Difficulty")[0],
                    Section = csv.GetField<string>("Section")

                };
                tasks.Add(task);
            }
            return tasks;
        }

    }
}
