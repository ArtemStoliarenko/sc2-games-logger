using Sc2ApiUpdater;
using System;
using System.Linq;

namespace Sc2GamesLogger.Core
{
    internal static class GameFormatter
    {
        private const string timeSpanFormat = @"hh\:mm\:ss";

        private const string playersSeparator = @" - ";

        private const string playerFormat = "{0} ({1})";

        private const string replayString = ", replay";

        private const string gameDataFormat = @"{0} - {1}, {2}{3}";

        public static string Format(GameData gameData, TimeSpan endTime)
        {
            TimeSpan formattedStartTime = gameData.StartTime.Ticks >= 0 ? gameData.StartTime : TimeSpan.Zero;

            return string.Format(gameDataFormat,
                formattedStartTime.ToString(timeSpanFormat),
                endTime.ToString(timeSpanFormat),
                string.Join(playersSeparator, gameData.GameObject.Players.Select(FormatPlayer)),
                gameData.GameObject.IsReplay ? replayString : string.Empty);
        }

        private static string FormatPlayer(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            return string.Format(playerFormat, player.Name, Char.ToUpper(player.Race[0]));
        }
    }
}
