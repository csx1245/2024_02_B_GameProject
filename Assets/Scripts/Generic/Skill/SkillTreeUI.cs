using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeUI : MonoBehaviour
{
    public SkillTree skillTree;
    public GameObject skillNodePrefabs;
    public RectTransform skillTreePanel;
    public float NodeSpacing = 100f;
    public Text SkillPointText;
    public int totalSkillPoint = 10;

    private Dictionary<string, Button> skillButtons = new Dictionary<string, Button>();

    void Start()
    {
        InitializeSkillTree();
        CreateSkillTreeUI();
        UpdateSkillPointsUI();
    }

    void InitializeSkillTree()
    {
        skillTree = new SkillTree();

        skillTree.AddNode(new SkillNode("Fireball 1", "Fireball 1",
            new Skill<ISkillTarget, DamageEffect>("Fireball 1", new DamageEffect(20)),
            new Vector2(0, 0), "Fireball", 1));

        skillTree.AddNode(new SkillNode("Fireball 2", "Fireball 2",
            new Skill<ISkillTarget, DamageEffect>("Fireball 2", new DamageEffect(30)),
            new Vector2(0, 1), "Fireball", 2, new List<string> { "Fireball 1" }));

        skillTree.AddNode(new SkillNode("Fireball 3", "Fireball 3",
            new Skill<ISkillTarget, DamageEffect>("Fireball 3", new DamageEffect(40)),
            new Vector2(0, 2), "Fireball", 3, new List<string> { "Fireball 2" }));

        skillTree.AddNode(new SkillNode("Fireball 4", "Fireball 4",
            new Skill<ISkillTarget, DamageEffect>("Fireball 4", new DamageEffect(50)),
            new Vector2(0, 3), "Fireball", 4, new List<string> { "Fireball 3" }));

        skillTree.AddNode(new SkillNode("Firebolt 1", "Firebolt 1",
            new Skill<ISkillTarget, DamageEffect>("Firebolt 1", new DamageEffect(90)),
            new Vector2(1, 1), "Firebolt", 1, new List<string> { "Fireball 2" }));

        skillTree.AddNode(new SkillNode("Firebolt 2", "Firebolt 2",
            new Skill<ISkillTarget, DamageEffect>("Firebolt 2", new DamageEffect(140)),
            new Vector2(2, 1), "Firebolt", 2, new List<string> { "Firebolt 1" }));

        skillTree.AddNode(new SkillNode("Firebolt 3", "Firebolt 3",
            new Skill<ISkillTarget, DamageEffect>("Firebolt 3", new DamageEffect(240)),
            new Vector2(3, 1), "Firebolt", 3, new List<string> { "Firebolt 2" }));
    }


    void CreateSkillTreeUI()
    {
        foreach (var node in skillTree.Nodes)
        {
            CreateSkillNodeUI(node);
        }
    }

    void CreateSkillNodeUI(SkillNode node)
    {
        GameObject nodeObj = Instantiate(skillNodePrefabs, skillTreePanel);
        RectTransform rectTransform = nodeObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = node.Position * NodeSpacing;

        Button button = nodeObj.GetComponent<Button>();
        Text text = nodeObj.GetComponentInChildren<Text>();
        text.text = node.Name;

        button.onClick.AddListener(()=> OnSkillNodeClicked(node.Id));
        skillButtons[node.Id] = button;
        UpdateNodeUI(node);
 
    }

    private void OnSkillNodeClicked(string skillId)
    {
        SkillNode node = skillTree.GetNode(skillId);

        if (node == null) return;

        if (node.isUnlocked)
        {
            if (skillTree.LockSkill(skillId))
            {
                totalSkillPoint++;
                UpdateSkillPointsUI();
                UpdateNodeUI(node);
                UpdateConnectedSkills(skillId);
            }
            else
            {
                Debug.Log("관련 연계 스킬이 있어 해제할 수 없습니다.");
            }
        }
        
        else if (totalSkillPoint > 0 && CanUnlockSkill(node))
        {
            if(skillTree.UnlockSkill(skillId))
            {
                totalSkillPoint--;
                UpdateSkillPointsUI();
                UpdateNodeUI(node);
                UpdateConnectedSkills(skillId);
            }
        }
    }

    private void UpdateNodeUI(SkillNode node)       //동작이 일어났을 때 UI 업데이트
    {
        if (skillButtons.TryGetValue(node.Id, out Button button))
        {
            bool canUnlock = !node.isUnlocked && CanUnlockSkill(node);
            button.interactable = (canUnlock && totalSkillPoint > 0) || node.isUnlocked;
            button.GetComponent<Image>().color = node.isUnlocked ? Color.green : (canUnlock ? Color.yellow : Color.red);
        }
    }

    private bool CanUnlockSkill(SkillNode node)     //Lock 해제 관련 함수
    {
        foreach (var requiredSkillId in node.RequiredSkills)
        {
            if (!skillTree.IsSkillUnlock(requiredSkillId))
            {
                return false;
            }
        }

        return true;
    }

    void UpdateSkillPointsUI()
    {
        SkillPointText.text = $"Skill Points: {totalSkillPoint}";
    }

    void UpdateConnectedSkills(string skillId)
    {
        foreach (var node in skillTree.Nodes)
        {
            if (node.RequiredSkills.Contains(skillId))
            {
                UpdateNodeUI(node);
            }
        }
    }
}
