﻿/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the GNU General Public License v3.0.
 * You should have received a copy of the GNU General Public License v3.0 license with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class NSTaskNode : Node {

    [TextArea(1, 16)] public string nstaskName;

    [Output] public string firstNode;

    // Use this for initialization
    protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {

        firstNode = "task" + ";" + nstaskName;

        if (port.fieldName == "firstNode")
        {
            return firstNode;
        }

        return null; // Replace this
    }
}