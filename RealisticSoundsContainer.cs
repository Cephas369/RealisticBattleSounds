using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace RealisticBattleSounds
{
    public static class RealisticSoundsContainer
    {

        public static MBFastRandom? RSRandom;

        private static string[] allSounds = new[]
        {
            "event:/voice/combat/cough",
            "event:/voice/combat/insult",
            "rbs/plate/blunt",
            "rbs/plate/blunt/crit",
            "rbs/plate/cut",
            "rbs/plate/cut/crit",
            "rbs/plate/pierce",
            "rbs/cloth/pierce",
            "rbs/flesh/blunt",
            "rbs/flesh/cut",
            "rbs/flesh/pierce",
            "rbs/flesh/pierce/crit",
            "rbs/flesh/crit",
            "rbs/leather/blunt",
            "rbs/leather/hit",
            "rbs/leather/hit/crit",
            "rbs/chainmail/hit",
            "rbs/chainmail/pierce"
        };
        public static Dictionary<string, int> RealisticSoundsDic = null;
        public static void StoreDic()
        {
            RSRandom = new MBFastRandom();
            RealisticSoundsDic = new Dictionary<string, int>();
            foreach (string id in allSounds)
                RealisticSoundsDic.Add(id, SoundEvent.GetEventIdFromString(id));
        }
    }
}
