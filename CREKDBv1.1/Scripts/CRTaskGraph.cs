/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the MIT License.
 * You should have received a copy of the MIT License with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu]
public class CRTaskGraph : NodeGraph
{
    public int uniqueNameSuffix;
    
    public void exportFullToEmptyDB(CRTaskDBController db)
    {
        createMotions(db);
        createMotionFlowNodesAndEdges(db);
        connectMotionEdges(db);
        connectMotionAliases(db);
        


        //these Flow functions only create flow edge, input edges created later
        createTasks(db); //<ck>// I do these first, because I need to update them in createNodesAndEdges(db)
        createMotionTINodesAndEdges(db); //temporarily connects edges, need to be updated in next pass
        createSkillTINodesAndEdges(db); //temporarily connects edges, need to be updated in next pass
        createBoolFlowNodesAndEdges(db); //temporarily connects edges, need to be updated in next pass
        createAliasFlowNodesAndEdges(db); //temporarily connects edges, need to be updated in next pass
        createTopicMessageTINodesAndEdges(db); //temporarily connects edges, need to be updated in next pass
        //createLocalTopicMessageTINodesAndEdges(db); //temporarily connects edges, need to be updated in next pass

        connectMotionTIEdges(db); //updates temporary edge connections
        connectSkillTIEdges(db); //updates temporary edge connections
        connectBoolFlowEdges(db); //updates temporary edge connections
        connectAliasFlowEdges(db); //updates temporary edge connections
        connectTopicMessageTIEdges(db); //updates temporary edge connections
        //connectLocalTopicMessageTIFlowEdges(db); //updates temporary edge connections



        //input nodes, and input edges
        createFunctionNodes(db); //creates rows in the FunctionNodes table
        createInputNodes(db); //FloatNodes, VectorNodes, VectorMathNodes, MathNodes, FloatsToFloat3Node



        connectTrajInputNodes(db);

        connectMotionTIInputNodes(db); //updates inputs JSON string in MotionTaskItemNodes table
        //connectSkillTIInputNodes(db); //as of 11/8/25, Skill TIs don't have inputs
        connectBoolFlowInputNodes(db); //updates inputs JSON string in BoolFlowNodes table
        connectTopicMessageTIInputNodes(db);
        //connectLocalTopicMessageTIInputNodes



    }
    public void listAllNames()
    {
        Debug.Log("all nodes...");
        for (int i = 0; i < nodes.Count; i++)
        {
            Debug.Log(nodes[i].name);
        }

    }


