using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public static CameraManager singleton;

    public bool lockOn;
    public float followSpeed = 9;
    public float mouseSpeed = 2;
    public float controllerSpeed = 7;

    public Transform target;
    public Transform lockOnTarget;

    public Transform pivot;
    public Transform camTrans;

    float turnSmoothing = .1f;
    public float minAngle = -35;
    public float maxAngle = 35;
    float smoothX;
    float smoothY;
    float smoothXVelocity;
    float smoothYVelocity;
    public float lookAngle;
    public float tiltAngle;

    public void Init(Transform t) {
        target = t;
        camTrans = Camera.main.transform;
        pivot = camTrans.parent;
    }

    public void Tick(float d) {
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");

        float c_h = Input.GetAxis("RightAxisX");
        float c_v = Input.GetAxis("RightAxisY");

        float targetSpeed = mouseSpeed;

        if(c_h != 0 || c_v !=0) {
            h = c_h;
            v = c_v;
            targetSpeed = controllerSpeed;
        }

        FollowTarget(d);
        HandleRotations(d, v, h, targetSpeed);
    }
    private void Awake() {
        if(singleton != null) {
            Destroy(gameObject);
        }
        singleton = this;
    }

    void FollowTarget(float d) {
        float speed = d * followSpeed;
        Vector3 targetPos = Vector3.Lerp(transform.position, target.position, speed);
        transform.position = targetPos;
    }

    void HandleRotations(float d, float v, float h, float targetSpeed) {
        if(turnSmoothing > 0) {
            smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXVelocity, turnSmoothing);
            smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYVelocity, turnSmoothing);
        }
        else {
            smoothX = h;
            smoothY = v;
        }

        tiltAngle -= smoothY * targetSpeed;
        tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
        pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
        

        if (lockOn && lockOnTarget != null) {
            Vector3 targetDir = lockOnTarget.position - transform.position;
            targetDir.Normalize();
            //targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = transform.forward;

            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, d * 9);
            lookAngle = transform.eulerAngles.y;
            return;
        }
        lookAngle += smoothX * targetSpeed;
        transform.rotation = Quaternion.Euler(0, lookAngle, 0);

      
    }
}
