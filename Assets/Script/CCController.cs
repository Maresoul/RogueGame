using System;

using DigitalRubyShared;

using UnityEngine;
using UnityEngine.UI;

public class CCController : MonoBehaviour
{
    FingersJoystickScript joystick;
    Image process;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        joystick=transform.Find("FingersJoystickPrefab").GetComponent<FingersJoystickScript>();
        CCControllerEvent.GetJoystick = GetJoystick;

        transform.Find("NtkButton").GetComponent<Button>().onClick.AddListener(OnNtkButton);
        transform.Find("NtkButton_1").GetComponent<Button>().onClick.AddListener(OnNtkButton1);
        transform.Find("NtkButton_2").GetComponent<Button>().onClick.AddListener(OnNtkButton2);
        transform.Find("NtkButton_3").GetComponent<Button>().onClick.AddListener(OnNtkButton3);
        transform.Find("NtkButton_4").GetComponent<Button>().onClick.AddListener(OnNtkButton4);

        process=transform.Find("Time/Process").GetComponent<Image>();
        process.fillAmount = 1;
    }

    private void Update()
    {
        process.fillAmount =Mathf.Clamp(1 - Time.time / 100,0,1);
    }

    private void OnNtkButton()
    {
        CCControllerEvent.NtkButtonOnClick?.Invoke(0);
    }

    private void OnNtkButton1()
    {
        CCControllerEvent.NtkButtonOnClick?.Invoke(1);
    }

    private void OnNtkButton2()
    {
        CCControllerEvent.NtkButtonOnClick?.Invoke(2);
    }

    private void OnNtkButton3()
    {
        CCControllerEvent.NtkButtonOnClick?.Invoke(3);
    }


    private void OnNtkButton4()
    {
        CCControllerEvent.NtkButtonOnClick?.Invoke(4);
    }

    internal Vector2 GetJoystick()
    {
        if (joystick != null)
        {
            return joystick.CurrentAmount;
        }
        return Vector2.zero;
    }

}


public class CCControllerEvent
{
    public static Func<Vector2> GetJoystick;
    public static Action<int> NtkButtonOnClick;
}