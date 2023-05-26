using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //TODO: set false to player after implementation of attack button
    public bool rapidFire = true;

    public AudioClip shootSound;

    public GameObject bullet;

    private int currentReloadTicks;
    public int maxReloadTicks = 200;

    //direction of shooting, if 0 than it does not shoot
    //private Vector2 target = Vector2.zero;
    private Vector2 target = Vector2.zero;

    void Start()
    {
        currentReloadTicks = maxReloadTicks;
        transform.position = transform.parent.transform.position+ Vector3.right;
        target = new Vector2 (1f,1f);
    }

    void FixedUpdate()
    {
        SetAim();
        if (currentReloadTicks > 0)
        {
            currentReloadTicks--;
        }
        if (rapidFire)
        {
            tryToShoot();
        }
    }
    

    //TODO: Add button that uses this function
    public void tryToShoot()
    {
        if (bullet == null)
        {
            return;
        }
        else if (currentReloadTicks == 0 && target != Vector2.zero)
        {
            var newBullet = Instantiate(bullet);
            newBullet.transform.position = transform.position;
            newBullet.GetComponent<Bullet>().SetMovement(new Vector2(-transform.parent.position.x + transform.position.x, -transform.parent.position.y + transform.position.y));
            currentReloadTicks = maxReloadTicks;
            SoundPlayer.PlaySound(shootSound);
        }
    }

    private void SetAim()
    {
        if (transform.parent.GetComponent<Entitiy>().getMovement() != Vector2.zero)
        {
            Vector2 parentMovement = transform.parent.GetComponent<Entitiy>().getMovement();
            Vector3 newRelativePos = new Vector3(parentMovement.x, parentMovement.y, 0);
            transform.position = transform.parent.transform.position + newRelativePos;
        }
    }
}
