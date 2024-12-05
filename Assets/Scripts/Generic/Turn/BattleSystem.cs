using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    //싱글톤 패턴
    public static BattleSystem Instance { get; private set; }

    //캐릭터 배열
    public Character[] players = new Character[3];
    public Character[] enemies = new Character[3];

    //UI 요소들
    public Button attackBtn;        //공격 버튼
    public TextMeshProUGUI turnText;    //현재 턴 표시
    public GameObject damageTextPrefab; //데미지 표시 프리팹
    public Canvas uiCanvas;                 //UI 캔버스
    
    Queue<Character> turnQueue = new Queue<Character>();        //턴 순서 큐
    Character currentChar;              //현재 턴 캐릭터
    bool selectingTarget;               //타겟 선택 중인지 여부

    void Awake() => Instance = this;

    //현재 턴 캐릭터 반환
    public Character GetCurrentChar() => currentChar;
    //공격 버튼 클릭 시 타겟 선택 모드 활성화
    void OnAttackClick() => selectingTarget = true;


    // Start is called before the first frame update
    void Start()
    {
        //캐릭터들을 속도 순으로 정렬하여 턴 순서 초기화
        var orderedChars = players.Concat(enemies).OrderByDescending(c => c.speed);

        foreach (var c in orderedChars)
        {
            turnQueue.Enqueue(c);
        }
        //공격 버튼에 이벤트 연결
        attackBtn.onClick.AddListener(OnAttackClick);

        NextTurn();
    }

    // Update is called once per frame
    void Update()
    {
        //타겟 선택 모드에서 마우스 클릭 처리
        if (selectingTarget && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                Character target = hit.collider.GetComponent<Character>();
                if(target != null)
                {
                    currentChar.Attack(target);             //공격 실행
                    ShowDamageText(target.transform.position, "20");        //데미지 텍스트 표시
                    selectingTarget = false;
                    NextTurn();
                }
            }
        }
    }

    //다음 턴으로 진행
    void NextTurn()
    {
        //현재 턴 캐릭터 설정
        currentChar = turnQueue.Dequeue();
        turnQueue.Enqueue(currentChar);
        turnText.text = turnText.text = $"{currentChar.name} 의 턴 (Speed: {currentChar.speed})";

        //플레이어, 적 턴 처리
        if (currentChar.isPlayer)
        {
            attackBtn.gameObject.SetActive(true);       //플레이어 턴: 공격 버튼 활성화
        }
        else
        {
            attackBtn.gameObject.SetActive(false);      //적 턴: 공격 버튼 비활성화
            Invoke("EnemyAttack", 1f);                  //1초 후 적 공격
        }

    }

    //데미지 텍스트 생성 및 표시
    void ShowDamageText(Vector3 position, string damage)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
        GameObject damageObj = Instantiate(damageTextPrefab, screenPos, Quaternion.identity, uiCanvas.transform);
        damageObj.GetComponent<TextMeshProUGUI>().text = damage;
        Destroy(damageObj, 1f);
    }

    //AI의 적 공격 처리

    void EnemyAttack()
    {
        //생존한 플레이어 중 랜덤 타겟 선택
        var aliveTargets = players.Where(p => p.gameObject.activeSelf).ToArray();

        if (aliveTargets.Length == 0) return;           //모든 플레이어가 죽었으면 리턴

        var target = aliveTargets[Random.Range(0, aliveTargets.Length)];
        currentChar.Attack(target);
        ShowDamageText(target.transform.position, "20");
        NextTurn();
    }
}
