using System;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using RealisticBattleSounds.Settings;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;

namespace RealisticBattleSounds
{
    public static class HandleBlowPatch
    {
        private static Agent _currentAgent = null!;
        private static sbyte _boneIndex;
        private static bool _isAlly;

        private static MethodInfo GetProtectorArmorMaterialOfBone =
            AccessTools.Method(typeof(Agent), "GetProtectorArmorMaterialOfBone");

        private static FieldInfo ___collisionResult =
            AccessTools.Field(typeof(AttackCollisionData), "_collisionResult");

        private static bool isMissile;

        [HarmonyPrefix]
        public static void Prefix(ref Blow b, in AttackCollisionData collisionData, Agent __instance)
        {
            _currentAgent = __instance;
            _boneIndex = b.BoneIndex;
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
                    _isAlly = true;
                    return;
                }

                if (RBSSettings.Instance?.DisableAllyCollision == true && attacker != null && victim != null && !attacker.IsEnemyOf(victim) && victim.IsHuman)
                {

                    _isAlly = true;
                    ___collisionResult.SetValue(collisionData, 0);
                    colReaction = MeleeCollisionReaction.ContinueChecking;
                }
                else
                    _isAlly = false;
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
                if (attackType == AgentAttackType.Standard && (!_isAlly || isMissile))
                {
                    ArmorComponent.ArmorMaterialTypes armor;
                    if (!_currentAgent.IsHuman)
                        try
                        {
                            armor = _currentAgent.SpawnEquipment[EquipmentIndex.HorseHarness].Item.ArmorComponent
                                .MaterialType;
                        }
                        catch (Exception)
                        {
                            armor = ArmorComponent.ArmorMaterialTypes.Leather;
                        }
                    else
                    {
                        armor = (ArmorComponent.ArmorMaterialTypes)GetProtectorArmorMaterialOfBone.Invoke(_currentAgent,
                            new object[] { _boneIndex });
                    }

                    int hitSound;

                    switch (damageType)
                    {
                        case DamageTypes.Cut:
                            hitSound = armor == ArmorComponent.ArmorMaterialTypes.Leather
                                ? (isCriticalBlow
                                    ? RealisticSoundsContainer.RealisticSoundsDic["rbs/leather/hit/crit"]
                                    : RealisticSoundsContainer.RealisticSoundsDic["rbs/leather/hit"])
                                : (armor == ArmorComponent.ArmorMaterialTypes.Chainmail
                                    ? RealisticSoundsContainer.RealisticSoundsDic["rbs/chainmail/hit"]
                                    : (armor == ArmorComponent.ArmorMaterialTypes.Plate
                                        ? (isCriticalBlow
                                            ? RealisticSoundsContainer.RealisticSoundsDic["rbs/plate/cut/crit"]
                                            : RealisticSoundsContainer.RealisticSoundsDic["rbs/plate/cut"])
                                        : (armor == ArmorComponent.ArmorMaterialTypes.Cloth
                                            ? RealisticSoundsContainer.RealisticSoundsDic["rbs/leather/hit"]
                                            : RealisticSoundsContainer.RealisticSoundsDic["rbs/flesh/cut"])));
                            break;
                        case DamageTypes.Pierce:
                            hitSound = armor == ArmorComponent.ArmorMaterialTypes.Leather
                                ? (isCriticalBlow
                                    ? RealisticSoundsContainer.RealisticSoundsDic["rbs/leather/hit/crit"]
                                    : RealisticSoundsContainer.RealisticSoundsDic["rbs/leather/hit"])
                                : (armor == ArmorComponent.ArmorMaterialTypes.Chainmail
                                    ? RealisticSoundsContainer.RealisticSoundsDic["rbs/chainmail/pierce"]
                                    : (armor == ArmorComponent.ArmorMaterialTypes.Plate
                                        ? RealisticSoundsContainer.RealisticSoundsDic["rbs/plate/pierce"]
                                        : (armor == ArmorComponent.ArmorMaterialTypes.Cloth
                                            ? (isCriticalBlow
                                                ? RealisticSoundsContainer.RealisticSoundsDic["rbs/flesh/pierce/crit"]
                                                : RealisticSoundsContainer.RealisticSoundsDic["rbs/cloth/pierce"])
                                            : (isCriticalBlow
                                                ? RealisticSoundsContainer.RealisticSoundsDic["rbs/flesh/pierce/crit"]
                                                : RealisticSoundsContainer.RealisticSoundsDic["rbs/flesh/pierce"]))));
                            break;
                        case DamageTypes.Blunt:
                            hitSound = armor == ArmorComponent.ArmorMaterialTypes.Leather
                                ? (isCriticalBlow
                                    ? RealisticSoundsContainer.RealisticSoundsDic["rbs/leather/hit/crit"]
                                    : RealisticSoundsContainer.RealisticSoundsDic["rbs/leather/blunt"])
                                : (armor == ArmorComponent.ArmorMaterialTypes.Chainmail
                                    ? RealisticSoundsContainer.RealisticSoundsDic["rbs/chainmail/hit"]
                                    : (armor == ArmorComponent.ArmorMaterialTypes.Plate
                                        ? (isCriticalBlow
                                            ? RealisticSoundsContainer.RealisticSoundsDic["rbs/plate/blunt/crit"]
                                            : RealisticSoundsContainer.RealisticSoundsDic["rbs/plate/blunt"])
                                        : (armor == ArmorComponent.ArmorMaterialTypes.Cloth
                                            ? RealisticSoundsContainer.RealisticSoundsDic["rbs/leather/blunt"]
                                            : RealisticSoundsContainer.RealisticSoundsDic["rbs/flesh/blunt"])));
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
        private Harmony _harmony = null!;

        private bool _isAgentPatchDone = false;
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
            _harmony = new Harmony("com.realistic_battle_sounds");
            _harmony.PatchAll();
        }

        public override void OnBeforeMissionBehaviorInitialize(Mission mission)
        {
            base.OnBeforeMissionBehaviorInitialize(mission);

            if (!_isAgentPatchDone)
            {
                _harmony.Patch(AccessTools.Method(typeof(Agent), "HandleBlow"),
                AccessTools.Method(typeof(HandleBlowPatch), "Prefix"));

                _isAgentPatchDone = true;
            }
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