using UnityEngine;

public class mouseview:MonoBehaviour
{
    public Transform playerBody;
    public float sensitivity = 100f;
    public float xClamp = 90f;

    private float xRotation = 0f;

    void Start()
    {
        // ȭ�� ������ ���콺 Ŀ���� Ż�� ���ϰ� ���� + ���콺 Ŀ�� ����
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // ���� ���콺�� x,y������ �󸶳� ���������� ��ġ�� �޾ƿ�
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // ���콺�� �����ӿ� ���� ī�޶� ȸ��
        xRotation -= mouseY; // ���콺�� ���� �ø��� ī�޶� ���� ���ư��� ������ �Ʒ��� ������ - �̸� �����ϱ� ���� �ݴ�� �����̵��� ����
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp); // ���� �ѵ� ���� ���� ���ư��� ���ϰ� -90��~90�� ���̷� ����

        // ���콺�� �̵��Կ� ���� �ڿ������� �÷��̾ ȸ�����Ѿ� ��
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // x�� ī�޶� ȸ�� ����
        playerBody.Rotate(Vector3.up * mouseX);  // ���������� �÷��̾ ���콺 �̵��� ���� �¿�� ȸ����
        
        // ī�޶��� y�� ȸ���� �÷��̾��� y�� ȸ���� ������ ����ȭ
        transform.rotation = Quaternion.Euler(xRotation, playerBody.eulerAngles.y, 0f);
        // ��ǻ� �¿� ȸ���� �÷��̾ ���� ������ '������' ���󰡵��� ��
    }
}
