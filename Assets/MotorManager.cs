using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MotorManager : MonoBehaviour
{
    public TCPTestClient TCPClient;

    [Header("Raw Input")]
    public float horizontalMovement;
    public float verticalMovement;

    [Header("General Arrow")]
    public GameObject generalArrow;

    [Header("Left Arrow")]
    public GameObject leftArrow;
    public TextMesh lNum1;
    public TextMesh lNum2;
    public TextMesh lPWMNum;

    [Header("Right Arrow")]
    public GameObject rightArrow;
    public TextMesh rNum1;
    public TextMesh rNum2;
    public TextMesh rPWMNum;

    [Header("Slider")]
    public Slider slider;

    [Header("Accelerometer")]
    public GameObject cube;
    public float accX;
    public float accY;
    public Text accXtext;
    public Text accYtext;


    [Header("Speeds")]
    public float generalSpeed = 0;


    [Header("Speeds")]
    public int leftMotor = 0;
    public int rightMotor = 0;

    [Header("Speeds")]
    public float leftSpeed = 0;
    public float rightSpeed = 0;



    private void Start()
    {
        StartCoroutine("SendDataToArduino");
    }

    void Update()
    {
        accX = Input.acceleration.x;
        accY = Input.acceleration.z;
        accXtext.text = "AccX: " + accX.ToString();
        accYtext.text = "AccY: " + accY.ToString();
        cube.transform.Rotate(accX, 0, -accY);

        a = new byte[] { 69, 0, 0, 0, 0, 0, 0 };

        horizontalMovement = accX;
        verticalMovement = accY/2;

        //horizontalMovement = SimpleInput.GetAxis("HorizontalStick");
        //verticalMovement = -SimpleInput.GetAxis("VerticalStick")/2;
        //verticalMovement = slider.value / 20;

        generalSpeed = verticalMovement;

        leftSpeed = generalSpeed - Mathf.Abs(Mathf.Clamp(horizontalMovement, -1, 0));
        rightSpeed = generalSpeed - Mathf.Abs(Mathf.Clamp(horizontalMovement, 0, 1));

        generalArrow.transform.localScale = new Vector3(1, 1, generalSpeed);
        leftArrow.transform.localScale = new Vector3(1, 1, leftSpeed);
        rightArrow.transform.localScale = new Vector3(1, 1, rightSpeed);

        a[1] = (byte)(Mathf.Abs(leftSpeed) * 255);
        a[6] = (byte)(Mathf.Abs(rightSpeed) * 255);

        lPWMNum.text = a[1].ToString();
        rPWMNum.text = a[6].ToString();

        if (leftSpeed == 0) //FALTA CRIAR TOLERANCIA, zero é quase inalcançavel
        {
            a[2] = 0;
            a[3] = 0;

            lNum1.text = a[2].ToString();
            lNum2.text = a[3].ToString();
        }
        else if (leftSpeed < 0)
        {
            a[2] = 1;
            a[3] = 0;

            lNum1.text = a[2].ToString();
            lNum2.text = a[3].ToString();
        }
        else if (leftSpeed > 0)
        {
            a[2] = 0;
            a[3] = 1;

            lNum1.text = a[2].ToString();
            lNum2.text = a[3].ToString();
        }

        if (rightSpeed == 0) //FALTA CRIAR TOLERANCIA, zero é quase inalcançavel
        {
            a[4] = 0;
            a[5] = 0;

            rNum1.text = a[4].ToString();
            rNum2.text = a[5].ToString();
        }
        else if (rightSpeed < 0)
        {
            a[4] = 1;
            a[5] = 0;

            rNum1.text = a[4].ToString();
            rNum2.text = a[5].ToString();
        }
        else if (rightSpeed > 0)
        {
            a[4] = 0;
            a[5] = 1;

            rNum1.text = a[4].ToString();
            rNum2.text = a[5].ToString();
        }

        print("Mensagem Enviada: ");
        foreach (byte x in a)
        {
            print(x);
        }
    }

    byte[] a;

    IEnumerator SendDataToArduino()
    {
        if (a != null)
        {
            TCPClient.SendMessage(a);
            print("Valores a serem enviados: [" + a[1] + ", " + a[2] + "] [" + a[3] + ", " + a[4] + "]");
        }

        yield return new WaitForSeconds(0.1f);
        StartCoroutine("SendDataToArduino");
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public void RestartApplication()
    {
        Application.LoadLevel(0);
    }
}
