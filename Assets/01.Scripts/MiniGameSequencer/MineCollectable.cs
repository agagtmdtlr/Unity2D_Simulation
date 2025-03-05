using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineCollectable : MonoBehaviour
{
    [SerializeField] float chargeGauge = 0;
    [SerializeField] float chargeGaugeSuccessLimitation = 3f;
    [SerializeField] float chargeGaugeMax = 5f;

    Collider2D collider2d;
    Sensor interaction;
    [SerializeField] float hp;
    [SerializeField] bool isPlaying = false;
    [SerializeField] bool isCharging = false;

    [SerializeField] float attackSpeed = 1f;


    Rigidbody2D playerRigidBody;
    Controllable control;
    Animator playerAnimator;
    SpriteRenderer playerRender;
    GameObject tool;

    private void Awake()
    {
        TryGetComponent(out interaction);
        TryGetComponent(out collider2d);
    }

    private void OnEnable()
    {
        hp = 2;
        isPlaying = false;
    }

    public void ReadyMining()
    {
        // �̹� ��Ȯ�� �Ǿ�����?
        if (hp <= 0)
            return;

        isPlaying = true;
        isCharging = false;
        collider2d.isTrigger = false;
        chargeGauge = 0f;
        GameObject player = interaction.interactor.gameObject;

        if (player.TryGetComponent(out playerRigidBody))
        {
            playerRigidBody.isKinematic = true;
            playerRigidBody.velocity = Vector2.zero;
        }
        if (player.TryGetComponent(out control))
        {
            control.InputLocked = true;
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

        StartCoroutine(PlayMining_Co());
    }

    void UpdateChargeAnimation()
    {
        Vector3 rotation = Vector3.zero;
        float ratio =  EasingFunction.EaseOutCubic(0, 1, chargeGauge / chargeGaugeMax);
        rotation.z = Mathf.Lerp(0f, 120f, ratio);

        tool.transform.rotation = Quaternion.Euler(rotation);
        playerRender.color = Color.Lerp(Color.white, Color.red, ratio);

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

            // ������¡���� ���ݽ���
            if (chargeGauge > chargeGaugeMax)
            {
                yield return StartCoroutine(OverCharging_Co());
            }
            // ���� ����
            else if (Input.GetKeyUp(KeyCode.E))
            {
                yield return StartCoroutine(Attack_Co());
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

    

    // ��Ȯ�� �Ϸ���� �ʰ� �߰� �ߴ܉������� ȣ��ȴ�.
    void StopMining()
    {
        isPlaying = false;
        collider2d.isTrigger = false;
        control.InputLocked = false;
        playerRigidBody.isKinematic = false;
        playerRigidBody.velocity = Vector2.zero;
        playerAnimator.SetBool("Mine", false);
        playerRender.color = Color.white;
        tool.SetActive(false);
    }

    // ��Ȯ�� �Ϸ�������� ȣ��ȴ�
    void EndMining()
    {

    }
}
