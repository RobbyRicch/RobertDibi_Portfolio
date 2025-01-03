using UnityEngine;

[CreateAssetMenu(fileName = "New Passive Skill SO", menuName = "Scriptable Objects/Passive Skills/New Passive Skill", order = 12)]
public class PassiveSkillNewSO : ScriptableObject
{
    public int Id;
    public string SkillName;
    public StatType StatToModify;

    public int Cost;
    public float Factor;
}
