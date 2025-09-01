using UnityEngine;
using System.Collections.Generic;

public class PlayerShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float shootDelay = 0.5f;
    public float simulationTimeStep = 0.05f;
    public int simulationSteps = 100;
    public float maxPreviewDistance = 10f;

    private bool canShoot = true;
    private LineRenderer lineRenderer;
    private SfxManager sfxManager;

    protected void PlaySoundShoot(){
        if (sfxManager != null) sfxManager.AddSoundsSource("shoot");
    }

    void Start(){
        GameObject sfxManagerObject = GameObject.Find("SfxManager");
        if (sfxManagerObject != null) sfxManager = sfxManagerObject.GetComponent<SfxManager>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    public LineRenderer GetLineRenderer(){ return lineRenderer; }
    public bool GetCanShoot(){ return canShoot; }

    protected virtual void Update(){
        if (!canShoot) return;

        if (Input.GetMouseButton(1)){
            UpdatePreview();
        }
        else if (Input.GetMouseButtonUp(1)){
            Shoot();
        }
        else{
            lineRenderer.enabled = false;
        }
    }

    protected virtual void UpdatePreview(){
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        Vector2 velocity = direction * projectileSpeed;

        Vector2 gravity = Physics2D.gravity * GetProjectileGravityScale();

        Vector2 position = transform.position;
        List<Vector3> points = new List<Vector3>();
        points.Add(position);

        float totalDistance = 0f;

        for (int i = 0; i < simulationSteps; i++){
            Vector2 newVelocity = velocity + gravity * simulationTimeStep;
            Vector2 newPosition = position + newVelocity * simulationTimeStep;

            totalDistance += Vector2.Distance(position, newPosition);
            if (totalDistance >= maxPreviewDistance)
                break;

            RaycastHit2D hit = Physics2D.Raycast(position, newPosition - position, Vector2.Distance(position, newPosition));
            if (hit.collider != null){
                if (hit.collider.tag != "Player" && hit.collider.tag != "Light" && hit.collider.tag != "LightArea"){
                    points.Add(hit.point);
                    break;
                }   
            }

            points.Add(newPosition);
            position = newPosition;
            velocity = newVelocity;
        }

        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++){
            lineRenderer.SetPosition(i, points[i]);
        }
        lineRenderer.enabled = true;
    }

    protected virtual void Shoot(){
        canShoot = false;

        PlaySoundShoot();

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * projectileSpeed;

        lineRenderer.enabled = false;
        Invoke(nameof(CanShootTrue), shootDelay);
    }

    protected float GetProjectileGravityScale(){
        Rigidbody2D rb = projectilePrefab.GetComponent<Rigidbody2D>();
        return rb != null ? rb.gravityScale : 1f;
    }

    protected void CanShootTrue(){
        canShoot = true;
    }
    protected void SetCanShoot(bool canShoot){
        this.canShoot = canShoot;
    }
}
