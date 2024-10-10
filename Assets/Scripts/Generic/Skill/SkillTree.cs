using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillNode
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public object Skill { get; private set; }
    public List<string> RequiredSkills { get; private set; }
    public bool isUnlocked { get; set; }
    public Vector2 Position { get; set; }
    public string SkillSeries { get; private set; }
    public int SkillLevel { get; private set; }
    public bool IsMaxLevel { get; set; }

    public SkillNode(string id, string name, object skill, Vector2 position, string skillSeries, int skillLevel, List<string> requiredSkillIds = null)
    {
        Id = id;
        Name = name;
        Skill = skill;
        Position = position;
        SkillSeries = skillSeries;
        RequiredSkills = requiredSkillIds ?? new List<string>();
        isUnlocked = false;
    }
}

public class SkillTree          //특성 트리 클래스
{
    public List<SkillNode> Nodes { get; private set; } = new List<SkillNode>();     //관리할 노드 List
    private Dictionary<string, SkillNode> nodeDictionary;

    public SkillTree()      //생성자
    {
        Nodes = new List<SkillNode>();
        nodeDictionary = new Dictionary<string, SkillNode>();
    }

    public void AddNode(SkillNode node)     //노드 추가 메서드
    {
        Nodes.Add(node);
        nodeDictionary[node.Id] = node;
    }

    public bool UnlockSkill(string skillId)
    {
        if (nodeDictionary.TryGetValue(skillId, out SkillNode node))
        {
            if (node.isUnlocked) return false;

            foreach(var requiredSkillId in node.RequiredSkills)
            {
                if (!nodeDictionary[requiredSkillId].isUnlocked)
                {
                    return false; 
                }
            }

            node.isUnlocked = true;
            return true;
        }

        return false;
    }

    public bool LockSkill(string skillId)
    {
        if(nodeDictionary.TryGetValue(skillId,out SkillNode node))
        {
            if (!node.isUnlocked) return false;

            foreach(var otherNode in Nodes) //이 스킬에 의존하는 다른 스킬이 있는지 확인
            {
                if(otherNode.isUnlocked && otherNode.RequiredSkills.Contains(skillId))
                {
                    return false;
                }    
            }

            node.isUnlocked = false;
            return true;
        }

        return false;
    }

    public bool IsSkillUnlock(string skillId)
    {
        return nodeDictionary.TryGetValue(skillId, out SkillNode node) && node.isUnlocked;
    }

    public SkillNode GetNode(string skillId)
    {
        nodeDictionary.TryGetValue(skillId, out SkillNode node);
        return node;
    }

    public List<SkillNode> GetAllNodes()
    {
        return new List<SkillNode>(Nodes);
    }
}
