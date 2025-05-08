using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
// rigidbody 못찾으면 자동으로 만듦

public class movingman : MonoBehaviour
{
    public float walkAccel = 10f;
    public float runAccel = 20f;
    // 걷고 뛸 때 얻는 가속도 수치
    public float maxWalkSpeed = 3f;
    public float maxRunSpeed = 6f;
    // 이속 상한선
    public Transform cameraTransform;
    // 시점이 움직이는 대로 이동 방향 조정

    private Rigidbody rb;
    // 플레이어한테 부여된 실제 rigidbody를 가져와서 rb 변수에 넣을 거임
    private Vector3 inputDir;
    // 어디로 이동할지 좌표를 담은 벡터
    private Animator anim;
    // 서있고 걷고 뛰는 애니메이션 주입용도
    private Vector3 collisionNormal = Vector3.zero;
    // 벽에 부딪히면 '바로 멈추는' 게 아니라 '미끄러지듯 자연스럽게' 움직이게 할 수 있도록

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // rb에 값넣는부분
        anim = GetComponentInChildren<Animator>();
        // 애니메이션 할당
        // rigidbody는 player에 붙어있는데 animator은 player 하위 x-bot에 있으므로 InChildren
    }

    void Update()
    // Update()로 매 프레임마다 키보드 '입력' 을 받음
    // 단순한 타이머나 키 입력
    {
        float v = Input.GetAxisRaw("Vertical");
        // W,S로 전후 이동
        float h = Input.GetAxisRaw("Horizontal");
        // A,D로 좌우 이동

        Vector3 camForward = Camera.main.transform.forward;
        // 현재 카메라의 정면 방향
        Vector3 camRight = Camera.main.transform.right;
        // 현재 카메라의 오른쪽 방향

        camForward.y = 0f;
        camRight.y = 0f;
        // 위/아래 방향 제거 (어차피 점프 안함)

        camForward.Normalize();
        camRight.Normalize();
        // 방향 벡터 정규화 (길이 1로 맞춤)

        inputDir = (camForward * v + camRight * h).normalized;
        /* 카메라의 방향을 기준으로 입력받은 방향 계산 및 정규화
         * inputDir = new Vector3(h, 0f, v).normalized
         * 위의 방식대로 한다면 그냥 고정 방향이라 카메라 회전을 반영하지 못함 */

        float currentSpeed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
        // 마지막으로 현재 속도 계산 - 애니메이션 상태 전환용
        // y축 속도가 0인 이유 : 점프할일 없으니 고정

        anim.SetFloat("Speed", currentSpeed);
        // Speed 파라미터 업데이트
    }

    void FixedUpdate()
    // 이쪽은 Update()와는 다르게 프레임 상관없이 일관된 작동
    // 물리 효과 (Rigidbody) 가 적용된 오브젝트 조정용
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        // 뛰고 있나? = 쉬프트(L) 눌러진 상태인가?
        float maxSpeed = isRunning ? maxRunSpeed : maxWalkSpeed;
        // 현 상태의 최고 속도? (isrunning 참이면 runspeed 아니면 walkspeed)
        float accel = isRunning ? runAccel : walkAccel;
        // 같은 조건으로 따지고 가속도 부여

        Vector3 worldDir = transform.TransformDirection(inputDir);
        //아래는 추가분량
        if (collisionNormal != Vector3.zero)
        {
            worldDir = Vector3.ProjectOnPlane(worldDir, collisionNormal).normalized;
        }
        // 벽에 닿았으면 이동 방향을 벽에 대해 보정함.
        Vector3 horizVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        // worldDir = inputDir를 플레이어의 방향으로 변환
        // horizVel = 수평 속도만 남기기

        if (horizVel.magnitude < maxSpeed)
            rb.AddForce(worldDir * accel, ForceMode.Acceleration);
        // horizVel 이동량 총합(현 속도)가 최고속도보다 낮으면 가속

        if (horizVel.magnitude > maxSpeed)
        {
            Vector3 clamped = horizVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(clamped.x, rb.linearVelocity.y, clamped.z);
        }
        // 현 속도가 최고속도보다 높으면 속도 제한
        // clamped에 최고속도를 반영하고 방향을 그대로 유지함
        // velocity가 최고속도를 넘기지 못하게, clamped로 한번 각공된 속도값을 덮어씀
    }
    // 추가분량
    void OnCollisionStay(Collision collision)
    // 캐릭터가 벽, 오브텍트에 붙어있는동안 지속적으로 호출됨 - 벽에 닿았다는 걸 프레임마다 감지 / 법선 기억
    {
        Vector3 averageNormal = Vector3.zero;
        foreach (ContactPoint contact in collision.contacts)
        {
            averageNormal += contact.normal;
        }
        // 역할 : 각 접촉 지점들을 가져와서, 해당 지점들의 법선벡터들을 싹 다 합쳐서 평균치를 냄
        averageNormal.Normalize();// 방향벡터로 일반화

        collisionNormal = averageNormal;    // 이걸 참조해서 벽에 부딪혔을때 미끄러지라고
    }

    void OnCollisionExit(Collision collision)
    // 캐릭터가 벽에서 '떨어질 때' 호출 - 법선벡터 넣을 피요 없으니 벡터 초기화를 위해서
    {
        collisionNormal = Vector3.zero; // Zero 벡터 삽입
    }
}


/*
 * 현 과제)
 * 1. 벽에 부딪히면 캐릭터가 아예 멈춰버림 - 자연스럽게 미끄러지게 - OnCollisionStay 로 해결
 * 2. 1번 기능을 만족하면서 캐릭터가 벽과 경사를 구별할 수 있게 - 캐릭터 y축을 그냥 고정시키기?
 */