using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class movement : MonoBehaviour
{
    [Serializable]
    public class ListItem
    {
        public Values[] Value;
    }


    [Serializable]
    public class Values
    {
        public float humidity;
        public float temperature;
        public float pressure;
        public float acceleration;
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
    private const string Endpoint = "http://1f6b3b7e.ngrok.io/data";
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

   



    // Update is called once per frame
    private void Rotate(Values values)
    {
        var realValues = new Values();
        float moveSpeed = 0.01F;
        
        

            if (values.acceleration_y < 0)

                realValues.acceleration_y = values.acceleration_y + _initialValues.acceleration_y;
            else
                realValues.acceleration_y = values.acceleration_y - _initialValues.acceleration_y;
            if (values.acceleration_z < 0)
                realValues.acceleration_z = values.acceleration_z + _initialValues.acceleration_z;
            else
                realValues.acceleration_z = values.acceleration_z - _initialValues.acceleration_z;

            float AngleRad = Mathf.Atan2(values.acceleration_y - _initialValues.acceleration_y, values.acceleration_x - _initialValues.acceleration_x);
            // Get Angle in Degrees
            Debug.Log("Angle Degree" + AngleRad);
            float angle = (180 / Mathf.PI) * AngleRad;
            Debug.Log("Angle " + angle);

            if (angle < -5.0F)
            {
                print("turn left");
            }
            else if (angle > 5.0F)
            {
                print("turn right");
            }
            else
            {
                print("forward");
            }

           transform.rotation=Quaternion.Euler(new Vector3(0, values.acceleration_x, values.acceleration_y*angle*moveSpeed));





    }
    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= seconds)
        {
            StartCoroutine(Upload());
            timer = 0;
        }

        Values jsonObj = new Values();
        if (_response != null)
        {
            jsonObj = JsonUtility.FromJson<Values>(_response);
            if (_cont == 0)
            {
                _initialValues = jsonObj;
            }
            _cont++;
        }
        Rotate(jsonObj);


    }






}


















            //Coordinates of the initial position of the satellite
            // Vector  where(position x == z realLifePosition, position y== x realLifePosition,position z == y realLifePosition)
         