using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance;

    public GameObject UsernameAndPasswordPanel;

    string usernameDATA = " ";
    string passwordDATA = " ";

    User onlineUser = null;
    string locationStatus;
    bool isLocationServiceActive = false;
    int maxWait = 30;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Invoke("CallUserSymbols", .1f);
    }

    private void CallUserSymbols()
    {
        WebServiceManager.Instance.GetAllUsers();
    }

    //Finds user uuid by username
    public string FindUserUUIDbyUsername(string symbolOwnerName)
    {
        string userUUID = string.Empty;
        string data = WebServiceManager.Instance.getAllUserData();
        List<User> allUsers = JsonConvert.DeserializeObject<List<User>>(data);
        for (int i = 0; i < allUsers.Count; i++)
        {
            if (allUsers[i].Username.ToLower().Trim().Equals(symbolOwnerName.ToLower().Trim()))
            {
                userUUID = allUsers[i].getUUID;
            }
        }
        return userUUID;
    }

    //Finds username by user uuid
    public string FindUsernamebyUserUUID(string userUUID)
    {
        string username = string.Empty;
        string data = WebServiceManager.Instance.getAllUserData();
        List<User> allUsers = JsonConvert.DeserializeObject<List<User>>(data);
        for (int i = 0; i < allUsers.Count; i++)
        {
            if (allUsers[i].getUUID.Equals(userUUID))
            {
                username = allUsers[i].Username;
            }
        }
        return username;
    }

    //Finds User by name or uuid
    public User FindUser(string info)
    {
        User user = null;
        string data = WebServiceManager.Instance.getAllUserData();
        List<User> allUsers = JsonConvert.DeserializeObject<List<User>>(data);
        for (int i = 0; i < allUsers.Count; i++)
        {
            if (allUsers[i].getUUID.Equals(info) || allUsers[i].Username.ToLower().Equals(info.ToLower()))
            {
                user = allUsers[i];
            }
        }
        return user;
    }

    public List<string> GetUsersSymbolNames(User dataUser)
    {
        string data = WebServiceManager.Instance.getAllSymbolsData();
        List<Symbol> allSymbols = JsonConvert.DeserializeObject<List<Symbol>>(data);
        List<string> userSymbolNames = new List<string>();

        foreach (Symbol s in allSymbols)
        {
            if (s.UserUUID.Equals(dataUser.getUUID))
            {
                userSymbolNames.Add(s.SymbolName);
            }
        }
        return userSymbolNames;
    }

    //TODO This will be temporary, because there need to cognito settings on service
    public void LoginControl()
    {
        /*
        usernameDATA = UsernameAndPasswordPanel.transform.Find("UsernameDATA").GetComponent<TMP_InputField>().text.Trim();

        passwordDATA = UsernameAndPasswordPanel.transform.Find("PasswordDATA").GetComponent<TMP_InputField>().text;
        */
        usernameDATA = "utku@etu.edu.tr";
        passwordDATA = "dasddasdf";

        onlineUser = null;
        string data = WebServiceManager.Instance.getAllUserData();
        if (data.Equals(""))
        {
            Debug.Log("Waiting connection!");
            return;
        }

        List<User> allUsers = JsonConvert.DeserializeObject<List<User>>(data);
        foreach (User user in allUsers)
        {
            if(user.Role != Role.Deleted)
            {
                if ((user.Username.ToLower().Equals(usernameDATA.ToLower())) && (user.Password.Equals(passwordDATA)))
                {
                    onlineUser = user;
                    break;
                }
            }
        }
        if(onlineUser != null)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("Username", usernameDATA);
            StartCoroutine(ControlLocationService());
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.Log("Username or password is wrong!");
        }
        // Stop service if there is no need to query location updates continuously
        //Input.location.Stop();
    }

    IEnumerator ControlLocationService()
    {
        //Check whether is in editor
        if (Application.isEditor)
        {            
            isLocationServiceActive = false;
            locationStatus = "Editor Mode.";
            yield break;
        }
        // Check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            isLocationServiceActive = false;
            locationStatus = "Device Location Service is Inactive.";
            CancelInvoke("LocationUpdater");
            yield break;
        }

        // Start service before querying location
        locationStatus = "Location Service is Started.";
        Input.location.Start(5, 5);

        // Wait until service initializes
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 30 seconds
        if (maxWait < 1)
        {
            isLocationServiceActive = false;
            locationStatus = "Location Service TimeOut!";

            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            isLocationServiceActive = false;
            locationStatus = "Location Service Failed!";
            CancelInvoke("LocationUpdater");
            yield break;
        }
        else
        {
            isLocationServiceActive = true;
            InvokeRepeating("LocationUpdater", 3, 3);
        }
    }

    public void LocationUpdater()
    {
        //locationStatus = "Location Service Active..";
        if(onlineUser != null)
        {
            onlineUser.Latitude = (decimal)Input.location.lastData.latitude;
            onlineUser.Longitude = (decimal)Input.location.lastData.longitude;
            onlineUser.Altitude = (decimal)Input.location.lastData.altitude;
            UIManager.Instance.AutoLoadLatLotAltPanel(onlineUser);
            GyroManagerForCamera.Instance.CameraPlacerByDistance();
            WebServiceManager.Instance.UpdateUser(onlineUser);
        }

        //user.horizontalAcc = Input.location.lastData.horizontalAccuracy;
        //user.timeStamp = Input.location.lastData.timestamp;
    }

    public void ExitBodyScene()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }

    private void LateUpdate()
    {
        if(onlineUser != null && isLocationServiceActive)
        {
            onlineUser.Latitude = (decimal)Input.location.lastData.latitude;
            onlineUser.Longitude = (decimal)Input.location.lastData.longitude;
            onlineUser.Altitude = (decimal)Input.location.lastData.altitude;
            UIManager.Instance.AutoLoadLatLotAltPanel(onlineUser);
            GyroManagerForCamera.Instance.CameraPlacerByDistance();
        }
    }

}
