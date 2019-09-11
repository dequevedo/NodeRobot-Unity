using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    public float actualAngle = 0;

    public float min = -45;
    public float max = 45;

    void Update()
    {
        float movement = Input.GetAxis("Horizontal");

        float inputHorizontal = SimpleInput.GetAxis("Horizontal");

        actualAngle += inputHorizontal;
        actualAngle = Mathf.Clamp(actualAngle, min, max);

        transform.rotation = Quaternion.Euler(0, actualAngle, 0);
    }

    public int ConvertToArduinoAngles()
    {
        //int a = actualAngle - 95;

        return 95 + (int)actualAngle*-1;
    }
}
