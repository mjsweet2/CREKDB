/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the MIT License.
 * You should have received a copy of the MIT License with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class IntNode : Node {

    [TextArea(1, 16)] public string intNodeName;
    //I don't think I need a float name, if I load this into the planner,
    //I'll just use the floatnode name as the float name

    public int v;
    [Output] public string value;
    NSInputJSON nsInputJSON;

    // Use this for initialization
    protected override void Init() {
		base.Init();
        nsInputJSON = new NSInputJSON();

    }

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {

        nsInputJSON.nodename = intNodeName;
        nsInputJSON.returntype = "int";
        value = JsonUtility.ToJson(nsInputJSON);// "{\"nodename\":\"" + intNodeName + "\",\"returntype\":\"int\"}";
        if (port.fieldName == "value")
        {
            return value;
        }

        return null; // Replace this
	}
}