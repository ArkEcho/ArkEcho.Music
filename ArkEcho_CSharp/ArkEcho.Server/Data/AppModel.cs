using System;

namespace ArkEcho.Server
{
    public class AppModel
    {
        private int counter;

        public int Counter { get { return counter; } set { counter = value; CounterChanged?.Invoke(); } }

        public event Action CounterChanged;

        public AppModel()
        {
            Counter = 0;
        }
    }
}
