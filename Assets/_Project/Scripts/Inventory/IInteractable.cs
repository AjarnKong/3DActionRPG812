using UnityEngine;
/// <summary>
/// Contract for anything the player can interact with by pressing a key.
/// Item pickups, shop keepers and save points all implement this.
/// </summary>
public interface IInteractable
{
    // Text shown on screen while this object is the closest target
    string InteractionPrompt { get; }
    // Called once when the player presses the interact key
    void Interact(GameObject interactor);
}