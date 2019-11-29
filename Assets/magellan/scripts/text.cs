using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;



public class text : MonoBehaviour
{


    [Serializable]
    public class ListItem
    {
        public Values[] Value;
    }


    [Serializable]
    public class Values
    {
        public Text humidity;
        public Text temperature;
        public Text pressure;
        public Text acceleration;
        public float acceleration_x;
        public float acceleration_y;
        public float acceleration_z;
        public float battery;
        public DateTime time;
        public string name;
    }

    public double seconds = 1;
    public double timer = 0;
    private Values _initialValues = new Values();
    private const string Endpoint = "http://ad4764a3.ngrok.io/data";
    private string _response;
    private int _cont;

    IEnumerator Upload()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        UnityWebRequest www = UnityWebRequest.Get(Endpoint);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            _response = www.downloadHandler.text;
            _response = _response.Substring(22, _response.Length - 23);
            Debug.Log(_response);
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
