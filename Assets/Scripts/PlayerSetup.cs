using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Necessary Component
[RequireComponent(typeof(CircleCollider2D))] // Necessary Component
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerAim))]

public class PlayerSetup : MonoBehaviour
{
    void Reset()
    {
        // As soon as you add this script, set the tag of that game object to "Player"
        gameObject.tag = "Player";
        Debug.Log("Player Tag is Ready");
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
