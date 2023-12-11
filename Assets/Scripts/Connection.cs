using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;


public class Connection : MonoBehaviour
{
    private SerialPort serialPort = null;
    string portName = "ESP32_BT";
    int baudRate = 115200;
    int readTimeOut = 100;
    // Start is called before the first frame update
    void Start()
    {
        string[] ports = SerialPort.GetPortNames();

        foreach (string port in ports)
        {
            try
            {
                serialPort = new SerialPort();
                serialPort.PortName = port;
                serialPort.BaudRate = baudRate;
                serialPort.ReadTimeout = readTimeOut;
                serialPort.Open();

                if (serialPort.IsOpen)
                {
                    portName = port; // Set the correct port name
                    Debug.Log("Connected to port: " + port);
                    break; // Exit the loop once a connection is successful
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Failed to connect to port " + port + ": " + e.Message);
            }
        }

        if (string.IsNullOrEmpty(portName))
        {
            Debug.LogError("Failed to connect to any available ports.");
        }
    }

    public void sendMessage(string cmd)
    {
    	try
    	{
    		serialPort.Write(cmd);
    	}catch(System.Exception e)
    	{	
    		Debug.LogWarning(e.ToString());
    	}
    }
}
