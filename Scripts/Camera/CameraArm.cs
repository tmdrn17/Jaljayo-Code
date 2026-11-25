using UnityEngine;

// 플레이어를 따라가며 마우스로 회전하는 카메라 팔. 벽에 가려지면 카메라를 당긴다.
public class CameraArm : MonoBehaviour
{
    [Header("따라가기")]
    [SerializeField] private Transform followTarget;
    [SerializeField] private float followSpeed = 10f;

    [Header("회전")]
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float clampAngle = 70f;

    [Header("카메라 거리")]
    [SerializeField] private Transform realCam;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float smoothness = 10f;

    private Vector3 dirNormalized;
    private Vector3 finalDir;
    private float finalDistance;
    private float rotX;
    private float rotY;

    void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNormalized = realCam.localPosition.normalized;
        finalDistance = realCam.localPosition.magnitude;
    }

    void Update()
    {
        if (GameManager.Instance.IsMoveLocked) return;

        rotX += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        transform.rotation = Quaternion.Euler(-rotX, rotY, 0);
    }

    void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            followTarget.position, followSpeed * Time.deltaTime);

        finalDir = transform.TransformPoint(dirNormalized * maxDistance);
        AvoidWall();
    }

    // 카메라와 플레이어 사이에 벽이 있으면 벽 앞까지 카메라를 당긴다
    private void AvoidWall()
    {
        if (Physics.Linecast(transform.position, finalDir, out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag(Tags.Wall))
            {
                finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            }
        }
        else
        {
            finalDistance = maxDistance;
        }

        realCam.localPosition = Vector3.Lerp(realCam.localPosition,
            dirNormalized * finalDistance, Time.deltaTime * smoothness);
    }
}
