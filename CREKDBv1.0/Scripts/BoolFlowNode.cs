/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the GNU General Public License v3.0.
 * You should have received a copy of the GNU General Public License v3.0 license with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class BoolFlowNode : Node {

    [TextArea(1, 16)] public string boolFlowNodeName;

    [Input] public string boolInput;

    [Input] public string entry;
    [Output] public string exitTrue;
    [Output] public string exitFalse;


    // Use this for initialization
    protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {

        boolInput = GetInputValue<string>("boolInput", this.entry);

        entry = GetInputValue<string>("entry", this.entry);

        exitTrue = boolFlowNodeName + "_" + "true";
        exitFalse = boolFlowNodeName + "_" + "false";

        if (port.fieldName == "entry")
        {
            return entry;
        }
        if (port.fieldName == "exitTrue")
        {
            return exitTrue;
        }
        if (port.fieldName == "exitFalse")
        {
            return exitFalse;
        }
        if (port.fieldName == "boolInput")
        {
            return boolInput;
        }


        return null; // Replace this
    }
}