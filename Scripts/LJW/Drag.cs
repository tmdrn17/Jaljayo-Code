using UnityEngine;

// 마우스로 끌어서 정답 위치(DropArea)에 놓는 오브젝트.
// 정답 영역의 태그(destinationTag)는 인스펙터에서 정한다. (예전 Drag2는 "DropArea2"를 썼다)
[RequireComponent(typeof(MeshRenderer), typeof(Collider))]
public class Drag : MonoBehaviour
{
    [SerializeField] private GameObject correctPos;
    [SerializeField] private GameObject originalPos;
    [SerializeField] private string destinationTag = "DropArea";

    private bool isDrag = true;
    private Vector3 mouseOffset;
    private MeshRenderer rend;
    private Collider col;

    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
    }

    #region 마우스 입력
    private void OnMouseDown()
    {
        if (isDrag == false) return;

        mouseOffset = Input.mousePosition - GetScreenPos();
        rend.material.color = Color.grey;
        col.enabled = false;
    }

    private void OnMouseDrag()
    {
        if (isDrag == false) return;

        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mouseOffset);
    }

    private void OnMouseUp()
    {
        rend.material.color = Color.white;
        col.enabled = true;
    }
    #endregion

    #region 놓기 판정
    private void OnTriggerEnter(Collider other)
    {
        // 1. 벽에 닿으면 처음 위치로 돌려보낸다
        if (other.CompareTag(Tags.DragWall))
        {
            transform.position = originalPos.transform.position;
        }

        // 2. 정답 영역에 닿으면 제자리에 고정하고 점수를 올린다
        if (other.CompareTag(destinationTag))
        {
            transform.position = correctPos.transform.position;
            isDrag = false;

            Stage2UI ui = GameObject.Find(ObjectNames.UIManager).GetComponent<Stage2UI>();
            ui.AddCorrect();
        }
    }
    #endregion

    private Vector3 GetScreenPos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }
}
