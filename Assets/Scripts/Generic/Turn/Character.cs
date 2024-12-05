using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Character : MonoBehaviour
{
    //캐릭터 스탯
    public bool isPlayer;           //플레이어 여부
    public int hp = 100;            //체력
    public int speed;               //속도 (턴 순서)

    //UI 요소
    TextMeshProUGUI nameText;       //이름 표시
    TextMeshProUGUI hpText;         //HP 표시
    Vector3 startPos;               //시작 위치 (공격 애니메이션용)


    // Start is called before the first frame update
    void Start()
    {
        SetUpNameText();
        startPos = transform.position;
    }
    public void Attack(Character target)
    {
        if (!target.gameObject.activeSelf) return;      //죽은 타겟이면 무시
        {
            StartCoroutine(AttackRoutine(target));  //공격 애니메이션 시작
        }
    }

    // Update is called once per frame
    void Update()
    {
        //캐릭터가 비활성화되면 UI 제거
        if (!gameObject.activeSelf)
        {
            if (nameText != null) Destroy(nameText.gameObject);
            if (hpText != null) Destroy(hpText.gameObject);
            return;
        }
        //UI 위치 업데이트
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2);
        nameText.transform.position = screenPos;
        hpText.transform.position = screenPos + Vector3.down * 30;

        //현재 턴 캐릭터 표시 (초록색)
        nameText.color = (BattleSystem.Instance.GetCurrentChar() == this) ? Color.green : Color.white;
        hpText.text = hp.ToString();
    }

    //공격 애니메이션 코루틴
    IEnumerator AttackRoutine(Character target)
    {
        //타겟쪽으로 이동
        Vector3 attackPos = target.transform.position + (target.transform.position - transform.position).normalized * 1.5f;
        float moveTime = 0.3f;
        float elapsed = 0;

        while (elapsed < moveTime)
        {
            //이동 애니메이션
            elapsed += Time.deltaTime;
            float t = elapsed / moveTime;
            transform.position = Vector3.Lerp(startPos, attackPos, t);
            yield return null;
        }

        //데미지 처리
        target.hp -= 20;
        if (target.hp <= 0) target.gameObject.SetActive(false);

        //원위치로 복귀
        elapsed = 0;
        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveTime;
            transform.position = Vector3.Lerp(attackPos, startPos, t);
            yield return null;
        }
    }

    //UI 텍스트 초기화
    void SetUpNameText()
    {
        //이름 텍스트 설정
        GameObject textObj = new GameObject("NameText");
        textObj.transform.SetParent(BattleSystem.Instance.uiCanvas.transform);
        nameText = textObj.AddComponent<TextMeshProUGUI>();
        nameText.text = isPlayer ? "P" : "E";
        nameText.fontSize = 36;
        nameText.alignment = TextAlignmentOptions.Center;

        //HP 텍스처 설정
        GameObject hpObj = new GameObject("HPText");
        hpObj.transform.SetParent(BattleSystem.Instance.uiCanvas.transform);
        hpText = hpObj.AddComponent<TextMeshProUGUI>();
        hpText.fontSize = 30;
        hpText.alignment = TextAlignmentOptions.Center;

    }

    

}
