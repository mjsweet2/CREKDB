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

    public string currentDBName;
    public InputField dbIF;
    public List<CRTaskGraph> nodeGraphs;
    public Dropdown noodlesDD;

    public int currNodeGraphIndex;
    public string currNodeGraphName;

    public CRTaskDBController crTaskdb;


    // Start is called before the first frame update
    void Start()
    {
        noodleDDRefresh();
        updateNodeGraphName();

    }

    // Update is called once per frame
    void Update()
    {

    }
    void noodleDDRefresh()
    {

        //string noodlesDirectory = Application.dataPath + "/CREKDBv1.0/XNode/ZXYETrike/";
        //Debug.Log(noodlesDirectory);
        //string[] allfiles = Directory.GetFiles(noodlesDirectory, "*.asset", SearchOption.AllDirectories);

        Dropdown.OptionData data;

        for (int i = 0; i < nodeGraphs.Count; i++)
        {
            data = new Dropdown.OptionData();
            data.text = nodeGraphs[i].name;
            noodlesDD.options.Add(data);

        }
        data = new Dropdown.OptionData();
        data.text = "None";
        noodlesDD.options.Add(data);
        noodlesDD.value = nodeGraphs.Count;



    }
    public void updateNodeGraphName()
    {
        currNodeGraphIndex = noodlesDD.value;


        currNodeGraphName = noodlesDD.options[currNodeGraphIndex].text;
  
       
        
    }

    public void doFullExport()
    {
        crTaskdb.dbName = dbIF.text;

        crTaskdb.createDB();
        crTaskdb.createBlankCRTaskDB();

        for (int i = 0; i < nodeGraphs.Count; i++)
        {
            nodeGraphs[i].exportFullToEmptyDB(crTaskdb);
        }


        crTaskdb.closeDB();

    }
    public void nameGraphTaskNodes()
    {
        if (currNodeGraphIndex < nodeGraphs.Count)
        {
            nodeGraphs[currNodeGraphIndex].nameTaskNodes();
        }
        

    }
    public void nameGraphMotionNodes()
    {
        if (currNodeGraphIndex < nodeGraphs.Count)
        {
            nodeGraphs[currNodeGraphIndex].nameMotionNodes();
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

        crTaskdb.listEdges(ref theRecords);
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
