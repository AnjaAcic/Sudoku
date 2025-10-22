using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Services
{
    public record Move(int Index, int? OldValue, int? NewValue);

    public class UndoService
    {
        private readonly Stack<Move> _stack = new();

        public void Push(Move m) => _stack.Push(m);
        public Move? Pop() => _stack.Count > 0 ? _stack.Pop() : null;
        public void Clear() => _stack.Clear();
        public bool CanUndo => _stack.Count > 0;
    }
}
