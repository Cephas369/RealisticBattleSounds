using RealisticBattleSounds.Settings;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RealisticBattleSounds;

internal class HumanSoundsMissionBehavior : MissionBehavior
{
    public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;
    private float CoughRate;
    private float InsultRate;
    private bool insultActivated;

    public override void OnDeploymentFinished()
    {
        InsultRate = Mission.Agents.Count * 0.0001f;
        CoughRate = Mission.Agents.Count * 0.00006f;
        insultActivated = RBSSettings.Instance?.EnableInsults == true;
    }
    public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
    {
        if (Mission.Agents.Count > 1)
        {
            InsultRate = Mission.Agents.Count * 0.0001f;
            CoughRate = Mission.Agents.Count * 0.00006f;
        }
        else
            CoughRate = 0;
    }

    public override void OnMissionTick(float dt)
    {
        if (!Mission.IsFinalized && RealisticSoundsContainer.RSRandom.NextFloat() <= InsultRate && Mission.Mode == MissionMode.Battle && !Mission.IsInPhotoMode && !MBCommon.IsPaused)
        {
            Agent agent = this.Mission.Agents.GetRandomElementWithPredicate(x => x.Health > 0 && !x.IsFemale && !x.IsMainAgent && !x.IsCheering && !x.IsRetreating());

            if (agent == null)
                return;
            
            if (insultActivated && agent.AttackDirection != Agent.UsageDirection.None)
                Mission.MakeSound(RealisticSoundsContainer.RealisticSoundsDic["event:/voice/combat/insult"], agent.Position, false, false, agent.Index, Agent.Main != null ? Agent.Main.Index : agent.Index);

            if (RealisticSoundsContainer.RSRandom.NextFloat() <= CoughRate)
            {
                if (agent.WalkMode || agent.HasMount)
                    Mission.MakeSound(RealisticSoundsContainer.RealisticSoundsDic["event:/voice/combat/cough"], agent.Position, false, false, agent.Index, Agent.Main != null ? Agent.Main.Index : agent.Index);
            }
        }

    }
}