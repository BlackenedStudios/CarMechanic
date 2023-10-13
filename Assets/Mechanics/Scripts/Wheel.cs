using UnityEngine;

public class Wheel : MonoBehaviour
{
    public bool rotatable = true;
    private float yangle;
    private float xangle;
    public void Animate(float angle,float speed)
    {
        xangle += speed;
        yangle = Mathf.Clamp(angle,-45f,45f);

        if(rotatable) transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, yangle, 0), Time.deltaTime * 5);

        transform.GetChild(0).localRotation = Quaternion.Euler(xangle, 0, 0);
    }
}
