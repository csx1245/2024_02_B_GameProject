using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyGame.QuestSystem
{
    //퀘스트 조건을 정의하는 인터페이스
    public interface IQuestCondition
    {
        bool IsMet();               //조건이 충족되었는지 여부 반환

        void Initialize();          //조건을 초기화하는 메서드

        float GetProgress();        //조건 충족도 범위

        string GetDescription();    //조건 설명 반환 메서드
    }
}

