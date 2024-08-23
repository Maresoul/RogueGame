#if ENABLE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM_PACKAGE
#define USE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.Controls;
#endif

using UnityEngine;

public class PFX_ProjectileObjectWeapon : MonoBehaviour
{
    public float FireRate = 0.15f;
    public float Speed = 15f;
    public float ImpactOffset = 0.15f;
    public GameObject FlashFX;
    public PFX_ProjectileObject ProjectileFX;
    public GameObject ImpactFX;
    public float DestroyDelay = 3f;
    public float FlashFXDestroyDelay = 2f;
    public float ImpactFXDestroyDelay = 2f;
    private bool _isButtonHold;
    private float _time;

    private void LateUpdate()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit))
            return;

        var lookDelta = hit.point - transform.position;
        var targetRot = Quaternion.LookRotation(lookDelta);
        transform.rotation = targetRot;

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetMouseButtonDown(0))
            _isButtonHold = true;
        else if (Input.GetMouseButtonUp(0))
            _isButtonHold = false;
#endif

        _time += Time.deltaTime;

        if (!_isButtonHold)
            return;

        if (_time < FireRate)
            return;

        var flash = Instantiate(FlashFX, transform.position, transform.rotation);
        flash.transform.localScale = transform.localScale;
        Destroy(flash.gameObject, FlashFXDestroyDelay);

        var projectile = Instantiate(ProjectileFX, transform.position, transform.rotation);

        projectile.transform.forward = gameObject.transform.forward;
        projectile.Speed = Speed;
        projectile.ImpactFX = ImpactFX;
        projectile.ImpactFXDestroyDelay = ImpactFXDestroyDelay;
        projectile.ImpactOffset = ImpactOffset;
        projectile.transform.localScale = transform.localScale;
       
        var trails = projectile.GetComponentsInChildren<TrailRenderer>();
        foreach (var trail in trails)
            trail.widthMultiplier *= transform.localScale.x;

        Destroy(projectile.gameObject, DestroyDelay);

        _time = 0;
    }
}
