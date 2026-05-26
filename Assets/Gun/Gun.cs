using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Gun : MonoBehaviour
{
    
    public GameObject bulletPrefab;
    public Transform muzzlePoint;
    public ParticleSystem muzzleFlash;
    public float bulletSpeed = 30f;
    public float fireRate = 0.3f;
    private float nextFireTime = 0f;
    private XRGrabInteractable grabInteractable;
    private bool isHeld = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            Debug.LogError("Falta GrabINteractable");
            return;
        }

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
        grabInteractable.activated.AddListener(OnTriggerPressed);
    }

    void OnDestroy()
    {
        if (grabInteractable == null)
        {
            return;
        }

        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
        grabInteractable.activated.RemoveListener(OnTriggerPressed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isHeld = true;
        Debug.Log("Pistola agarrada");
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isHeld = false;
    }

    private void OnTriggerPressed(ActivateEventArgs args)
    {
        TryShoot();
    }

    private void TryShoot()
    {
        if (Time.time < nextFireTime) return;
        if (bulletPrefab == null || muzzlePoint == null)
        {
            Debug.LogWarning("Asigna bulletPrefab y muzzlePoint");
            return;
        }
        nextFireTime = Time.time + fireRate;
        Shoot();
    }

    private void Shoot()
    {
        GameObject bulletGO = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Launch(muzzlePoint.forward, bulletSpeed);
        else
        {
            Rigidbody rb = bulletGO.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = muzzlePoint.forward * bulletSpeed;
        }

        if (muzzleFlash != null)
            muzzleFlash.Play();
        
    }
#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            TryShoot();
    }
#endif
}
