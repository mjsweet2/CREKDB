/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the MIT License.
 * You should have received a copy of the MIT License with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class CRMotionTaskItemNode : Node {

    [TextArea(1, 16)] public string crMotionTaskItemNodeName; // this the unique container, which points to motion asset,


    public string motionName;

    public enum OP { START, LOAD, CXL };
    public OP op;

    [Input] public string param1;
    [Input] public string param2;
    [Input] public string param3;
    [Input] public string param4;

    [Input] public string entry;
    [Output] public string exit;


    // Use this for initialization
    protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {

        entry = GetInputValue<string>("entry", this.entry);
        exit = crMotionTaskItemNodeName;

        param1 = GetInputValue<string>("param1", "");
        param2 = GetInputValue<string>("param2", "");
        param3 = GetInputValue<string>("param3", "");
        param4 = GetInputValue<string>("param4", "");

        

        if (port.fieldName == "param1")
        {
            return param1;
        }
        if (port.fieldName == "param2")
        {
            return param2;
        }
        if (port.fieldName == "param3")
        {
            return param3;
        }
        if (port.fieldName == "param4")
        {
            return param4;
        }
        if (port.fieldName == "entry")
        {
            return entry;
        }
        if (port.fieldName == "exit")
        {
            return exit + "_a";
        }

        return null; // Replace this
    }
}