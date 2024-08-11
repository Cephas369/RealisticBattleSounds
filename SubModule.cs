using HarmonyLib;
using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using System.Reflection;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Base.Global;
using TaleWorlds.ModuleManager;
using RealisticBattleSounds.Settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.GauntletUI;
using Module = TaleWorlds.MountAndBlade.Module;
using System.Text.RegularExpressions;
using TaleWorlds.CampaignSystem.Party;

namespace RealisticBattleSounds
{
    [HarmonyPatch(typeof(Agent), "HandleBlow")]
    public static class HandleBlowPatch
    {
        private static Agent currentAgent = null;
        private static sbyte BoneIndex;
        private static bool isally;

        private static MethodInfo GetProtectorArmorMaterialOfBone =
            AccessTools.Method(typeof(Agent), "GetProtectorArmorMaterialOfBone");

        private static FieldInfo ___collisionResult =
            AccessTools.Field(typeof(AttackCollisionData), "_collisionResult");

        private static bool isMissile;

        [HarmonyPrefix]
        public static void Prefix(ref Blow b, in AttackCollisionData collisionData, Agent __instance)
        {
            currentAgent = __instance;
            BoneIndex = b.BoneIndex;
        }


        [HarmonyPatch(typeof(Mission), "MeleeHitCallback")]
        [HarmonyAfter("com.basic_overhaul")]
        public static class MeleeHitCallbackPatch
        {
            static void Prefix(
                ref AttackCollisionData collisionData,
                Agent attacker,
                Agent victim,
                GameEntity realHitEntity,
                ref float inOutMomentumRemaining,
                ref MeleeCollisionReaction colReaction,
                CrushThroughState crushThroughState,
                Vec3 blowDir,
                Vec3 swingDir,
                ref HitParticleResultData hitParticleResultData,
                bool crushedThroughWithoutAgentCollision)
            {
                if (colReaction == MeleeCollisionReaction.ContinueChecking)
                {
                    isally = true;
                    return;
                }
                else if (RBSSettings.Instance?.DisableAllyCollision == true && attacker != null && victim != null && !attacker.IsEnemyOf(victim) && victim.IsHuman)
                {

                    isally = true;
                    ___collisionResult.SetValue(collisionData, 0);
                    colReaction = MeleeCollisionReaction.ContinueChecking;
                }
                else
                    isally = false;
            }
        }


