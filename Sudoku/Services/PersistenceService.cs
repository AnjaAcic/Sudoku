using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sudoku.Services
{
    public class SaveDto
    {
        public string Difficulty { get; set; } = "medium";
        public int ElapsedSeconds { get; set; }
        public int Mistakes { get; set; }
        public int?[,] Cells { get; set; } = new int?[9, 9];


    }

    public static class PersistenceService
    {
        public static void Save(Board board, string difficulty, int elapsedSeconds, int mistakes, string path)
        {
            var dto = new SaveDto
            {
                Difficulty = difficulty,
                ElapsedSeconds = elapsedSeconds,
                Mistakes = mistakes,
                Cells = board.ToArray()
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(path, JsonSerializer.Serialize(dto, options));
        }

        public static SaveDto Load(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<SaveDto>(json)!;   
        }
    }
}
