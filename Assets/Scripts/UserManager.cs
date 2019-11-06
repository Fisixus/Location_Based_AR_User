using Newtonsoft.Json;
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

    private void Awake()
    {
        Instance = this;
    }

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

    //This will be temporary, because need to cognito settings on service
    public void LoginControl()
    {
        /*
        usernameDATA = UsernameAndPasswordPanel.transform.Find("UsernameDATA").GetComponent<TMP_InputField>().text.Trim();

        passwordDATA = UsernameAndPasswordPanel.transform.Find("PasswordDATA").GetComponent<TMP_InputField>().text;
        */
        usernameDATA = "utku@etu.edu.tr";
        passwordDATA = "dasddasdf";

        User dataUser = null;
        string data = WebServiceManager.Instance.getAllUserData();
        if (data.Equals(""))
        {
            Debug.Log("Waiting connection!");
            return;
        }

        List<User> allUsers = JsonConvert.DeserializeObject<List<User>>(data);
        foreach (User user in allUsers)
        {
            if ((user.Username.ToLower().Equals(usernameDATA.ToLower())) && (user.Password.Equals(passwordDATA)))
            {
                //TODO dataUser should be list of selected users
                dataUser = user;
                break;
            }
        }
        if(dataUser != null)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("Username", usernameDATA);
            SceneManager.LoadScene(1);            
        }
        else
        {
            Debug.Log("Username or password is wrong!");
        }
    }

    public void ExitBodyScene()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }

}
