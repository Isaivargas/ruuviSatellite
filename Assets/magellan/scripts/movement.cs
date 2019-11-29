using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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


    public Text humidity;
    public Text pressure;
    public Text altitud;

    public Text angleCalculate;
    public Text temperature;
    public Text x;
    public Text y;
    public Text z;
    public Text functionTransform;
    public Text functtionTorque;
    public Int32 presionAt = 10132;
    public float torque;

    public Int64 Inercia =  (Int64)((2) * (Math.Pow(1, 2) + Math.Pow(2, 2)));

    public double seconds = 1;
    public double timer = 0;
    private Values _initialValues = new Values();
    private const string Endpoint = "http://6458f94c.ngrok.io/data";
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
        float moveSpeed = 0.001F;

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


        transform.rotation = Quaternion.Euler(new Vector3(0, values.acceleration_x, values.acceleration_y * angle * moveSpeed));

        torque = (float)((AngleRad / Inercia) * (-0.8));
        Debug.Log("Inercia:"+Inercia);
        functtionTorque.text = torque.ToString();
        temperature.text = values.temperature.ToString();
        humidity.text = values.humidity.ToString();
        pressure.text = values.pressure.ToString();
        x.text = values.acceleration_x.ToString();
        y.text = values.acceleration_y.ToString();
        z.text = values.acceleration_z.ToString();
        Int64 altituds = (Int64)(((values.pressure - presionAt) / (9.77 * 1.1))+9100)*10;
        altitud.text = altituds.ToString();
        angleCalculate.text = angle.ToString();




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