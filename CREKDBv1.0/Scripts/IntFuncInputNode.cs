/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the GNU General Public License v3.0.
 * You should have received a copy of the GNU General Public License v3.0 license with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class IntFuncInputNode : Node {



    [TextArea(1, 16)] public string intFuncNodeName;
    [TextArea(1, 16)] public string functionName;

    [Output] public string retValue;

    NSInputJSON nsInputJSON;

    // Use this for initialization
    protected override void Init() {
		base.Init();
        nsInputJSON = new NSInputJSON();

    }

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {

        nsInputJSON.nodename = intFuncNodeName;
        nsInputJSON.returntype = "int";
        retValue = JsonUtility.ToJson(nsInputJSON);

        //retValue = "{\"nodename\":\"" + intFuncNodeName + "\",\"returntype\":\"int\"}";
        if (port.fieldName == "retValue")
        {
            return retValue;
        }


        return null; // Replace this
	}
}