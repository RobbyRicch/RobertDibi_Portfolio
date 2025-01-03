using System.Collections.Generic;

public class SkillTreePath
{
    public SkillType PathType { get; private set; }
    public List<PassiveSkill> Skills { get; private set; }

    public SkillTreePath(SkillType pathType)
    {
        PathType = pathType;
        Skills = new List<PassiveSkill>();
    }

    public void AddSkill(PassiveSkill skill)
    {
        Skills.Add(skill);
    }

    public int GetUnlockedSkillCount()
    {
        int count = 0;
        foreach (PassiveSkill skill in Skills)
        {
            if (skill.isUnlocked)
            {
                count++;
            }
        }
        return count;
    }
}
