using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using DG.Tweening;
using System.Linq;
using Newtonsoft.Json;
using Best.SocketIO;
using Best.SocketIO.Events;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

public class SocketIOManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset myJsonFile;
    [SerializeField]
    private TextAsset ResultJsonFile;

    protected string SocketURI = "https://dev.casinoparadize.com";

    internal ResultData tempresult;

    private SocketManager manager;

    [SerializeField]
    private string testToken;

    protected string gameID = "SL-VIK";

    string myAuth = null;
    private const int maxReconnectionAttempts = 5;
    private readonly TimeSpan reconnectionDelay = TimeSpan.FromSeconds(2);

    private void Start()
    {
        //ParseMyJson(myJsonFile.ToString(), false);
        OpenSocket();
    }

    private void OpenSocket()
    {
        // Create and setup SocketOptions
        SocketOptions options = new SocketOptions();
        options.ReconnectionAttempts = maxReconnectionAttempts;
        options.ReconnectionDelay = reconnectionDelay;
        options.Reconnection = true;

#if UNITY_WEBGL && !UNITY_EDITOR
        _jsManager.RetrieveAuthToken("token", authToken =>
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                Debug.Log("Auth token is " + authToken);
                Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
                {
                    return new
                    {
                        token = authToken
                    };
                };
                options.Auth = authFunction;
                // Proceed with connecting to the server
                SetupSocketManager(options);
            }
            else
            {
                Application.ExternalEval(@"
                window.addEventListener('message', function(event) {
                    if (event.data.type === 'authToken') {
                        // Send the message to Unity
                        SendMessage('SocketManager', 'ReceiveAuthToken', event.data.cookie);
                    }});");

                // Start coroutine to wait for the auth token
                StartCoroutine(WaitForAuthToken(options));
            }
        });
#else
        Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
        {
            return new
            {
                token = testToken
            };
        };
        options.Auth = authFunction;
        // Proceed with connecting to the server
        SetupSocketManager(options);
#endif
    }

    private IEnumerator WaitForAuthToken(SocketOptions options)
    {
        // Wait until myAuth is not null
        while (myAuth == null)
        {
            Debug.Log("My Auth is null");
            yield return null;
        }

        Debug.Log("My Auth is not null");
        // Once myAuth is set, configure the authFunction
        Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
        {
            return new
            {
                token = myAuth
            };
        };
        options.Auth = authFunction;

        Debug.Log("Auth function configured with token: " + myAuth);

        // Proceed with connecting to the server
        SetupSocketManager(options);
    }

    private void SetupSocketManager(SocketOptions options)
    {
        // Create and setup SocketManager
        this.manager = new SocketManager(new Uri(SocketURI), options);

        // Set subscriptions
        this.manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        this.manager.Socket.On<string>(SocketIOEventTypes.Disconnect, OnDisconnected);
        this.manager.Socket.On<string>(SocketIOEventTypes.Error, OnError);
        this.manager.Socket.On<string>("message", OnListenEvent);
        // Start connecting to the server
    }
    // Connected event handler implementation
    void OnConnected(ConnectResponse resp)
    {
        Debug.Log("Connected!");
        InitRequest("AUTH");
    }

    private void OnDisconnected(string response)
    {
        Debug.Log("Disconnected from the server");
    }

    private void OnListenEvent(string data)
    {
        Debug.Log("Received some_event with data: " + data);
        //ParseResponse(data);
    }

    private void InitRequest(string eventName)
    {
        InitData message = new InitData();
        message.Data = new AuthData();
        message.Data.GameID = gameID;
        message.id = "Auth";
        // Serialize message data to JSON
        string json = JsonUtility.ToJson(message);
        Debug.Log(json);
        // Send the message
        if (this.manager.Socket != null && this.manager.Socket.IsOpen)
        {
            this.manager.Socket.Emit(eventName, json);
            Debug.Log("JSON data sent: " + json);
        }
        else
        {
            Debug.LogWarning("Socket is not connected.");
        }
    }

    private void OnError(string response)
    {
        Debug.LogError("Error: " + response);
    }

    private void ParseMyJson(string jsonObject, bool type)
    {
        try
        {
            jsonObject = jsonObject.Replace("\\", string.Empty);
            jsonObject = jsonObject.Trim();
            jsonObject = jsonObject.TrimStart('"').TrimEnd('"');
            if (!type)
            {
                InitialSlotData initialslots = JsonUtility.FromJson<InitialSlotData>(jsonObject);
                PopulateSlotSocket(initialslots.PopulateSlot, initialslots.X_values, initialslots.Y_values, initialslots.LineIDs);
            }
            else
            {
                ResultData slotResult = JsonUtility.FromJson<ResultData>(jsonObject);
                tempresult = slotResult;
            }
        }
        catch(Exception e)
        {
            Debug.Log("Error while parsing Json " + e.Message);
        }
    }

    private void PopulateSlotSocket(List<string> slotPop, List<string> x_val, List<string> y_val, List<int> LineIds)
    {
        for (int i = 0; i < slotPop.Count; i++)
        {
            List<int> points = slotPop[i]?.Split(',')?.Select(Int32.Parse)?.ToList();
            //slotManager.PopulateInitalSlots(i, points);
        }

        for (int i = 0; i < slotPop.Count; i++)
        {
            //slotManager.LayoutReset(i);
        }

        for (int i = 0; i < LineIds.Count; i++)
        {
            //slotManager.FetchLines(x_val[i], y_val[i], LineIds[i], i);
        }

    }

    internal void AccumulateResult()
    {
        ParseMyJson(ResultJsonFile.ToString(), true);
    }
}

[Serializable]
public class InitialSlotData
{
    public List<string> PopulateSlot;
    public List<string> X_values;
    public List<string> Y_values;
    public List<int> LineIDs;
}

[Serializable]
public class ResultData
{
    public string StopList;
    public List<int> resultLine;
    public List<string> x_animResult;
    public List<string> y_animResult;
}

[Serializable]
public class InitData
{
    public AuthData Data;
    public string id;
}

[Serializable]
public class AuthData
{
    public string GameID;
    //public double TotalLines;
}