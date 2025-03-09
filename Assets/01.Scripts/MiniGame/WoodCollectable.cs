using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WoodCollectable : MonoBehaviour
{

    GameObject cuttedTreeTop;

    SpriteRenderer treeRenderer;
    Sensor interaction;
    Spawnable spawnable;
    Collider2D collider2d;

    bool consumed;
    [SerializeField] public int currentState = 0;
    [SerializeField] Sprite[] spritePerStages;
    [SerializeField] LayerMask whatIsTool;

    [SerializeField] bool isToolInteracting = false;
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
        TryGetComponent(out spawnable);
        TryGetComponent(out collider2d);

        cuttedTreeTop = transform.GetChild(0).gameObject;
        leafEffect = GetComponent<ParticleSystem>();

    }

    private void OnEnable()
    {
        consumed = false;
        interaction.interactEvent.AddListener(BeginHarvest);
    }

    private void OnDisable()
    {
        interaction.interactEvent.RemoveListener(BeginHarvest);
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
        collider2d.enabled = false;


        yield return new WaitForSeconds(4.0f);

        spawnable.Spawn(transform.position);
        cuttedTreeTop.SetActive(false);


        yield break;
    }

    public void BeginHarvest(Sensor sensor) 
    {
        if (consumed)
            return;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LayerMask colLayer = 1 << collision.gameObject.layer;
        if(colLayer == whatIsTool)
        {
            isToolInteracting = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        LayerMask colLayer = 1 << collision.gameObject.layer;
        if (colLayer == whatIsTool)
        {
            isToolInteracting = false;
        }
    }
}
