using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovementController : MonoBehaviour
{
    public CarSO car;
    Rigidbody rb;
    public Vector3 thrust = new Vector3(0, 0, 50.0f);
    public Vector3 rotationTorque = new Vector3(0, 10.0f, 0);

    private bool isMovementEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        thrust = new Vector3(0, 0, car.speed);
    }

    /*** enables car movement after countdown is over ***/
    public void EnableMovement()
    {
        isMovementEnabled = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (!isMovementEnabled) return;

        //forward movement
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(thrust);
        }
        //backward movement
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeForce(-thrust);
        }
        //steer left
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddRelativeTorque(-rotationTorque);
        }
        //steer right
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddRelativeTorque(rotationTorque);
        }

    }

}
