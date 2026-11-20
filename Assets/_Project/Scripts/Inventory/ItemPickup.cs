using UnityEngine;
/// <summary>
/// An item lying on the ground that the player can pick up.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour, IInteractable
{
    [Header("Contents")]
    [SerializeField] private ItemData item;
    [SerializeField] private int amount = 1;
    [Header("Idle Motion")]
    [SerializeField] private float spinSpeed = 60f;
    [SerializeField] private float bobHeight = 0.15f;
    [SerializeField] private float bobSpeed = 2f;
    private Vector3 _startPosition;
    public string InteractionPrompt =>
    item == null ? string.Empty : $"Pick up {item.displayName} x{amount}";
    private void Start()
    {
        _startPosition = transform.position;
    }
    private void Update()
    {
        // Spinning and bobbing makes dropped items easier to notice
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = _startPosition + Vector3.up * offset;
    }
    public void Interact(GameObject interactor)
    {
        
        if (!interactor.TryGetComponent(out InventorySystem inventory))
        {
            return;
        }
        if (!inventory.AddItem(item, amount))
        {
            Debug.Log("Inventory is full");
            return;
        }
        Destroy(gameObject);
        
    }
    // Lets the loot system configure a freshly spawned pickup
    public void Setup(ItemData newItem, int newAmount)
    {
        item = newItem;
        amount = newAmount;
    }
}