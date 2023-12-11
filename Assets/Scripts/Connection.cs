using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;


public class Connection : MonoBehaviour
{
    private SerialPort serialPort = null;
    string portName = "COM1";
    int baudRate = 115200;
    int readTimeOut = 100;
    // Start is called before the first frame update
    void Start()
    {
        try {
            serialPort = new SerialPort();
            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
            serialPort.ReadTimeout = readTimeOut;
            serialPort.Open();
        } catch (System.Exception e) {
            Debug.Log(e.Message);
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
