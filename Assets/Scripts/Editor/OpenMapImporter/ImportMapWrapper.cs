﻿using UnityEngine;


/*
    Copyright (c) 2018 Sloan Kelly

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

internal sealed class ImportMapWrapper
{

    private ImportMapDataEditorWindow _window;
    private string _mapFile;
    public static Transform _obstacles;
    public static Transform _walkable;

    
    public GameObject Walkable = CreateParent("Walkable");


    public ImportMapWrapper(ImportMapDataEditorWindow window, string mapFile)
                            
    {
        _window = window;
        _mapFile = mapFile;
        
    }

    public void Import()
    {
        
        var mapReader = new MapReader();
        mapReader.Read(_mapFile);

        //Transform Landuse = CreateParent("Landuse");
        //Transform Water = CreateParent("Water");
        //Transform Residential = CreateParent("Residential");
        //Transform Pedestrian = CreateParent("Pedestrian");


        var buildingMaker = new BuildingMaker(mapReader);
        var roadMaker = new WayMaker(mapReader);
        var FlatMaker = new FlatMaker(mapReader);



        Process(buildingMaker, "Importing buildings");
        Process(roadMaker, "Importing roads");
        Process(FlatMaker, "Importing Flat things");

   
        
        Combine.CombineMeshes(Walkable);

    }

    private void Process(BaseInfrastructureMaker maker, string progressText)
    {
        float nodeCount = maker.NodeCount;
        var progress = 0f;

     foreach (var node in maker.Process())
        {
           progress = node / nodeCount;
            _window.UpdateProgress(progress, progressText, false);
        }
        _window.UpdateProgress(0, string.Empty, true);
    }



    public static GameObject CreateParent(string name)
    {


        GameObject go = new GameObject("name");
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        return go;
    }


}