    public void nameTaskNodes()
    {
        // This is to auto-name all the nodes according to the task they are part of, TaskName_001
        // need to make recursive function for each type of node
        //this only names nodes that are connected usefully.
        //for now I want to name them by hand after this function is run

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].name == "CR Task")
            {
                CRTaskNode tNode = (CRTaskNode)nodes[i];          
                Debug.Log("TaskNode: " + tNode.crtaskName);
                //no node rename necessary

                if (tNode.GetPort("firstNode").IsConnected)
                {
                    NodePort conn = tNode.GetPort("firstNode").GetConnection(0);


                    uniqueNameSuffix = 0;
                    if (conn.node.name == "CR Motion Task Item")
                    {
                        nameMotionTINodeInTask(conn.node, tNode.crtaskName, uniqueNameSuffix);
                    }
                    if (conn.node.name == "Bool Flow")
                    {
                        nameBoolFlowNodeInTask(conn.node, tNode.crtaskName, uniqueNameSuffix);
                    }
                    if (conn.node.name == "Alias Flow")
                    {
                        nameAliasFlowNodeInTask(conn.node, tNode.crtaskName, uniqueNameSuffix);
                    }
                    if (conn.node.name == "Topic Message TI")
                    {
                        nameTopicMessageTINodeInTask(conn.node, tNode.crtaskName, uniqueNameSuffix);
                    }
                }            
            }
        }  
    }

    public void nameMotionNodes()
    {

        //this only names nodes that are connected usefully.
        //for now I want to name them by hand after this function is run

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].name == "Motion")
            {
                MotionNode mNode = (MotionNode)nodes[i];
                Debug.Log("MotionNode: " + mNode.motionName);
                //no node rename necessary

                if (mNode.GetPort("firstNode").IsConnected)
                {
                    NodePort conn = mNode.GetPort("firstNode").GetConnection(0); 


                    uniqueNameSuffix = 0;
                    if (conn.node.name == "Traj")
                    {
                        nameTrajFlowNodeInMotion(conn.node, mNode.motionName, uniqueNameSuffix);
                    }
 
                }

                




            }
        }



    }

    void nameMotionTINodeInTask(Node n, string taskName, int suffix)
    {
        CRMotionTaskItemNode mNode = (CRMotionTaskItemNode)n;
        uniqueNameSuffix++;
        Debug.Log("MotionTINode: " + taskName + ":" + mNode.crMotionTaskItemNodeName + ":" + uniqueNameSuffix.ToString("D3"));
        mNode.crMotionTaskItemNodeName = taskName + uniqueNameSuffix.ToString("D3");


        if (mNode.GetPort("exit").IsConnected)
        {
            NodePort conn = mNode.GetPort("exit").GetConnection(0);
            if (conn.node.name == "CR Motion Task Item")
            {
                nameMotionTINodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
            if (conn.node.name == "Bool Flow")
            {
                nameBoolFlowNodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
            if (conn.node.name == "Alias Flow")
            {
                nameAliasFlowNodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
            if (conn.node.name == "Topic Message TI")
            {
                nameTopicMessageTINodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
        }

        //rename input ports , param1, param2, param3, param4
        //rename input nodes
        if (mNode.GetPort("param1").IsConnected)
        {
            renameInput(mNode.GetPort("param1").GetConnection(0).node, taskName, uniqueNameSuffix);
        }
        if (mNode.GetPort("param2").IsConnected)
        {
            renameInput(mNode.GetPort("param2").GetConnection(0).node, taskName, uniqueNameSuffix);
        }
        if (mNode.GetPort("param3").IsConnected)
        {
            renameInput(mNode.GetPort("param3").GetConnection(0).node, taskName, uniqueNameSuffix);
        }
        if (mNode.GetPort("param4").IsConnected)
        {
            renameInput(mNode.GetPort("param4").GetConnection(0).node, taskName, uniqueNameSuffix);
        }

    }
    void nameTopicMessageTINodeInTask(Node n, string taskName, int suffix)
    {
        TopicMessageTINode tNode = (TopicMessageTINode)n;
        uniqueNameSuffix++;
        Debug.Log("TopicMessageTINode: " + taskName + ":" + tNode.topicMessageTINodeName + ":" + uniqueNameSuffix.ToString("D3"));
        tNode.topicMessageTINodeName = taskName + uniqueNameSuffix.ToString("D3");


        if (tNode.GetPort("exit").IsConnected)
        {
            NodePort conn = tNode.GetPort("exit").GetConnection(0);
            if (conn.node.name == "CR Motion Task Item")
            {
                nameMotionTINodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
            if (conn.node.name == "Bool Flow")
            {
                nameBoolFlowNodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
            if (conn.node.name == "Alias Flow")
            {
                nameAliasFlowNodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
            if (conn.node.name == "Topic Message TI")
            {
                nameTopicMessageTINodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
        }

        
        //rename input nodes, a
        if (tNode.GetPort("a").IsConnected)
        {
            renameInput(tNode.GetPort("a").GetConnection(0).node, taskName, uniqueNameSuffix);
        }
       

    }

    void nameBoolFlowNodeInTask(Node n, string taskName, int suffix)
    {
        BoolFlowNode bNode = (BoolFlowNode)n;
        uniqueNameSuffix++;
        Debug.Log("BoolFlowNode: " + taskName + ":" + bNode.boolFlowNodeName + ":" + uniqueNameSuffix.ToString("D3"));
        bNode.boolFlowNodeName = taskName + uniqueNameSuffix.ToString("D3");



        if (bNode.GetPort("exitTrue").IsConnected)
        {
            NodePort conn = bNode.GetPort("exitTrue").GetConnection(0);
            if (conn.node.name == "CR Motion Task Item")
            {
                nameMotionTINodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
            if (conn.node.name == "Bool Flow")
            {
                nameBoolFlowNodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
            if (conn.node.name == "Alias Flow")
            {
                nameAliasFlowNodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
            if (conn.node.name == "Topic Message TI")
            {
                nameTopicMessageTINodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
            
        }

        
        if (bNode.GetPort("exitFalse").IsConnected)
        {
            NodePort conn = bNode.GetPort("exitFalse").GetConnection(0);
            if (conn.node.name == "CR Motion Task Item")
            {
                nameMotionTINodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
            if (conn.node.name == "Bool Flow")
            {
                nameBoolFlowNodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
            if (conn.node.name == "Alias Flow")
            {
                nameAliasFlowNodeInTask(conn.node, taskName, uniqueNameSuffix);
            }
        }

        
        //rename input nodes
        if (bNode.GetPort("boolInput").IsConnected) 
        {
            renameInput(bNode.GetPort("boolInput").GetConnection(0).node, taskName, uniqueNameSuffix);
           
        }
    }
    void nameAliasFlowNodeInTask(Node n, string taskName, int suffix)
    {
        AliasFlowNode aNode = (AliasFlowNode)n;
        uniqueNameSuffix++;
        Debug.Log("AliasFlowNode: " + taskName + ":" + aNode.aliasFlowNodeName + ":" + uniqueNameSuffix.ToString("D3"));
        aNode.aliasFlowNodeName = taskName + uniqueNameSuffix.ToString("D3");
    }

    void nameTrajFlowNodeInMotion(Node n, string motionName, int suffix)
    {
        TrajNode tNode = (TrajNode)n;
        uniqueNameSuffix++;
        Debug.Log("TrajNode: " + motionName + ":" + tNode.trajNodeName + ":" + uniqueNameSuffix.ToString("D3"));
        tNode.trajNodeName = motionName + uniqueNameSuffix.ToString("D3");

        if (tNode.GetPort("auto").IsConnected)
        {
            NodePort conn = tNode.GetPort("auto").GetConnection(0);
            if (conn.node.name == "Traj")
            {
                nameTrajFlowNodeInMotion(conn.node, motionName, uniqueNameSuffix);
            }
            

        }
        if (tNode.GetPort("zero").IsConnected)
        {
            NodePort conn = tNode.GetPort("zero").GetConnection(0);
            if (conn.node.name == "Traj")
            {
                nameTrajFlowNodeInMotion(conn.node, motionName, uniqueNameSuffix);
            }
        }
        if (tNode.GetPort("negative").IsConnected)
        {
            NodePort conn = tNode.GetPort("negative").GetConnection(0);
            if (conn.node.name == "Traj")
            {
                nameTrajFlowNodeInMotion(conn.node, motionName, uniqueNameSuffix);
            }
        }
        if (tNode.GetPort("positive").IsConnected)
        {
            NodePort conn = tNode.GetPort("positive").GetConnection(0);
            if (conn.node.name == "Traj")
            {
                nameTrajFlowNodeInMotion(conn.node, motionName, uniqueNameSuffix);
            }
        }


        //rename input nodes
        if (tNode.GetPort("inputNodeA").IsConnected)
        {
            renameInput(tNode.GetPort("inputNodeA").GetConnection(0).node, motionName, uniqueNameSuffix);
        }
        if (tNode.GetPort("inputNodeB").IsConnected)
        {
            renameInput(tNode.GetPort("inputNodeB").GetConnection(0).node, motionName, uniqueNameSuffix);
        }
        if (tNode.GetPort("inputNodeC").IsConnected)
        {
            renameInput(tNode.GetPort("inputNodeC").GetConnection(0).node, motionName, uniqueNameSuffix);
        }
    }
    //Input node types: FloatBool, FloatFuncInput, Float, IntFuncInput, Int, Math, VectorFuncInput, VectorMath, Vector, ControllerInput
    void renameInput(Node n, string taskName, int suffix)
    {
        if(n.name == "Float Bool")
        {
            FloatBoolNode node = (FloatBoolNode)n;
            uniqueNameSuffix++;
            Debug.Log("FloatBoolNode: " + taskName + ":" + node.floatBoolNodeName + ":" + uniqueNameSuffix.ToString("D3"));
            node.floatBoolNodeName = taskName + uniqueNameSuffix.ToString("D3");
            //rename my inputs
            if (node.GetPort("a").IsConnected)
            {
                renameInput(node.GetPort("a").GetConnection(0).node, taskName, uniqueNameSuffix);

            }
            if (node.GetPort("b").IsConnected)
            {
                renameInput(node.GetPort("b").GetConnection(0).node, taskName, uniqueNameSuffix);
            }
        }
        if (n.name == "Float Func Input")
        {
            FloatFuncInputNode node = (FloatFuncInputNode)n;
            uniqueNameSuffix++;
            Debug.Log("FloatFuncInputNode: " + taskName + ":" + node.floatFuncNodeName + ":" + uniqueNameSuffix.ToString("D3"));
            node.floatFuncNodeName = taskName + uniqueNameSuffix.ToString("D3");


            //no inputs
        }
        if (n.name == "Float")
        {
            FloatNode node = (FloatNode)n;
            uniqueNameSuffix++;
            Debug.Log("FloatNode: " + taskName + ":" + node.floatNodeName + ":" + uniqueNameSuffix.ToString("D3"));
            node.floatNodeName = taskName + uniqueNameSuffix.ToString("D3");


            //no inputs
        }
        if (n.name == "Int Func Input")
        {
            IntFuncInputNode node = (IntFuncInputNode)n;
            uniqueNameSuffix++;
            Debug.Log("IntFuncInputNode: " + taskName + ":" + node.intFuncNodeName + ":" + uniqueNameSuffix.ToString("D3"));
            node.intFuncNodeName = taskName + uniqueNameSuffix.ToString("D3");


            //no inputs
        }
        if (n.name == "Int")
        {
            IntNode node = (IntNode)n;
            uniqueNameSuffix++;
            Debug.Log("IntNode: " + taskName + ":" + node.intNodeName + ":" + uniqueNameSuffix.ToString("D3"));
            node.intNodeName = taskName + uniqueNameSuffix.ToString("D3");


            //no inputs
        }

        if (n.name == "Math")
        {
            MathNode node = (MathNode)n;
            uniqueNameSuffix++;
            Debug.Log("MathNode: " + taskName + ":" + node.mathNodeName + ":" + uniqueNameSuffix.ToString("D3"));
            node.mathNodeName = taskName + uniqueNameSuffix.ToString("D3");
            //rename my inputs
            if (node.GetPort("a").IsConnected)
            {
                renameInput(node.GetPort("a").GetConnection(0).node, taskName, uniqueNameSuffix);

            }
            if (node.GetPort("b").IsConnected)
            {
                renameInput(node.GetPort("b").GetConnection(0).node, taskName, uniqueNameSuffix);
            }
        }
        if (n.name == "Vector Func Input")
        {
            VectorFuncInputNode node = (VectorFuncInputNode)n;
            uniqueNameSuffix++;
            Debug.Log("VectorFuncInputNode: " + taskName + ":" + node.vectorFuncNodeName + ":" + uniqueNameSuffix.ToString("D3"));
            node.vectorFuncNodeName = taskName + uniqueNameSuffix.ToString("D3");


            //no inputs
        }
        if (n.name == "Vector Math")
        {
            VectorMathNode node = (VectorMathNode)n;
            uniqueNameSuffix++;
            Debug.Log("VectorMathNode: " + taskName + ":" + node.vectorMathNodeName + ":" + uniqueNameSuffix.ToString("D3"));
            node.vectorMathNodeName = taskName + uniqueNameSuffix.ToString("D3");
            //rename my inputs
            if (node.GetPort("a").IsConnected)
            {
                renameInput(node.GetPort("a").GetConnection(0).node, taskName, uniqueNameSuffix);
            }
            if (node.GetPort("bf").IsConnected)
            {
                renameInput(node.GetPort("bf").GetConnection(0).node, taskName, uniqueNameSuffix);
            }
            if (node.GetPort("bv").IsConnected)
            {
                renameInput(node.GetPort("bv").GetConnection(0).node, taskName, uniqueNameSuffix);
            }
        }
        if (n.name == "Vector")
        {
            VectorNode node = (VectorNode)n;
            uniqueNameSuffix++;
            Debug.Log("VectorNode: " + taskName + ":" + node.vectorNodeName + ":" + uniqueNameSuffix.ToString("D3"));
            node.vectorNodeName = taskName + uniqueNameSuffix.ToString("D3");


            //no inputs
        }
        if (n.name == "Controller Input")
        {
            ControllerInputNode node = (ControllerInputNode)n;
            uniqueNameSuffix++;
            Debug.Log("ControllerInputNode: " + taskName + ":" + node.controllerInputNodeName + ":" + uniqueNameSuffix.ToString("D3"));
            node.controllerInputNodeName = taskName + uniqueNameSuffix.ToString("D3");


            //no inputs
        }
        if (n.name == "Floats To Float 3")
        {
            FloatsToFloat3Node node = (FloatsToFloat3Node)n;
            uniqueNameSuffix++;
            Debug.Log("FloatsToFloat3Node: " + taskName + ":" + node.floatToFloat3NodeName + ":" + uniqueNameSuffix.ToString("D3"));
            node.floatToFloat3NodeName = taskName + uniqueNameSuffix.ToString("D3");


            //rename my inputs
            if (node.GetPort("a").IsConnected)
            {
                renameInput(node.GetPort("a").GetConnection(0).node, taskName, uniqueNameSuffix);
            }
            if (node.GetPort("b").IsConnected)
            {
                renameInput(node.GetPort("b").GetConnection(0).node, taskName, uniqueNameSuffix);
            }
            if (node.GetPort("c").IsConnected)
            {
                renameInput(node.GetPort("c").GetConnection(0).node, taskName, uniqueNameSuffix);
            }
        }



    }


    void createMotions(CRTaskDBController db)
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

    void createMotionFlowNodesAndEdges(CRTaskDBController db)
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
                                db.insertEdge(pieces[1], tNode.trajNodeName);
                                
                            }
                            if (secondPieces[1] == "b")
                            {
                                Debug.Log("Inserting EdgeB: " + pieces[1]);
                                db.insertEdge(pieces[1], tNode.trajNodeName);
                                
                            }
                            if (secondPieces[1] == "c")
                            {
                                Debug.Log("Inserting EdgeC: " + pieces[1]);
                                db.insertEdge(pieces[1], tNode.trajNodeName);
                               
                            }
                            if (secondPieces[1] == "d")
                            {
                                Debug.Log("Inserting EdgeD: " + pieces[1]);
                                db.insertEdge(pieces[1], tNode.trajNodeName);
                                
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
                    db.insertEdge(tNode.entry, tNode.trajNodeName);

                }

            }

        }

    }


    void connectMotionEdges(CRTaskDBController db)
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
                        db.updateTrajNodeEdgeA(pieces[0], tNode.entry);
                    if (pieces[1] == "b")
                        db.updateTrajNodeEdgeB(pieces[0], tNode.entry);
                    if (pieces[1] == "c")
                        db.updateTrajNodeEdgeC(pieces[0], tNode.entry);
                    if (pieces[1] == "d")
                        db.updateTrajNodeEdgeD(pieces[0], tNode.entry);


                }

            }

        }

    }

    void connectMotionAliases(CRTaskDBController db)
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
                    db.insertEdge(aNode.entry, aNode.trajNodeName);
                    db.updateTrajNodeEdgeA(pieces[0], aNode.entry);
                }
                if (pieces[1] == "b")
                {
                    db.insertEdge(aNode.entry, aNode.trajNodeName);
                    db.updateTrajNodeEdgeB(pieces[0], aNode.entry);
                }
                if (pieces[1] == "c")
                {
                    db.insertEdge(aNode.entry, aNode.trajNodeName);
                    db.updateTrajNodeEdgeC(pieces[0], aNode.entry);
                }
                if (pieces[1] == "d")
                {
                    db.insertEdge(aNode.entry, aNode.trajNodeName);
                    db.updateTrajNodeEdgeD(pieces[0], aNode.entry);
                }

            }

        }

    }

    void connectTrajInputNodes(CRTaskDBController db)
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
                        tNode.inputNodeA = JsonUtility.ToJson(new CRInputJSON());
                    }
                    if (tNode.inputNodeB == "")
                    {
                        tNode.inputNodeB = JsonUtility.ToJson(new CRInputJSON());
                    }
                    if (tNode.inputNodeC == "")
                    {
                        tNode.inputNodeC = JsonUtility.ToJson(new CRInputJSON());
                    }
                    CRInputsJSON nsInputsJSON = new CRInputsJSON();
                    nsInputsJSON.inputs.Add(JsonUtility.FromJson<CRInputJSON>(tNode.inputNodeA));
                    nsInputsJSON.inputs.Add(JsonUtility.FromJson<CRInputJSON>(tNode.inputNodeB));
                    nsInputsJSON.inputs.Add(JsonUtility.FromJson<CRInputJSON>(tNode.inputNodeC));

                    string inputString = JsonUtility.ToJson(nsInputsJSON);

                    db.UpdateInputsOnTrajectoryNode(tNode.trajNodeName, inputString);

                }

            }

        }


    }
    void createTasks(CRTaskDBController db)
    {

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].name == "CR Task")
            {
                CRTaskNode tNode = (CRTaskNode)nodes[i];

                //only put entry in if not blank
                if (tNode.firstNode != "")
                {
                    Debug.Log("Inserting Task: " + tNode.crtaskName);
                    db.insertTask(tNode.crtaskName, tNode.firstNode); //temporary
                    db.insertAllNodesNameType(tNode.crtaskName, "CRTask");
                }
            }
        }
    }
    void createMotionTINodesAndEdges(CRTaskDBController db)
    {

        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "CR Motion Task Item")
            {

                CRMotionTaskItemNode mNode = (CRMotionTaskItemNode)nodes[i];
                mNode.entry = mNode.GetInputValue<string>("entry", mNode.entry);
                db.insertMotionTaskItemNode(mNode.crMotionTaskItemNodeName, mNode.motionName);
                db.insertAllNodesNameType(mNode.crMotionTaskItemNodeName, "CRMotionTI");

                //firstNode = "task" + ";" + nstaskName;
                string[] pieces;
                pieces = mNode.entry.Split(';');

                //came from TaskNode
                if (pieces.Length == 2)
                {
                    pieces[1] = pieces[1].Trim();
                    if (pieces[1] != "")
                    {
                        db.updateTaskFirstNode(pieces[1], mNode.crMotionTaskItemNodeName);                     
                    }

                } //not from TaskNode
                else
                {
                    Debug.Log("Inserting Edge??: " + mNode.entry);
                    db.insertEdge(mNode.entry, mNode.crMotionTaskItemNodeName); 
                }

            }
        }
    }
    void createSkillTINodesAndEdges(CRTaskDBController db)
    {

        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "CR Skill Task Item")
            {

                CRSkillTaskItemNode sNode = (CRSkillTaskItemNode)nodes[i];
                sNode.entry = sNode.GetInputValue<string>("entry", sNode.entry);
                db.insertSkillTaskItemNode(sNode.crSkillTaskItemNodeName, sNode.skillName);
                db.insertAllNodesNameType(sNode.crSkillTaskItemNodeName, "CRSkillTI");

                //firstNode = "task" + ";" + nstaskName;
                string[] pieces;
                pieces = sNode.entry.Split(';');

                //came from TaskNode
                if (pieces.Length == 2)
                {
                    pieces[1] = pieces[1].Trim();
                    if (pieces[1] != "")
                    {
                        db.updateTaskFirstNode(pieces[1], sNode.crSkillTaskItemNodeName);
                    }

                } //not from TaskNode
                else
                {
                    Debug.Log("Inserting Edge??: " + sNode.entry);
                    db.insertEdge(sNode.entry, sNode.crSkillTaskItemNodeName);
                }

            }
        }
    }
    void createTopicMessageTINodesAndEdges(CRTaskDBController db)
    {
        
        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "Topic Message TI")
            {

                TopicMessageTINode tNode = (TopicMessageTINode)nodes[i];
                tNode.entry = tNode.GetInputValue<string>("entry", tNode.entry);
                tNode.a = tNode.GetInputValue<string>("a", tNode.a);
                db.insertTopicMessageTINode(tNode.topicMessageTINodeName, tNode.topic, tNode.message);
                db.insertAllNodesNameType(tNode.topicMessageTINodeName, "TopicMessageTI");

                //firstNode = "task" + ";" + nstaskName;
                string[] pieces;
                pieces = tNode.entry.Split(';');

                //came from TaskNode
                if (pieces.Length == 2)
                {
                    pieces[1] = pieces[1].Trim();
                    if (pieces[1] != "")
                    {
                        db.updateTaskFirstNode(pieces[1], tNode.topicMessageTINodeName);
                    }

                } //not from TaskNode
                else
                {
                    Debug.Log("Inserting Edge??: " + tNode.entry);
                    db.insertEdge(tNode.entry, tNode.topicMessageTINodeName); 
                }

            }
        }
    }
    

    void createBoolFlowNodesAndEdges(CRTaskDBController db)
    {
        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "Bool Flow")
            {
                BoolFlowNode bfNode = (BoolFlowNode)nodes[i];
                bfNode.entry = bfNode.GetInputValue<string>("entry", bfNode.entry);
                db.insertBoolFlowNode(bfNode.boolFlowNodeName, bfNode.boolInput);
                db.insertAllNodesNameType(bfNode.boolFlowNodeName, "BoolFlow");

                //firstNode = "task" + ";" + nstaskName;
                string[] pieces;
                pieces = bfNode.entry.Split(';');

                //came from TaskNode
                if (pieces.Length == 2)
                {
                    pieces[1] = pieces[1].Trim();
                    if (pieces[1] != "")
                    {
                        db.updateTaskFirstNode(pieces[1], bfNode.boolFlowNodeName);
                    }

                } //not from TaskNode
                else
                {
                    Debug.Log("Inserting Edge??: " + bfNode.entry);
                    db.insertEdge(bfNode.entry, bfNode.boolFlowNodeName); 
                }

            }
        }
    }

    void createAliasFlowNodesAndEdges(CRTaskDBController db)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].name == "Alias Flow")
            {
                AliasFlowNode aNode = (AliasFlowNode)nodes[i];
                aNode.entry = aNode.GetInputValue<string>("entry", aNode.entry);
                db.insertAliasFlowNode(aNode.aliasFlowNodeName, aNode.entry, aNode.nextFlowNodeName);//doesn't use edge, to avoid loops
                db.insertAllNodesNameType(aNode.aliasFlowNodeName, "AliasFlow");

                //firstNode = "task" + ";" + nstaskName;
                string[] pieces;
                pieces = aNode.entry.Split(';');

                //came from TaskNode
                if (pieces.Length == 2)
                {
                    pieces[1] = pieces[1].Trim();
                    if (pieces[1] != "")
                    {
                        db.updateTaskFirstNode(pieces[1], aNode.aliasFlowNodeName);
                    }

                } //not from TaskNode
                else
                {
                    Debug.Log("Inserting Edge??: " + aNode.entry);
                    db.insertEdge(aNode.entry, aNode.aliasFlowNodeName); //temp insert, updated later
                }
            }
        }
    }


    //_a is from MotionTaskItem
    //_true and _false is from BoolFlow
    void connectTopicMessageTIEdges(CRTaskDBController db)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].name == "Topic Message TI")
            {

                TopicMessageTINode tNode = (TopicMessageTINode)nodes[i];

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
                    pieces[0] = pieces[0].Trim();
                    pieces[1] = pieces[1].Trim();

                    Debug.Log("updating edge: " + pieces[0] + ":" + pieces[1]);

                    //_a is from MotionTaskItem, TopicMessageTINode, LocalTopicMessageTINode
                    //_true and _false is from BoolFlow
                    if (pieces[1] == "a")
                    {
                        string flowNodeType = db.getTypeByNode(pieces[0]);
                        if (flowNodeType == "CRMotionTI")
                        {
                            db.updateMotionTINodeExit(pieces[0], tNode.entry);
                        }
                        if (flowNodeType == "CRSkillTI")
                        {
                            db.updateSkillTINodeExit(pieces[0], tNode.entry);
                        }
                        if (flowNodeType == "TopicMessageTI")
                        {
                            db.updateTopicMessageTINodeExit(pieces[0], tNode.entry);
                        }

                    }

                    if (pieces[1] == "true")
                        db.updateBoolFlowNodeExitTrue(pieces[0], tNode.entry);
                    if (pieces[1] == "false")
                        db.updateBoolFlowNodeExitFalse(pieces[0], tNode.entry);


                }

            }

        }
    }


    void connectMotionTIEdges(CRTaskDBController db)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].name == "CR Motion Task Item")
            {

                CRMotionTaskItemNode mNode = (CRMotionTaskItemNode)nodes[i];

                string[] pieces;
                pieces = mNode.entry.Split(';');

                //
                if (pieces.Length == 2)
                {
                    ;//do nothing, already done

                }
                else //
                {
                    //Debug.Log("no ;: " + pNode.entry);

                    pieces = mNode.entry.Split('_');
                    pieces[0] = pieces[0].Trim();
                    pieces[1] = pieces[1].Trim();

                    Debug.Log("updating edge: " + pieces[0] + ":" + pieces[1]);

                    //_a is from MotionTaskItem, TopicMessageTINode, LocalTopicMessageTINode
                    //_true and _false is from BoolFlow
                    if (pieces[1] == "a")
                    {
                        string flowNodeType = db.getTypeByNode(pieces[0]);
                        if (flowNodeType == "CRMotionTI")
                        {
                            db.updateMotionTINodeExit(pieces[0], mNode.entry);
                        }
                        if (flowNodeType == "CRSkillTI")
                        {
                            db.updateSkillTINodeExit(pieces[0], mNode.entry);
                        }
                        if (flowNodeType == "TopicMessageTI")
                        {
                            db.updateTopicMessageTINodeExit(pieces[0], mNode.entry);
                        }

                    }

                    if (pieces[1] == "true")
                        db.updateBoolFlowNodeExitTrue(pieces[0], mNode.entry);
                    if (pieces[1] == "false")
                        db.updateBoolFlowNodeExitFalse(pieces[0], mNode.entry);


                }

            }

        }

    }
    void connectSkillTIEdges(CRTaskDBController db)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].name == "CR Skill Task Item")
            {

                CRSkillTaskItemNode sNode = (CRSkillTaskItemNode)nodes[i];

                string[] pieces;
                pieces = sNode.entry.Split(';');

                //
                if (pieces.Length == 2)
                {
                    ;//do nothing, already done

                }
                else //
                {
                    //Debug.Log("no ;: " + pNode.entry);

                    pieces = sNode.entry.Split('_');
                    pieces[0] = pieces[0].Trim();
                    pieces[1] = pieces[1].Trim();

                    Debug.Log("updating edge: " + pieces[0] + ":" + pieces[1]);

                    //_a is from MotionTaskItem, TopicMessageTINode, LocalTopicMessageTINode
                    //_true and _false is from BoolFlow
                    if (pieces[1] == "a")
                    {
                        string flowNodeType = db.getTypeByNode(pieces[0]);
                        if (flowNodeType == "CRMotionTI")
                        {
                            db.updateMotionTINodeExit(pieces[0], sNode.entry);
                        }
                        if (flowNodeType == "CRSkillTI")
                        {
                            db.updateSkillTINodeExit(pieces[0], sNode.entry);
                        }
                        if (flowNodeType == "TopicMessageTI")
                        {
                            db.updateTopicMessageTINodeExit(pieces[0], sNode.entry);
                        }

                    }

                    if (pieces[1] == "true")
                        db.updateBoolFlowNodeExitTrue(pieces[0], sNode.entry);
                    if (pieces[1] == "false")
                        db.updateBoolFlowNodeExitFalse(pieces[0], sNode.entry);


                }

            }

        }

    }

    void connectBoolFlowEdges(CRTaskDBController db)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].name == "Bool Flow")
            {

                BoolFlowNode bfNode = (BoolFlowNode)nodes[i];

                string[] pieces;
                pieces = bfNode.entry.Split(';');

                //
                if (pieces.Length == 2)
                {
                    ;//do nothing, already done

                }
                else //
                {
                    //Debug.Log("no ;: " + pNode.entry);

                    pieces = bfNode.entry.Split('_');
                    pieces[1] = pieces[1].Trim();
                    pieces[0] = pieces[0].Trim();
                    Debug.Log("updating edge: " + pieces[0] + ":" + pieces[1]);

                    //_a is from MotionTaskItem, TopicMessageTINode, LocalTopicMessageTINode
                    //_true and _false is from BoolFlow
                    if (pieces[1] == "a")
                    {
                        string flowNodeType = db.getTypeByNode(pieces[0]);
                        if (flowNodeType == "CRMotionTI")
                        {
                            db.updateMotionTINodeExit(pieces[0], bfNode.entry);
                        }
                        if (flowNodeType == "CRSkillTI")
                        {
                            db.updateSkillTINodeExit(pieces[0], bfNode.entry);
                        }
                        if (flowNodeType == "TopicMessageTI")
                        {
                            db.updateTopicMessageTINodeExit(pieces[0], bfNode.entry);
                        }

                    }
                    if (pieces[1] == "true")
                        db.updateBoolFlowNodeExitTrue(pieces[0], bfNode.entry);
                    if (pieces[1] == "false")
                        db.updateBoolFlowNodeExitFalse(pieces[0], bfNode.entry);


                }

            }

        }

    }

    void connectAliasFlowEdges(CRTaskDBController db)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].name == "Alias Flow")
            {

                AliasFlowNode aNode = (AliasFlowNode)nodes[i];

                string[] pieces;
                pieces = aNode.entry.Split(';');

                //
                if (pieces.Length == 2)
                {
                    ;//do nothing, already done

                }
                else //
                {
                    //Debug.Log("no ;: " + pNode.entry);

                    pieces = aNode.entry.Split('_');
                    pieces[1] = pieces[1].Trim();
                    pieces[0] = pieces[0].Trim();
                    Debug.Log("updating edge: " + pieces[0] + ":" + pieces[1]);

                    //_a is from MotionTaskItem, TopicMessageTINode, LocalTopicMessageTINode
                    //_true and _false is from BoolFlow
                    if (pieces[1] == "a")
                    {
                        string flowNodeType = db.getTypeByNode(pieces[0]);
                        if (flowNodeType == "CRMotionTI")
                        {
                            db.updateMotionTINodeExit(pieces[0], aNode.entry);
                        }
                        if (flowNodeType == "CRSkillTI")
                        {
                            db.updateSkillTINodeExit(pieces[0], aNode.entry);
                        }
                        if (flowNodeType == "TopicMessageTI")
                        {
                            db.updateTopicMessageTINodeExit(pieces[0], aNode.entry);
                        }

                    }
                    if (pieces[1] == "true")
                        db.updateBoolFlowNodeExitTrue(pieces[0], aNode.entry);
                    if (pieces[1] == "false")
                        db.updateBoolFlowNodeExitFalse(pieces[0], aNode.entry);


                }

            }

        }

    }
    void createFunctionNodes(CRTaskDBController db)
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
    void createInputNodes(CRTaskDBController db)
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
            if (nodes[i].name == "Controller Input")
            {
                ControllerInputNode cNode = (ControllerInputNode)nodes[i];
                //cNode.value = cNode.GetInputValue<string>("value", cNode.value);
                Debug.Log("Inserting Controller Input Node: " + cNode.controllerInputNodeName);
                db.insertControllerInputNode(cNode.controllerInputNodeName,"notused");
                db.insertAllNodesNameType(cNode.controllerInputNodeName, "ControllerInput");
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
                if (vmNode.op == VectorMathNode.OP.ADD) { operation = "ADD"; }
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

            if (nodes[i].name == "Float Bool") //{ EQL, NOTEQL, LT, GT, LTEQL, GTEQL }
            {
                FloatBoolNode fbNode = (FloatBoolNode)nodes[i];
                fbNode.value = fbNode.GetInputValue<string>("value", fbNode.value);
                fbNode.op = fbNode.GetInputValue<FloatBoolNode.OP>("op", fbNode.op);
                fbNode.a = fbNode.GetInputValue<string>("a", fbNode.a);
                fbNode.b = fbNode.GetInputValue<string>("b", fbNode.b);

                string operation = "EQL";
                if (fbNode.op == FloatBoolNode.OP.EQL) { operation = "EQL"; }
                else if (fbNode.op == FloatBoolNode.OP.NOTEQL) { operation = "NOTEQL"; }
                else if (fbNode.op == FloatBoolNode.OP.LT) { operation = "LT"; }
                else if (fbNode.op == FloatBoolNode.OP.GT) { operation = "GT"; }
                else if (fbNode.op == FloatBoolNode.OP.LTEQL) { operation = "LTEQL"; }
                else if (fbNode.op == FloatBoolNode.OP.GTEQL) { operation = "GTEQL"; }

                Debug.Log("Inserting Float Bool Function: " + fbNode.value);
                db.insertFloatBoolNode(fbNode.floatBoolNodeName, operation, fbNode.a, fbNode.b);
                db.insertAllNodesNameType(fbNode.floatBoolNodeName, "FloatBool");
            }
            //FloatsToFloat3Node
            if (nodes[i].name == "Floats To Float 3") //{ EQL, NOTEQL, LT, GT, LTEQL, GTEQL }
            {
                FloatsToFloat3Node ftfNode = (FloatsToFloat3Node)nodes[i];
                ftfNode.a = ftfNode.GetInputValue<string>("a", ftfNode.a);
                ftfNode.b = ftfNode.GetInputValue<string>("b", ftfNode.b);
                ftfNode.c = ftfNode.GetInputValue<string>("c", ftfNode.c);

                Debug.Log("Inserting FloatsToFloat3Node: " + ftfNode.floatToFloat3NodeName);
                db.insertFloatsToFloat3Node(ftfNode.floatToFloat3NodeName, ftfNode.a, ftfNode.b, ftfNode.c);
                db.insertAllNodesNameType(ftfNode.floatToFloat3NodeName, "FloatsToFloat3");
            }

        }

    }
    void connectMotionTIInputNodes(CRTaskDBController db)
    {

        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "CR Motion Task Item")
            {
                CRMotionTaskItemNode mNode = (CRMotionTaskItemNode)nodes[i];
                mNode.param1 = mNode.GetInputValue<string>("param1", "");
                mNode.param2 = mNode.GetInputValue<string>("param2", "");
                mNode.param3 = mNode.GetInputValue<string>("param3", "");
                mNode.param4 = mNode.GetInputValue<string>("param4", "");
                Debug.Log("Updating Inputs on MotionTI Node: " + mNode.crMotionTaskItemNodeName);

                CRInputsJSON crInputsJSON = new CRInputsJSON();
                

                if (mNode.param1 != "")
                {
                    crInputsJSON.inputs.Add(JsonUtility.FromJson<CRInputJSON>(mNode.param1));
                }
                if (mNode.param2 != "")
                {
                    crInputsJSON.inputs.Add(JsonUtility.FromJson<CRInputJSON>(mNode.param2));
                }
                if (mNode.param3 != "")
                {
                    crInputsJSON.inputs.Add(JsonUtility.FromJson<CRInputJSON>(mNode.param3));
                }
                if (mNode.param4 != "")
                {
                    crInputsJSON.inputs.Add(JsonUtility.FromJson<CRInputJSON>(mNode.param4));
                }
                

                string inputString = JsonUtility.ToJson(crInputsJSON);

                db.UpdateInputsOnMotionTINode(mNode.crMotionTaskItemNodeName, inputString);

            }

        }

    }
    void connectSkillTIInputNodes(CRTaskDBController db)
    {

        //as of 11/8/25, Skill Task Items don't have inputs, so this function doesn't technically do anything

        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "CR Skill Task Item")
            {
                CRSkillTaskItemNode sNode = (CRSkillTaskItemNode)nodes[i];

                Debug.Log("Updating Inputs on SkillTI Node: " + sNode.crSkillTaskItemNodeName);

                CRInputsJSON crInputsJSON = new CRInputsJSON();

                string inputString = JsonUtility.ToJson(crInputsJSON);

                //db.UpdateInputsOnSkillTINode(mNode.crMotionTaskItemNodeName, inputString);

            }

        }

    }
    void connectTopicMessageTIInputNodes(CRTaskDBController db)
    {

        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "Topic Message TI")
            {
                TopicMessageTINode mNode = (TopicMessageTINode)nodes[i];
                mNode.a = mNode.GetInputValue<string>("a", "");

                Debug.Log("Updating Inputs on TopicMessageTI Node: " + mNode.topicMessageTINodeName);

                CRInputsJSON nsInputsJSON = new CRInputsJSON();

                if (mNode.a != "")
                {
                    nsInputsJSON.inputs.Add(JsonUtility.FromJson<CRInputJSON>(mNode.a));
                }

                string inputString = JsonUtility.ToJson(nsInputsJSON);

                db.UpdateInputsOnTopicMessageTINode(mNode.topicMessageTINodeName, inputString);
            }

        }

    }

    void connectBoolFlowInputNodes(CRTaskDBController db)
    {

        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "Bool Flow")
            {

                BoolFlowNode bNode = (BoolFlowNode)nodes[i];
                bNode.boolInput = bNode.GetInputValue<string>("boolInput", "");
  
                Debug.Log("Updating Inputs on BoolFlow Node: " + bNode.boolFlowNodeName);

                if (bNode.boolInput == "")
                {
                    bNode.boolInput = JsonUtility.ToJson(new CRInputJSON());
                }
     
                CRInputsJSON nsInputsJSON = new CRInputsJSON();
                nsInputsJSON.inputs.Add(JsonUtility.FromJson<CRInputJSON>(bNode.boolInput));
                    

                string inputString = JsonUtility.ToJson(nsInputsJSON);

                db.UpdateInputsOnBoolFlowNode(bNode.boolFlowNodeName, inputString);

            }

        }

    }


}