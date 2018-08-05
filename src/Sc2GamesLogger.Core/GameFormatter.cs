using System;
using System.Linq;

namespace Sc2GamesLogger.Core
{
    internal static class GameFormatter
    {
        private const string timeSpanFormat = @"hh\:mm\:ss";

        private const string playersSeparator = @" - ";

        private const string replayString = ", replay";

        private const string gameDataFormat = @"{0} - {1}, {2}{3}";

        public static string Format(GameData gameData, TimeSpan? endTime)
        {
            if (gameData.GameObject == null)
                return null;

            TimeSpan formattedStartTime = gameData.StartTime.Ticks >= 0 ? gameData.StartTime : TimeSpan.Zero;
            TimeSpan formattedEndTime = GetFormattedEndTime(gameData, formattedStartTime, endTime);

            return string.Format(gameDataFormat,
                formattedStartTime.ToString(timeSpanFormat),
                formattedEndTime.ToString(timeSpanFormat),
                string.Join(playersSeparator, gameData.GameObject.Players.Select(p => p.Name)),
                gameData.GameObject.IsReplay ? replayString : string.Empty);
        }

        private static TimeSpan GetFormattedEndTime(GameData gameData, TimeSpan formattedStartTime, TimeSpan? endTime)
        {
            if (endTime.HasValue)
            {
                return endTime.Value;
            }
            else
            {
                TimeSpan gameLength = TimeSpan.FromSeconds(gameData.GameObject.DisplayTime);
                TimeSpan difference = formattedStartTime - gameData.StartTime;

                return gameData.StartTime + gameLength - difference;
            }
        }
    }
}
