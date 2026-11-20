using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Builds the slot grid and keeps it in sync with the inventory data.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventorySystem inventory;
    [SerializeField] private EquipmentManager equipment;
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private InventorySlotUI slotPrefab;
    [Header("Details")]
    [SerializeField] private TextMeshProUGUI detailNameText;
    [SerializeField] private TextMeshProUGUI detailDescriptionText;
    private readonly List<InventorySlotUI> _slotViews = new List<InventorySlotUI>();
    private bool _isOpen;
    private void Start()
    {
        BuildSlots();
        panelRoot.SetActive(false);
    }
    private void OnEnable()
    {
        inventory.OnInventoryChanged += Refresh;
    }
    private void OnDisable()
    {
        inventory.OnInventoryChanged -= Refresh;
    }
    private void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            Toggle();
        }
    }

    private void BuildSlots()
    {
        // Views are created once, then reused for the whole session
        foreach (InventorySlot slot in inventory.Slots)
        {
            InventorySlotUI view = Instantiate(slotPrefab, slotContainer);
            view.Bind(slot, this);
            _slotViews.Add(view);
        }
    }
    private void Refresh()
    {
        for (int i = 0; i < _slotViews.Count; i++)
        {
            _slotViews[i].Bind(inventory.Slots[i], this);
        }
    }
    public void Toggle()
    {
        _isOpen = !_isOpen;
        panelRoot.SetActive(_isOpen);
        Refresh();
        // Free the cursor and stop the character from moving while browsing
        Cursor.lockState = _isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isOpen;
    }
    public void HandleSlotClicked(InventorySlot slot)
    {
        detailNameText.text = slot.item.displayName;
        detailDescriptionText.text = slot.item.description;
        if (slot.item is WeaponData weapon)
        {
            equipment.Equip(weapon);
            return;
        }
        if (slot.item.itemType == ItemType.Consumable)
        {
            // Consumables are handled in the exercise at the end of this chapter
            Debug.Log($"Used {slot.item.displayName}");
        }
    }
}