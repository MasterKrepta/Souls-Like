using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {
    [Header("Init")]
    public GameObject activeModel;

    [Header("Inputs")]
    public float vertical;
    public float horizontal;
    public float moveAmount;
    public Vector3 moveDir;
    public bool rt, rb, lt, lb;
    public bool rollInput;

    [Header("Stats")]
    public float moveSpeed = 3;
    public float runSpeed = 3.5f;
    public float rotSpeed = 5;
    public float toGround = .5f;
    public float rollSpeed = 1;
    

    [Header("States")]
    public bool run;
    public bool onGround;
    public bool lockOn;
    public bool inAction;
    public bool canMove;
    public bool isTwoHanded;
    

    [Header("Other")]
    public EnemyTarget lockOnTarget;

    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public Rigidbody rBody;
    [HideInInspector]
    public float delta;
    [HideInInspector]
    public LayerMask ignoreLayers;
    [HideInInspector]
    public AnimatorHook a_hook;

    float _actionDelay;

    public void Init() {
        SetupAnimator();
        rBody = GetComponent<Rigidbody>();
        rBody.angularDrag = 999;
        rBody.drag = 4;
        rBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        a_hook = activeModel.AddComponent<AnimatorHook>();
        a_hook.Init(this);

        gameObject.layer = 8;
        ignoreLayers = ~(1 << 9);

        
    }

    void SetupAnimator() {
        if (activeModel == null) {
            anim.GetComponentInChildren<Animator>();
            if (anim == null) {
                Debug.LogWarning("No model found");
            }
        }
        anim = activeModel.GetComponent<Animator>();

    }

    

    public void FixedTick(float d) {
        delta = d;

        DetectAction();

        if (inAction) {
            anim.applyRootMotion = true;
            _actionDelay += delta;
            if(_actionDelay > 0.3f) {
                inAction = false;
                _actionDelay = 0;
            }
            else {
                return;
            }
            
        }
        
        canMove = anim.GetBool("canMove");

        if (!canMove)
            return;

        a_hook.rm_Multiplier = 1;
        HandleRolls();
        

        anim.applyRootMotion = false;
        rBody.drag = (moveAmount > 0 || onGround == false) ? 0 : 4;

        float targetSpeed = moveSpeed;
        if (run) {
            targetSpeed = runSpeed;
        }

        if (onGround) {
            rBody.velocity = moveDir * (targetSpeed * moveAmount);
        }
        if (run) 
            lockOn = false;
        
        Vector3 targetDir = (lockOnTarget == false)? moveDir 
            : lockOnTarget.transform.position - transform.position;

        targetDir.y = 0;
        if (targetDir == Vector3.zero) 
            targetDir = transform.forward;
        
        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRot = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotSpeed);
        transform.rotation = targetRot;

        anim.SetBool("lockOn", lockOn);

        //TODO Debug this, we are stuck in lockon animations no matter what
        if (lockOn == false) {
            Debug.Log("Do normal");
            HandleMovementAnimations();
        }
        else {
            Debug.Log("Do lockon");
            HandleLockOnAnimations(moveDir);
        }
            
    }

    public void DetectAction() {
        if (canMove == false) {
            return;
        }
        if (rb == false && rt == false && lt == false && lb == false) {
            return;
        }
        string targetAnim = null;

        //TODO For testing only
        if (rb)
            targetAnim = "oh_attack_1";
        if (rt)
            targetAnim = "oh_attack_2";
        if (lt)
            targetAnim = "oh_attack_3";
        if (lb)
            targetAnim = "th_attack_1";

        if (string.IsNullOrEmpty(targetAnim))
            return;

        canMove = false;
        inAction = true;
        anim.CrossFade(targetAnim, 0.2f);
        //rBody.velocity = Vector3.zero;
    }

    public void HandleRolls() {
        if (!rollInput)
            return;

        float v = vertical;
        float h = horizontal;
        v = (moveAmount > 0.3f) ? 1 : 0;
        h = 0;


        //if (!lockOn) {
        //    v = (moveAmount > 0.3f)? 1 : 0;
        //    h = 0;
        //}
        //else {
        //    //remove small amounts of inputs
        //    if(Mathf.Abs(v) < 0.3f) 
        //        v = 0;
        //    if (Mathf.Abs(h) < 0.3f)
        //        h = 0;

        //}

        //HACK

        if(v != 0){

            if (moveDir == Vector3.zero)
                moveDir = transform.forward;

            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = targetRot;
        }

        a_hook.rm_Multiplier = rollSpeed;

        anim.SetFloat("vertical", v);
        anim.SetFloat("horizontal", h);

        canMove = false;
        inAction = true;
        anim.CrossFade("Rolls", 0.2f);

    }
    public void Tick(float d) {
        delta = d;
        onGround = OnGround();

        anim.SetBool("onGround", onGround);
    }
    void HandleMovementAnimations() {
        anim.SetBool("run", run);
        anim.SetFloat("vertical", moveAmount, 0.4f, delta);

    }

    void HandleLockOnAnimations(Vector3 moveDir) {
        Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
        float h = relativeDir.x;
        float v = relativeDir.z;

        anim.SetFloat("vertical", v, 0.2f, delta);
        anim.SetFloat("horizontal", h, 0.2f, delta);

    }

    public bool OnGround() {
        bool r = false;
        Vector3 origin = transform.position + (Vector3.up * toGround);
        Vector3 dir = -Vector3.up;
        float dist = toGround + 0.3f;

        RaycastHit hit;
        if(Physics.Raycast(origin, dir, out hit, dist, ignoreLayers)) {
            r = true;
            Vector3 targetPos = hit.point;
            transform.position = targetPos;
        }

        return r;

    }

    public void HandleTwoHanded() {
        anim.SetBool("two_handed", isTwoHanded);
    }
}
