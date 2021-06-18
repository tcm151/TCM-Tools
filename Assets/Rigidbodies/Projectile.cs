
using UnityEngine;
using TCM.Interfaces;


namespace TCM.Rigidbodies
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        //> PROJECTILE DATA CONTAINER
        [System.Serializable] public class Data
        {
            public string origin = "null";
            public float life, expiry = 5f;
            public float mass = 1f;
            public float damage = 1f;
            public float knockback = 1f;
        }
        
        //- COMPONENTS
        new private Rigidbody rigidbody;

        //- STATE CHECKS
        virtual protected bool IsMoving => rigidbody.velocity.magnitude > 0;
        virtual protected bool IsExpired => data.life >= data.expiry;

        private Vector3 previousPosition;
        private Data data;

        //> INITIALIZATION
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.mass = data.mass;
            
            previousPosition = rigidbody.position;
            data.life = 0;
        }

        //> FIRE WITH A GIVEN DIRECTION AND SPEED
        virtual public void Launch(Vector3 position, Vector3 direction, float speed, Data data) => Launch(position, direction * speed, data);

        //> FIRE WITH A GIVEN VELOCITY
        virtual public void Launch(Vector3 position, Vector3 velocity, Data data)
        {
            this.data = data;
            rigidbody.position = previousPosition = position;
            rigidbody.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
            rigidbody.AddForce(velocity * rigidbody.mass, ForceMode.Impulse);
        }

        //> UPDATE EVERY FRAME
        virtual protected void Update()
        {
            if (this.IsExpired) Destroy(this.gameObject);
            if (this.IsMoving) transform.rotation = Quaternion.LookRotation(rigidbody.velocity, Vector3.up);

            CheckImpact();
        }
        
        //> CHECK IMPACT FOR BALLISTIC PROJECTILES
        virtual protected void CheckImpact()
        {
            //@ ADD LAYER MASK CHECKING
            // if (Physics.Linecast(previousPosition, rigidbody.position, out RaycastHit hit, layerMask))
            
            if (Physics.Linecast(previousPosition, rigidbody.position, out RaycastHit hit))
            {
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                damageable?.TakeDamage(data.damage, data.knockback, data.origin);

                ImpactAt(hit.point);
            }
            else previousPosition = rigidbody.position;
        }

        //> DO THINGS ON IMPACT
        virtual protected void ImpactAt(Vector3 impactPoint)
        {
            // ParticleGroup impact = particleFactory.Get(Data.particles);
            // impact.position = impactPoint;
            // impact.forward = transform.forward;
            // impact.Play();
            
            Destroy(this.gameObject);
        }
    }
}