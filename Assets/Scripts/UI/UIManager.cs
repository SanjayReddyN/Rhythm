// filepath: /rhythm-game/rhythm-game/Assets/Scripts/UI/UIManager.cs
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void ShowUI(GameObject uiElement)
    {
        uiElement.SetActive(true);
    }

    public void HideUI(GameObject uiElement)
    {
        uiElement.SetActive(false);
    }
}