//
// using System.Collections.Generic;
// using UnityEngine;
//
// using TCM.Interfaces;
// using TCM.Rigidbodies;
//
//
// namespace KOS.Weapons
// {
//     [SelectionBase]
//     public class Firearm : MonoBehaviour, IUnlockable
//     {
//
//         [SerializeField] private ProjectileFactory projectileFactory = default;
//         [SerializeField] private ParticleFactory particleFactory = default;
//         [SerializeField] private WeaponData weapon = default;
//
//         public WeaponData Data => weapon;
//         
//         [SerializeField]
//         private Transform aimPoint;
//         private ProjectileOrigin origin;
//
//         private PlayerStats playerStats; // for getting damage boost
//         private Rigidbody player;
//         
//         private bool firing, fired, canFire;
//         private float fireInterval, damageMultiplier;
//
//         //> INITIALIZE
//         private void Awake()
//         {
//             // set some stuff
//             fired = false;
//             firing = false;
//             canFire = true;
//             fireInterval = 60f;
//             Unlocked = weapon.unlocked;
//
//             // get components
//             player = GetComponentInParent<Rigidbody>();
//             origin = GetComponentInChildren<ProjectileOrigin>();
//             playerStats = GetComponentInParent<PlayerStats>();
//
//             EventManager.Active.onPlacingTower += NoFire;
//             EventManager.Active.onStoppedPlacingTower += CanFire;
//         }
//
//         private void NoFire(TowerType tower) => canFire = false;
//         private void CanFire() => canFire = true;
//
//         public bool Unlocked { get; private set; }
//         public int UnlockCost => weapon.unlockCost;
//         
//         //> CHECK IF WEAPON CAN BE UNLOCKED
//         public bool IsUnlockable() => Bank.Connect.HasBalance(weapon.unlockCost);
//
//         //> ATTEMPT TO UNLOCK THIS WEAPON
//         public bool Unlock()
//         {
//             if (!Bank.Connect.HasBalance(weapon.unlockCost)) return false;
//
//             Bank.Connect.Charge(weapon.unlockCost);
//             this.Unlocked = true;
//             return true;
//         }
//
//         //> GET INPUT AND CHECK IF CAN FIRE
//         private void Update()
//         {
//             // get input
//             firing = Input.GetMouseButton(0);
//             if (Input.GetMouseButtonUp(0)) fired = false; // clear if released trigger
//
//             if ((fireInterval += weapon.fireRate.value * Time.deltaTime) >= 60f)
//             {
//                 // handy dandy automatic/semiautomatic checking
//                 if ((firing && !fired && !weapon.automatic) || (firing && weapon.automatic))
//                 {
//                     fired = true;
//                     fireInterval = 0;
//                     Fire((int)weapon.projectileAmount.value);
//                 }
//             }
//
//             damageMultiplier = playerStats.GetDamageMultiplier(); // REFACTOR
//         }
//
//         //> ADD SOME SPREAD TO THE FIRE DIRECTION
//         private Vector3 DirectionWithSpread()
//         {
//             float spreadX = Random.Range(weapon.horizontalSpread.x, weapon.horizontalSpread.y);
//             float spreadY = Random.Range(weapon.verticalSpread.x, weapon.verticalSpread.y);
//
//             Vector3 spread = transform.forward;
//             spread.x += spreadX;
//             spread.y += spreadY;
//
//             return spread;
//         }
//         
//         //> FIRE THE WEAPON
//         private void Fire(int amount)
//         {
//             if (Cursor.lockState != CursorLockMode.Locked || !canFire) return;
//
//             IEnumerable<Projectile> projectiles = projectileFactory.Get(weapon.projectileType, (int)weapon.projectileAmount.value);
//             foreach (var projectile in projectiles) // fire every projectile
//             {
//                 projectile.position = origin.position;
//                 Vector3 fireDirection = DirectionWithSpread();
//                 projectile.Launch(origin.position, fireDirection, weapon.projectileSpeed.value, weapon.projectileDamage.value * damageMultiplier, weapon.name);
//             }
//
//             // trigger the muzzle flash
//             ParticleGroup muzzleFlash = particleFactory.Get(ParticleType.MuzzleFlash);
//             muzzleFlash.position = origin.position;
//             muzzleFlash.forward = origin.forward;
//             muzzleFlash.transform.SetParent(player.transform);
//             muzzleFlash.Play();
//             
//             EventManager.Active.WeaponFired(weapon);
//             AudioManager.Active.PlayOneShot(weapon.soundName);
//         }
//     }
// }
