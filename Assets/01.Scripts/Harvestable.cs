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
    // ��Ȯ ���� ���Խ�Ű��

    private void Awake()
    {
        TryGetComponent(out interaction);
        interaction.Interact.HasInteracted += BeginHarvest;
    }

    private void Update()
    {
        // �����ϸ�
        if(currentProgress == CompleteCount)
        {
            // Collectable �������� �����ϰ� ��Ȯ�Ұ����� ���°� �ȴ�.
            isNowHarvestable = false; // ���߿� �Ϸ簡 ������ �ʱ�ȭ �ȴ�.
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
            Debug.LogAssertion("HarvestController �� ���� ���� �ʽ��ϴ�. Interactor�� HarvestController�� �����ϴ� Ȯ�����ּ���.");
        }

    }

}
