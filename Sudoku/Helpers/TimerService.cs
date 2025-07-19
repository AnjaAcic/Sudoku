using Sudoku.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Sudoku.Helpers
{
    public class TimerService : NotifyBase
    {
        private readonly DispatcherTimer _timer;
        private TimeSpan _elapsed = TimeSpan.Zero;

        public string Elapsed => _elapsed.ToString(@"hh\:mm\:ss");

        public TimerService()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (_, __) =>
            {
                _elapsed = _elapsed.Add(TimeSpan.FromSeconds(1));
                Raise(nameof(Elapsed));
            };
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
        public void Reset()
        {
            _timer.Stop();
            _elapsed = TimeSpan.Zero;
            Raise(nameof(Elapsed));
        }
    }
}
