
using UnityEngine;

public class DirectionalLightRotator : MonoBehaviour
{

    void FixedUpdate()
    {
        float x = transform.rotation.eulerAngles.x;
        float y = transform.rotation.eulerAngles.y;
        float z = transform.rotation.eulerAngles.z;
        transform.rotation = Quaternion.Euler(x, y + Time.deltaTime * 2, z);
    }

}
