using TMPro;
using UnityEngine;
/// <summary>
/// Shows or hides the on screen interaction hint.
/// </summary>
public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    private void Awake()
    {
        Show(null);
    }
    // Passing null or an empty string hides the hint
    public void Show(string message)
    {
        bool hasMessage = !string.IsNullOrEmpty(message);
        promptText.gameObject.SetActive(hasMessage);
        if (hasMessage)
        {
            promptText.text = $"[E] {message}";
        }
    }
}