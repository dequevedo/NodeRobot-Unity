using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections;

public class TCPTestClient : MonoBehaviour
{
    public Action<TCPTestClient> OnConnected = delegate { };
    public Action<TCPTestClient> OnDisconnected = delegate { };
    public Action<string> OnLog = delegate { };
    public Action<TCPTestServer.ServerMessage> OnMessageReceived = delegate { };

    public bool IsConnected
    {
        get { return socketConnection != null && socketConnection.Connected; }
    }

    public string IPAddress = "localhost";
    public int Port = 8052;

    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    private NetworkStream stream;
    private bool running;

    public void ConnectToTcpServer()
    {
        try
        {
            OnLog(string.Format("Connecting to {0}:{1}", IPAddress, Port));
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            OnLog("On client connect exception " + e);
        }
    }

    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient(IPAddress, Port);
            OnConnected(this);
            OnLog("Connected");

            Byte[] bytes = new Byte[1024];
            running = true;
            while (running)
            {
                // Get a stream object for reading
                using (stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incoming stream into byte array. 					
                    while (running && stream.CanRead)
                    {
                        length = stream.Read(bytes, 0, bytes.Length);
                        if (length != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            // Convert byte array to string message. 						
                            string serverJson = Encoding.ASCII.GetString(incomingData);
                            TCPTestServer.ServerMessage serverMessage = JsonUtility.FromJson<TCPTestServer.ServerMessage>(serverJson);
                            MessageReceived(serverMessage);
                        }
                    }
                }
            }
            socketConnection.Close();
            OnLog("Disconnected from server");
            OnDisconnected(this);
        }
        catch (SocketException socketException)
        {
            OnLog("Socket exception: " + socketException);
        }
    }

    public void CloseConnection()
    {
        socketConnection.Close();
        SendMessage("!disconnect");
        running = false;
    }

    public void MessageReceived(TCPTestServer.ServerMessage serverMessage)
    {
        OnMessageReceived(serverMessage);
    }

    public Canon canon;

    private void Start()
    {
        //StartCoroutine("UpdateAngle");
    }

    private void Update()
    {
        /*float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");*/

        /*float horizontalMovement = SimpleInput.GetAxis("Horizontal");
        float verticalMovement = SimpleInput.GetAxis("Vertical");


        byte[] a = { 1, 0 };

        if (horizontalMovement > 0)
        {
            a[1] = 11; //Direita
        }

        if (horizontalMovement < 0)
        {
            a[1] = 10; //Esquerda
        }

        if (verticalMovement > 0)
        {
            a[1] = 13; //Frente
        }

        if (verticalMovement < 0)
        {
            a[1] = 12; //Tras
        }

        if (verticalMovement == 0 && horizontalMovement == 0)
        {
            a[1] = 14; //Parar
        }

        SendMessage(a);*/
    }

    public bool lightOn = false;
    public void SwitchLight()
    {
        if (lightOn)
        {
            byte[] a = { 2, 0 }; //Desligar
            lightOn = false;
            SendMessage(a);
            print("Desligou Farol");
        } else
        {
            byte[] a = { 2, 1 }; //Ligar
            lightOn = true;
            SendMessage(a);
            print("Ligou Farol");
        }
    }

    IEnumerator UpdateAngle()
    {
        if (IsConnected)
        {
            byte a = Convert.ToByte(canon.ConvertToArduinoAngles());

            byte[] array = { a };

            for (int i = 0; i < array.Length; i++)
            {
                print("Array [" + i + "] = " + array[i]);
            }

            SendMessage(array);
        }

        yield return new WaitForSeconds(0.01f);
        StartCoroutine("UpdateAngle");
    }

    public bool SendMessage(string clientMessage)
    {
        if (socketConnection != null && socketConnection.Connected)
        {
            try
            {
                // Get a stream object for writing. 			
                NetworkStream stream = socketConnection.GetStream();
                if (stream.CanWrite)
                {
                    // Convert string message to byte array.                 
                    byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                    // Write byte array to socketConnection stream.                 
                    stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                    OnSentMessage(clientMessage);
                    return true;
                }
            }
            catch (SocketException socketException)
            {
                OnLog("Socket exception: " + socketException);
            }
        }

        return false;
    }

    public bool SendMessage(byte[] clientMessage)
    {
        if (socketConnection != null && socketConnection.Connected)
        {
            try
            {
                NetworkStream stream = socketConnection.GetStream();
                if (stream.CanWrite)
                {
                    stream.Write(clientMessage, 0, clientMessage.Length);
                    return true;
                }
            }
            catch (SocketException socketException)
            {
                OnLog("Socket exception: " + socketException);
            }
        }

        return false;
    }

    public virtual void OnSentMessage(string message)
    {
    }
}