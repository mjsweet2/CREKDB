/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the MIT License.
 * You should have received a copy of the MIT License with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CRTaskExportApp : MonoBehaviour
{

    public InputField dbIF;
    public List<CRTaskGraph> nodeGraphs;

    public CRTaskDBController crTaskdb;


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
        crTaskdb.dbName = dbIF.text;

        crTaskdb.createDB();
        crTaskdb.createBlankCRTaskDB();

        for (int i = 0; i < nodeGraphs.Count; i++)
        {
            nodeGraphs[i].exportToDB(crTaskdb);
        }


        crTaskdb.closeDB();

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

        crTaskdb.dbName = dbIF.text;
        crTaskdb.createDB();

        List<string> theRecords = new List<string>();

        crTaskdb.listTables(ref theRecords);

        Debug.Log("**********table list**********");
        for(int i = 0; i < theRecords.Count;i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();


        crTaskdb.listAllNodes(ref theRecords);
        Debug.Log("**********AllNodes**********");
        for (int i = 0; i < theRecords.Count; i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();


        crTaskdb.listTaskNodes(ref theRecords);
        Debug.Log("**********TaskNodes**********");
        for (int i = 0; i < theRecords.Count; i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();

        crTaskdb.listMotionTINodes(ref theRecords);
        Debug.Log("**********MotionTaskItemsNodes**********");
        for (int i = 0; i < theRecords.Count; i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();

        crTaskdb.listBoolFlowNodes(ref theRecords);
        Debug.Log("**********listBoolFlowNodes**********");
        for (int i = 0; i < theRecords.Count; i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();

        crTaskdb.listAliasFlowNodes(ref theRecords);
        Debug.Log("**********listAliasFlowNodes**********");
        for (int i = 0; i < theRecords.Count; i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();

        crTaskdb.listTaskItemEdges(ref theRecords);
        Debug.Log("**********listTaskItemEdges**********");
        for (int i = 0; i < theRecords.Count; i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();

        crTaskdb.listFloatBoolNodes(ref theRecords);
        Debug.Log("**********listFloatBoolNodes**********");
        for (int i = 0; i < theRecords.Count; i++)
        {
            Debug.Log(theRecords[i]);
        }
        theRecords.Clear();




        crTaskdb.closeDB();

    }

   


}
