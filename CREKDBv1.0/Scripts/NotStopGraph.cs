/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the GNU General Public License v3.0.
 * You should have received a copy of the GNU General Public License v3.0 license with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu]
public class NotStopGraph : NodeGraph
{


    public void exportToDB(NSMotionDBController db)
    {

        createMotions(db); // I do these first, because I need to update them in createNodesAndEdges(db)
        createFlowNodesAndEdges(db); //temporaryily connects edges, need to be updated in next pass
        connectEdges(db);   //updates the edge.nextNode value
        connectAliases(db);


        //adding support for gametime Trajectory Creation
        createFunctionNodes(db); //creates rows in the FunctionNodes table
        createInputNodes(db); //FloatNodes, VectorNodes, VectorMathNodes, MathNodes
        connectTrajInputNodes(db); //updates inputs JSON string in TrajectoryNodes table

    }
    public void listAllNames()
    {
        Debug.Log("all nodes...");
        for (int i = 0; i < nodes.Count; i++)
        {

            Debug.Log(nodes[i].name);

        }

    }

    public void specialChangeMotionNames()
    {
        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "Motion")
            {

                MotionNode mNode = (MotionNode)nodes[i];
                // populate the inputString
                Debug.Log(mNode.motionName);
                string baseName = mNode.motionName.Substring(0, mNode.motionName.Length - 2);
                string delimiter = ".";
                string channelName = mNode.motionName.Substring(mNode.motionName.Length - 2, 2);
                Debug.Log(baseName + delimiter + channelName);
                mNode.motionName = baseName + delimiter + channelName;

            }

        }


    }
    void createMotions(NSMotionDBController db)
    {
        
        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "Motion")
            {

                MotionNode mNode = (MotionNode)nodes[i];
                // populate the inputString
                mNode.setInputString();


                //only put entry in if not blank
                if (mNode.firstNode != "")
                {
                    Debug.Log("Inserting Motion: " + mNode.motionName);
                    db.insertMotion(mNode.motionName, mNode.firstNode); //temporary
                    db.UpdateInputsOnMotionNode(mNode.motionName, mNode.inputString);
                    db.insertAllNodesNameType(mNode.motionName, "Motion");

                }
                
            }

        }

    }
    void createFlowNodesAndEdges(NSMotionDBController db)
    {
        
        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "Traj")
            {

                TrajNode tNode = (TrajNode)nodes[i];
                tNode.entry = tNode.GetInputValue<string>("entry", tNode.entry);
                Debug.Log("Inserting Node Trajectory: " + tNode.trajNodeName);
                db.insertTrajectoryNode(tNode.trajNodeName, tNode.trajName);
                db.insertAllNodesNameType(tNode.trajNodeName, "Trajectory");


                //firstNode = "motion" + ";" + motionName;
                string[] pieces;
                pieces = tNode.entry.Split(';');

                //came from motion node, update motion firstNode, else insert edge.nextNode
                if (pieces.Length == 2)
                {
                    

                    if (pieces[1] != "")
                    {
                        //not using firstNode, using edges??
                        //db.updateMotionFirstNode(pieces[1], tNode.trajNodeName);
                        string[] secondPieces;
                        secondPieces = pieces[1].Split('_');
                        if (secondPieces.Length == 2) // edge isn't from firstNode
                        {
                            secondPieces[1] = secondPieces[1].Trim();
                            secondPieces[0] = secondPieces[0].Trim();
                            Debug.Log("updating edge: " + secondPieces[0] + ":" + secondPieces[1]);

                            if (secondPieces[1] == "a")
                            {
                                Debug.Log("Inserting EdgeA: " + pieces[1]);
                                db.insertEdgeTrajNextNode(pieces[1], tNode.trajNodeName);
                                db.updateMotionEdgeA(secondPieces[0], pieces[1]);
                            }
                            if (secondPieces[1] == "b")
                            {
                                Debug.Log("Inserting EdgeB: " + pieces[1]);
                                db.insertEdgeTrajNextNode(pieces[1], tNode.trajNodeName);
                                db.updateMotionEdgeB(secondPieces[0], pieces[1]);
                            }
                            if (secondPieces[1] == "c")
                            {
                                Debug.Log("Inserting EdgeC: " + pieces[1]);
                                db.insertEdgeTrajNextNode(pieces[1], tNode.trajNodeName);
                                db.updateMotionEdgeC(secondPieces[0], pieces[1]);
                            }
                            if (secondPieces[1] == "d")
                            {
                                Debug.Log("Inserting EdgeD: " + pieces[1]);
                                db.insertEdgeTrajNextNode(pieces[1], tNode.trajNodeName);
                                db.updateMotionEdgeD(secondPieces[0], pieces[1]);
                            }
                        }
                        else //edge is from firstNode
                        {
                            db.updateMotionFirstNode(pieces[1], tNode.trajNodeName);

                        }
                    }
                   
                }
                else
                {
                    Debug.Log("Inserting Edge??: " + tNode.entry);
                    db.insertEdgeTrajNextNode(tNode.entry, tNode.trajNodeName);

                }

            }

        }

    }

    void connectEdges(NSMotionDBController db)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].name == "Traj")
            {

                TrajNode tNode = (TrajNode)nodes[i];

                string[] pieces;
                pieces = tNode.entry.Split(';');

                //
                if (pieces.Length == 2)
                {
                    ;//do nothing, already done

                }
                else //
                {
                    //Debug.Log("no ;: " + pNode.entry);

                    pieces = tNode.entry.Split('_');
                    pieces[1] = pieces[1].Trim();
                    pieces[0] = pieces[0].Trim();
                    Debug.Log("updating edge: " + pieces[0] + ":" + pieces[1]);

                    if (pieces[1] == "a")
                        db.updateNodeEdgeA(pieces[0], tNode.entry);
                    if (pieces[1] == "b")
                        db.updateNodeEdgeB(pieces[0], tNode.entry);
                    if (pieces[1] == "c")
                        db.updateNodeEdgeC(pieces[0], tNode.entry);
                    if (pieces[1] == "d")
                        db.updateNodeEdgeD(pieces[0], tNode.entry);
                   

                }

            }

        }
        
    }

    void connectAliases(NSMotionDBController db)
    {
        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "Alias")
            {
                AliasNode aNode = (AliasNode)nodes[i];
                Debug.Log("alias: " + aNode.entry + ": " + aNode.trajNodeName);

                string[] pieces;
                pieces = aNode.entry.Split(';');

                pieces = aNode.entry.Split('_');
                pieces[1] = pieces[1].Trim();
                pieces[0] = pieces[0].Trim();
                Debug.Log("updating edge: " + pieces[0] + ":" + pieces[1]);

                if (pieces[1] == "a")
                {
                    db.insertEdgeTrajNextNode(aNode.entry, aNode.trajNodeName);
                    db.updateNodeEdgeA(pieces[0], aNode.entry);
                }
                if (pieces[1] == "b")
                {
                    db.insertEdgeTrajNextNode(aNode.entry, aNode.trajNodeName);
                    db.updateNodeEdgeB(pieces[0], aNode.entry);
                }
                if (pieces[1] == "c")
                {
                    db.insertEdgeTrajNextNode(aNode.entry, aNode.trajNodeName);
                    db.updateNodeEdgeC(pieces[0], aNode.entry);
                }
                if (pieces[1] == "d")
                {
                    db.insertEdgeTrajNextNode(aNode.entry, aNode.trajNodeName);
                    db.updateNodeEdgeD(pieces[0], aNode.entry);
                }

            }

        }

    }
    void createFunctionNodes(NSMotionDBController db)
    {
        
        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "Int Func Input")
            {                
                IntFuncInputNode ifNode = (IntFuncInputNode)nodes[i];
                ifNode.retValue = ifNode.GetInputValue<string>("retValue", ifNode.retValue);
                Debug.Log("Inserting Int Function Node: " + ifNode.retValue);
                db.insertFunctionNode(ifNode.intFuncNodeName, ifNode.functionName, "int");
                db.insertAllNodesNameType(ifNode.intFuncNodeName, "IntFunction");
            }

            if (nodes[i].name == "Float Func Input")
            {
                FloatFuncInputNode ffNode = (FloatFuncInputNode)nodes[i];
                ffNode.retValue = ffNode.GetInputValue<string>("retValue", ffNode.retValue);
                Debug.Log("Inserting Float Function Node: " + ffNode.retValue);
                db.insertFunctionNode(ffNode.floatFuncNodeName, ffNode.functionName, "float");
                db.insertAllNodesNameType(ffNode.floatFuncNodeName, "FloatFunction");
            }

            if (nodes[i].name == "Vector Func Input")
            {
                VectorFuncInputNode vfNode = (VectorFuncInputNode)nodes[i];
                vfNode.retValue = vfNode.GetInputValue<string>("retValue", vfNode.retValue);
                Debug.Log("Inserting Vector Function: " + vfNode.retValue);
                db.insertFunctionNode(vfNode.vectorFuncNodeName, vfNode.functionName, "float3");
                db.insertAllNodesNameType(vfNode.vectorFuncNodeName, "VectorFunction");
            }

        }
     

    }
    void createInputNodes(NSMotionDBController db)
    {
        //FloatNodes, VectorNodes, VectorMathNodes, MathNodes
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].name == "Int")
            {

                IntNode iNode = (IntNode)nodes[i];
                iNode.value = iNode.GetInputValue<string>("value", iNode.value);
                Debug.Log("Inserting Int Node: " + iNode.value);
                db.insertIntNode(iNode.intNodeName, iNode.v.ToString());
                db.insertAllNodesNameType(iNode.intNodeName, "Int");
            }

            if (nodes[i].name == "Float")
            {

                FloatNode fNode = (FloatNode)nodes[i];
                fNode.value = fNode.GetInputValue<string>("value", fNode.value);
                Debug.Log("Inserting Float Node: " + fNode.value);
                db.insertFloatNode(fNode.floatNodeName, fNode.v.ToString("F6"));
                db.insertAllNodesNameType(fNode.floatNodeName, "Float");
            }

            if (nodes[i].name == "Vector")
            {
                VectorNode vNode = (VectorNode)nodes[i];
                vNode.value = vNode.GetInputValue<string>("value", vNode.value);

                Debug.Log("Inserting Vector Function: " + vNode.value);
                db.insertVectorNode(vNode.vectorNodeName, vNode.v.x.ToString(), vNode.v.y.ToString(), vNode.v.z.ToString());
                db.insertAllNodesNameType(vNode.vectorNodeName, "Vector");
            }
            if (nodes[i].name == "Vector Math")
            {
                VectorMathNode vmNode = (VectorMathNode)nodes[i];
                vmNode.value = vmNode.GetInputValue<string>("value", vmNode.value);
                vmNode.op = vmNode.GetInputValue<VectorMathNode.OP>("op", vmNode.op);
                vmNode.a = vmNode.GetInputValue<string>("a", vmNode.a);
                vmNode.bv = vmNode.GetInputValue<string>("bv", vmNode.bv);
                vmNode.bf = vmNode.GetInputValue<string>("bf", vmNode.bf);

                string operation = "ADD";
                if(vmNode.op == VectorMathNode.OP.ADD) { operation = "ADD"; }
                else if (vmNode.op == VectorMathNode.OP.SUB) { operation = "SUB"; }
                else if (vmNode.op == VectorMathNode.OP.MUL) { operation = "MUL"; }
                else if (vmNode.op == VectorMathNode.OP.DIV) { operation = "DIV"; }

                Debug.Log("Inserting Vector Math Node: " + vmNode.value);
                db.insertVectorMathNode(vmNode.vectorMathNodeName, operation, vmNode.a, vmNode.bv, vmNode.bf);
                db.insertAllNodesNameType(vmNode.vectorMathNodeName, "VectorMath");
            }
            if (nodes[i].name == "Math")
            {
                MathNode mNode = (MathNode)nodes[i];
                mNode.value = mNode.GetInputValue<string>("value", mNode.value);
                mNode.op = mNode.GetInputValue<MathNode.OP>("op", mNode.op);
                mNode.a = mNode.GetInputValue<string>("a", mNode.a);
                mNode.b = mNode.GetInputValue<string>("b", mNode.b);

                string operation = "ADD";
                if (mNode.op == MathNode.OP.ADD) { operation = "ADD"; }
                else if (mNode.op == MathNode.OP.SUB) { operation = "SUB"; }
                else if (mNode.op == MathNode.OP.MUL) { operation = "MUL"; }
                else if (mNode.op == MathNode.OP.DIV) { operation = "DIV"; }

                Debug.Log("Inserting Vector Function: " + mNode.value);
                db.insertMathNode(mNode.mathNodeName, operation, mNode.a, mNode.b);
                db.insertAllNodesNameType(mNode.mathNodeName, "Math");
            }

        }

    }
    void connectTrajInputNodes(NSMotionDBController db)
    {
        {

            for (int i = 0; i < nodes.Count; i++)
            {

                if (nodes[i].name == "Traj")
                {

                    TrajNode tNode = (TrajNode)nodes[i];
                    tNode.inputNodeA = tNode.GetInputValue<string>("inputNodeA", "");
                    tNode.inputNodeB = tNode.GetInputValue<string>("inputNodeB", "");
                    tNode.inputNodeC = tNode.GetInputValue<string>("inputNodeC", "");
                    Debug.Log("Updating Inputs on Trajectory Node: " + tNode.trajNodeName);

                    if (tNode.inputNodeA == "")
                    {
                        tNode.inputNodeA = JsonUtility.ToJson(new NSInputJSON());
                    }
                    if (tNode.inputNodeB == "")
                    {
                        tNode.inputNodeB = JsonUtility.ToJson(new NSInputJSON());
                    }
                    if (tNode.inputNodeC == "")
                    {
                        tNode.inputNodeC = JsonUtility.ToJson(new NSInputJSON());
                    }
                    NSInputsJSON nsInputsJSON = new NSInputsJSON();
                    nsInputsJSON.inputs.Add(JsonUtility.FromJson<NSInputJSON>(tNode.inputNodeA));
                    nsInputsJSON.inputs.Add(JsonUtility.FromJson<NSInputJSON>(tNode.inputNodeB));
                    nsInputsJSON.inputs.Add(JsonUtility.FromJson<NSInputJSON>(tNode.inputNodeC));

                    string inputString = JsonUtility.ToJson(nsInputsJSON);
                
                    db.UpdateInputsOnTrajectoryNode(tNode.trajNodeName, inputString);      

                }

            }

        }


    }




}