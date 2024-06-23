using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Server : MonoBehaviour
{
    public int m_Port = 7777;
    private TcpListener m_TcpListener;
    private List<TcpClient> m_Clients = new List<TcpClient>(new TcpClient[0]);
    private Thread m_ThrdtcpListener;
    private TcpClient m_Client;
    private Queue<string[]> messageQueue = new Queue<string[]>(); // 메시지 큐를 문자열 배열로 변경

    float timer;
    float waitingTime;

    public InputField[] outputAreas; // 60개의 InputField 배열로 선언

    void Start()
    {
        m_ThrdtcpListener = new Thread(new ThreadStart(ListenForIncommingRequests));
        m_ThrdtcpListener.IsBackground = true;
        m_ThrdtcpListener.Start();

        outputAreas = new InputField[60]; // 배열 초기화
        for (int i = 0; i < 60; i++)
        {
            outputAreas[i] = GameObject.Find("InputField" + (i + 1)).GetComponent<InputField>();
        }
    
        StartCoroutine(ProcessMessages()); // 코루틴 시작
    }

    void Update()
    {
        for (int i = 0; i < m_Clients.Count; i++)
        {
            if (!m_Clients[i].Connected)
                m_Clients.RemoveAt(i);
            else
            {
                SendMessage(m_Clients[i], "connected"); // 보내는 값
            }
        }
    }

    IEnumerator ProcessMessages()
    {
        while (true)
        {
            if (messageQueue.Count > 0)
            {
                lock (messageQueue)
                {
                    string[] message = messageQueue.Dequeue(); // 큐에서 메시지 꺼내기
                    for (int i = 0; i < outputAreas.Length; i++)
                    {
                        if (i < message.Length)
                        {
                            outputAreas[i].text = message[i]; // 메시지를 각 InputField에 표시
                        }
                    }
                }
            }

            yield return new WaitForSeconds(1f); // 메시지 처리 간에 1초 딜레이 주기
        }
    }

    void OnApplicationQuit()
    {
        m_ThrdtcpListener.Abort();

        if (m_TcpListener != null)
        {
            m_TcpListener.Stop();
            m_TcpListener = null;
        }
    }

    void ListenForIncommingRequests()
    {
        m_TcpListener = new TcpListener(IPAddress.Any, m_Port);
        m_TcpListener.Start();
        ThreadPool.QueueUserWorkItem(ListenerWorker, null);
    }

    void ListenerWorker(object token)
    {
        while (m_TcpListener != null)
        {
            m_Client = m_TcpListener.AcceptTcpClient();
            m_Clients.Add(m_Client);
            ThreadPool.QueueUserWorkItem(HandleClientWorker, m_Client);
        }
    }

    void HandleClientWorker(object token)
    {
        Byte[] bytes = new Byte[1024];
        using (var client = token as TcpClient)
        using (var stream = client.GetStream())
        {
            int length;

            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                var incomingData = new byte[length];
                Array.Copy(bytes, 0, incomingData, 0, length);
                string receivedMessage = Encoding.UTF8.GetString(incomingData);
                Debug.Log(receivedMessage);

                // 메시지를 줄 단위로 분리
                string[] lines = receivedMessage.Split('\n');

                foreach (var line in lines)
                {
                    // 각 줄을 콤마로 분리하여 개별 데이터 항목 처리
                    string[] items = line.Split(',');

                    lock (messageQueue)
                    {
                        // 큐에 각 줄의 데이터 항목 배열 추가
                        messageQueue.Enqueue(items);
                    }
                }
            }
        }
    }

    void SendMessage(object token, string message)
    {
        var client = token as TcpClient;
        if (client == null) return;

        try
        {
            NetworkStream stream = client.GetStream();
            if (stream.CanWrite)
            {
                byte[] serverMessageAsByteArray = Encoding.UTF8.GetBytes(message);
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
            }
        }
        catch (SocketException ex)
        {
            Debug.Log(ex);
        }
    }
}
