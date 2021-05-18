using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Make buildings.
/// </summary>
internal sealed class FlatMaker : BaseInfrastructureMaker
{

    private Material Material;

    public override int NodeCount
    
    {
        get
        {
            return map.ways.FindAll((w) => { return w.structureType == OsmWay.OSMStructureType.Boundary; ; }).Count;
        }
    }


    public FlatMaker(MapReader mapReader)
        : base(mapReader)
    {}

    public override IEnumerable<int> Process()
    {
        int count = 0;

        // Iterate through all the boundarys in the 'ways' list
        foreach (var way in map.ways.FindAll((w) => { return w.structureType == OsmWay.OSMStructureType.Boundary; }))
        { 
            // Create the object
           

            CreateObject(way, way._material, way.Name, way.type); 

            count++;
            yield return count;
        }   
    }

    /// <summary>
    /// Build the object using the data from the OsmWay instance.
    /// </summary>
    /// <param name="way">OsmWay instance</param>
    /// <param name="origin">The origin of the structure</param>
    /// <param name="vectors">The vectors (vertices) list</param>
    /// <param name="normals">The normals list</param>
    /// <param name="uvs">The UVs list</param>
    /// <param name="indices">The indices list</param>

    protected override void OnObjectCreated(OsmWay way, Vector3 origin, List<Vector3> vertices, List<Vector3> normals, List<Vector2> uvs, List<int> triangles)
    {
        for (int i = 1; i < way.NodeIDs.Count; i++)
        {

            OsmNode p1 = map.nodes[way.NodeIDs[i - 1]];
            OsmNode p2 = map.nodes[way.NodeIDs[i]];
 
            //drawing lines between points
            Vector3 v3 = new Vector3(0,0,0);
            Vector3 v1 = p1 - origin;
            Vector3 v2 = p2 - origin;
            
            vertices.Add(v3);
            vertices.Add(v1);
            vertices.Add(v2);
            

            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(0, 0));

            normals.Add(-Vector3.forward);
            normals.Add(-Vector3.forward);
            normals.Add(-Vector3.forward);

            int idx1, idx2;
            idx1 = vertices.Count - 2;
            idx2 = vertices.Count - 1;

            // And now the roof triangles
            triangles.Add(idx1);
            triangles.Add(idx2);
            triangles.Add(0);
            
            // Don't forget the upside down one!
            triangles.Add(idx2);
            triangles.Add(idx1);
            triangles.Add(0);
        }
    }
}
