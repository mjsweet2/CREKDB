/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the GNU General Public License v3.0.
 * You should have received a copy of the GNU General Public License v3.0 license with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

    /*
     * this node is for converting, float3 to strings, I'm not sure if I need this?
     * since TopicMessageTINodes intrinsicly use strings, I would never not expect a string
     * I could just resolve the input as usual, and if it's an int, and put the int in an IntJSON
       and if the input is a float3, just put the input in a Float3JSON, if the input is a bool
       just put the input in a bool json, and if the input is a string(not yet implemented) send as is
       If I want to send more complex message, I use JSON nodes to do that, and pass in a string to TopicMessageTINodes
  
    
    */ 


public class Float3JSONNode : Node {

    [TextArea(1, 16)] public string float3JSONNodeName;


    [Input] public string a;
    [Output] public string value;
    NSInputJSON nsInputJSON;
    // Use this for initialization
    protected override void Init() {
		base.Init();
        nsInputJSON = new NSInputJSON();
    }

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port)
    {

        a = GetInputValue<string>("a", this.a);

        nsInputJSON.nodename = float3JSONNodeName;
        nsInputJSON.returntype = "string";
        value = JsonUtility.ToJson(nsInputJSON);


        if (port.fieldName == "a")
        {
            return a;
        }

        if (port.fieldName == "value")
        {
            return value;
        }


        return null; // Replace this
    }
}