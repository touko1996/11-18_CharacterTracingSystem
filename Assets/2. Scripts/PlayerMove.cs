using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("움직임 세팅")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float blendSmoothSpeed = 10f;

    private CharacterController controller;
    private Animator anim;

    private Vector3 moveDir;
    private float currentBlend = 0f;

    // 피격 연출용 변수
    private SkinnedMeshRenderer meshRenderer;
    private Color originalColor;

    // 효과음 추가
    [Header("피격 효과음")]
    [SerializeField] private AudioClip hitSFX;
    private AudioSource audioSource;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // 스킨 메시 탐색
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (meshRenderer != null)
            originalColor = meshRenderer.material.color;
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(h, 0f, v).normalized;

        moveDir = input * moveSpeed;
        controller.Move(moveDir * Time.deltaTime);

        if (input.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(input);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotateSpeed * Time.deltaTime
            );
        }

        float targetBlend = input.magnitude;
        currentBlend = Mathf.Lerp(currentBlend, targetBlend, blendSmoothSpeed * Time.deltaTime);

        anim.SetFloat("Blend", currentBlend);
    }

    // 피격 처리
    public void TakeHit()
    {
        if (meshRenderer != null)
            StartCoroutine(CoHitFlash());

        // 피격 효과음 재생
        if (hitSFX != null)
            audioSource.PlayOneShot(hitSFX);
    }

    private IEnumerator CoHitFlash()
    {
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        meshRenderer.material.color = originalColor;
    }
}
