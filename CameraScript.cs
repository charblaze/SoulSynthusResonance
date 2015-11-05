using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class CameraScript : MonoBehaviour
{

    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    private Rigidbody rigidbody;

    float x = 0.0f;
    float y = 0.0f;
    
    GameObject player;
    PlayerMovement pm;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        rigidbody = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rigidbody != null)
        {
            rigidbody.freezeRotation = true;
        }
        player = GameObject.FindGameObjectWithTag("Player");
        pm = player.GetComponent<PlayerMovement>();
    }
    bool lookedat = false;
    float timer = 1f;

    void LateUpdate()
    {
        if (pm.MenuIsUp)
        {
            return;
        }
        // performed when you're not targetting or when target can't be found. manual control
        if (!pm.isTargeting)
        {
            if (target)
            {
                x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);

                Quaternion rotation = Quaternion.Euler(y, x, 0);

                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

               // RaycastHit hit;
                //if (Physics.Linecast(target.position, transform.position, out hit, 1))
               // {
                //    distance -= hit.distance;
                //}
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + target.position;


                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10f);
                transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 10f);
            }
        } else
        {
            x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            

            y = ClampAngle(y, 0, 0);
            x = ClampAngle(x, 0, 0);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
            /*
            RaycastHit hit;
            if (Physics.Linecast(target.position, transform.position, out hit))
            {
                distance -= hit.distance;
            }*/
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            Vector3 away = (target.position - pm.TARGET.position).normalized;
            away.y = 0;
            away = away.normalized;
            transform.position = Vector3.Lerp(transform.position, target.position + away * 2.6f + Vector3.up * 0.6f + target.right * 0.8f, Time.deltaTime * 4f);
            transform.LookAt(target.position);
/*
            transform.position = Vector3.Lerp(transform.position, pm.TARGET.position + (target.position - pm.TARGET.position) * negDistance.magnitude * 1.2f + Vector3.up * 0.6f + target.right * 0.9f, Time.deltaTime * 4f);
            transform.LookAt(pm.TARGET.position + (target.position - pm.TARGET.position) * 0.5f);*/
            //transform.LookAt(pm.TARGET.position + (target.position - pm.TARGET.position) * 0.5f);
            /*
            Vector3 targetpos = pm.TARGET.position + (target.position - pm.TARGET.position) + Vector3.up * 3f;
            if(transform.position.x < targetpos.x)
            {
                x += 0.5f * xSpeed * distance * 0.02f;
            } else if (transform.position.x > targetpos.x)
            {
                x -= 0.5f * xSpeed * distance * .02f;
            }
            if (transform.position.y < targetpos.y)
            {
                y += 0.5f * xSpeed *  0.02f;
            }
            else if (transform.position.y > targetpos.y)
            {
                y -= 0.5f * xSpeed *  .02f;
            }

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            RaycastHit hit;
            if (Physics.Linecast(target.position, transform.position, out hit))
            {
                distance -= hit.distance;
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;


            transform.rotation = rotation;
            transform.position = position;*/
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}