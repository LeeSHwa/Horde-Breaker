// RicochetDataSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "RicochetData", menuName = "Stats/Weapon Data/RicochetData")]
public class RicochetDataSO : WeaponDataSO
{
    [Header("Ricochet Stats")]

    public GameObject projectilePrefab; 
    public float projectileSpeed = 10f; 
    public int maxBounces = 3;          
    public float bounceRange = 7f;      
    public float lifetime = 5f;         
}