using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSpearEffect : MonoBehaviour
{
    public float Lifetime = 5f;
    public float RotateSpeed;
    private float RotationSpeed;
    public GameObject RotationPoint;
    private bool FinishedRotating = false;
    public float MaxRotationPoint; //This determines at what point the spear and laser stop rotating (angles expressed on a -1 to 1 scale).
    /// </summary>

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Lifetime -= Time.deltaTime;
        if (FinishedRotating == false)
        {
            RotationSpeed += RotateSpeed * Time.deltaTime;
            this.transform.RotateAround(RotationPoint.transform.position, Vector3.left, RotationSpeed * Time.deltaTime);
            Debug.Log(this.transform.rotation.x);
        }
        if (this.transform.rotation.x <= MaxRotationPoint)
        {
            FinishedRotating = true;
        }
        if (Lifetime <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
