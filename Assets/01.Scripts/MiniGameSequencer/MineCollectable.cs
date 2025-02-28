using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineCollectable : MonoBehaviour
{
    [SerializeField] float chargeGauge = 0;
    [SerializeField] float chargeGaugeMax = 5f;

    SpriteRenderer renderer2d;
    Interactable interaction;
    [SerializeField] int hp;
    [SerializeField] bool isPlaying = false;
    [SerializeField] bool isCharging = false;
    [SerializeField] float chargingTime;

    private void Awake()
    {
        TryGetComponent(out renderer2d);
        TryGetComponent(out interaction);

    }

    private void OnEnable()
    {
        hp = 2;
        isPlaying = false;
    }

    public void ReadyMining()
    {
        // �̹� ��Ȯ�� �Ǿ�����?
        if (hp == 0)
            return;

        PlayMining();
    }

    Rigidbody2D playerRigidBody;
    PlayerController playerController;
    Animator playerAnimator;
    void PlayMining()
    {
        isPlaying = true;
        isCharging = false;
        chargingTime = 0f;
        GameObject player = interaction.interactor.gameObject;

        if (player.TryGetComponent(out playerRigidBody))
        {
            playerRigidBody.isKinematic = true;
            playerRigidBody.velocity = Vector2.zero;
        }
        if (player.TryGetComponent(out playerController))
        {
            playerController.inputLocked = true;
        }
        if (player.TryGetComponent(out playerAnimator))
        {
            playerAnimator.SetBool("Mine", true);
        }
    }

    float chargeStartTime;

    private void Update()
    {
        if (!isPlaying)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            isCharging = true;
            chargingTime = 0f;
        }

        if(isCharging && Input.GetKey(KeyCode.E))
        {
            chargingTime += Time.deltaTime;
        }

        playerAnimator.SetFloat("Charge", chargingTime);
        if(isCharging && Input.GetKeyUp(KeyCode.E))
        {
            playerAnimator.SetTrigger("MineAttack");
        }
    }


    // ��Ȯ�� �Ϸ���� �ʰ� �߰� �ߴ܉������� ȣ��ȴ�.
    void StopMining()
    {
        playerController.inputLocked = false;
        playerRigidBody.isKinematic = false;
        playerRigidBody.velocity = Vector2.zero;
        playerAnimator.SetBool("Mine", false);
    }

    // ��Ȯ�� �Ϸ�������� ȣ��ȴ�
    void EndMining()
    {

    }
}
