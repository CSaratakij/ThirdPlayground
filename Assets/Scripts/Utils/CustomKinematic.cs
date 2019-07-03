using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playground
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    [AddComponentMenu("Physics 2D/Custom Kinematic")]
    public class CustomKinematic : MonoBehaviour
    {
        const float minMoveDistance = 0.001f;
        const float minGroundNormalY = 0.65f;
        const float shellRedius = 0.01f;

        public bool Grounded => grounded;

        [SerializeField]
        internal float gravityMultipler = 1.0f;

        internal Vector2 velocity = Vector2.zero;
        internal bool grounded = false;

        Vector2 groundNormal = Vector2.up;
        ContactFilter2D contactFilter;

        Rigidbody2D rigid;
        RaycastHit2D[] hitBuffer;

        void Reset()
        {
            gravityMultipler = 1.0f;

            rigid = GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Kinematic;
            rigid.simulated = true;
            rigid.useFullKinematicContacts = true;
            rigid.freezeRotation = true;
        }

        void OnValidate()
        {
            gravityMultipler = Mathf.Abs(gravityMultipler);
        }

        void Awake()
        {
            Initialize();
        }

        void FixedUpdate()
        {
            velocity += (gravityMultipler * Physics2D.gravity) * Time.fixedDeltaTime;
            var deltaPosition = velocity * Time.fixedDeltaTime;

            /* var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x); */
            var moveAlongGround = (Vector2.right * groundNormal.y) + (Vector2.up * -groundNormal.x);
            var move = moveAlongGround * deltaPosition.x;
            Movement(move, false);

            move = Vector2.up * deltaPosition.y;
            Movement(move, true);
        }

        void Initialize()
        {
            rigid = GetComponent<Rigidbody2D>();
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            contactFilter.useLayerMask = true;
            hitBuffer = new RaycastHit2D[16];
        }

        void Movement(Vector2 move, bool isYMovement)
        {
            grounded = false;
            float distance = move.magnitude;

            if (distance < minMoveDistance)
                return;

            int totalHit = rigid.Cast(move, contactFilter, hitBuffer, distance + shellRedius);

            for (int i = 0; i < totalHit; ++i)
            {
                Vector2 currentNormal = hitBuffer[i].normal;

                if (currentNormal.y > minGroundNormalY)
                {
                    grounded = true;

                    if (isYMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);

                if (projection < 0)
                {
                    velocity = velocity - (projection * currentNormal);
                }

                float modifiedDistance = hitBuffer[i].distance - shellRedius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }

            rigid.position = rigid.position + (move.normalized * distance);
        }

        internal void SetVelocity(Vector2 velocity)
        {
            MoveHorizontal(velocity.x);
            MoveVertical(velocity.y);
        }

        internal void MoveHorizontal(float speed)
        {
            velocity.x = speed;
        }

        internal void MoveVertical(float speed)
        {
            velocity.y = speed;
        }
    }
}

