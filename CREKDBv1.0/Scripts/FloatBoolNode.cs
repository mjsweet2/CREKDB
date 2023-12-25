/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the MIT License.
 * You should have received a copy of the MIT License with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class FloatBoolNode : Node {

    [TextArea(1, 16)] public string floatBoolNodeName;
    public enum OP { EQL, NOTEQL, LT, GT, LTEQL, GTEQL };

    public OP op;

    [Input] public string a;
    [Input] public string b;
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
        b = GetInputValue<string>("b", this.b);

        nsInputJSON.nodename = floatBoolNodeName;
        nsInputJSON.returntype = "bool";
        value = JsonUtility.ToJson(nsInputJSON);


        if (port.fieldName == "a")
        {
            return a;
        }
        if (port.fieldName == "b")
        {
            return b;
        }

        if (port.fieldName == "value")
        {
            return value;
        }
        if (port.fieldName == "op")
        {// EQL, NOTEQL, LT, GT, LTEQL, GTEQL
            string retOp = "EQUAL";
            if (op == OP.EQL) { retOp = "EQL"; }
            else if (op == OP.NOTEQL) { retOp = "NOTEQL"; }
            else if (op == OP.LT) { retOp = "LT"; }
            else if (op == OP.GT) { retOp = "GT"; }
            else if (op == OP.LTEQL) { retOp = "LTEQL"; }
            else if (op == OP.GTEQL) { retOp = "GTEQL"; }
            return retOp;
        }

        return null; // Replace this
    }
}