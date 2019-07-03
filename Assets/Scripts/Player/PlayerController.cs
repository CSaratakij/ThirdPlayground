using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playground
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CustomKinematic))]
    [AddComponentMenu("Game/Player/Player Controller")]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        float speed = 6.0f;

        [SerializeField]
        float jumpSpeed = 6.0f;

        Vector2 inputAxis;
        CustomKinematic kinematic;

        void Awake()
        {
            Initialize();
        }

        void Update()
        {
            MovementHandler();
        }

        void Initialize()
        {
            kinematic = GetComponent<CustomKinematic>();
        }

        void MovementHandler()
        {
            inputAxis.x = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump") && kinematic.grounded)
            {
                kinematic.MoveVertical(jumpSpeed);
            }

            kinematic.MoveHorizontal(inputAxis.x * speed);
        }
    }
}

