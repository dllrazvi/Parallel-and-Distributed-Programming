﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lab_4.Utils;
using Lab4.Model;
using static System.Net.Mime.MediaTypeNames;

namespace Lab4.Implementation
{
    internal static class DirectCallBack
    {
        private static List<string> HOSTS;
        private static string response = String.Empty;

        public static void Run(List<string> hostnames)
        {
            HOSTS = hostnames;

            for (var i = 0; i < HOSTS.Count; i++)
            {
                DoStart(i);
            }
        }

        private static void DoStart(object idObject)
        {
            var id = (int)idObject;
            StartClient(HOSTS[id], id);

            Thread.Sleep(2000);
        }

        private static void StartClient(string host, int id)
        {
            // establish the remote endpoint of the server
            var hostInfo = Dns.GetHostEntry(host.Split('/')[0]);
            var ipAddress = hostInfo.AddressList[0];
            var remoteEndpoint = new IPEndPoint(ipAddress, 80);

            // create the TCP/IP socket
            var client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // create a state containing the connection information
            var state = new StateObject
            {
                workSocket = client,
                hostname = host.Split('/')[0],
                endpointPath = host.Contains("/") ? host.Substring(host.IndexOf("/")) : "/",
                remoteEndpoint = remoteEndpoint,
                clientID = id
            };

            // connect to the remote endpoint  
            state.workSocket.BeginConnect(state.remoteEndpoint, OnConnect, state);
            state.connectDone.WaitOne();

            var url = HttpUtils.parseURL(host);

            // Send test data to the remote device.
            Send(state, string.Format("GET /{0} HTTP/1.1\r\nHOST: {1}\r\n\r\n", url.Item2, url.Item1));
            state.sendDone.WaitOne();

            // Receive the response from the remote device.
            Receive(state);
            state.receiveDone.WaitOne();

            // Write the response to the console.
            //Console.WriteLine("Response received : {0}", response);
            Console.WriteLine("{0} - Response received : \n{1}", state.clientID, response);

            // Release the socket.
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        private static void OnConnect(IAsyncResult ar)
        {

            // retrieve the details from the connection information wrapper
            var state = (StateObject)ar.AsyncState;
            var clientSocket = state.workSocket;

            //// complete the connection  
            clientSocket.EndConnect(ar);

            Console.WriteLine("Socket connected to {0}",
                clientSocket.RemoteEndPoint.ToString());

            // Signal that the connection has been made.
            state.connectDone.Set();

        }


        private static void Receive(StateObject state)
        {
            try
            {
                // Begin receiving the data from the remote device.
                state.workSocket.BeginReceive(state.receiveBuffer, 0, StateObject.BUFFER_SIZE, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.responseContent.Append(Encoding.ASCII.GetString(state.receiveBuffer, 0, bytesRead));


                    // If the response header has not been fully obtained, get the next chunk of data
                    if (!HttpUtils.responseHeaderFullyObtained(state.responseContent.ToString()))
                    {
                        // Get the rest of the data.
                        client.BeginReceive(state.receiveBuffer, 0, StateObject.BUFFER_SIZE, 0,
                            new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        // The header response has been fully obtained
                        // Now we get the body

                        var responseBody = HttpUtils.getResponseBody(state.responseContent.ToString());

                        // The custom header parser is being used to check if the data received so far has the length
                        // Specified in the response headers
                        var contentLengthHeaderValue = HttpUtils.getContentLength(state.responseContent.ToString());

                        if(contentLengthHeaderValue > responseBody.Length)
                        {
                            // If it isn't, than more data is to be retrieved
                            client.BeginReceive(state.receiveBuffer, 0, StateObject.BUFFER_SIZE, 0,
                                new AsyncCallback(ReceiveCallback), state);
                        }
                        else
                        {
                            // Otherwise, all the data has arrived; put it in response.
                            if(state.responseContent.Length > 1)
                            {
                                response = state.responseContent.ToString();
                            }

                            // Signal that all bytes have been received
                            state.receiveDone.Set();


                        }

                    }

                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.responseContent.Length > 1)
                    {
                        response = state.responseContent.ToString();
                    }
                    // Signal that all bytes have been received.
                    state.receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        private static void Send(StateObject state, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            state.workSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), state);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                StateObject state = (StateObject)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = state.workSocket.EndSend(ar);
                Console.WriteLine("{0} - Sent {1} bytes to server.", state.clientID, bytesSent);

                // Signal that all bytes have been sent.
                state.sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


    }
}
