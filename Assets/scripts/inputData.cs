using UnityEngine;
using System.Collections;
using System.IO;

public class inputData : MonoBehaviour
{
    static int ld = 18;
    static int lm = 55;
    static int ls = 0;
    static int hrs = 900;
    static int da = 1;
    static int mt = 5;

    private string latDegree = ld.ToString();
    private string latMinute = lm.ToString();
    private string latSecond = ls.ToString();
    private string hours = hrs.ToString();
    private string Day = da.ToString();
    private string Month = mt.ToString();

    // Button
    static string buttonText = "Run";
	
	// Update is called once per frame
    void Update()
    {
    }
  
    void OnGUI()
    {
        // Latitude Fields
        GUI.Label(new Rect(140, 60, 120, 20), "Latitude:");

        GUI.Label(new Rect(140, 100, 120, 20), "Degree: ");
        latDegree = GUI.TextArea(new Rect(200, 100, 120, 20), latDegree, 50);

        GUI.Label(new Rect(140, 130, 120, 20), "Minute: ");
        latMinute = GUI.TextArea(new Rect(200, 130, 120, 20), latMinute, 50);
        
        GUI.Label(new Rect(140, 160, 120, 20), "Second: ");
        latSecond = GUI.TextArea(new Rect(200, 160, 120, 20), latSecond, 50);

        // Date
        GUI.Label(new Rect(400, 100, 120, 20), "Day: ");
        Day = GUI.TextArea(new Rect(480, 100, 120, 20), Day, 50);

        GUI.Label(new Rect(400, 130, 120, 20), "Month: ");
        Month = GUI.TextArea(new Rect(480, 130, 120, 20), Month, 50);

        // Hours
        GUI.Label(new Rect(400, 160, 120, 20), "Time (IST): ");
        hours = GUI.TextArea(new Rect(480, 160, 120, 20), hours, 50);

        if (GUI.Button(new Rect(200, 200, 120, 20), buttonText))
        {
            generateBuildings.latDeg = float.Parse(latDegree);
            generateBuildings.latMin = float.Parse(latMinute);
            generateBuildings.latSec = float.Parse(latSecond);
            generateBuildings.IST = float.Parse(hours);
            generateBuildings.day = int.Parse(Day);
            generateBuildings.month = int.Parse(Month);

            Application.LoadLevel(1);
        }

        if (GUI.Button(new Rect(200, 230, 120, 20), "Exit"))
        {
            Application.Quit();
        }
    }
}
