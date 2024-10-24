using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.QuestSystem
{
    //몬스터 처치 퀘스트의 조건을 구현하는 클래스
    public class KillQuestCondition : IQuestCondition
    {
        private string enemyType;        //처치해야 할 적의 유형
        private int requiredKills;        //처치해야 할 총 적의 수
        private int currentKills;        //현재까지 처치한 적의 수

     
        public KillQuestCondition(string enemyType, int requiredKills)   //처치 퀘스트 조건 초기화 생성자
        {
            this.enemyType = enemyType;
            this.requiredKills = requiredKills;
            this.currentKills = 0;
        }


        public bool IsMet() => currentKills >= requiredKills;        //목표 처치 수를 달성했는지 확인
        public void Initialize() => currentKills = 0;                //처치 수를 0으로 초기화
        public float GetProgress() => (float)currentKills / requiredKills; //현재 처치 진행도를 퍼센트로 변환
        public string GetDescription() => $"Defeat {requiredKills} {enemyType} ({currentKills}/{requiredKills})";   //퀘스트 조건 설명을 문자열로 변환
        public void EnemyKilled(string enemyType)           //적 처치 시 호출되는 메서드
        {
            if (enemyType == enemyType)
            {
                currentKills++;
            }
        }
    }
}

