using Sc2ApiUpdater;
using System;
using System.IO;

namespace Sc2GamesLogger.Core
{
    public sealed class FileLogger : IDisposable
    {
        private const string undecidedResult = "Undecided";

        private readonly ApiUpdater apiUpdater;

        private readonly StreamWriter streamWriter;

        private readonly object streamWriterLock;

        private DateTime startTime;

        private GameData current;

        private bool disposed = false;

        public FileLogger(string path, int periodSeconds, int timeoutSeconds)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path));

            if (periodSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(periodSeconds));

            if (timeoutSeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(timeoutSeconds));

            this.apiUpdater = new ApiUpdater(ApiCalls.Game, periodSeconds, timeoutSeconds);
            apiUpdater.GameChanged += GameChangedHandler;

            this.streamWriter = new StreamWriter(path, true);
            this.streamWriterLock = new object();

            this.current = new GameData(TimeSpan.Zero, null);
        }

        public void Start()
        {
            startTime = DateTime.UtcNow;
            current = new GameData(TimeSpan.Zero, null);
            apiUpdater.Start();
        }

        public void Stop()
        {
            apiUpdater.Stop();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                apiUpdater.Dispose();
                streamWriter.Dispose();
                disposed = true;
            }
        }

        private void GameChangedHandler(object sender, GameObject newGameObject)
        {
            TimeSpan currentTimeSpan = DateTime.UtcNow - startTime;
            WriteGameToFile(newGameObject, currentTimeSpan);
            current = new GameData(currentTimeSpan, newGameObject);
        }

        private void WriteGameToFile(GameObject newGameObject, TimeSpan currentTimeSpan)
        {
            string formattedGame = GetFormattedGame(newGameObject, currentTimeSpan);
            if (formattedGame != null)
            {
                lock (streamWriterLock)
                {
                    streamWriter.WriteLine();
                    streamWriter.Flush();
                }
            }
        }

        private string GetFormattedGame(GameObject newGameObject, TimeSpan currentTimeSpan)
        {
            if (newGameObject != null &&
                newGameObject.Players?.Length > 0 &&
                !newGameObject.Players[0].Result.Equals(undecidedResult, StringComparison.OrdinalIgnoreCase))
            {
                return GameFormatter.Format(current, null);
            }
            else
            {
                return GameFormatter.Format(current, currentTimeSpan);
            }
        }
    }
}
