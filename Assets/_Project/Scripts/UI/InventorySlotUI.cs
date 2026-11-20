using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Displays the contents of a single inventory slot.
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;
    private InventorySlot _slot;
    private InventoryUI _owner;
    public void Bind(InventorySlot slot, InventoryUI owner)
    {
        _slot = slot;
        _owner = owner;
        Redraw();
    }
    private void Redraw()
    {
        bool hasItem = _slot != null && !_slot.IsEmpty;
        iconImage.enabled = hasItem;
        amountText.gameObject.SetActive(hasItem && _slot.amount > 1);

        if (!hasItem)
        {
            return;
        }
        iconImage.sprite = _slot.item.icon;
        amountText.text = _slot.amount.ToString();
    }
    // Hooked to the Button component in the Inspector
    public void OnClicked()
    {
        if (_slot == null || _slot.IsEmpty)
        {
            return;
        }
        _owner.HandleSlotClicked(_slot);
    }
}