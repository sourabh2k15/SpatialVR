using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

public class NewBehaviourScript : MonoBehaviour
{
    public int position = 0;
    public int samplerate = 44100;
    public float frequency = 440;
    public int x, y, z, d;
    public int screen_num = 0;

    public Button mybutton;
    public Text starttext;
    public Camera head;
    public GameObject capsule, capsule2, capsule3;
    public AudioSource asource;
    public GameObject s;

    public AudioClip clip;
    // Use this for initialization
    void Start() {
        //starttext = GameObject.Find("start").GetComponent<Text>();
        head      = GameObject.Find("Camera").GetComponent<Camera>();
        s    = GameObject.Find("cursor");

        //capsule   = GameObject.Find("Capsule");
        //capsule2  = GameObject.Find("Capsule1");
        //capsule3  = GameObject.Find("Capsule2");

        asource   = GameObject.Find("testaudio").GetComponent<AudioSource>();
        //InvokeRepeating("moveSource", 0.0f, 2.0f);

        Debug.Log("initiated");
    }

    void simulate3DAudio() {
        x = Random.Range(-50, 50);
        y = Random.Range(20, 50);
        z = Random.Range(-50, 50);
        d = (int)Random.Range(40, 100);
        //clip = AudioClip.Create("MySinusoid", samplerate * 1, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
        //AudioSource.PlayClipAtPoint(clip, new Vector3(x, y, z), 15);

        asource.transform.position = new Vector3(x, y, z);
        asource.Play();
        s.transform.position = new Vector3(x, y, z);

        //capsule2.transform.position = new Vector3(x + d, y + d, z + d);
        //capsule.transform.position = new Vector3(x, y, z);
        //capsule3.transform.position = new Vector3(x - d, y - d, z - d);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = new Vector3(0.2f, 0.2f, 0.5f);
        //cursor.transform.position = Vector3.Lerp(cursor.transform.position, transform.position, Time.deltaTime * 2.0f);

        //cursor.transform.position = transform.position + temp;
        OVRInput.Update();

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            ClickHandler();
        }
    }

    void ClickHandler()
    {
 
        if (screen_num == 0)
        {
            Debug.Log("starting game!");
            Destroy(starttext);
            startGame();
            screen_num++;
        }
        else if (screen_num == 1) {
            Debug.Log("sound guess made");
            calculateError();
        }
    }

    void calculateError() {
        Vector3 headPosition = transform.position;
        Vector3 audioPosition = new Vector3(x, y, z);

        float radius = Vector3.Distance(headPosition, audioPosition);

        Ray guessedray = new Ray(headPosition, transform.forward);
        Vector3 nearest = guessedray.GetPoint(radius);

        Vector3 guessed = nearest - headPosition;
        Vector3 correct = audioPosition - headPosition;
        float error = Vector3.Angle(guessed, correct);

        Debug.Log(error);

        // GameObject lightSource = new GameObject("The Light");
        // Light lightComp = lightSource.AddComponent<Light>();
        // lightComp.color = Color.blue;
        // lightSource.transform.position = new Vector3(x, y, z);

        simulate3DAudio();
    }

    void startGame() {
        simulate3DAudio();
    }

    void moveSource()
    {
        x += 10;
        //Debug.Log("source moving");
    }

    void OnAudioRead(float[] data)
    {
        int count = 0;
        while (count < data.Length)
        {
            data[count] = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * frequency * position / samplerate));
            position++;
            count++;
        }
    }

    void OnAudioSetPosition(int newPosition)
    {
        position = newPosition;
    }

}
