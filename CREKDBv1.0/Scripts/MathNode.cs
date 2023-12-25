/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the MIT License.
 * You should have received a copy of the MIT License with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class MathNode : Node {

    [TextArea(1, 16)] public string mathNodeName;
    public enum OP { ADD, SUB, MUL, DIV };

    public OP op;

    [Input] public string a;
    [Input] public string b;
    [Output] public string value;

    CRInputJSON nsInputJSON;

    // Use this for initialization
    protected override void Init() {
		base.Init();
        nsInputJSON = new CRInputJSON();
    }

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {

        a = GetInputValue<string>("a", this.a);
        b = GetInputValue<string>("b", this.b);

        nsInputJSON.nodename = mathNodeName;
        nsInputJSON.returntype = "float";

        value = JsonUtility.ToJson(nsInputJSON);// "{\"nodename\":\"" + intNodeName + "\",\"returntype\":\"int\"}";


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
        {
            string retOp = "ADD";
            if(op == OP.ADD) { retOp = "ADD"; }
            else if (op == OP.SUB) { retOp = "SUB"; }
            else if (op == OP.MUL) { retOp = "MUL"; }
            else if (op == OP.DIV) { retOp = "DIV"; }

            return retOp;
        }

        return null; // Replace this
	}
}