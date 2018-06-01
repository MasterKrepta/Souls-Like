using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {

    float vertical;
    float horizontal;
    bool b_input;
    bool a_input;
    bool x_input;
    bool y_input;

    bool rb_input;
    float rt_axis;
    bool rt_input;
    bool lb_input;
    float lt_axis;
    bool lt_input;


    StateManager states;
    CameraManager cameraManager;
    float delta;

	// Use this for initialization
	void Start () {
        states = GetComponent<StateManager>();
        states.Init();

        cameraManager = CameraManager.singleton;
        cameraManager.Init(this.transform);
	}

    private void FixedUpdate() {
        delta = Time.fixedDeltaTime;
        GetInput();
        UpdateStates();
        states.FixedTick(delta);
        cameraManager.Tick(delta);

    }
    private void Update() {
        delta = Time.deltaTime;
        states.Tick(delta);

    }

    void GetInput() {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        b_input = Input.GetButton("B");
        x_input = Input.GetButton("X");
        y_input = Input.GetButtonUp("Y");
        a_input = Input.GetButton("A");
        rt_input = Input.GetButton("RT");
        rt_axis = Input.GetAxis("RT");
        if(rt_axis != 0) {
            rt_input = true;
        }

        lt_input = Input.GetButton("LT");
        lt_axis = Input.GetAxis("LT");
        if (lt_axis != 0) {
            lt_input = true;
        }

        rb_input = Input.GetButton("RB");
        lb_input = Input.GetButton("LB");

    }

    void UpdateStates() {
        states.horizontal = horizontal;
        states.vertical = vertical;

        Vector3 v = vertical * cameraManager.transform.forward;
        Vector3 h = horizontal * cameraManager.transform.right;

        states.moveDir = (v + h).normalized;
        float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
        states.moveAmount = Mathf.Clamp01(m);

        if (b_input) {
            states.run = (states.moveAmount > 0);
        }  else {
            states.run = false;
        }

        states.rt = rt_input;
        states.lt = lt_input;
        states.rb = rb_input;
        states.lb = lb_input;


        if (y_input) {
            states.isTwoHanded = !states.isTwoHanded;
            states.HandleTwoHanded();
        }
        

             

    }
}
