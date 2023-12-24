/* Copyright (C) 2023 Michael Sweet - All Rights Reserved
 * mjcsweet2@outlook.com
 * You may use, distribute and modify this code under the terms of the GNU General Public License v3.0.
 * You should have received a copy of the GNU General Public License v3.0 license with this file.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NSExportApp : MonoBehaviour
{

    public InputField dbIF;
    public List<NotStopGraph> nodeGraphs;

    public NSMotionDBController nsMotiondb;


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
        nsMotiondb.dbName = dbIF.text;

        nsMotiondb.createDB();
        nsMotiondb.createBlankNSMotionDB();

        for (int i = 0; i < nodeGraphs.Count; i++)
        {
            nodeGraphs[i].exportToDB(nsMotiondb);
        }


        nsMotiondb.closeDB();



    }
    public void doList()
    {
        for (int i = 0; i < nodeGraphs.Count; i++)
        {
            nodeGraphs[i].listAllNames();
        }
    }

    


}
