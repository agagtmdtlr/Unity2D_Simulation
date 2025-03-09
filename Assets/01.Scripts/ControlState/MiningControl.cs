using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MiningControl : PlayerControlState
{
    [SerializeField] GameObject tool;


    public enum MiningMode
    {
        Idle,
        Charing,
        OverCharging,
        Attack
    }

    public override Mode GetMode() { return Mode.Mining; }

    public abstract class State
    {
        MiningControl context;

        public State(MiningControl context)
        {
            this.context = context;
        }

        public abstract void Update();
        public abstract void Start();
        public abstract void End();
        public abstract bool NeedChagne(out MiningMode mode);
        public void Check()
        {
            if (NeedChagne(out MiningMode mode))
            {
                context.ChangeState(mode);
            }
        }
    }

    MiningMode currentMode;
    Dictionary<MiningMode, State> modes;
    SpriteRenderer toolRender;
    Light2D toolLight;

    float chargeGauge = 0;
    [SerializeField] float attackLimitation = 3f;
    [SerializeField] float chargeGaugeMax = 5f;
    [SerializeField] float attackSpeed = 1f;

    [SerializeField] float startPosXOffset;

    bool isPlaying = false;
    MineCollectable mine;

    public void ChangeState(MiningMode mode)
    {
        modes[mode].End();
        currentMode = mode;
        modes[currentMode].Start();
    }
    public override void NeedChagne()
    {
        if (control.InputEsc || !isPlaying || mine.HP <= 0f)
        {
            context.ChangeState(PlayerControlState.Mode.Groud);
        }
    }
    public override void Enter()
    {
        isPlaying = true;

        body.isKinematic = true;
        body.velocity = Vector2.zero;
        tool.SetActive(true);
        tool.transform.GetChild(0).TryGetComponent(out toolLight);
        tool.TryGetComponent(out toolRender);

        

        animator.SetBool("Mine", true);

        mine = context.interactor.Interacted.GetComponent<MineCollectable>();
        var pos = body.position;
        pos.x = mine.transform.position.x + startPosXOffset;
        body.position = pos;


        StopCoroutine("PlayMining_Co");
        StartCoroutine("PlayMining_Co");

    }
    public override void Exit()
    {
        StopCoroutine("PlayMining_Co");

        body.isKinematic = false;
        animator.SetBool("Mine", false);
        tool.SetActive(false);

        animator.SetBool("Mine", false);
        toolRender.color = Color.white;
        tool.SetActive(false);
        toolLight.intensity = 0;
    }

    IEnumerator OverCharging_Co()
    {
        animator.SetBool("MineAttack", false);
        animator.SetBool("OverCharge", true);
        chargeGauge = 0f;

        yield return new WaitForSeconds(2f);
        animator.SetBool("OverCharge", false);
    }

    IEnumerator Attack_Co()
    {
        bool mineSuccess = attackLimitation <= chargeGauge;
        float mineDamage = chargeGauge / chargeGaugeMax;
        animator.SetBool("MineSuccess", mineSuccess);
        animator.SetBool("MineAttack", false);

        while (chargeGauge >= 0f)
        {
            chargeGauge -= Time.deltaTime * attackSpeed;

            UpdateChargeAnimation();
            yield return null;
        }

        mine.TakeDamage(mineDamage);
        chargeGauge = 0f;
        animator.SetBool("MineSuccess", false);
    }


    IEnumerator PlayMining_Co()
    {
        while (isPlaying && mine.HP > 0f)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                chargeGauge = 0f;
                animator.SetBool("MineAttack", true);
            }

            bool isoverCharging = chargeGauge > chargeGaugeMax;
            if (isoverCharging)
            {
                yield return StartCoroutine(OverCharging_Co());
            }
            else
            {
                if (Input.GetKey(KeyCode.E))
                {
                    chargeGauge += Time.deltaTime;
                }
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                yield return StartCoroutine(Attack_Co());
            }

            UpdateChargeAnimation();
            yield return null;
        }

        yield break;
    }

    public override void UpdateState()
    {
    }

    void UpdateChargeAnimation()
    {
        Vector3 rotation = Vector3.zero;
        float ratio = EasingFunction.EaseOutCubic(0, 1, chargeGauge / chargeGaugeMax);
        rotation.z = Mathf.Lerp(0f, 120f, ratio);

        tool.transform.rotation = Quaternion.Euler(rotation);
        toolLight.intensity = Mathf.Lerp(0f, 15f, ratio);

        Color gaugeColor = Color.Lerp(Color.white, Color.red, ratio);
        toolRender.color = gaugeColor;
        animator.SetFloat("Charge", ratio);
    }
}
