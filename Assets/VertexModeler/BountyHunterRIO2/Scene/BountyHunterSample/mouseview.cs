using UnityEngine;

public class mouseview:MonoBehaviour
{
    public Transform playerBody;
    public float sensitivity = 100f;
    public float xClamp = 90f;

    private float xRotation = 0f;

    void Start()
    {
        // 화면 밖으로 마우스 커서가 탈출 못하게 막음 + 마우스 커서 숨김
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 각각 마우스를 x,y축으로 얼마나 움직였는지 수치를 받아옴
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // 마우스의 움직임에 따라 카메라 회전
        xRotation -= mouseY; // 마우스를 위로 올리면 카메라가 위로 돌아가며 시점은 아래로 내려감 - 이를 방지하기 위해 반대로 움직이도록 적용
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp); // 고개가 한도 끝도 없이 돌아가지 못하게 -90도~90도 사이로 제한

        // 마우스가 이동함에 따라 자연스럽게 플레이어도 회전시켜야 함
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // x축 카메라 회전 적용
        playerBody.Rotate(Vector3.up * mouseX);  // 마찬가지로 플레이어도 마우스 이동에 따라 좌우로 회전함
        
        // 카메라의 y축 회전을 플레이어의 y축 회전과 강제로 동기화
        transform.rotation = Quaternion.Euler(xRotation, playerBody.eulerAngles.y, 0f);
        // 사실상 좌우 회전은 플레이어가 보는 방향을 '무조건' 따라가도록 함
    }
}
