﻿using UnityEngine;

// 회전·위아래 떠다님·크기 변화 연출. 켠 항목만 동작한다.
// 필드 이름은 프리팹 오버라이드(propertyPath)에 쓰이므로 바꾸지 않는다.
public class AnimationScript : MonoBehaviour
{
    [SerializeField] private bool isAnimated = false;

    [Header("회전")]
    [SerializeField] private bool isRotating = false;
    [SerializeField] private Vector3 rotationAngle;
    [SerializeField] private float rotationSpeed;

    [Header("떠다님")]
    [SerializeField] private bool isFloating = false;
    [SerializeField] private float floatSpeed;
    [SerializeField] private float floatRate;
    private bool goingUp = true;
    private float floatTimer;

    [Header("크기")]
    [SerializeField] private bool isScaling = false;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private Vector3 endScale;
    [SerializeField] private float scaleSpeed;
    [SerializeField] private float scaleRate;
    private bool scalingUp = true;
    private float scaleTimer;

    void Update()
    {
        if (isAnimated == false) return;

        // 1. 회전
        if (isRotating)
        {
            transform.Rotate(rotationAngle * rotationSpeed * Time.deltaTime);
        }

        // 2. 떠다님
        if (isFloating)
        {
            UpdateFloat();
        }

        // 3. 크기 변화
        if (isScaling)
        {
            UpdateScale();
        }
    }

    void UpdateFloat()
    {
        floatTimer += Time.deltaTime;
        Vector3 moveDir = new Vector3(0.0f, 0.0f, floatSpeed);
        transform.Translate(moveDir);

        if (floatTimer < floatRate) return;

        // TODO(리팩토링): 이동 방향 전환 로직이 한 번만 부호를 바꿔서 위·아래가 번갈아 움직이지 않는다 (위, 아래, 아래, 위, 위 …).
        //   Translate도 deltaTime을 곱하지 않아 프레임 속도에 따라 빨라진다. 동작 유지를 위해 그대로 둔다.
        if (goingUp)
        {
            goingUp = false;
            floatSpeed = -floatSpeed;
        }
        else
        {
            goingUp = true;
        }
        floatTimer = 0;
    }

    void UpdateScale()
    {
        scaleTimer += Time.deltaTime;

        Vector3 target = scalingUp ? endScale : startScale;
        transform.localScale = Vector3.Lerp(transform.localScale, target, scaleSpeed * Time.deltaTime);

        if (scaleTimer >= scaleRate)
        {
            scalingUp = (scalingUp == false);
            scaleTimer = 0;
        }
    }
}
