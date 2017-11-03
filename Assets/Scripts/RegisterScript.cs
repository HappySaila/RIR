using UnityEngine;
using UnityEngine.UI;

public class RegisterScript : APIScript {

    public InputField username;
    public InputField password;
    public InputField confirmPassword;
    public Text error;
    public GameObject toHide;
    public GameObject toShow;
    public void buttonPressed()
    {
        //toShow.SetActive(true);
        //toHide.SetActive(false);
        if (string.IsNullOrEmpty(username.text) || string.IsNullOrEmpty(password.text) || string.IsNullOrEmpty(confirmPassword.text))
        {
            error.text = "no fields can be blank";
            return;
        }
        if (!password.text.Equals(confirmPassword.text))
        {
            error.text = "passwords must match";
            return;
        }
        bool registered = RegisterUser(username.text,password.text);
        Debug.LogFormat("registered {0}", registered);

        if (registered){
			UIManager.instance.loggedIn();
            GlobalVariables.instance.me.Name = username.text;
            Debug.LogFormat("username: {0}", username.text);
        } else {
            //throw error
        }
    }
}
