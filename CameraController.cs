using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera1;
    public Camera mainCamera2;
    public GameObject[] inputFields;
    public GameObject cube1;
    public GameObject cube2;

    void Start()
    {
        // 초기 상태 설정
        ShowFirstSetOfInputFields();
    }

    void Update()
    {
        // 키보드 입력 처리
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowFirstSetOfInputFields();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowSecondSetOfInputFields();
        }
    }

    void ShowFirstSetOfInputFields()
    {
        mainCamera1.gameObject.SetActive(true);
        mainCamera2.gameObject.SetActive(false);
        cube1.SetActive(true);
        cube2.SetActive(true);

        for (int i = 0; i < inputFields.Length; i++)
        {
            if (i < 30)
            {
                inputFields[i].SetActive(true);
            }
            else
            {
                inputFields[i].SetActive(false);
            }
        }
    }

    void ShowSecondSetOfInputFields()
    {
        mainCamera1.gameObject.SetActive(false);
        mainCamera2.gameObject.SetActive(true);
        cube1.SetActive(false);
        cube2.SetActive(false);

        for (int i = 0; i < inputFields.Length; i++)
        {
            if (i >= 30)
            {
                inputFields[i].SetActive(true);
            }
            else
            {
                inputFields[i].SetActive(false);
            }
        }
    }
}
