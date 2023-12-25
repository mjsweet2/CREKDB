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
public class NSTaskGraph : NodeGraph
{
    public int uniqueNameSuffix;
    
    public void exportToDB(NSTaskDBController db)
    {

        //these Flow functions only create flow edge, input edges created later
        createTasks(db); //<ck>// I do these first, because I need to update them in createNodesAndEdges(db)
        createMotionTINodesAndEdges(db); //temporarily connects edges, need to be updated in next pass
        createBoolFlowNodesAndEdges(db); //temporarily connects edges, need to be updated in next pass
        createAliasFlowNodesAndEdges(db); //temporarily connects edges, need to be updated in next pass
        createTopicMessageTINodesAndEdges(db); //temporarily connects edges, need to be updated in next pass
        //createLocalTopicMessageTINodesAndEdges(db); //temporarily connects edges, need to be updated in next pass

        connectMotionTIEdges(db); //updates temporary edge connections
        connectBoolFlowEdges(db); //updates temporary edge connections
        connectAliasFlowEdges(db); //updates temporary edge connections
        connectTopicMessageTIEdges(db); //updates temporary edge connections
        //connectLocalTopicMessageTIFlowEdges(db); //updates temporary edge connections



        //input nodes, and input edges
        createFunctionNodes(db); //creates rows in the FunctionNodes table
        createInputNodes(db); //FloatNodes, VectorNodes, VectorMathNodes, MathNodes, FloatsToFloat3Node


       
        connectMotionTIInputNodes(db); //updates inputs JSON string in MotionTaskItemNodes table
        connectBoolFlowInputNodes(db); //updates inputs JSON string in BoolFlowNodes table
        connectTopicMessageTIInputNodes(db);
        ////connectLocalTopicMessageTIInputNodes



    }
    public void listAllNames()
    {
        Debug.Log("all nodes...");
        for (int i = 0; i < nodes.Count; i++)
        {
            Debug.Log(nodes[i].name);
        }

    }

