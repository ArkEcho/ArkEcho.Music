﻿using ArkEcho.Core;

namespace ArkEcho.RazorPage.Data
{
    public class ConnectionStatus
    {
        private Rest rest;
        private System.Timers.Timer timer;

        public bool Connected { get; private set; } = false;

        public event Action ConnectionStatusChanged;

        public ConnectionStatus(Rest rest)
        {
            this.rest = rest;

            timer = new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 5000;
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Start();
        }

        public async Task<bool> CheckConnectionAsync()
        {
            bool result = await rest.CheckConnection();
            if (result != Connected)
            {
                Connected = result;
                ConnectionStatusChanged?.Invoke();
            }
            return result;
        }

        private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            await CheckConnectionAsync();
        }
    }
}
