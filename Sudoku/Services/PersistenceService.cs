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
        public int?[,] Cells { get; set; } = new int?[9, 9]; 
    }

    public static class PersistenceService
    {
        public static void Save(Board board, Difficulty difficulty, int elapsedSeconds, int mistakes, string path)
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

        public static Board ToBoard(SaveDto dto)
        {
            var board = new Board();
            board.LoadFromArray(dto.Cells); 
            return board;
        }
    }

}
