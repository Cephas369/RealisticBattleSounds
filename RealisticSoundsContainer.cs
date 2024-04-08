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
            "blunt_armor",
            "blunt_armor_crit",
            "cloth_pierce",
            "cut_armor",
            "cut_armor_crit",
            "flesh_blunt",
            "flesh_cut",
            "flesh_pierce",
            "flesh_pierce_crit",
            "flesh_hit_crit",
            "leather_blunt",
            "leather_hit",
            "leather_hit_crit",
            "chainmail_hit",
            "pierce_armor",
            "pierce_mail"
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
