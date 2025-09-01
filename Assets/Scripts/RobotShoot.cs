using UnityEngine;
using System.Collections.Generic;

public class RobotShoot : PlayerShoot
{
    public WaveManager waveManager;
    public Transform shooterTransform;

    protected override void Update() {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
            UpdatePreview();
        } else {
            GetLineRenderer().enabled = false;
        }

        if (!GetCanShoot()) return;

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
            Shoot();
        }
    }

    protected override void Shoot() {
        SetCanShoot(false);

        PlaySoundShoot();

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 direction = (mouseWorldPos - shooterTransform.position).normalized;

        GetComponent<SpriteRenderer>().flipX = direction.x < 0f;

        GameObject projectile = Instantiate(projectilePrefab, shooterTransform.position, Quaternion.identity);
        projectile.GetComponent<ProjectileRobotTrigger>().SetWaveManager(waveManager);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * projectileSpeed;

        GetLineRenderer().enabled = false;
        Invoke(nameof(CanShootTrue), shootDelay);
    }

    protected override void UpdatePreview() {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 direction = (mouseWorldPos - shooterTransform.position).normalized;
        Vector2 velocity = direction * projectileSpeed;
        Vector2 gravity = Physics2D.gravity * GetProjectileGravityScale();

        Vector2 position = shooterTransform.position;
        List<Vector3> points = new List<Vector3> { position };

        float totalDistance = 0f;

        for (int i = 0; i < simulationSteps; i++) {
            Vector2 newVelocity = velocity + gravity * simulationTimeStep;
            Vector2 newPosition = position + newVelocity * simulationTimeStep;

            totalDistance += Vector2.Distance(position, newPosition);
            if (totalDistance >= maxPreviewDistance) break;

            RaycastHit2D hit = Physics2D.Raycast(position, newPosition - position, Vector2.Distance(position, newPosition));
            if (hit.collider != null && hit.collider.tag != "Player" && hit.collider.tag != "Light" && hit.collider.tag != "LightArea") {
                points.Add(hit.point);
                break;
            }

            points.Add(newPosition);
            position = newPosition;
            velocity = newVelocity;
        }

        GetLineRenderer().positionCount = points.Count;
        for (int i = 0; i < points.Count; i++) {
            GetLineRenderer().SetPosition(i, points[i]);
        }

        GetLineRenderer().enabled = true;
    }
}
