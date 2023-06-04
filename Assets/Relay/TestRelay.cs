using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay.Models;
using System;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using SickDev.CommandSystem;

public class TestRelay : MonoBehaviour
{ 
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Signed in {AuthenticationService.Instance.PlayerId}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }


    [Command]
    private static async void CreateRelay()
    {
        try
        {
            int maxConnections = 4;
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            string joinCodeID = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log($"Joined code id: {joinCodeID}");

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
    }


    [Command]
    private static async void JoinRelay(string relayJoinCodeID)
    {
        try
        {
            JoinAllocation joinAllocation =  await RelayService.Instance.JoinAllocationAsync(relayJoinCodeID);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    /*private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            CreateRelay();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            JoinRelay(joinCodeID);
        }
    }*/
}
