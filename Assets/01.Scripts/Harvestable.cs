using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvestable : MonoBehaviour
{
    GameObject cuttedTreeTop;

    Rigidbody2D rb;
    SpriteRenderer renderer2d;
    Interactable interaction;
    WoodCuttingSequencer sequencer;

    bool isNowHarvestable;
    float progress = 0;
    [SerializeField] public int currentState = 0;

    [SerializeField] int spawnCount;
    [SerializeField] ItemStat spwanItem;
    [SerializeField] Sprite[] spritePerStages;
    [SerializeField] GameObject spawnPrefab;

    public bool isEndState()
    {
        return (currentState == spritePerStages.Length - 1);
    }

    public void IncreamentStage()
    {
        currentState = Mathf.Min(currentState + 1, spritePerStages.Length - 1);

        renderer2d.sprite = spritePerStages[currentState];


        // ended
        if(currentState == spritePerStages.Length - 1)
        {
            cuttedTreeTop.SetActive(true);
        }
       
    }

    private void Awake()
    {
        TryGetComponent(out renderer2d);
        TryGetComponent(out interaction);
        interaction.Interact.HasInteracted += BeginHarvest;

        cuttedTreeTop = transform.GetChild(0).gameObject;

        sequencer = FindAnyObjectByType<WoodCuttingSequencer>(FindObjectsInactive.Include);
    }

    private void OnEnable()
    {
        isNowHarvestable = true;
    }

    private void Update()
    {
        // �����ܰ�� �Ѿ� �� �� �ִ���
        if(progress >= 1f)
        {
        }

        // ��� �ܰ迡 �����ߴ���
        if(isNowHarvestable && isEndState())
        {
            EndHarvest();
        }
    }

    void EndHarvest()
    {
        StartCoroutine("OnHarvested_co");
    }

    IEnumerator OnHarvested_co()
    {
        // Collectable �������� �����ϰ� ��Ȯ�Ұ����� ���°� �ȴ�.
        isNowHarvestable = false; // ���߿� �Ϸ簡 ������ �ʱ�ȭ �ȴ�.
        cuttedTreeTop.SetActive(true);


        yield return new WaitForSeconds(4.0f);

        GameObject spawnObject = Instantiate(spawnPrefab, transform.position, Quaternion.identity);
        if (TryGetComponent(out Collectable collectable))
        {
            collectable.item.itemInformation = spwanItem;
            collectable.item.itemAmount = spawnCount;
        }


        cuttedTreeTop.SetActive(false);


        yield break;
    }

    public void BeginHarvest() 
    {
        if (!isNowHarvestable)
            return;

        sequencer.OnPlaySequencer(interaction.interactor.gameObject, this);
    }

}
