using UnityEngine;
using DG.Tweening;

public class mainCamera : MonoBehaviour
{
    [SerializeField]
    private Camera mainCam;
    [SerializeField]
    private Transform trfTarget;

    [Header("Camera Move Settings")]
    [SerializeField]
    private float moveDuration = 1f; // 카메라 이동 속도

    [Header("Shake Settings")]
    [SerializeField]
    private float shakeStrength = 0.1f; // 흔들림 강도
    [SerializeField]
    private int shakeVibrato = 10; // 흔들림 진동 횟수
    [SerializeField]
    private float shakeDuration = 0.5f; // 흔들림 지속 시간

    [Header("Drag Settings")]
    [SerializeField]
    private float dragSensitivity = 0.5f; // 드래그 민감도

    private Vector3 lastMousePos;
    private bool isDragging = false;
    private Sequence activeSequence;
    private float dragThreshold = 5f; // 드래그 판정 거리

    private void Update()
    {
        HandleMouseInput();
    }

    /// <summary>
    /// 버튼 클릭 시 trfTarget을 중심으로 카메라 이동
    /// </summary>
    public void MoveToTarget()
    {
        if (trfTarget == null)
        {
            Debug.LogWarning("trfTarget이 설정되지 않았습니다.");
            return;
        }

        // 기존 애니메이션 취소
        if (activeSequence != null && activeSequence.IsActive())
        {
            activeSequence.Kill();
        }

        // 카메라를 trfTarget 위치로 이동 (오프셋 적용, Z축 유지)

        Vector3 targetPos = trfTarget.position;

        transform.DOMove(targetPos, moveDuration).SetEase(Ease.InOutQuad);
    }

    /// <summary>
    /// 카메라 흔들림 효과
    /// </summary>
    public void ShakeCamera()
    {
        if (mainCam == null)
        {
            Debug.LogWarning("mainCam이 설정되지 않았습니다.");
            return;
        }

        transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, randomness: 90f);
    }

    /// <summary>
    /// 마우스 입력 처리
    /// 1. 마우스 눌렀을 때: 이벤트 없음
    /// 2. 마우스 누른 상태에서 드래그: 화면 이동
    /// 3. 마우스 뗄 때: BattleMgr.HandleTileClick() 실행
    /// </summary>
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스를 눌렀을 때
            isDragging = false;
            lastMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            // 마우스를 누르고 있을 때
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 delta = lastMousePos - currentMousePos;

            // 드래그 거리가 임계값을 넘으면 드래그로 판정
            if (delta.magnitude > dragThreshold)
            {
                isDragging = true;
            }

            // 드래그 중이면 카메라 이동
            if (isDragging)
            {
                // 마우스 이동을 월드 스페이스로 변환
                Vector3 moveDirection = mainCam.ScreenToWorldPoint(new Vector3(delta.x, delta.y, 0))
                                        - mainCam.ScreenToWorldPoint(Vector3.zero);

                // 카메라 이동
                transform.position += moveDirection * dragSensitivity;
            }

            lastMousePos = currentMousePos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // 마우스를 뗄 때
            if (!isDragging)
            {
                // 드래그하지 않았으면 (클릭만 했으면) BattleMgr의 HandleTileClick 실행
                BattleMgr.Instance.HandleTileClick();
            }

            isDragging = false;
        }
    }

    /// <summary>
    /// 카메라 이동 속도 설정
    /// </summary>
    public void SetMoveDuration(float duration)
    {
        moveDuration = Mathf.Max(0.1f, duration); // 최소 0.1초
    }

    /// <summary>
    /// 흔들림 강도 설정
    /// </summary>
    public void SetShakeStrength(float strength)
    {
        shakeStrength = Mathf.Max(0, strength);
    }

    /// <summary>
    /// 드래그 민감도 설정
    /// </summary>
    public void SetDragSensitivity(float sensitivity)
    {
        dragSensitivity = Mathf.Max(0, sensitivity);
    }
}
