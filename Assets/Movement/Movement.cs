using System.Collections.Generic;
using TCM.Tools;
using UnityEngine;
using UnityEngine.InputSystem;


namespace TCM.Movement
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        //> HOUSE KEEPING //>=========================================================================
        
        new private Rigidbody rigidbody;
        private float minGroundDot, minStairDot, minClimbDot;

        //> INITIALIZE THE PLAYER
        private void Awake()
        {
            currentStamina = lowestStamina = maxStamina;

            collisionList = new HashSet<GameObject>();
            
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            
            controls = new FirstPersonControls();
            controls.FirstPerson.Enable();
            
            OnValidate();
        }

        //> UPDATE DOT PRODUCTS ON INSPECTOR CHANGES
        private void OnValidate()
        {
            minGroundDot = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
            minStairDot  = Mathf.Cos(maxStairAngle  * Mathf.Deg2Rad);
            minClimbDot  = Mathf.Cos(maxClimbAngle  * Mathf.Deg2Rad);
        }

        // //> RETURN TO YOUR LAST VALID POSITION BEFORE DEATH
        // private void Death()
        // {
        //     // Debug.Log("YOU DIED!");
        //     // this.gameObject.transform.position = lastGroundedPosition;
        // }





        //> INPUT //> ================================================================================

        public Transform inputSpace = default;
        private FirstPersonControls controls;

        private Vector2 movementInput;
        private Vector3 Up, Right, Forward, gravity;
        private bool jumping, sprinting, climbing, wallRunning;

        //> GET INPUT EVERY FRAME
        private void Update()
        {
            UpdateInputSpace(); // update arbitrary axes

            movementInput = controls.FirstPerson.Movement.ReadValue<Vector2>();
            // movementInput = Vector2.ClampMagnitude(movementInput, 1);

            climbing = Mouse.current.leftButton.isPressed;
            jumping |= Keyboard.current.spaceKey.wasPressedThisFrame;
            sprinting |= Keyboard.current.leftShiftKey.isPressed;
            wallRunning = (sprinting && climbing);
            
            transform.rotation = Quaternion.LookRotation(Forward, Up); //? Might need to be reworked

            if (movementInput.magnitude <= 0 || movementInput.x > 0.5f || movementInput.x < -0.5f || movementInput.y < 0)
            {
                if (!wallRunning) sprinting = false;
            }
        }

        //> USE THE PLAYER RESPECTIVE AXES
        private void UpdateInputSpace()
        {
            // if (inputSpace)
            // {
                Right = Vector3.ProjectOnPlane(inputSpace.right, Up);
                Forward = Vector3.ProjectOnPlane(inputSpace.forward, Up);
            //     Right = ProjectDirectionOnPlane(inputSpace.right, Up);
            //     Forward = ProjectDirectionOnPlane(inputSpace.forward, Up);
            // }
            // else
            // {
            //     Right = ProjectDirectionOnPlane(Vector3.right, Up);
            //     Forward = ProjectDirectionOnPlane(Vector3.forward, Up);
            // }
        }




        //> MOVEMENT //>==============================================================================

        public float defaultSpeed = 10f, sprintSpeed = 15f, climbSpeed = 4f, exhaustedSpeed = 1f;
        public float defaultAcceleration = 25f, sprintAcceleration = 25f, airAcceleration = 8f, climbAcceleration = 20f;
        public float jumpHeight = 1.5f;
        public int   maxJumps = 1;
        public float maxGroundAngle = 25f, maxStairAngle = 50f, maxClimbAngle = 140f, groundSnappingThreshold = 12f;

        [SerializeField]
        private LayerMask groundMask = -1, stairMask = -1, climbMask = -1;

        private Vector3 newVelocity;
        private float groundedDistance = 1.25f;
        private int timeSinceGrounded, timeSinceJumped;

        //> MOVEMENT LOGIC LOOP
        private void FixedUpdate()
        {
            (gravity, Up) = Gravity.Get(rigidbody.position);

            //+ UP AND GRAVITY DEBUG RAYS
            // Debug.DrawRay(rigidbody.position, Up.normalized * 2.5f, Color.green);
            // Debug.DrawRay(rigidbody.position, gravity, Color.red);

            UpdateState();
            // CheckStamina();
            CalculateVelocity();

            if (jumping) Jump();
            if (CanClimb || CanWallRun) newVelocity -= contactNormal * (climbAcceleration * Time.deltaTime);
            else newVelocity += gravity * Time.deltaTime;
            
            // Debug.DrawRay(rigidbody.position, newVelocity, Color.blue);

            UpdateStamina();
            rigidbody.velocity = newVelocity;
            ResetState();
        }

        //> UPDATE THE PLAYER STATE
        private void UpdateState()
        {
            timeSinceGrounded += 1;
            timeSinceJumped += 1;
            newVelocity = rigidbody.velocity;

            if (OnGround) onLadder = false;

            if (IsWallRunning() || IsClimbing() || OnGround || SnappingToGround() || MultipleSteepContacts())
            {
                timeSinceGrounded = 0;

                if (timeSinceJumped > 1) jumpPhase = 0;
                if (groundContacts > 1) contactNormal.Normalize();
            }
            else contactNormal = Up;

            // UpdateBody();
        }

        //> RESET THE PLAYER STATE
        private void ResetState()
        {
            groundContacts = steepContacts = climbContacts = 0;
            contactNormal = steepNormal = climbNormal = bodyVelocity = Vector3.zero;
            previousBody = currentBody;
            currentBody = null;
        }

        //> CALCULATE THE INTERMEDIARY VELOCITY EACH PHYSICS STEP
        private void CalculateVelocity()
        {
            float acceleration, maxSpeed;
            Vector3 xAxis, zAxis;

            // if (CanClimb)
            // {
            //     acceleration = climbAcceleration;
            //     maxSpeed = climbSpeed;
            //     xAxis = Vector3.Cross(contactNormal, Up);
            //     zAxis = Up;
            // }
            // else if (CanWallRun)
            // {
            //     acceleration = sprintAcceleration;
            //     maxSpeed = sprintSpeed;
            //     xAxis = contactNormal;
            //     zAxis = Forward;
            //     newVelocity.y = 0f;
            // }
            // else
            {
                acceleration = (OnGround) ? defaultAcceleration : airAcceleration;
                acceleration = (sprinting) ? sprintAcceleration : acceleration;
                maxSpeed = (sprinting) ? sprintSpeed : defaultSpeed;
                xAxis = Right;
                zAxis = Forward;
            }
            // if (exhausted) maxSpeed = exhaustedSpeed;

            xAxis = Vector3.ProjectOnPlane(xAxis, contactNormal);
            zAxis = Vector3.ProjectOnPlane(zAxis, contactNormal);
            // xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
            // zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);
            
            //+ CONTACT NORMAL, X-AXIS, Z-AXIS DEBUGGING RAYS
            // Debug.DrawRay(rigidbody.position, contactNormal.normalized * 2.5f, Color.magenta);
            // Debug.DrawRay(rigidbody.position, xAxis.normalized * 1f, Color.white);
            // Debug.DrawRay(rigidbody.position, zAxis.normalized * 2.5f, Color.white);
                
            //- Connected body relative-velocity calculations
            Vector3 relativeVelocity = newVelocity - bodyVelocity;
            float currentX = Vector3.Dot(relativeVelocity, xAxis);
            float currentZ = Vector3.Dot(relativeVelocity, zAxis);

            float maxSpeedDelta = acceleration * Time.deltaTime;

            float newX = Mathf.MoveTowards(currentX, movementInput.x * maxSpeed, maxSpeedDelta);
            float newZ = Mathf.MoveTowards(currentZ, movementInput.y * maxSpeed, maxSpeedDelta);

            newVelocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
        }

        //> PROJECTION HELPER
        // private Vector3 ProjectDirectionOnPlane(Vector3 d, Vector3 n) => (d - n * Vector3.Dot(d, n)).normalized;

        //> DETERMINING IF PLAYER IS ABLE TO JUMP
        private void Jump()
        {
            jumping = false;
            climbing = false;
            onLadder = false;
            // ResetWallRunning();
            Vector3 jumpDirection;

            if (OnGround)
            {
                jumpDirection = contactNormal;
            }
            else if (OnSteep)
            {
                jumpDirection = steepNormal;
                jumpPhase--;
            }
            else if (maxJumps > 0 && jumpPhase <= maxJumps)
            {
                if (jumpPhase == 0) jumpPhase = 1; // skip first jump phase if jumping
                jumpDirection = contactNormal;
            }
            else return;

            timeSinceJumped = 0;
            jumpPhase++;

            float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * jumpHeight); // calculate required jump speed
            jumpDirection = (jumpDirection + Up).normalized; // add upwards bias to jump direction
            float alignedSpeed = Vector3.Dot(newVelocity, jumpDirection); // align jump speed to jump direction
            if (alignedSpeed > 0f) jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f); // limit maximum vertical speed

            newVelocity += jumpDirection * jumpSpeed;
            // Exert(jumpCost);
        }

        //> DETERMINE IF WE SHOULD SNAP THE PLAYER TO THE GROUND
        private bool SnappingToGround()
        {
            // cancel if not on ground or recently jumping
            if (timeSinceGrounded > 1 || timeSinceJumped <= 2) return false;
            // cancel if moving faster than snapping threshold
            if (newVelocity.magnitude > groundSnappingThreshold) return false;
            // cancel if grounding check fails
            if (!Physics.Raycast(rigidbody.position, -Up, out RaycastHit hit, groundedDistance, groundMask)) return false;
            // cancel if grounding check successful but surface angle is too steep
            if (Vector3.Dot(Up, hit.normal) < GetMinDP(hit.collider.gameObject.layer)) return false;
            // IF ALL CONDITIONS NOT TRUE THEN WE CAN SNAP TO THE GROUND


            groundContacts = 1; // only care about surface we hit
            contactNormal = hit.normal; // use hit surface as contact normal
            float dot = Vector3.Dot(newVelocity, hit.normal); // project our speed onto the surface normal
            currentBody = hit.rigidbody;

            // if our new speed is greater than 0, then our new velocity is our previous speed
            // projected onto the surface normal
            if (dot > 0) newVelocity = (newVelocity - hit.normal * dot).normalized * newVelocity.magnitude;
            return true; // snap was successful
        }

        // layer masks are stored in binary...
        private float GetMinDP(int layer) => (stairMask.Contains(layer)) ? minGroundDot : minStairDot;




        //> CONNECTED BODY //>========================================================================

        private Rigidbody currentBody, previousBody;
        private Vector3 bodyVelocity, bodyPositionW, bodyPositionL;

        //> UPDATE THE STATE OF THE BODY WE ARE CONNECTED TO
        private void UpdateBody()
        {
            if (currentBody && (currentBody.isKinematic || currentBody.mass >= rigidbody.mass))
            {
                if (currentBody == previousBody)
                {
                    Vector3 connectedBodyMovement = currentBody.transform.TransformPoint(bodyPositionL) - bodyPositionW;
                    bodyVelocity = connectedBodyMovement / Time.deltaTime;
                }
                bodyPositionW = rigidbody.position;
                bodyPositionL = currentBody.transform.InverseTransformPoint(bodyPositionW);
            }
        }




        //> CLIMBING //>==============================================================================

        private bool onLadder;
        private bool CanClimb => (climbContacts > 0 && timeSinceJumped > 2 && !CanWallRun);

        //> IF THIS IS TRUE WE MAY BE STUCK IN A CREVASE
        private bool MultipleSteepContacts()
        {
            if (steepContacts > 1) // touching multiple steep surfaces
            {
                steepNormal.Normalize(); // the average normal

                // if the average normal is greater than the minimum ground normal
                float UpDP = Vector3.Dot(Up, steepNormal);
                if (UpDP >= minGroundDot)
                {
                    // pretend we are on the ground so we can escape
                    steepContacts = 0;
                    groundContacts = 1;
                    contactNormal = steepNormal;
                    return true;
                }
            }
            return false; // not on multiple steep surfaces
        }

        //> IS THE PLAYER IS CURRENTLY CLIMBING
        private bool IsClimbing()
        {
            if (CanClimb)
            {
                if (climbContacts > 1)
                {
                    climbNormal.Normalize();
                    float UpDP = Vector3.Dot(Up, climbNormal);
                    if (UpDP >= minGroundDot) climbNormal = lastClimbNormal;
                }

                contactNormal = climbNormal;
                return true;
            }
            return false;
        }




        //> WALL RUNNING //>==========================================================================

        public  float wallRunTime = 1f;
        private float wallRunProgress;
        private bool  wallRunTimeReached;

        private bool CanWallRun => (wallRunning && climbContacts == 1);

        //> IS THE PLAYER CURRENTLY WALL RUNNING
        private bool IsWallRunning()
        {
            if (OnGround) ResetWallRunning();

            if (wallRunProgress >= wallRunTime)
            {
                wallRunTimeReached = true;
                return false;
            }

            if (CanWallRun && !wallRunTimeReached)
            {
                    wallRunProgress += Time.deltaTime;
                    contactNormal = climbNormal;
                    return true;
            }
            return false;
        }

        //> RESET WALL RUNNING PROGRESS
        private void ResetWallRunning()
        {
            wallRunProgress = 0;
            wallRunTimeReached = false;
        }




        //> STAMINA //>===============================================================================
        
        public float walkCost, sprintCost = 8f, jumpCost = 13f, climbCost = 15f;
        public float recoveryRatio = 10f, impulseThreshold = 15f;
        public float maxStamina = 100f, regenRate = 10f;
        
        private float timeToRecover, recoveryProgress;
        private float currentStamina, lowestStamina;
        // private bool  exhausted;

        //> GET THE CURRENT AND MAX STAMINA
        public (float, float) Stamina => (currentStamina, maxStamina);

        //> ALLOW STAMINA TOKENS TO INCREASE THE MAXIMUM STAMINA
        public void AddStamina(float amount) => maxStamina += amount;

        //> EXERT A CERTAIN AMOUNT OF STAMINA
        private void Exert(float amount) => currentStamina = Mathf.Max(currentStamina - amount, 0);

        //> CHECK IF THE PLAYER HAS ENOUGH STAMINA TO DO ANYTHING
        private void CheckStamina()
        {
            if (currentStamina <= 0)
            {
                // exhausted = true;
                jumping = false;
                climbing = false;
                sprinting = false;
            }
        }

        //> UPDATE THE STAMINA EVERY FRAME
        private void UpdateStamina()
        {
            // update lowest stamina point
            if (currentStamina < lowestStamina) lowestStamina = currentStamina;

            if (sprinting && OnGround) Exert(sprintCost * Time.deltaTime);
            if (IsWallRunning()) Exert(sprintCost * Time.deltaTime);
            if (CanClimb && !onLadder) Exert(climbCost * Time.deltaTime);
            if (CanClimb && onLadder)  Exert((climbCost / 3) * Time.deltaTime);
            
            RegenStamina(); // attempt to regen stamina
        }

        //> ATTEMPT TO REGEN STAMINA IF ABLE
        private void RegenStamina()
        {
            timeToRecover = (maxStamina - lowestStamina) / recoveryRatio;

            if (CanClimb) return;
            if (OnGround && newVelocity.magnitude <= defaultSpeed + 0.1f)
            {
                if (recoveryProgress < timeToRecover) recoveryProgress += Time.deltaTime;
                
                else if (currentStamina < maxStamina)
                {
                    float regenAmount = regenRate * Time.deltaTime;
                    // Debug.Log("REGEN: " + regenAmount);
                    currentStamina = Mathf.Min(currentStamina + regenAmount, maxStamina);
                }
                else
                {
                    recoveryProgress = 0;
                    lowestStamina = maxStamina;
                    // exhausted = false;
                }
            }
        }




        //> COLLISIONS //>============================================================================
        
        private HashSet<GameObject> collisionList; // can't add duplicates
        private Vector3 contactNormal, steepNormal, climbNormal, lastClimbNormal, lastGroundedPosition;
        private int jumpPhase, groundContacts, steepContacts, climbContacts;

        private bool OnGround => groundContacts > 0;
        private bool OnSteep  => steepContacts > 0;

        //> MANAGE SURFACE CONTACTS
        private void OnCollisionExit(Collision collision) => EvaluateCollision(collision);
        private void OnCollisionStay(Collision collision) => EvaluateCollision(collision);
        // private void OnCollisionEnter(Collision collision) => CheckDeath(collision);

        //> CHECK EVERY SURFACE CONTACT AND SET THE CONTACT NORMAL
        private void EvaluateCollision(Collision collision)
        {
            if (collision.gameObject.tag == "Checkpointable") lastGroundedPosition = transform.position;
            if (collision.gameObject.tag == "Ladder") onLadder |= true;

            collisionList.Clear(); // maybe optional

            int layer = collision.gameObject.layer;
            float minDP = GetMinDP(layer);

            // check every surface player is currently touching
            for (int i = 0; i < collision.contactCount; i++)
            {
                if (!collisionList.Add(collision.gameObject)) continue; // ignore repeats

                Vector3 normal = collision.GetContact(i).normal;
                float UpDP = Vector3.Dot(Up, normal);

                // if a standable surface add it to the surface normal
                if (UpDP >= minDP)
                {
                    groundContacts += 1;
                    contactNormal += normal;
                    currentBody = collision.rigidbody;
                }
                else
                {
                    // this is a wall
                    if (UpDP > -0.01f)
                    {
                        steepContacts += 1;
                        steepNormal += normal;
                        if (groundContacts == 0) currentBody = collision.rigidbody;
                    }

                    // this is a climbable wall
                    if (climbing && UpDP >= minClimbDot && climbMask.Contains(layer))
                    {
                        climbContacts += 1;
                        climbNormal += normal;
                        lastClimbNormal = normal;
                        currentBody = collision.rigidbody;
                    }
                }
            }
        }

        // //> CHECK IF FALL IMPACT CAUSED DEATH
        // private void CheckDeath(Collision collision)
        // {
        //     float impulse = collision.impulse.y;
        //
        //     if (impulse >= impulseThreshold)
        //     {
        //         if (impulse >= 13f) Death();
        //         else if (impulse >= 06f) Exert(25f);
        //         else if (impulse >= 08f) Exert(50f);
        //         else if (impulse >= 10f) Exert(75f);
        //         else if (impulse >= 12f) Exert(100f);
        //
        //         CheckStamina();
        //         UpdateStamina();
        //     }
        // }
    }
}

