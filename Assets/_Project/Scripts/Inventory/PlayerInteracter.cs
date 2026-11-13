using UnityEngine;
/// <summary>
/// Finds the nearest interactable object and forwards the interact key to it.
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private InteractionPromptUI promptUI;
    [SerializeField] private float interactRadius = 2f;
    [SerializeField] private LayerMask interactableLayers;
    private readonly Collider[] _buffer = new Collider[8];
    private IInteractable _current;
    private void Update()
    {
        FindClosestInteractable();
        if (_current != null && input.InteractPressed)
        {
            _current.Interact(gameObject);
            _current = null;
        }
    }
    private void FindClosestInteractable()
    {
        int count = Physics.OverlapSphereNonAlloc(
        transform.position, interactRadius, _buffer, interactableLayers);

        IInteractable closest = null;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < count; i++)
        {
            if (!_buffer[i].TryGetComponent(out IInteractable candidate))
            {
                continue;
            }
            // Squared distance is enough for comparing, and avoids a square root
            float distance = (_buffer[i].transform.position
            - transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = candidate;
            }
        }
        _current = closest;
        promptUI.Show(closest?.InteractionPrompt);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}