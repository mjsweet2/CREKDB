/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the GNU General Public License v3.0.
 * You should have received a copy of the GNU General Public License v3.0 license with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class VectorMathNode : Node {

    [TextArea(1, 16)] public string vectorMathNodeName;
    public enum OP { ADD, SUB, MUL, DIV };

    public OP op;

    [Input] public string a;
    [Input] public string bv;//for addition to vector
    [Input] public string bf; // for mult scaler
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
        bv = GetInputValue<string>("bv", this.bv);
        bf = GetInputValue<string>("bf", this.bf);
        nsInputJSON.nodename = vectorMathNodeName;
        nsInputJSON.returntype = "float3";
        value = JsonUtility.ToJson(nsInputJSON);// value = "{\"nodename\":\"" + vectorMathNodeName + "\",\"returntype\":\"float3\"}"; 


        if (port.fieldName == "a")
        {
            return a;
        }
        if (port.fieldName == "bv")
        {
            return bv;
        }
        if (port.fieldName == "bf")
        {
            return bf;
        }

        if (port.fieldName == "value")
        {
            return value;
        }
        if (port.fieldName == "op")
        {
            string retOp = "ADD";
            if (op == OP.ADD) { retOp = "ADD"; }
            else if (op == OP.SUB) { retOp = "SUB"; }
            else if (op == OP.MUL) { retOp = "MUL"; }
            else if (op == OP.DIV) { retOp = "DIV"; }

            return retOp;
        }

        return null; // Replace this
    }
}