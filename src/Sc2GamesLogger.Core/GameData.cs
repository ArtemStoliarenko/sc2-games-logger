using Sc2ApiUpdater;
using System;

namespace Sc2GamesLogger.Core
{
    internal struct GameData
    {
        public GameObject GameObject { get; }

        public TimeSpan StartTime { get; }

        public GameData(GameObject gameObject, TimeSpan startTime)
        {
            this.GameObject = gameObject;
            if (GameObject != null)
                startTime -= TimeSpan.FromSeconds(GameObject.DisplayTime);

            this.StartTime = startTime;
        }
    }
}
