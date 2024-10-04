using CsvHelper.Configuration;
using PeakHub.Models;
using System.Globalization;

namespace PeakHub.Utilities {
    public class BoardLoader {
        public List<Board> getBoards() {
            List<Board> boards = new();

            using var reader = new StreamReader("wwwroot/boards.csv");
            using var csv = new CsvHelper.CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) {
                HasHeaderRecord = true
            });

            csv.Read();
            csv.ReadHeader();

            while (csv.Read()) {
                boards.Add(new Board {
                    BoardID = csv.GetField<int>("Rank"),
                    Name = csv.GetField<string>("Peak"),
                    Section = csv.GetField<string>("Section"),
                    Image = csv.GetField<string>("ImageLink"),
                    Posts = new()
                });
            }

            return boards;
        }
    }
}
