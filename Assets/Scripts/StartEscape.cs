using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartEscape : MonoBehaviour
{
    private EscapeButton Esc;
    void Start()
    {
        Esc = GetComponent<EscapeButton>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Esc.SettingAnim != null)
            {
                if (Esc.SettingAnim.GetBool("settingIn"))
                {
                    Esc.SettingClose();
                }
                else { Application.Quit(); }
            }
            else { Application.Quit(); }
        }
    }
}
