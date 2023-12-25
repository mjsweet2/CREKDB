/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the MIT License.
 * You should have received a copy of the MIT License with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class CRTaskNode : Node {

    [TextArea(1, 16)] public string crtaskName;

    [Output] public string firstNode;

    // Use this for initialization
    protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {

        firstNode = "task" + ";" + crtaskName;

        if (port.fieldName == "firstNode")
        {
            return firstNode;
        }

        return null; // Replace this
    }
}