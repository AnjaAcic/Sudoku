using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Services
{
    public class JsonStorageService : IStorageService
    {
        public (int?[,], int[,]) Load(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException();
            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            var p = doc.RootElement.GetProperty("puzzle").Deserialize<int?[,]>();
            var s = doc.RootElement.GetProperty("solution").Deserialize<int[,]>();
            return (p, s);
        }

        public void Save(int?[,] puzzle, int[,] solution, string path)
        {
            var dto = new { puzzle, solution };
            File.WriteAllText(path, JsonSerializer.Serialize(dto));
        }
    }
}
