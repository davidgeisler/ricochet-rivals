using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultySelection : MonoBehaviour
{
    public enum DifficultyLevel { Easy, Medium, Hard }

    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;
    public RectTransform header;

    public GameObject difficultyCanvas;
    public GameObject mainGameCanvas;

    public float headerWiggleAmount = 5f;
    public float headerWiggleSpeed = 5f;

    private Vector3 headerOriginalPos;
    private homeLoadScences mainLoad = new homeLoadScences();

    void Start()
    {
        headerOriginalPos = header.localPosition;

        AssignButton(easyButton, DifficultyLevel.Easy);
        AssignButton(mediumButton, DifficultyLevel.Medium);
        AssignButton(hardButton, DifficultyLevel.Hard);
    }

    void AssignButton(Button button, DifficultyLevel difficulty)
    {
        button.onClick.AddListener(() => LoadMainGame(difficulty));

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        // OnPointerEnter - scale up
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) =>
        {
            LeanTween.scale(button.gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setEaseOutSine();
        });

        // OnPointerExit - scale down
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) =>
        {
            LeanTween.scale(button.gameObject, Vector3.one, 0.1f).setEaseInSine();
        });

        trigger.triggers.Add(entryEnter);
        trigger.triggers.Add(entryExit);
    }

    void LoadMainGame(DifficultyLevel difficulty)
    {
        // Store as string (or int) in PlayerPrefs
        PlayerPrefs.SetString("SelectedDifficulty", difficulty.ToString());
        mainLoad.LoadSceneByName("RicochetRival_Main");
    }

    void Update()
    {
        float offsetX = Mathf.Sin(Time.time * headerWiggleSpeed) * headerWiggleAmount;
        float offsetY = Mathf.Cos(Time.time * headerWiggleSpeed * 1.2f) * headerWiggleAmount;
        header.localPosition = headerOriginalPos + new Vector3(Mathf.Round(offsetX), Mathf.Round(offsetY), 0f);
    }
}
