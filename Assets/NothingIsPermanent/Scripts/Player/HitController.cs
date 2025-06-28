using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HitController : MonoBehaviour {
    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            DoHit();
        }
    }
    
    void DoHit() {
        var cam = Camera.main;
        var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        if (Physics.Raycast(ray, out RaycastHit hit, 100f)) {
            var dissolveObject = hit.collider.GetComponent<DissolveObject>();
            dissolveObject.StartDestroy(hit.point);
            
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 3.0f);
            Debug.Log("Попал в: " + hit.collider.name);
        }
        else {
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 3.0f);
        }
    }
}
