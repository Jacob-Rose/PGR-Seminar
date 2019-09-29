using System.Collections;
using System.Collections.Generic;
using Valve.Sockets;
using UnityEngine;

public class VNetwork : MonoBehaviour
{
    private StatusCallback status;
    private NetworkingSockets networkClient;
    private uint listenSocket;
    private bool m_IsServer;

    private uint m_Connection;

    public static VNetwork Instance;
}
