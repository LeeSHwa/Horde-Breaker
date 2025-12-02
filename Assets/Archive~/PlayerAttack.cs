using UnityEngine;


public class PlayerAttack : MonoBehaviour
{
    // A variable that can hold any type of Weapon, not just a specific one like a Gun.
    public Weapon currentWeapon;

    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }


    void Update()
    {
        // If a weapon is currently equipped, simply send the signal to attack.
        if (currentWeapon != null)
        {
            currentWeapon.TryAttack(playerController.AimDirection);
        }
    }

}