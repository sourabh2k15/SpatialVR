using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    public bool gameStarted = false, blah = false, blah2 = false;
    public int position = 0;
    public int samplerate = 44100;
    public float frequency = 440;
    public int x, y, z, d;
    public int screen_num = 0;
    public int threshold = 10;
    public int avgTime = 0;
    public int uid; 

    public Button mybutton, existinguserbtn, newuserbtn, startgame;
    public Text starttext;
    public Camera head;
    public AudioSource asource;
    public GameObject s;
    public Canvas canvas;
    public Button gazetracker;
    public InputField uuid;
    public int userstatus = -1;

    public long prev, next;
    public int plays = 50;
    public int plays2;

    public Text score;
    string scoretext = "";
    int acc = 0;

    public Text accuracy;
    public string accuracytext = "";

    public AudioClip clip;
    // Use this for initialization
    void Start() {
        head      = GameObject.Find("Camera").GetComponent<Camera>();
        s         = GameObject.Find("cursor");
        asource   = GameObject.Find("testaudio").GetComponent<AudioSource>();
        canvas    = GameObject.Find("Canvas").GetComponent<Canvas>();
        gazetracker = GameObject.Find("gazetracker").GetComponent<Button>();
        existinguserbtn = GameObject.Find("existing").GetComponent<Button>();
        newuserbtn = GameObject.Find("newuser").GetComponent<Button>();
        startgame = GameObject.Find("startgame").GetComponent<Button>();
        startgame.gameObject.SetActive(false);

        uuid = GameObject.Find("uuid").GetComponent<InputField>();
        uuid.gameObject.SetActive(false);

        plays     = 20;
        plays2    = plays;

        score = GameObject.Find("score").GetComponent<Text>();
        //score.color = new Color(0, 0, 0);

        var colors = gazetracker.colors;
        colors.normalColor = Color.black;

        gazetracker.colors = colors;

        scoretext = "This game helps improve spatial hearing abilities. \nYour challenge is to locate where the sound is coming \n from and click the moon in that direction. " +
            "\n Press left key on remote if you're a returning user.\n Press right key if you are a new user.";
        accuracy = GameObject.Find("accuracy").GetComponent<Text>();
        score.text = scoretext;

        accuracytext = "";
        accuracy.text = accuracytext;
        Debug.Log("initiated");
    }

    void getUUID(string s) {
        Debug.Log(s);
    }
    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            if (blah) getUUID();
            else if (blah2) newuserPlay();
            else if(gameStarted) ClickHandler();
        }

        if (OVRInput.GetDown(OVRInput.Button.DpadUp)) {
            Debug.Log("dpad up clicked!!");
        }

        if (OVRInput.GetDown(OVRInput.Button.DpadLeft))
        {
            if(userstatus == -1)
            {
                Debug.Log("dpad left clicked!!");
                Debug.Log("existing user");

                userstatus = 1;
                nextScreen();
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.DpadRight))
        {
            if (userstatus == -1) {
                Debug.Log("dpad right clicked!!");
                Debug.Log("new user");
                userstatus = 0;
                nextScreen();
            }
        }
    }

    void getUUID() {
        uuid.Select();
        blah = false;
        var uuidse = new InputField.SubmitEvent();
        uuidse.AddListener(getUUID2);
        uuid.onEndEdit = uuidse;
    }

    void getUUID2(string _uid) {
        uid = int.Parse(_uid);
        uuid.gameObject.SetActive(false);
        gameStarted = true;
        var colors = gazetracker.colors;
        colors.normalColor = Color.white;
        startgame.gameObject.SetActive(false);
        scoretext = "Start !";
        score.text = scoretext;
        gazetracker.colors = colors;
        ClickHandler();
        Debug.Log(uid);
    }

    void newuserPlay() {
        blah2 = false;
        gameStarted = true;
        ClickHandler();
    }

    void nextScreen() {
        if (screen_num == 0)
        {
            score.color = new Color(0, 0, 0);
            //SceneManager.LoadScene("init");
            newuserbtn.gameObject.SetActive(false);
            existinguserbtn.gameObject.SetActive(false);

            score.color = new Color(255, 255, 255);

            if (userstatus == 1)
            {
                scoretext = "Please Enter Your Unique ID : ";
                uuid.gameObject.SetActive(true);
                startgame.gameObject.SetActive(true);
                blah = true;
            }
            else {
                uid = generateID();
                scoretext = "This would be your Unique ID : " + uid + ". Press Center to continue";
                blah2 = true;
                var colors = gazetracker.colors;
                colors.normalColor = Color.black;

                gazetracker.colors = colors;
                //uuid.gameObject.SetActive(false);
            }
            startgame.gameObject.SetActive(true);
            score.text = scoretext;
            screen_num++;
        }
        else if (screen_num == 1) {

        }
    }
    void ClickHandler()
    {
 
        if (screen_num == 1)
        {
            Debug.Log("starting game!");
            Destroy(starttext);
            startGame();
            screen_num++;
            prev = UnixTimeNow();
            scoretext = "Chances Left : " + plays + " \n Time Taken   : " + (0) + " s";
            score.text = scoretext;
        }
        else if (screen_num == 2) {
            calculateError();
        }
    }

    void simulate3DAudio()
    {
        x = Random.Range(-50, 50);
        y = Random.Range(20, 50);
        z = Random.Range(-50, 50);

        asource.transform.position = new Vector3(x, y, z);
        asource.Play();

        s.transform.position = new Vector3(x, y, z);
    }

    void calculateError() {
        if (plays > 0)
        {
            Vector3 headPosition = transform.position;
            Vector3 audioPosition = new Vector3(x, y, z);

            float radius = Vector3.Distance(headPosition, audioPosition);

            Ray guessedray = new Ray(headPosition, transform.forward);
            Vector3 nearest = guessedray.GetPoint(radius);

            Vector3 guessed = nearest - headPosition;
            Vector3 correct = audioPosition - headPosition;

            float anglerror = Vector3.Angle(guessed, correct);
            float disterror = Vector3.Distance(nearest, audioPosition);

            next = UnixTimeNow();
            long timeTaken = next - prev;

            if (plays != plays2 && plays % 5 == 0)
            {
                avgTime = avgTime / 5;
                makeHttpPost(avgTime, uid, (int)next);
                avgTime = 0;
            }
            else
            {
                avgTime += (int)timeTaken;
                prev = next;
            }


            if (disterror <= threshold)
            {
                Debug.Log("correct");
                acc++;
                accuracytext = "It's a Hit !! \n";
            }
            else
            {
                Debug.Log("wrong");
                Debug.Log("angular error: " + anglerror);
                Debug.Log("distance error: " + disterror);
                accuracytext = "Miss :( \n";
            }

            accuracytext += ("accuracy : " + acc + " / " + plays2);
            accuracy.text = accuracytext;

            Debug.Log("time: " + timeTaken);
            Debug.Log(plays);

            plays--;
            scoretext = "Chances Left : " + plays + " \n Time Taken   : " + ((float)timeTaken / 1000) + " s";
            score.text = scoretext;

            simulate3DAudio();
        }
        else {
            scoretext  = "Game Over";
            score.text = scoretext;
        }
    }

    void startGame() {
        startgame.gameObject.SetActive(false);
        scoretext = "Start !";
        score.text = scoretext;
        var colors = gazetracker.colors;
        colors.normalColor = Color.white;

        gazetracker.colors = colors;
        simulate3DAudio();
    }

    /* Utilitiy functions */

    public long UnixTimeNow()
    {
        var timeSpan = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0));
        return (long)timeSpan.TotalMilliseconds;
    }

    public int generateID()
    {
        return Random.Range(1,1000);
    }

    void makeHttpPost(int a, int b, int c)
    {
        Debug.Log("sending server : " + a +" "+ b +" "+ c);
        WWWForm form = new WWWForm();
        form.AddField("avgTime", a);
        form.AddField("uid", b);
        form.AddField("time", c);

        WWW postRequest = new WWW("http://38512291.ngrok.io/api/results", form);
    }

}