        [HarmonyPatch(typeof(BlowWeaponRecord), "GetHitSound")]
        public static class GetHitSoundPatch
        {
            static bool Prefix(bool isOwnerHumanoid, bool isCriticalBlow, bool isLowBlow, bool isNonTipThrust,
                AgentAttackType attackType, DamageTypes damageType, ref int __result, BlowWeaponRecord __instance)
            {
                isMissile = RBSSettings.Instance?.DisableFarMissileSounds == true &&
                            (__instance.IsMissile || __instance.IsAmmo || __instance.IsRanged);
                if (attackType == AgentAttackType.Standard && (!isally || isMissile))
                {
                    ArmorComponent.ArmorMaterialTypes armor;
                    if (!currentAgent.IsHuman)
                        try
                        {
                            armor = currentAgent.SpawnEquipment[EquipmentIndex.HorseHarness].Item.ArmorComponent
                                .MaterialType;
                        }
                        catch (Exception)
                        {
                            armor = ArmorComponent.ArmorMaterialTypes.Leather;
                        }
                    else
                    {
                        armor = (ArmorComponent.ArmorMaterialTypes)GetProtectorArmorMaterialOfBone.Invoke(currentAgent,
                            new object[] { BoneIndex });
                    }

                    int hitSound;

                    switch (damageType)
                    {
                        case DamageTypes.Cut:
                            hitSound = armor == ArmorComponent.ArmorMaterialTypes.Leather
                                ? (isCriticalBlow
                                    ? RealisticSoundsContainer.RealisticSoundsDic["leather_hit_crit"]
                                    : RealisticSoundsContainer.RealisticSoundsDic["leather_hit"])
                                : (armor == ArmorComponent.ArmorMaterialTypes.Chainmail
                                    ? RealisticSoundsContainer.RealisticSoundsDic["chainmail_hit"]
                                    : (armor == ArmorComponent.ArmorMaterialTypes.Plate
                                        ? (isCriticalBlow
                                            ? RealisticSoundsContainer.RealisticSoundsDic["cut_armor_crit"]
                                            : RealisticSoundsContainer.RealisticSoundsDic["cut_armor"])
                                        : (armor == ArmorComponent.ArmorMaterialTypes.Cloth
                                            ? RealisticSoundsContainer.RealisticSoundsDic["leather_hit"]
                                            : RealisticSoundsContainer.RealisticSoundsDic["flesh_cut"])));
                            break;
                        case DamageTypes.Pierce:
                            hitSound = armor == ArmorComponent.ArmorMaterialTypes.Leather
                                ? (isCriticalBlow
                                    ? RealisticSoundsContainer.RealisticSoundsDic["leather_hit_crit"]
                                    : RealisticSoundsContainer.RealisticSoundsDic["leather_hit"])
                                : (armor == ArmorComponent.ArmorMaterialTypes.Chainmail
                                    ? RealisticSoundsContainer.RealisticSoundsDic["pierce_mail"]
                                    : (armor == ArmorComponent.ArmorMaterialTypes.Plate
                                        ? RealisticSoundsContainer.RealisticSoundsDic["pierce_armor"]
                                        : (armor == ArmorComponent.ArmorMaterialTypes.Cloth
                                            ? (isCriticalBlow
                                                ? RealisticSoundsContainer.RealisticSoundsDic["flesh_pierce_crit"]
                                                : RealisticSoundsContainer.RealisticSoundsDic["cloth_pierce"])
                                            : (isCriticalBlow
                                                ? RealisticSoundsContainer.RealisticSoundsDic["flesh_pierce_crit"]
                                                : RealisticSoundsContainer.RealisticSoundsDic["flesh_pierce"]))));
                            break;
                        case DamageTypes.Blunt:
                            hitSound = armor == ArmorComponent.ArmorMaterialTypes.Leather
                                ? (isCriticalBlow
                                    ? RealisticSoundsContainer.RealisticSoundsDic["leather_hit_crit"]
                                    : RealisticSoundsContainer.RealisticSoundsDic["leather_blunt"])
                                : (armor == ArmorComponent.ArmorMaterialTypes.Chainmail
                                    ? RealisticSoundsContainer.RealisticSoundsDic["chainmail_hit"]
                                    : (armor == ArmorComponent.ArmorMaterialTypes.Plate
                                        ? (isCriticalBlow
                                            ? RealisticSoundsContainer.RealisticSoundsDic["blunt_armor_crit"]
                                            : RealisticSoundsContainer.RealisticSoundsDic["blunt_armor"])
                                        : (armor == ArmorComponent.ArmorMaterialTypes.Cloth
                                            ? RealisticSoundsContainer.RealisticSoundsDic["leather_blunt"]
                                            : RealisticSoundsContainer.RealisticSoundsDic["flesh_blunt"])));
                            break;

                        case DamageTypes.Invalid:
                        default:
                            hitSound = 0;
                            break;
                    }

                    __result = hitSound;

                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(CombatSoundContainer), "SoundCodeMissionCombatPlayerhit", MethodType.Getter)]
        public static class SoundCodeMissionCombatPlayerhitPatch
        {
            static void Postfix(ref int __result)
            {
                if (isMissile)
                    __result = 0;
            }
        }
    }


    public class SubModule : MBSubModuleBase
    {
        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            try
            {
                RealisticSoundsContainer.StoreDic();
                mission.AddMissionBehavior(new HumanSoundsMissionBehavior());
            }
            catch (Exception)
            {
                InformationManager.DisplayMessage(new InformationMessage("RealisticSounds error"));
            }
        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Harmony harmony = new Harmony("com.realistic_battle_sounds");
            harmony.PatchAll();
        }
    }
}


namespace RealisticBattleSounds.Settings
{
    internal sealed class RBSSettings : AttributeGlobalSettings<RBSSettings>
    {
        public override string Id => "RealisticBattleSounds";

        public override string DisplayName =>
            $"Realistic Battle Sounds {Regex.Replace(ModuleHelper.GetModuleInfo(Assembly.GetExecutingAssembly().GetName().Name).Version.ToString(), @"v|\.0$", string.Empty)}";

        public override string FolderName => "Realistic Battle Sounds";
        public override string FormatType => "json";

        [SettingPropertyBool("Disable weapon collision and blow sounds for allies", Order = 0, RequireRestart = false,
            HintText = "Your hits will pass through allies instead hitting them, improves the polearm efficiency.")]
        [SettingPropertyGroup("General")]
        public bool DisableAllyCollision { get; set; } = true;

        [SettingPropertyBool("Disable missile hit sounds being heard wherever the victim is at", Order = 1,
            RequireRestart = false, HintText = "You won't hear when a missile hits a troop if it's far away from you.")]
        [SettingPropertyGroup("General")]
        public bool DisableFarMissileSounds { get; set; } = true;
        
        [SettingPropertyBool("Enable insults from warband", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("General")]
        public bool EnableInsults { get; set; } = true;
    }
}