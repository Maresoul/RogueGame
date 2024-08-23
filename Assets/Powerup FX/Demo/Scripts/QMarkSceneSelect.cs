using UnityEngine;
using UnityEngine.SceneManagement;

namespace QMarkFX {

public class QMarkSceneSelect : MonoBehaviour
{
	public bool GUIHide = false;
	public bool GUIHide2 = false;
	public bool GUIHide3 = false;
	
    public void LoadPowerupDemo01()
    {
        SceneManager.LoadScene("Powerup01");
    }
    public void LoadPowerupDemo02()
    {
        SceneManager.LoadScene("Powerup02");
    }
	public void LoadPowerupDemo03()
    {
        SceneManager.LoadScene("Powerup03");
    }
	public void LoadPowerupDemo04()
    {
        SceneManager.LoadScene("Powerup04");
    }
	public void LoadPowerupDemo05()
    {
        SceneManager.LoadScene("Powerup05");
    }
	public void LoadPowerupDemo06()
    {
        SceneManager.LoadScene("Powerup06");
    }
	public void LoadPowerupDemo07()
    {
        SceneManager.LoadScene("Powerup07");
    }

	void Update ()
	 {
 
     if(Input.GetKeyDown(KeyCode.J))
	 {
         GUIHide = !GUIHide;
     
         if (GUIHide)
		 {
             GameObject.Find("CanvasSceneSelect").GetComponent<Canvas> ().enabled = false;
         }
		 else
		 {
             GameObject.Find("CanvasSceneSelect").GetComponent<Canvas> ().enabled = true;
         }
     }
	      if(Input.GetKeyDown(KeyCode.K))
	 {
         GUIHide2 = !GUIHide2;
     
         if (GUIHide2)
		 {
             GameObject.Find("Canvas").GetComponent<Canvas> ().enabled = false;
         }
		 else
		 {
             GameObject.Find("Canvas").GetComponent<Canvas> ().enabled = true;
         }
     }
		if(Input.GetKeyDown(KeyCode.L))
	 {
         GUIHide3 = !GUIHide3;
     
         if (GUIHide3)
		 {
             GameObject.Find("CanvasTips").GetComponent<Canvas> ().enabled = false;
         }
		 else
		 {
             GameObject.Find("CanvasTips").GetComponent<Canvas> ().enabled = true;
         }
     }
}
}
}