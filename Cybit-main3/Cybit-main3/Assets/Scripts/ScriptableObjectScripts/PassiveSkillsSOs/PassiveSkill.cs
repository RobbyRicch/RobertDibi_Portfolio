using UnityEngine;

[CreateAssetMenu(fileName = "New Passive Skill", menuName = "Passive Skill Tree/Passive Skill", order = 11)]
public class PassiveSkill : ScriptableObject
{
    public string skillName;
    public SkillType skillType;
    public StatType statToModify;
    public int cost;
    public float value;
    public bool isApplyingEffect = true;
    public bool isUnlocked = false; // if was bought
    public bool isLocked = true; // after two paths chosen
}