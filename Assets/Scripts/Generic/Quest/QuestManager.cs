using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.QuestSystem;
using System.Linq;
using System;
using System.Reflection;

public class QuestManager : Singleton<QuestManager>
{
    private Dictionary<string, Quest> allQuests = new Dictionary<string, Quest>();              //게임의 모든 퀘스트를 저장하는 딕셔너리 
    private Dictionary<string, Quest> activeQuests = new Dictionary<string, Quest>();           //현재 진행 중인 퀘스틀를 저장하는 딕셔너리 
    private Dictionary<string, Quest> completedQuests = new Dictionary<string, Quest>();        //완료된 퀘스트를 저장하는 딕셔너리 

    public event Action<Quest> OnQuestStarted;              //퀘스트 시작 시 발생하는 이벤트 
    public event Action<Quest> OnQuestCompleted;            //퀘스트 완료 시 발생하는 이벤트 
    public event Action<Quest> OnQuestFailed;              //퀘스트 시패 시 발생하는 이벤트 

    public void Start()
    {
        InitialIzeQuests();
    }

    //기본 퀘스트들을 생성하고 등록하는 메서드
    private void InitialIzeQuests()
    {
        //쥐 사냥 퀘스트 생성 예시
        var ratHuntQuest = new Quest("Q001", "Rat Problem", "Clear the basement of rate", QuestType.Kill, 1);
        ratHuntQuest.AddCondition(new KillQuestCondition("Rat", 5));
        ratHuntQuest.AddReward(new ExperienceReward(100));
        ratHuntQuest.AddReward(new ItemReward("Gold", 50));

        //약초 수집 퀘스트
        var herbQuest = new Quest("Q002", "Herb Collection", "Collect herbs for the healer", QuestType.Collection, 1);
        herbQuest.AddCondition(new CollectionQuestContidion("Herb", 3));
        herbQuest.AddReward(new ExperienceReward(50));

        //퀘스트 매니저에 퀘스트 추가 
        allQuests.Add(ratHuntQuest.Id, ratHuntQuest);
        allQuests.Add(herbQuest.Id, herbQuest);

        //테스트 위해서 바로 시작 (StartQuest 함수)
        StartQuest("Q001");
        StartQuest("Q002");

    }

    public bool CanStartQuest(string questId)       //특정 퀘스트를 시작할 수 있는지 검사하는 메서드 
    {
        if (!allQuests.TryGetValue(questId, out Quest quest)) return false;
        if (activeQuests.ContainsKey(questId)) return false;
        if (completedQuests.ContainsKey(questId)) return false;

        //선행 퀘스트 완료 여부 확인
        foreach (var perrequisiteld in quest.GetType().GetField("prerequisiteQuestIds")?.GetValue(quest) as List<string> ?? new List<string>())
        {
            if (!completedQuests.ContainsKey(perrequisiteld)) return false;
        }

        //Type questType = quest.GetType();                                             //Qeust 객체의 타임을 가져온다.
        //FieldInfo perrequisiteldsField = questType.GetField("prerequisiteQuestIds");  //Quest Type에서 필드를 검색
        //object perrequisiteldsValue = perrequisiteldsField?.GetValue(quest);          //필드 값을 가져온다.
        //List<string> prerequisiteQuestIds = perrequisiteldsValue as List<string>;     //List 로 변환하고
        //prerequisiteQuestIds = prerequisiteQuestIds ?? new List<string>();            //null 값일 경우에는 빈 List 사용한다. 
        //?? null 병합 연산자 -> 왼쪽 값이 null 인지 확인하여 null 일 경우 오른쪽으로 봔환 

        return true;
    }

    //퀘스트를 시작하는 메서드 
    public void StartQuest(string questId)
    {
        if (!CanStartQuest(questId)) return;

        var quest = allQuests[questId];
        quest.Start();
        activeQuests.Add(questId, quest);
        OnQuestStarted?.Invoke(quest);
    }

    //퀘스트 진행 상황을 업데이트 하는 메서드
    public void UpdateQuestProgress(string questId)
    {
        if (!activeQuests.TryGetValue(questId, out Quest quest)) return;

        if (quest.CheckCompletion())
        {
            CompleteQuest(questId);
        }
    }

    //퀘스트를 완료 처리 하는 메서드 
    private void CompleteQuest(string questId)
    {
        if (!activeQuests.TryGetValue(questId, out Quest quest)) return;

        //플레이어 찾기 실패해도 퀘스트는 완료 
        var player = GameObject.FindGameObjectWithTag("Player");
        quest.Complete(player);                 //player가 null 이여도 진행되도록 함

        activeQuests.Remove(questId);
        completedQuests.Add(questId, quest);
        OnQuestCompleted?.Invoke(quest);

        Debug.Log($"Quest completed : {quest.Title}");
    }

    //시작 가능한 퀘스트 목록을 반환하는 메서드 
    public List<Quest> GetAvailableQuests()
    {
        return allQuests.Values.Where(q => CanStartQuest(q.Id)).ToList();
    }

    //현재 진행 중인 퀘스트 목록을 반환하는 메서드 

    public List<Quest> GetActiveQuest()
    {
        return activeQuests.Values.ToList();
    }

    //완료된 퀘스트 목록을 반환하는 메서드 

    public List<Quest> GetCompletedQuest()
    {
        return completedQuests.Values.ToList();
    }


    //적 처시 시 호출되는 이벤트 핸들러 
    public void OnEnemyKilled(string enemyType)
    {
        //활성 퀘스트의 복사본을 만들어서 사용
        var activeQuestsList = activeQuests.Values.ToList();

        foreach (var quest in activeQuestsList)
        {
            foreach (var condition in quest.GetConditions())
            {
                if (condition is KillQuestCondition killCondition)
                {
                    killCondition.EnemyKilled(enemyType);
                    UpdateQuestProgress(quest.Id);
                }
            }
        }
    }

    //수집 시 호출 되는 이벤트 핸들러
    public void OnItemCollected(string itemid)
    {
        //활성 퀘스트의 복사본을 만들어서 사용
        var activeQuestsList = activeQuests.Values.ToList();

        foreach (var quest in activeQuestsList)
        {
            foreach (var condition in quest.GetConditions())
            {
                if (condition is CollectionQuestContidion collectCondition)
                {
                    collectCondition.ItemCollected(itemid);
                    UpdateQuestProgress(quest.Id);
                }
            }
        }

    }


}