using UnityEngine;
using System.Collections;
using System.IO;

public class generateBuildings : MonoBehaviour 
{
    // Direction Variables
    public GameObject buildingPrefab;
    private GameObject MainCamera;
    private float X;
    private float Z;
    public static float ScaleX;
    public static float ScaleY;
    public static float ScaleZ;
    private int i;

    public int LevelArea = 10;

    // Mouse Scroll Variables
    public int ScrollArea = 10;
    public int ScrollSpeed = 10;
    public int DragSpeed = 20;

    public int ZoomSpeed = 25;
    public int ZoomMin = 20;
    public int ZoomMax = 100;

    public int PanSpeed = 50;
    public int PanAngleMin = 25;
    public int PanAngleMax = 80;

    // Views variables
    private string topViewButton = "Top View";
    private string perspectiveViewButton = "3D View";
    private string exit = "Exit";
    private string show = "Output";
    
    // File Read Variables
    string fileInput = "input.txt";
    string fileOutput = "output.rtf";
    public string[] lines;// = new string[100];
    public string[] coordinates;

    // Sun Variables
    GameObject sun;
    float sunX;
    float sunY; 

    // Static Variables
    //Input
    public static float latDeg;
    public static float latMin;
    public static float latSec;
    public static float IST;
    //public static int n = 105;
    public static int day;
    public static int month;
    //Output
    private string dailyGlobal;
    private string dailyDiffuse;
    private string hourlyGlobal;
    private string hourlyDiffuse;
    private string length;
    private string sunrise;
    private string hour;
    private string zenith;
    private string azimuth;
    private string outputData;
    float sunRiseTime;
    float sunSetTime;

    // GUI
    private GUIStyle style;

    // Degree
    private float degree = (float)Mathf.PI / 180;

    // dates
    private int[] dates = {17, 16, 16, 15, 15, 11, 17, 16, 15, 15, 14, 10};

    // Initialization
	void Start ()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        // Reading data from a text file(buildingsData.txt)
        StreamReader sr = new StreamReader(Application.dataPath + "/" + fileInput);
        string fileContents = sr.ReadToEnd();
        sr.Close();

        // Stores data in 'line' array
        lines = fileContents.Split("\n"[0]);
        
