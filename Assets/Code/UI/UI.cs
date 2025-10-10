using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreateSlider : MonoBehaviour
{
    private Slider m_cohesion_slider;
    private Text m_cohesion_slider_text;

    private Slider m_alignment_slider;
    private Text m_alignment_slider_text;

    private Slider m_avoidance_slider;
    private Text m_avoidance_slider_text;

    public void SetSlidersActive(bool active)
    {
        if (m_cohesion_slider != null) m_cohesion_slider.gameObject.SetActive(active);
        if (m_cohesion_slider_text != null) m_cohesion_slider_text.gameObject.SetActive(active);

        if (m_alignment_slider != null) m_alignment_slider.gameObject.SetActive(active);
        if (m_alignment_slider_text != null) m_alignment_slider_text.gameObject.SetActive(active);

        if (m_avoidance_slider != null) m_avoidance_slider.gameObject.SetActive(active);
        if (m_avoidance_slider_text != null) m_avoidance_slider_text.gameObject.SetActive(active);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // H for hide
        {
            SetSlidersActive(false);
        }
        if (Input.GetKeyDown(KeyCode.S)) // S for show
        {
            SetSlidersActive(true);
        }
    }

    void Start()
    {

        // Ensure EventSystem exists
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        // Create Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800, 600);

        canvasGO.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = canvasGO.GetComponent<RectTransform>();
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;
        canvasRect.offsetMin = canvasRect.offsetMax = Vector2.zero;

        // Create sliders in top-left
        CreateSliderUI(canvasGO.transform, "Cohesion", new Vector2(10, -10), out m_cohesion_slider_text, out m_cohesion_slider);
        CreateSliderUI(canvasGO.transform, "Alignment", new Vector2(10, -40), out m_alignment_slider_text, out m_alignment_slider);
        CreateSliderUI(canvasGO.transform, "Avoidance", new Vector2(10, -70), out m_avoidance_slider_text, out m_avoidance_slider);

        // Slider value events
        m_cohesion_slider.onValueChanged.AddListener((v) => m_cohesion_slider_text.text = $"Cohesion: {v:F0}");
        m_alignment_slider.onValueChanged.AddListener((v) => m_alignment_slider_text.text = $"Alignment: {v:F0}");
        m_avoidance_slider.onValueChanged.AddListener((v) => m_avoidance_slider_text.text = $"Avoidance: {v:F0}");

        SetSlidersActive(false);
    }

    private void CreateSliderUI(Transform parent, string name, Vector2 anchoredPos, out Text text_ui, out Slider slider_ui)
    {
        // Slider parent
        GameObject sliderGO = new GameObject(name);
        sliderGO.transform.SetParent(parent, false);

        RectTransform rect = sliderGO.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(150, 15); // smaller slider
        rect.anchorMin = new Vector2(0, 1); // top-left
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1); // pivot top-left
        rect.anchoredPosition = anchoredPos;

        // Slider
        slider_ui = sliderGO.AddComponent<Slider>();
        slider_ui.direction = Slider.Direction.LeftToRight;

        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderGO.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = Color.gray;
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = bgRect.offsetMax = Vector2.zero;
        slider_ui.targetGraphic = bgImg;

        // Fill
        GameObject fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(sliderGO.transform, false);
        Image fillImg = fillGO.AddComponent<Image>();
        fillImg.color = Color.green;
        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = fillRect.offsetMax = Vector2.zero;
        slider_ui.fillRect = fillRect;

        // Handle
        GameObject handleGO = new GameObject("Handle");
        handleGO.transform.SetParent(sliderGO.transform, false);
        Image handleImg = handleGO.AddComponent<Image>();
        handleImg.color = Color.white;

        RectTransform handleRect = handleGO.GetComponent<RectTransform>();
        handleRect.anchorMin = handleRect.anchorMax = new Vector2(0.5f, 0.5f);
        handleRect.anchoredPosition = Vector2.zero;

        // Handle size same height as slider, smaller width
        handleRect.sizeDelta = new Vector2(10, 5);

        slider_ui.handleRect = handleRect;

        slider_ui.minValue = 0;
        slider_ui.maxValue = 100;
        slider_ui.value = 50;

        // Text
        GameObject textGO = new GameObject("ValueText");
        textGO.transform.SetParent(sliderGO.transform, false);
        text_ui = textGO.AddComponent<Text>();
        text_ui.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text_ui.fontSize = 14; // smaller text
        text_ui.color = Color.black;
        text_ui.alignment = TextAnchor.MiddleRight;
        text_ui.text = name + ": 50";

        RectTransform textRect = text_ui.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(100, 20);
        textRect.anchorMin = textRect.anchorMax = new Vector2(1, 0.5f);
        textRect.pivot = new Vector2(0, 0.5f);
        textRect.anchoredPosition = new Vector2(5, 0);
    }
}
