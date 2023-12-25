/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the MIT License.
 * You should have received a copy of the MIT License with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NSTaskExportApp : MonoBehaviour
{

    public InputField dbIF;
    public List<NSTaskGraph> nodeGraphs;

    public NSTaskDBController nsTaskdb;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void doExport()
    {
        nsTaskdb.dbName = dbIF.text;

        nsTaskdb.createDB();
        nsTaskdb.createBlankNSTaskDB();

        for (int i = 0; i < nodeGraphs.Count; i++)
        {
            nodeGraphs[i].exportToDB(nsTaskdb);
        }


        nsTaskdb.closeDB();

    }
    public void nameGraphNodes()
    {
        for (int i = 0; i < nodeGraphs.Count; i++)
        {
            nodeGraphs[i].nameTaskNodes();
        }

    }
    public void listDB()
    {

        nsTaskdb.dbName = dbIF.text;
        nsTaskdb.createDB();

        List<string> theRecords = new List<string>();

        nsTaskdb.listTables(ref theRecords);

        Debug.Log("**********table list**********");
        for(int i = 0; i < theRecords.Count;i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();


        nsTaskdb.listAllNodes(ref theRecords);
        Debug.Log("**********AllNodes**********");
        for (int i = 0; i < theRecords.Count; i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();


        nsTaskdb.listTaskNodes(ref theRecords);
        Debug.Log("**********TaskNodes**********");
        for (int i = 0; i < theRecords.Count; i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();

        nsTaskdb.listMotionTINodes(ref theRecords);
        Debug.Log("**********MotionTaskItemsNodes**********");
        for (int i = 0; i < theRecords.Count; i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();

        nsTaskdb.listBoolFlowNodes(ref theRecords);
        Debug.Log("**********listBoolFlowNodes**********");
        for (int i = 0; i < theRecords.Count; i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();

        nsTaskdb.listAliasFlowNodes(ref theRecords);
        Debug.Log("**********listAliasFlowNodes**********");
        for (int i = 0; i < theRecords.Count; i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();

        nsTaskdb.listTaskItemEdges(ref theRecords);
        Debug.Log("**********listTaskItemEdges**********");
        for (int i = 0; i < theRecords.Count; i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();

        nsTaskdb.listFloatBoolNodes(ref theRecords);
        Debug.Log("**********listFloatBoolNodes**********");
        for (int i = 0; i < theRecords.Count; i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();




        nsTaskdb.closeDB();

    }

   


}
