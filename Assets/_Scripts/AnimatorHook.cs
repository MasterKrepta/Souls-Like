using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHook : MonoBehaviour {

    Animator anim;
    StateManager states;

    public float rm_Multiplier;

    public void Init(StateManager st) {
        states = st;
        anim = states.anim;
    }
    void OnAnimatorMove() {
        if (states.canMove)
            return;

        states.rBody.drag = 0;

        if (rm_Multiplier == 0)
            rm_Multiplier = 1;

        Vector3 delta = anim.deltaPosition;
        delta.y = 0;
        Vector3 v = (delta * rm_Multiplier) / states.delta;
        states.rBody.velocity = v;

    }


}
