using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHook : MonoBehaviour {

    Animator anim;
    StateManager states;

    public void Init(StateManager st) {
        states = st;
        anim = states.anim;
    }
    void OnAnimatorMove() {
        if (states.canMove)
            return;

        states.rBody.drag = 0;
        float multiplier = 1;

        Vector3 delta = anim.deltaPosition;
        delta.y = 0;
        Vector3 v = (delta * multiplier) / states.delta;
        states.rBody.velocity = v;

    }


}