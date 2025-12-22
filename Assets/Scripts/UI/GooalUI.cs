using UnityEngine;
using TMPro;
using System.Collections;

public class GoalUI : MonoBehaviour
{
    public static GoalUI Instance;

    public GameObject goalText;   // "골인!" 텍스트 오브젝트
    public float showDuration = 2f;  //표시 시간

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        goalText.SetActive(false); // 시작 시 숨김
    }

    public void ShowGoal()  //플레이어가 호출
    {
        StopAllCoroutines();      // 중복 호출 방지
        StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        goalText.SetActive(true);  //Text를 표시하고
        yield return new WaitForSeconds(showDuration);  //표시 시간을 기다림
        goalText.SetActive(false);  //Text를 숨김
    }
}
