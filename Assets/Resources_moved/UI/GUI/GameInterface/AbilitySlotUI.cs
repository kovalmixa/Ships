using UnityEngine;
using UnityEngine.UIElements;

public class AbilitySlotUI : MonoBehaviour
{
    private VisualElement slotButton;
    private VisualElement dropdownMenu;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Находим нужные элементы по именам или классам
        slotButton = root.Q<VisualElement>("AbilitySlotButton");
        dropdownMenu = root.Q<VisualElement>("AbilityDropdownMenu");

        // Подписываемся на событие клика
        slotButton.RegisterCallback<ClickEvent>(OnSlotClicked);
    }

    private void OnSlotClicked(ClickEvent evt)
    {
        // Переключаем класс видимости (.active)
        dropdownMenu.ToggleInClassList("active");
    }
}