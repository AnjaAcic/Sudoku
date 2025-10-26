using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Sudoku.Services
{
    public class SaveDto
    {
        public Difficulty Difficulty { get; set; } 
        public int ElapsedSeconds { get; set; }
        public int Mistakes { get; set; }
        public int Score { get; set; }
        public bool IsGameOver { get; set; }
        public int?[][] Cells { get; set; } = new int?[9][];
        public bool[][] Given { get; set; } = new bool[9][];
    }

    public static class PersistenceService
    {
        public static void Save(Board board, Difficulty difficulty, int score, int elapsedSeconds, int mistakes, string path)
        {
            var dto = new SaveDto
            {
                Difficulty = difficulty,
                ElapsedSeconds = elapsedSeconds,
                Mistakes = mistakes,
                Score = score,
                Cells = board.ToArray(),
                Given = board.ToGivenArray(),
                IsGameOver = false
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
