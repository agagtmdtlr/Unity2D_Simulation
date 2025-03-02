using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Harvestable : MonoBehaviour
{
    GameObject cuttedTreeTop;


    SpriteRenderer treeRenderer;
    Sensor interaction;
    WoodCuttingSequencer sequencer;

    bool consumed;
    [SerializeField] public int currentState = 0;

    [SerializeField] int spawnCount;
    [SerializeField] ItemStat spwanItem;
    [SerializeField] Sprite[] spritePerStages;
    [SerializeField] GameObject spawnPrefab;

    bool isToolInteracting = false;
    ParticleSystem leafEffect;
    float effectPlayTime = 0f;


    public bool isEndState()
    {
        return (currentState == spritePerStages.Length - 1);
    }

    public void IncreamentStage()
    {
        currentState = Mathf.Min(currentState + 1, spritePerStages.Length - 1);

        treeRenderer.sprite = spritePerStages[currentState];

         
       
    }

    private void Awake()
    {
        TryGetComponent(out treeRenderer);
        TryGetComponent(out interaction);
        interaction.Interact.HasInteracted += BeginHarvest;

        cuttedTreeTop = transform.GetChild(0).gameObject;

        sequencer = FindAnyObjectByType<WoodCuttingSequencer>(FindObjectsInactive.Include);

        leafEffect = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        consumed = false;
    }

    private void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        if(isToolInteracting && Mathf.Abs(inputX) > 0f)
        {
            effectPlayTime = 1.5f;
            if (!leafEffect.isPlaying)
                leafEffect.Play();
        }
        effectPlayTime -=  Time.deltaTime;

        if( effectPlayTime < 0f)
        {
            leafEffect.Stop();
        }

        // 다음단계로 넘어 갈 수 있는지
        // 모든 단계에 도달했는지
        if (!consumed && isEndState())
        {
            EndHarvest();
        }
    }

    void EndHarvest()
    {
        StartCoroutine("PlayFallingTreeAnimation_Co");
    }

    IEnumerator PlayFallingTreeAnimation_Co()
    {
        // Collectable 아이템을 스폰하고 수확불가능한 상태가 된다.
        consumed = true; // 나중에 하루가 지나면 초기화 된다.
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
        if (consumed)
            return;

        sequencer.OnPlaySequencer(interaction.interactor.gameObject, this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Tool"))
        {
            isToolInteracting = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Tool"))
        {
            isToolInteracting = false;
        }
    }
}
