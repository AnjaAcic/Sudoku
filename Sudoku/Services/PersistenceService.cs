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
        public int?[][] Cells { get; set; } = new int?[9][];
        public bool[][] Given { get; set; } = new bool[9][];
        public bool IsGameOver { get; set; }
    }

    public static class PersistenceService
    {
        private static readonly string AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sudoku");
        public static readonly string Filename = Path.Combine(AppFolder, "sudoku_game.json");

        private static void EnsureFolderExists()
        {
            if (!Directory.Exists(AppFolder))
                Directory.CreateDirectory(AppFolder);
        }

        public static void Save(Board board, Difficulty difficulty, int score, int elapsedSeconds, int mistakes, bool isGameOver)
        {
            EnsureFolderExists();

            var dto = new SaveDto
            {
                Difficulty = difficulty,
                ElapsedSeconds = elapsedSeconds,
                Mistakes = mistakes,
                Score = score,
                Cells = board.ToArray(),
                Given = board.ToGivenArray(),
                IsGameOver = isGameOver
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(Filename, JsonSerializer.Serialize(dto, options));
        }

        public static SaveDto? Load()
        {
            EnsureFolderExists();

            if (!File.Exists(Filename)) return null;

            try
            {
                var json = File.ReadAllText(Filename);
                return JsonSerializer.Deserialize<SaveDto>(json);
            }
            catch
            {
                return null;
            }
        }

        public static void Delete()
        {
            if (File.Exists(Filename))
                File.Delete(Filename);
        }


    }

}
