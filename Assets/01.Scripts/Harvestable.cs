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
        // 다음단계로 넘어 갈 수 있는지
        if(progress >= 1f)
        {
        }

        // 모든 단계에 도달했는지
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
        // Collectable 아이템을 스폰하고 수확불가능한 상태가 된다.
        isNowHarvestable = false; // 나중에 하루가 지나면 초기화 된다.
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
