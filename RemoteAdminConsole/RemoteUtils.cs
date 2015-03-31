﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace RemoteAdminConsole
{
    class RemoteUtils
    {
        public Connecton conn;

        public RemoteUtils()
        {
            conn = new Connecton("Grandpa", "gp", "192.168.0.18:7878");
        }
        private string token = "";

        public bool getToken()
        {

            string sendCommand = "http://" + conn.Server + "/" + "v2/token/create/" + conn.Password + "?username=" + conn.UserId;

            JObject results = Utils.GetHTTP(sendCommand);

            if (results == null)
            {
                MessageBox.Show("Invalid userid/password/server", "Remote Log Maintenance", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // this can be a string or null
            string status = (string)results["status"];
            if (status.Equals("200"))
            {
                if (results["token"] != null)
                    token = (string)results["token"];
            }
            else
            {
                token = "";
                return false;
            }

            return true;
        }

        public JObject communicateWithTerraria(String command, String options)
        {

           
            string errorResponse = "{ 'status': '100' }";
            JObject response = JObject.Parse(errorResponse);

            // And now use this to connect server
            if (conn.Server.Length == 0)
            {
                //			JOptionPane.showMessageDialog(null, "The server cannot be contacted\n", PROGRAMNAME, JOptionPane.ERROR_MESSAGE);
                return response;
            }

            if (token.Length == 0 && conn.Server.Length > 0)
            {
                return response;
            }

            String sendCommand;
            if (command.Length > 0)
                sendCommand = "http://" + conn.Server + "/" + command + "?token=" + token;
            else
                sendCommand = "http://" + conn.Server + "/" + "token=" + token;
            if (options.Length > 0)
                sendCommand = sendCommand + "&" + options.Replace(" ", "%20");


            if (GUIMain.DEBUG)
                Console.WriteLine(sendCommand);
            JObject results = Utils.GetHTTP(sendCommand);

            if (GUIMain.DEBUG)
                Console.WriteLine(results);

            // this can be a string or null
            string status = (string)results["status"];
            if (status.Equals("200"))
            {
 //               if (results["response"] != null)
                    return results;
            }
 
            return results;

        }
    }

    public class Response
    {
        [DataMember(Name = "status")]
        public string status { get; set; }
        [DataMember(Name = "response")]
        public string response { get; set; }
        [DataMember(Name = "token")]
        public string token { get; set; }
        [DataMember(Name = "rows")]
        public string rows { get; set; }
    }

}