        // Splitting lines 
        for (i = 0; i < (lines.Length); i++)
        {
            coordinates = lines[i].Split(' ');
            
            X = float.Parse(coordinates[0]);
            Z = float.Parse(coordinates[1]);
            ScaleX = float.Parse(coordinates[2]);
            ScaleY = float.Parse(coordinates[3]);
            ScaleZ = float.Parse(coordinates[4]);

            // Creating Bulidings by instantiating buildings models
            GameObject clone;
            clone = (GameObject)Instantiate(buildingPrefab, transform.position, transform.rotation);
            clone.transform.localScale = new Vector3(ScaleX, ScaleY, ScaleZ);
            clone.transform.position = new Vector3(X, ScaleY / 2, Z);

            // Referencing sun and initializing direction of sun
            if (GameObject.FindGameObjectWithTag("sun"))
            {
                sun = GameObject.FindGameObjectWithTag("sun");
                sun.transform.eulerAngles = (new Vector3(90, 0, 0));

                int avgDayNo = daysNo(dates[month-1], month);

                int c0 = daysNo(day, month);
                float c1 = latAngle(latDeg, latMin, latSec);
                float c2 = decAngle(c0);
                float c3 = sunriseAngle(latAngle(latDeg, latMin, latSec), decAngle(c0));
                float c4 = dayLength(sunriseAngle(latAngle(latDeg, latMin, latSec), decAngle(c0)));
                float c5 = dailyGlobalRadClearDay(avgDayNo, c1, c2, c3, c4);
                float c6 = dailyGlobalRadLoc(c5, c4, 9.5f);
                float c7 = dailyDiffuseRadLoc(c6, c5);
                float c8 = hourAngle(IST, c0, c1);
                float c9 = hourlyGlobalMonthAvg(avgDayNo, c1, c2, c8);
                float c10 = hourlyGlobalRadLoc (c8, c3, c6, c5, c9);
                float c11 = hourlyDiffuseRadLoc(c8, c3, c6, c7, c5, c9);
                float sunsetAngle = sunriseAngle(latAngle(latDeg, latMin, latSec), -decAngle(c0));

                sunRiseTime = 12 - (c3 / 15);
                sunSetTime = 12 - (sunsetAngle / 15) + 12;

                dailyGlobal = c6.ToString();
                dailyDiffuse = c7.ToString();
                hourlyGlobal = c10.ToString();
                hourlyDiffuse = c11.ToString();
                length = c4.ToString();
                hour = c8.ToString();
                sunrise = c3.ToString();
     
                // Changing direction of sun according to time
                sunY = zenithAngle(c2, c1, c8);
                sunX = azimuthAngle(sunY, c1, c2);

                if (c8 >= 0)
                {
                    sunX = sunX + 90;
                }

                sun.transform.eulerAngles = (new Vector3(sunY+90, sunX, 0));

                zenith = sunY.ToString();
                azimuth = sunX.ToString();
            }
        }
	}

    // Updating once per frame
    void Update()
    {
        // Mouse Scrolling
        var translation = Vector3.zero;
        //var rotation = Vector3.zero;

        // Move camera with mouse
        if (Input.GetButton("Fire1")) // MMB
        {
            // Hold button and drag camera around
            translation -= new Vector3(Input.GetAxis("Mouse X") * DragSpeed * Time.deltaTime, 0,
                               Input.GetAxis("Mouse Y") * DragSpeed * Time.deltaTime);
        }

        translation += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        camera.transform.position += translation;
    }

    // Graphical Interface
    void OnGUI()
    {
        outputData = "Daily Global: " + "\n " + dailyGlobal + "\n\n"
            + "Daily Diffused: " + "\n " + dailyDiffuse + "\n\n" + "Hourly Global: "
                + "\n " + hourlyGlobal + "\n\n" + "Hourly Diffused: " + "\n " + hourlyDiffuse
                + "\n\n" + "Day Length: " + "\n " + length + "\n\n" + "Hour Angle: " + "\n "
                + hour + "\n\n" + "Sunrise Time: " + "\n " + sunRiseTime + "\n\n"
                + "Sunset Time: " + "\n " + sunSetTime + "\n\n"
                + "Zenith Angle: " + "\n " + zenith + "\n\n" + "Azimuth Angle: " + "\n " + azimuth;

        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        if (GUI.Button(new Rect(10, 10, 120, 20), topViewButton))
        {
            MainCamera.transform.position = new Vector3(0, 30, 0);
            MainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        if (GUI.Button(new Rect(10, 30, 120, 20), perspectiveViewButton))
        {
            MainCamera.transform.position = new Vector3(-10.6f, 9f, -13.7f);
            MainCamera.transform.rotation = Quaternion.Euler(-1.1f, 43.7f, 0);
        }

        if (GUI.Button(new Rect(10, 50, 120, 20), "Save Data"))
        {
            StreamWriter sw = new StreamWriter(Application.dataPath + "/" + fileOutput);
            sw.AutoFlush = true;
            // while (true)
            {
                sw.Write(outputData);
            }
        }

        if (GUI.Button(new Rect(10, 70, 120, 20), "Back"))
        {
            Application.LoadLevel(0);
        }
      
        GUI.TextArea(new Rect(10, 100, 200, 450), outputData, 400);
    }

    // No. of days Calculator
    int daysNo(int day, int month)
    {
        int n = 0;
        int[] monthDays = {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};

        for (int i = 0; i < (month-1); i++)
        {
            n = n + monthDays[i];
        }
       
        return n + day;
    }

    // Declination Angle Function
    float decAngle (int n)
    {
        float theta = 23.45f * Mathf.Sin(((float)360 / 365 * (284 + n)) * Mathf.Deg2Rad);

        return theta;
    }

    // Latitude Angle Function
    float latAngle (float deg, float min, float sec)
    {
        float theta = deg + (float)min/60 + (float)sec/3600; 
        
        return theta;
    }

    // Hour Angle Function
    float hourAngle(float IST, int n, float latAngle)
    {
        float B = (((float)360 / 365) * (n - 1))*degree;
        float E = 229.18f * (0.000075f + 0.001868f * Mathf.Cos(B) - 
            0.032077f * Mathf.Sin(B) - 0.014615f * Mathf.Cos(2 * B) - 0.04089f * Mathf.Sin(2 * B));
        float LAT = (IST - 4 * (82.50f - latAngle) + E)/100;
        float theta = 15 * (12 - (float)IST/100);

        return theta;
    }

    // Zenithh Angle Function
    float zenithAngle(float decAngle, float latAngle, float hourAngle)
    {
        float theta = Mathf.Acos((Mathf.Sin(latAngle*Mathf.Deg2Rad)*Mathf.Sin(decAngle*Mathf.Deg2Rad)
             + Mathf.Cos(latAngle * Mathf.Deg2Rad)*Mathf.Cos(decAngle * Mathf.Deg2Rad)*
            Mathf.Cos(hourAngle * Mathf.Deg2Rad))) * Mathf.Rad2Deg;
        
        return theta;
    }

    float azimuthAngle (float zenithAngle, float latAngle, float decAngle)
    {
        float theta = Mathf.Acos((Mathf.Cos(zenithAngle * Mathf.Deg2Rad) * Mathf.Sin(latAngle * Mathf.Deg2Rad)
            - Mathf.Sin(decAngle * Mathf.Deg2Rad)) / (Mathf.Sin(zenithAngle * Mathf.Deg2Rad)
            * Mathf.Cos(latAngle * Mathf.Deg2Rad)))* Mathf.Rad2Deg;

        return theta;
    }

    float sunriseAngle(float latAngle, float decAngle)
    {
        float theta =  Mathf.Acos(-Mathf.Tan(latAngle * degree)*Mathf.Tan(decAngle*degree))/degree;

        return theta;
    }

    float dayLength(float sunriseAngle)
    {
        float length = ((float)2 / 15) * sunriseAngle;

        return length;
    }

    float dailyGlobalRadClearDay (int n, float latAngle, float decAngle, float sunriseAngle, float dayLength)
    {
        float Isc = 1.367f;

        float Ho = ((float)24 / Mathf.PI) * Isc * 3600 * (1 + (float)0.033f * Mathf.Cos(((float)360 / 365) * n * degree))
            * ((float)sunriseAngle * degree * Mathf.Sin(latAngle * degree) * Mathf.Sin(decAngle * degree) +
                (float)Mathf.Cos(latAngle * degree) * Mathf.Cos(decAngle * degree) * Mathf.Sin(sunriseAngle * degree));

        return Ho;
    }

    float dailyGlobalRadLoc (float dailyGlobalRadClearDay, float dayLength, float sunshineHours)
    {
        float a = 0.28f;
        float b = 0.48f;
        
        float Hg = dailyGlobalRadClearDay*(a + b*((float)sunshineHours/dayLength));

        return Hg;
    }

    float dailyDiffuseRadLoc (float dailyGlobalRadLoc, float dailyGlobalRadClearDay)
    {
        float Hd = dailyGlobalRadLoc * (1.411f - 1.696f * ((float)dailyGlobalRadLoc / dailyGlobalRadClearDay));

        return Hd;
    }

    float hourlyGlobalMonthAvg(float n, float latAngle, float decAngle, float hourAngle)
    {
        float Isc = 1.367f;

        float Io = Isc * 3600 * (1 + (float)0.033f * Mathf.Cos(((float)360 / 365) * n * degree))
            * (Mathf.Sin(latAngle * degree) * Mathf.Sin(decAngle * degree)
            + Mathf.Cos(latAngle * degree) * Mathf.Cos(decAngle * degree) * Mathf.Cos(hourAngle * degree));

        return Io;
    }

    float hourlyGlobalRadLoc (float hourAngle, float sunriseAngle, float Hg, float Ho, float Io)
    {
        float a = 0.409f + 0.5016f * Mathf.Sin((sunriseAngle - 60) * degree);
        float b = 0.6609f - 0.4767f * Mathf.Sin((sunriseAngle - 60) * degree);
        float fc = a + 0.5f * b * (((((float)Mathf.PI * sunriseAngle) / 180)
            - Mathf.Sin(sunriseAngle * degree) * Mathf.Cos(sunriseAngle * degree)) 
            / (Mathf.Sin(sunriseAngle*degree) - (((float)Mathf.PI*sunriseAngle)/180)
            *Mathf.Cos(sunriseAngle*degree)));

        float Ig = Hg * ((float)Io / Ho) * (a + b * (Mathf.Cos(hourAngle * degree))) / fc;

        return Ig;
    }

    float hourlyDiffuseRadLoc(float hourAngle, float sunriseAngle, float Hg, float Hd, float Ho, float Io)
    {
        float k = (float)Hd / Hg;
        float a;
        float b;
        float Id;

        if (k >= 0.1 && k <= 0.7)
        {
            a = 0.4922f + (float)0.27f / k;
            b = 2 * (1 - a) * (Mathf.Sin(sunriseAngle * degree) - ((float)sunriseAngle*degree) * Mathf.Cos(sunriseAngle*degree))
            / (((float)sunriseAngle*degree) - 0.5f * Mathf.Sin((float)2 * sunriseAngle*degree));
            Id = Hd * ((float)Io / Ho) * (a + b * Mathf.Cos(hourAngle * degree));

            return Id;
        }

        else if (k > 7 && k <= 0.9f)
        {
            a = 0.76f + (float)0.113f / k;
            b = 2 * (1 - a) * (Mathf.Sin(sunriseAngle * degree) - ((float)sunriseAngle / degree) * Mathf.Cos(sunriseAngle))
            / (((float)sunriseAngle*degree) - 0.5f * Mathf.Sin((float)2 * sunriseAngle / degree));
            Id = Hd * ((float)Io / Ho) * (a + b * Mathf.Cos(hourAngle * degree));

            return Id;
        }

        else
        {
            print("k = ?");
            return 0;
        }  
    }
}



