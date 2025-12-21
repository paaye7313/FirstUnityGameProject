using UnityEngine;

public class GoalUI : MonoBehaviour
{
    public static GoalUI Instance;
    public GameObject goalText;

    private void Awake()
    {
        Instance = this;
        goalText.SetActive(false);
    }

    public void ShowGoal()
    {
        goalText.SetActive(true);
    }
}
