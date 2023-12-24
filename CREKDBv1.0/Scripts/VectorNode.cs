/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the GNU General Public License v3.0.
 * You should have received a copy of the GNU General Public License v3.0 license with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class VectorNode : Node {

    [TextArea(1, 16)] public string vectorNodeName;

    public Vector3 v;
    [Output] public string value;
    NSInputJSON nsInputJSON;

    // Use this for initialization
    protected override void Init() {
		base.Init();
        nsInputJSON = new NSInputJSON();
    }

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {

        nsInputJSON.nodename = vectorNodeName;
        nsInputJSON.returntype = "float3";
        value = JsonUtility.ToJson(nsInputJSON); //value = "{\"nodename\":\"" + vectorNodeName + "\",\"returntype\":\"float3\"}";
        if (port.fieldName == "value")
        {
            return value;
        }

        return null; // Replace this
	}
}