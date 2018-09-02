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

        private readonly object updateCurrentObjectLock;

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
            this.updateCurrentObjectLock = new object();

            this.current = new GameData(null, TimeSpan.Zero);
        }

        public void Start()
        {
            startTime = DateTime.UtcNow;
            current = new GameData(null, TimeSpan.Zero);
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

            lock (updateCurrentObjectLock)
            {
                WriteGameToFile(newGameObject, currentTimeSpan);
                current = new GameData(newGameObject, currentTimeSpan);
            }
        }

        private void WriteGameToFile(GameObject newGameObject, TimeSpan currentTimeSpan)
        {
            string formattedGame = GetFormattedGame(newGameObject, currentTimeSpan);
            if (formattedGame != null)
            {
                streamWriter.WriteLine(formattedGame);
                streamWriter.Flush();
            }
        }

        private string GetFormattedGame(GameObject newGameObject, TimeSpan currentTimeSpan)
        {
            if (!HasGameEnded(current.GameObject))
            {
                if (HasGameEnded(newGameObject))
                {
                    return GameFormatter.Format(current, GetEndTime(newGameObject.DisplayTime));
                }
                else
                {
                    return GameFormatter.Format(current, currentTimeSpan);
                }
            }
            else
            {
                return null;
            }
        }

        private TimeSpan GetEndTime(double displayTime) => current.StartTime + TimeSpan.FromSeconds(displayTime);

        private static bool HasGameEnded(GameObject gameObject)
        {
            return IsCorrectGameObject(gameObject) &&
                !undecidedResult.Equals(gameObject.Players[0].Result, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsCorrectGameObject(GameObject gameObject) => gameObject != null && gameObject.Players?.Length > 0;
    }
}
