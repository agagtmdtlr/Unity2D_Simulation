using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvestable : MonoBehaviour
{

    Interactable interaction;

    bool isNowHarvestable;
    int currentProgress = 0;
    int CompleteCount = 3;

    [SerializeField] GameObject interationBubbleUI;
    // 수확 모드로 진입시키기

    private void Awake()
    {
        TryGetComponent(out interaction);
        interaction.Interact.HasInteracted += BeginHarvest;
    }

    private void Update()
    {
        // 성공하면
        if(currentProgress == CompleteCount)
        {
            // Collectable 아이템을 스폰하고 수확불가능한 상태가 된다.
            isNowHarvestable = false; // 나중에 하루가 지나면 초기화 된다.
        }
    }

    void OnHarvested()
    {
        StartCoroutine("OnHarvested_co");
    }

    void OnHarvested_co()
    {

    }

    void BeginHarvest()
    {
        if (!isNowHarvestable)
            return;

        if ( interaction.interactor.TryGetComponent(out HarvestController harvestor) )
        {
            harvestor.StartHarvestMode(this);
        }
        else
        {
            Debug.LogAssertion("HarvestController 가 존재 하지 않습니다. Interactor에 HarvestController가 존재하는 확인해주세요.");
        }

    }

}
