using UnityEngine;
using TMPro;
using System.Collections;

public class GoalUI : MonoBehaviour
{
    public static GoalUI Instance;

    public GameObject goalText;   // "골인!" 텍스트 오브젝트
    public float showDuration = 2f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        goalText.SetActive(false); // 시작 시 숨김
    }

    public void ShowGoal()
    {
        StopAllCoroutines();      // 중복 호출 방지
        StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        goalText.SetActive(true);
        yield return new WaitForSeconds(showDuration);
        goalText.SetActive(false);
    }
}
