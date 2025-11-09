/* Copyright (C) 2025 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the MIT License.
 * You should have received a copy of the MIT License with this file.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class CRSkillTaskItemNode : Node
{

    [TextArea(1, 16)] public string crSkillTaskItemNodeName; // this the unique container, which points to skill asset,


    public string skillName;

 



    [Input] public string entry;
    [Output] public string exit;


    // Use this for initialization
    protected override void Init()
    {
        base.Init();

    }

    // Return the correct value of an output port when requested
    public override object GetValue(NodePort port)
    {

        entry = GetInputValue<string>("entry", this.entry);
        exit = crSkillTaskItemNodeName;


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