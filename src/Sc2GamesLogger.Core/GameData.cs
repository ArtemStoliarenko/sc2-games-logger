using Sc2ApiUpdater;
using System;

namespace Sc2GamesLogger.Core
{
    internal struct GameData
    {
        public TimeSpan StartTime { get; }

        public GameObject GameObject { get; }

        public GameData(TimeSpan startTime, GameObject gameObject)
        {
            this.GameObject = gameObject;
            if (GameObject != null)
                startTime -= TimeSpan.FromSeconds(GameObject.DisplayTime);

            this.StartTime = startTime;
        }
    }
}
