using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyGame.QuestSystem;
using UnityEngine.UI;

public class SimpleQuestProgressUI : MonoBehaviour
{
    [Header("Quest List")]
    [SerializeField] private Transform questListParent;             //퀘스트 목록이 표시될 부모 Tranform
    [SerializeField] private GameObject questPrefabs;               //퀘스트 UI 프리팹

    [Header("Pregress Test")]
    [SerializeField] private Button KillEnemyButton;                //적 처치 테스트 버튼
    [SerializeField] private Button CollectItemButton;              //아이템 수집 터스트 버튼

    private QuestManager questManager;

    // Start is called before the first frame update
    void Start()
    {
        questManager = QuestManager.Instance;

        //버튼 이벤트 설정
        KillEnemyButton.onClick.AddListener(OnKillEnemy);
        CollectItemButton.onClick.AddListener(OnCollectItem);

        //이벤트 등록
        questManager.OnQuestStarted += UpdateQuestUI;
        questManager.OnQuestCompleted += UpdateQuestUI;

        //초기 퀘스트 생성 표시 
        RefreshQuestList();
    }

    private void CreateQuestUI(Quest quest) //개별 퀘스트 UI 생성
    {
        GameObject questObj = Instantiate(questPrefabs, questListParent);

        TextMeshProUGUI titleText = questObj.transform.Find("TitleText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI progressText = questObj.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>();

        titleText.text = quest.Title;
        progressText.text = $"Progress: {quest.GetProgress():P0}";
    }

    private void UpdateQuestUI(Quest quest)         //퀘스트 상태 변경시 UI 업데이트 
    {
        RefreshQuestList();
    }

    private void RefreshQuestList() //퀘스트 목록 새로고침 
    {

        foreach (Transform child in questListParent)  //기존 UI 제거 
        {
            Destroy(child.gameObject);
        }

        foreach (var quest in questManager.GetActiveQuest())    //활성 퀘스트 표시 
        {
            CreateQuestUI(quest);
        }
    }

    //적 처치 버튼 이벤트
    private void OnKillEnemy()
    {
        questManager.OnEnemyKilled("Rat");
        RefreshQuestList();
    }

    //아이템 수진 버튼 이벤트 
    private void OnCollectItem()
    {
        questManager.OnItemCollected("Herb");
        RefreshQuestList();
    }

}