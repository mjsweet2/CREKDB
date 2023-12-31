﻿/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the MIT License.
 * You should have received a copy of the MIT License with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class TopicMessageTINode : Node {


    [TextArea(1, 16)] public string topicMessageTINodeName;

    public string topic;
    public string message;


    //for flow
    [Input] public string entry;
    [Output] public string exit;


    //for input, if message = "", then this is used
    [Input] public string a;


    // Use this for initialization
    protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port)
    {

        a = GetInputValue<string>("a", this.a);

        entry = GetInputValue<string>("entry", this.entry);
        exit = topicMessageTINodeName;
        

        if (port.fieldName == "a")
        {
            return a;
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