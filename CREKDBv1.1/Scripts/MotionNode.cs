/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the MIT License.
 * You should have received a copy of the MIT License with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.IO;
using System;

public class MotionNode : Node {


    [TextArea(1, 10)] public string motionName;
    [Output] public string firstNode;

    CRInputJSON v00JSON;
    CRInputJSON v01JSON;
    CRInputJSON v02JSON;
    CRInputJSON v03JSON;
    CRInputJSON v04JSON;

    CRInputJSON f05JSON;
    CRInputJSON f06JSON;
    CRInputJSON f07JSON;

    CRInputJSON i08JSON;
    CRInputJSON i09JSON;
    CRInputsJSON inputsJSON;

    [Output] public string v00;
    [Output] public string v01;
    [Output] public string v02;
    [Output] public string v03;
    [Output] public string v04;

    [Output] public string f05;
    [Output] public string f06;
    [Output] public string f07;

    [Output] public string i08;
    [Output] public string i09;

    [TextArea(5, 10)] public string inputString;

    // Use this for initialization
    protected override void Init() {
		base.Init();
        
        

    }
    public void setInputString()
    {
        firstNode = "motion" + ";" + motionName;
        v00JSON = new CRInputJSON();
        v01JSON = new CRInputJSON();
        v02JSON = new CRInputJSON();
        v03JSON = new CRInputJSON();
        v04JSON = new CRInputJSON();

        f05JSON = new CRInputJSON();
        f06JSON = new CRInputJSON();
        f07JSON = new CRInputJSON();

        i08JSON = new CRInputJSON();
        i09JSON = new CRInputJSON();

        //the list of all the connected output used in the motion node
        inputsJSON = new CRInputsJSON();


        v00JSON.nodename = motionName + ".f300";
        v01JSON.nodename = motionName + ".f301";
        v02JSON.nodename = motionName + ".f302";
        v03JSON.nodename = motionName + ".f303";
        v04JSON.nodename = motionName + ".f304";

        f05JSON.nodename = motionName + ".f05";
        f06JSON.nodename = motionName + ".f06";
        f07JSON.nodename = motionName + ".f07";

        i08JSON.nodename = motionName + ".i08";
        i09JSON.nodename = motionName + ".i09";

        v00JSON.returntype = "float3";
        v01JSON.returntype = "float3";
        v02JSON.returntype = "float3";
        v03JSON.returntype = "float3";
        v04JSON.returntype = "float3";

        f05JSON.returntype = "float";
        f06JSON.returntype = "float";
        f07JSON.returntype = "float";

        i08JSON.returntype = "int";
        i09JSON.returntype = "int";


        inputsJSON.inputs.Clear();
        inputString = "";
        if (GetPort("v00").IsConnected) inputsJSON.inputs.Add(v00JSON);
        if (GetPort("v01").IsConnected) inputsJSON.inputs.Add(v01JSON);
        if (GetPort("v02").IsConnected) inputsJSON.inputs.Add(v02JSON);
        if (GetPort("v03").IsConnected) inputsJSON.inputs.Add(v03JSON);
        if (GetPort("v04").IsConnected) inputsJSON.inputs.Add(v04JSON);

        if (GetPort("f05").IsConnected) inputsJSON.inputs.Add(f05JSON);
        if (GetPort("f06").IsConnected) inputsJSON.inputs.Add(f06JSON);
        if (GetPort("f07").IsConnected) inputsJSON.inputs.Add(f07JSON);

        if (GetPort("i08").IsConnected) inputsJSON.inputs.Add(i08JSON);
        if (GetPort("i09").IsConnected) inputsJSON.inputs.Add(i09JSON);

        inputString = JsonUtility.ToJson(inputsJSON);

    }

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {



        setInputString();

        if (port.fieldName == "firstNode")
        {
            return firstNode;
        }

        if (port.fieldName == "v00")
        {
            return JsonUtility.ToJson(v00JSON);
        }
        if (port.fieldName == "v01")
        {
            return JsonUtility.ToJson(v01JSON);
        }
        if (port.fieldName == "v02")
        {
            return JsonUtility.ToJson(v02JSON);
        }
        if (port.fieldName == "v03")
        {
            return JsonUtility.ToJson(v03JSON);
        }
        if (port.fieldName == "v04")
        {
            return JsonUtility.ToJson(v04JSON);
        }


        if (port.fieldName == "f05")
        {
            return JsonUtility.ToJson(f05JSON); 
        }
        if (port.fieldName == "f06")
        {
            return JsonUtility.ToJson(f06JSON);
        }
        if (port.fieldName == "f07")
        {
            return JsonUtility.ToJson(f07JSON);
        }
        if (port.fieldName == "i08")
        {
            return JsonUtility.ToJson(i08JSON);
        }
        if (port.fieldName == "i09")
        {
            return JsonUtility.ToJson(i09JSON);
        }

        return null; // Replace this
    }
}