using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GyroManagerForCamera : MonoBehaviour
{
    public static GyroManagerForCamera Instance;

    bool gyroEnabled;
    Gyroscope gyro;
    Quaternion rot;
    Vector3 gyroscope;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Invoke("CameraPlacerByDistance", 3f);
        gyroEnabled = EnableGyro();
    }

    public void CameraPlacerByDistance()
    {
        //This is for editor
        if (Application.isEditor)
        {
            UIManager.Instance.AutoLoadLatLotAltPanel(UserManager.Instance.FindUser(UIManager.Instance.getUsername()));
        }
        User user = UserManager.Instance.FindUser(UIManager.Instance.getUsername());
        LatLonH latlon = new LatLonH((float)user.Longitude, (float)user.Latitude, (float)user.Altitude);
        Vector diff = CoordinateManager.Instance.ToWorldCoord(latlon);
        transform.parent.position = diff.toVector3();
        /*
        float latdif = (float)UserManager.Instance.FindUser(UIManager.Instance.getUsername()).Latitude * 100;
        float londif = (float)UserManager.Instance.FindUser(UIManager.Instance.getUsername()).Longitude * 100;
        float altdif = (float)UserManager.Instance.FindUser(UIManager.Instance.getUsername()).Altitude * 1;
        Vector diff = new Vector(londif, altdif, latdif);
        transform.parent.position = diff.toVector3();
        */
    }

    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;


            transform.parent.rotation = Quaternion.Euler(90f, 0f, 0f);
            rot = new Quaternion(-1, 0, 0, 0);

            return true;
        }
        return false;
    }

    private void Update()
    {
        if (gyroEnabled)
        {
            transform.localRotation = gyro.attitude * rot;
        }
    }
}
