/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the GNU General Public License v3.0.
 * You should have received a copy of the GNU General Public License v3.0 license with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class TrajNode : Node {


    [TextArea(1, 16)] public string trajNodeName; // this the unique container, which points to traj asset, resolution and possible outputs
    [TextArea(1, 16)] public string trajName;     //this is the traj asset

    public int resolution;
   
    [Input] public string entry;


    [Output] public string auto;
    [Output] public string zero;
    [Output] public string negative;
    [Output] public string positive;

    [Input] public string inputNodeA;
    [Input] public string inputNodeB;
    [Input] public string inputNodeC;


    // Use this for initialization
    protected override void Init() {
		base.Init();

    }

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port)
    {
        entry = GetInputValue<string>("entry",this.entry);

        inputNodeA = GetInputValue<string>("inputNodeA", "");
        inputNodeB = GetInputValue<string>("inputNodeB", "");
        inputNodeC = GetInputValue<string>("inputNodeC", "");



        auto = "a";
        zero = "b";
        negative = "c";
        positive = "d";


        if (port.fieldName == "entry")
        {
            return entry;
        }

        if (port.fieldName == "auto")
        {
            return trajNodeName + "_" + auto;
        }
        if (port.fieldName == "zero")
        {
            return trajNodeName + "_" + zero;
        }
        if (port.fieldName == "negative")
        {
            return trajNodeName + "_" + negative;
        }
        if (port.fieldName == "positive")
        {
            return trajNodeName + "_" + positive;
        }
        if (port.fieldName == "inputNodeA")
        {
            return inputNodeA;
        }
        if (port.fieldName == "inputNodeB")
        {
            return inputNodeB;
        }
        if (port.fieldName == "inputNodeC")
        {
            return inputNodeC;
        }





        return null; // Replace this
    }
}