    // This is to auto-name all the nodes according to the task they are part of, TaskName_001
    // need to make recursive function for each type of node
    public void nameTaskNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].name == "NS Task")
            {
                NSTaskNode tNode = (NSTaskNode)nodes[i];          
                Debug.Log("TaskNode: " + tNode.nstaskName);
                //no node rename necessary

                if (tNode.GetPort("firstNode").IsConnected)
                {
                    NodePort conn = tNode.GetPort("firstNode").GetConnection(0);


                    uniqueNameSuffix = 0;
                    if (conn.node.name == "NS Motion Task Item")
                    {
                        nameMotionTINodeInTask(conn.node, tNode.nstaskName, uniqueNameSuffix);
                    }
                    if (conn.node.name == "Bool Flow")
                    {
                        nameBoolFlowNodeInTask(conn.node, tNode.nstaskName, uniqueNameSuffix);
                    }
                    if (conn.node.name == "Alias Flow")
                    {
                        nameAliasFlowNodeInTask(conn.node, tNode.nstaskName, uniqueNameSuffix);
                    }
                    if (conn.node.name == "Topic Message TI")
                    {
                        nameTopicMessageTINodeInTask(conn.node, tNode.nstaskName, uniqueNameSuffix);
                    }
                }            
            }
        }  
    }
    void nameMotionTINodeInTask(Node n, string taskName, int suffix)
    {
        NSMotionTaskItemNode mNode = (NSMotionTaskItemNode)n;
        uniqueNameSuffix++;
        Debug.Log("MotionTINode: " + taskName + ":" + mNode.nsMotionTaskItemNodeName + ":" + uniqueNameSuffix.ToString("D3"));
        mNode.nsMotionTaskItemNodeName = taskName + uniqueNameSuffix.ToString("D3");


        if (mNode.GetPort("exit").IsConnected)
        {
            NodePort conn = mNode.GetPort("exit").GetConnection(0);
            if (conn.node.name == "NS Motion Task Item")
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
            if (conn.node.name == "NS Motion Task Item")
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
            if (conn.node.name == "NS Motion Task Item")
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
            if (conn.node.name == "NS Motion Task Item")
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
 
    void createTasks(NSTaskDBController db)
    {

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].name == "NS Task")
            {
                NSTaskNode tNode = (NSTaskNode)nodes[i];

                //only put entry in if not blank
                if (tNode.firstNode != "")
                {
                    Debug.Log("Inserting Task: " + tNode.nstaskName);
                    db.insertTask(tNode.nstaskName, tNode.firstNode); //temporary
                    db.insertAllNodesNameType(tNode.nstaskName, "NSTask");
                }
            }
        }
    }
    void createMotionTINodesAndEdges(NSTaskDBController db)
    {

        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "NS Motion Task Item")
            {

                NSMotionTaskItemNode mNode = (NSMotionTaskItemNode)nodes[i];
                mNode.entry = mNode.GetInputValue<string>("entry", mNode.entry);
                db.insertMotionTaskItemNode(mNode.nsMotionTaskItemNodeName, mNode.motionName);
                db.insertAllNodesNameType(mNode.nsMotionTaskItemNodeName, "NSMotionTI");

                //firstNode = "task" + ";" + nstaskName;
                string[] pieces;
                pieces = mNode.entry.Split(';');

                //came from TaskNode
                if (pieces.Length == 2)
                {
                    pieces[1] = pieces[1].Trim();
                    if (pieces[1] != "")
                    {
                        db.updateTaskFirstNode(pieces[1], mNode.nsMotionTaskItemNodeName);                     
                    }

                } //not from TaskNode
                else
                {
                    Debug.Log("Inserting Edge??: " + mNode.entry);
                    db.insertTaskItemEdge(mNode.entry, mNode.nsMotionTaskItemNodeName); 
                }

            }
        }
    }

    void createTopicMessageTINodesAndEdges(NSTaskDBController db)
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
                    db.insertTaskItemEdge(tNode.entry, tNode.topicMessageTINodeName); 
                }

            }
        }
    }
    

    void createBoolFlowNodesAndEdges(NSTaskDBController db)
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
                    db.insertTaskItemEdge(bfNode.entry, bfNode.boolFlowNodeName); 
                }

            }
        }
    }

    void createAliasFlowNodesAndEdges(NSTaskDBController db)
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
                    db.insertTaskItemEdge(aNode.entry, aNode.aliasFlowNodeName); //temp insert, updated later
                }
            }
        }
    }


    //_a is from MotionTaskItem
    //_true and _false is from BoolFlow
    void connectTopicMessageTIEdges(NSTaskDBController db)
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
                        if (flowNodeType == "NSMotionTI")
                        {
                            db.updateMotionTINodeExit(pieces[0], tNode.entry);
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


    void connectMotionTIEdges(NSTaskDBController db)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].name == "NS Motion Task Item")
            {

                NSMotionTaskItemNode mNode = (NSMotionTaskItemNode)nodes[i];

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
                        if (flowNodeType == "NSMotionTI")
                        {
                            db.updateMotionTINodeExit(pieces[0], mNode.entry);
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

    void connectBoolFlowEdges(NSTaskDBController db)
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
                        if (flowNodeType == "NSMotionTI")
                        {
                            db.updateMotionTINodeExit(pieces[0], bfNode.entry);
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

    void connectAliasFlowEdges(NSTaskDBController db)
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
                        if (flowNodeType == "NSMotionTI")
                        {
                            db.updateMotionTINodeExit(pieces[0], aNode.entry);
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
    void createFunctionNodes(NSTaskDBController db)
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
    void createInputNodes(NSTaskDBController db)
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
    void connectMotionTIInputNodes(NSTaskDBController db)
    {

        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "NS Motion Task Item")
            {
                NSMotionTaskItemNode mNode = (NSMotionTaskItemNode)nodes[i];
                mNode.param1 = mNode.GetInputValue<string>("param1", "");
                mNode.param2 = mNode.GetInputValue<string>("param2", "");
                mNode.param3 = mNode.GetInputValue<string>("param3", "");
                mNode.param4 = mNode.GetInputValue<string>("param4", "");
                Debug.Log("Updating Inputs on MotionTI Node: " + mNode.nsMotionTaskItemNodeName);

                NSInputsJSON nsInputsJSON = new NSInputsJSON();
                

                if (mNode.param1 != "")
                {
                    nsInputsJSON.inputs.Add(JsonUtility.FromJson<NSInputJSON>(mNode.param1));
                }
                if (mNode.param2 != "")
                {
                    nsInputsJSON.inputs.Add(JsonUtility.FromJson<NSInputJSON>(mNode.param2));
                }
                if (mNode.param3 != "")
                {
                    nsInputsJSON.inputs.Add(JsonUtility.FromJson<NSInputJSON>(mNode.param3));
                }
                if (mNode.param4 != "")
                {
                    nsInputsJSON.inputs.Add(JsonUtility.FromJson<NSInputJSON>(mNode.param4));
                }
                

                string inputString = JsonUtility.ToJson(nsInputsJSON);

                db.UpdateInputsOnMotionTINode(mNode.nsMotionTaskItemNodeName, inputString);

            }

        }

    }
    void connectTopicMessageTIInputNodes(NSTaskDBController db)
    {

        for (int i = 0; i < nodes.Count; i++)
        {

            if (nodes[i].name == "Topic Message TI")
            {
                TopicMessageTINode mNode = (TopicMessageTINode)nodes[i];
                mNode.a = mNode.GetInputValue<string>("a", "");

                Debug.Log("Updating Inputs on TopicMessageTI Node: " + mNode.topicMessageTINodeName);

                NSInputsJSON nsInputsJSON = new NSInputsJSON();

                if (mNode.a != "")
                {
                    nsInputsJSON.inputs.Add(JsonUtility.FromJson<NSInputJSON>(mNode.a));
                }

                string inputString = JsonUtility.ToJson(nsInputsJSON);

                db.UpdateInputsOnTopicMessageTINode(mNode.topicMessageTINodeName, inputString);
            }

        }

    }

    void connectBoolFlowInputNodes(NSTaskDBController db)
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
                    bNode.boolInput = JsonUtility.ToJson(new NSInputJSON());
                }
     
                NSInputsJSON nsInputsJSON = new NSInputsJSON();
                nsInputsJSON.inputs.Add(JsonUtility.FromJson<NSInputJSON>(bNode.boolInput));
                    

                string inputString = JsonUtility.ToJson(nsInputsJSON);

                db.UpdateInputsOnBoolFlowNode(bNode.boolFlowNodeName, inputString);

            }

        }

    }


}