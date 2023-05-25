using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera: MonoBehaviour {
    [SerializeField]
    private CameraTarget target;

    private void Update() {
        if(target == null) {
            return;
        }

        Vector3 position = target.transform.position;
        // Do NOT modify the Z coordinate.
        position.z = transform.position.z;
        transform.position = position;
    }

    public void AttachTo(CameraTarget target) {
        this.target = target;
        Update();
    }

    private void FixedUpdate()
    {
        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<CameraTarget>();
        }
    }
}
