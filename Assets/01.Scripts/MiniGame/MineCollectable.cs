using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MineCollectable : MonoBehaviour
{
    [SerializeField] float chargeGauge = 0;
    [SerializeField] float chargeGaugeSuccessLimitation = 3f;
    [SerializeField] float chargeGaugeMax = 5f;

    [SerializeField] float hp;
    [SerializeField] bool isPlaying = false;
    [SerializeField] bool isCharging = false;
    [SerializeField] float attackSpeed = 1f;

    Collider2D collider2d;

    Sensor sensor;

    Rigidbody2D playerRigidBody;
    Controllable control;
    Animator playerAnimator;
    SpriteRenderer playerRender;
    GameObject tool;
    SpriteRenderer toolRender;
    Light2D toolLight;

    private void Awake()
    {
        TryGetComponent(out sensor);
        TryGetComponent(out collider2d);
    }

    private void OnEnable()
    {
        hp = 2;
        isPlaying = false;
        collider2d.enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void ReadyMining()
    {
        // 이미 수확이 되었나요?
        if (hp <= 0)
            return;

        isPlaying = true;
        isCharging = false;
        chargeGauge = 0f;
        GameObject player = sensor.interactor.gameObject;

        if (player.TryGetComponent(out playerRigidBody))
        {
            playerRigidBody.isKinematic = true;
            playerRigidBody.velocity = Vector2.zero;
        }
        if (player.TryGetComponent(out control))
        {
            control.Lock(this);
        }
        if (player.TryGetComponent(out playerAnimator))
        {
            playerAnimator.SetBool("Mine", true);
        }
        if (player.TryGetComponent(out playerRender))
        {
        }
        tool = player.transform.Find("tool_pickax").gameObject;
        tool.SetActive(true);
        tool.transform.GetChild(0).TryGetComponent(out toolLight);
        tool.TryGetComponent(out toolRender);

        StartCoroutine(PlayMining_Co());
    }

    void UpdateChargeAnimation()
    {
        Vector3 rotation = Vector3.zero;
        float ratio =  EasingFunction.EaseOutCubic(0, 1, chargeGauge / chargeGaugeMax);
        rotation.z = Mathf.Lerp(0f, 120f, ratio);

        tool.transform.rotation = Quaternion.Euler(rotation);
        toolLight.intensity = Mathf.Lerp(0f, 15f, ratio);

        Color gaugeColor = Color.Lerp(Color.white, Color.red, ratio);
        toolRender.color = gaugeColor;
        playerRender.color = gaugeColor;


        playerAnimator.SetFloat("Charge", ratio);
    }
    
    IEnumerator PlayMining_Co()
    {
        while(isPlaying && hp > 0f)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isCharging = true;
                chargeGauge = 0f;
                playerAnimator.SetBool("MineAttack", true);
            }

            if (isCharging && Input.GetKey(KeyCode.E))
            {
                chargeGauge += Time.deltaTime;
            }

            bool isoverCharging = chargeGauge > chargeGaugeMax;
            if (isoverCharging)
            {
                yield return StartCoroutine(OverCharging_Co());
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                yield return StartCoroutine(Attack_Co());
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                isPlaying = false;
            }

            UpdateChargeAnimation();
            yield return null;
        }

        StopMining();
        yield break;
    }

    IEnumerator OverCharging_Co()
    {
        playerAnimator.SetBool("MineAttack", false);
        playerAnimator.SetBool("OverCharge", true);
        isCharging = false;
        chargeGauge = 0f;

        yield return new WaitForSeconds(2f);
        playerAnimator.SetBool("OverCharge", false);
    }

    IEnumerator Attack_Co()
    {
        bool mineSuccess = chargeGaugeSuccessLimitation <= chargeGauge;
        float mineDamage = chargeGauge / chargeGaugeMax;
        playerAnimator.SetBool("MineSuccess", mineSuccess);
        playerAnimator.SetBool("MineAttack", false);
        isCharging = false;

        while (chargeGauge >= 0f)
        {
            chargeGauge -= Time.deltaTime * attackSpeed;

            UpdateChargeAnimation();
            yield return null;
        }

        if(mineSuccess)
        {
            hp -= mineDamage;
        }
        chargeGauge = 0f;
        playerAnimator.SetBool("MineSuccess", false);

    }

    

    void StopMining()
    {
        if(hp <= 0)
        {
            if(TryGetComponent(out Spawnable spawnable))
            {
                spawnable.Spawn(transform.position);
            }

            GetComponent<SpriteRenderer>().enabled = false;
            collider2d.enabled = false;
        }

        chargeGauge = 0;

        isPlaying = false;
        control.UnLock(this);
        playerRigidBody.isKinematic = false;
        playerRigidBody.velocity = Vector2.zero;
        playerAnimator.SetBool("Mine", false);
        playerRender.color = Color.white;
        toolRender.color = Color.white;
        tool.SetActive(false);
        toolLight.intensity = 0;
    }

}
