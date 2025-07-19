using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Helpers
{
    public class UndoRedoManager
    {
        private readonly Stack<Action> _undoStack = new();
        private readonly Stack<Action> _redoStack = new();

        public void Register(Action undoAction, Action redoAction) 
        {
            _undoStack.Push(undoAction);
            _redoStack.Clear();
            _redoStack.Push(redoAction);
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public void Undo() 
        {
            if (!CanUndo) return;
            var action = _undoStack.Pop();
            action();
        }

        public void Redo()
        {
            if (!CanRedo) return;
            var action = _redoStack.Pop();
            action();
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
