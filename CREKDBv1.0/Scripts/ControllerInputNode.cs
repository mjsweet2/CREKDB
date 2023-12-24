/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the GNU General Public License v3.0.
 * You should have received a copy of the GNU General Public License v3.0 license with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class ControllerInputNode : Node {

    [TextArea(1, 16)] public string controllerInputNodeName;
   
    [Output] public string leftX;
    [Output] public string leftY;
    [Output] public string rightX;
    [Output] public string rightY;

    [Output] public string pos01;
    [Output] public string rot01;

    [Output] public string pos02;
    [Output] public string rot02;

    NSInputJSON nsInputJSON;

    // Use this for initialization
    protected override void Init() {
		base.Init();
        nsInputJSON = new NSInputJSON();

    }

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {




        if (port.fieldName == "leftX")
        {
            nsInputJSON.nodename = controllerInputNodeName + ".leftX";
            nsInputJSON.returntype = "float";
            leftX = JsonUtility.ToJson(nsInputJSON);
            return leftX;
        }
        if (port.fieldName == "leftY")
        {
            nsInputJSON.nodename = controllerInputNodeName + ".leftY";
            nsInputJSON.returntype = "float";
            leftY = JsonUtility.ToJson(nsInputJSON);
            return leftY;
        }
        if (port.fieldName == "rightX")
        {
            nsInputJSON.nodename = controllerInputNodeName + ".rightX";
            nsInputJSON.returntype = "float";
            rightX = JsonUtility.ToJson(nsInputJSON);
            return rightX;
        }
        if (port.fieldName == "rightY")
        {
            nsInputJSON.nodename = controllerInputNodeName + ".rightY";
            nsInputJSON.returntype = "float";
            rightY = JsonUtility.ToJson(nsInputJSON);
            return rightY;
        }

        if (port.fieldName == "pos01")
        {
            nsInputJSON.nodename = controllerInputNodeName + ".pos01";
            nsInputJSON.returntype = "float3";
            pos01 = JsonUtility.ToJson(nsInputJSON);
            return pos01;
        }
        if (port.fieldName == "rot01")
        {
            nsInputJSON.nodename = controllerInputNodeName + ".rot01";
            nsInputJSON.returntype = "float3";
            rot01 = JsonUtility.ToJson(nsInputJSON);
            return rot01;
        }

        if (port.fieldName == "pos02")
        {
            nsInputJSON.nodename = controllerInputNodeName + ".pos02";
            nsInputJSON.returntype = "float3";
            pos02 = JsonUtility.ToJson(nsInputJSON);
            return pos02;
        }
        if (port.fieldName == "rot02")
        {
            nsInputJSON.nodename = controllerInputNodeName + ".rot02";
            nsInputJSON.returntype = "float3";
            rot02 = JsonUtility.ToJson(nsInputJSON);
            return rot02;
        }



        return null; // Replace this
    }
}