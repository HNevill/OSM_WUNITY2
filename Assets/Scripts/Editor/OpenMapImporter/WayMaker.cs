using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Road infrastructure maker.
/// </summary>
internal sealed class WayMaker : BaseInfrastructureMaker
{
    private Material roadMaterial;
    //public List<GameObject> combinedRoad = new List<GameObject>();
    //public List<GameObject> allRoads = new List<GameObject>();
   // private Vector3 fin1;
   // private Vector3 fin2;



    public override int NodeCount
    {
        get { return map.ways.FindAll((w) => { return w.structureType == OsmWay.OSMStructureType.Way;   }).Count; }
    }

    public WayMaker(MapReader mapReader)
        : base(mapReader) { }

    /// <summary>
    /// Create the roads.
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<int> Process()
    {
        int count = 0;

        // Iterate through the roads and build each one
        foreach (var way in map.ways.FindAll((w) => { return w.structureType == OsmWay.OSMStructureType.Way; }))
        {
            CreateObject(way, way._material, way.Name, way.type);
            yield return count;
        }
    }


    protected override void OnObjectCreated(OsmWay way, Vector3 origin, List<Vector3> vectors, List<Vector3> normals, List<Vector2> uvs, List<int> indices)
    {
        //finding the initial values
        
        OsmNode q1 = map.nodes[way.NodeIDs[0]];
        OsmNode q2 = map.nodes[way.NodeIDs[1]];


        Vector3 h1 = q1 - origin;
        Vector3 h2 = q2 - origin;

 //       Vector3 h1 = new Vector3(F1.x, 30, F1.y);
 //       Vector3 h2 = new Vector3(F2.x, 30, F2.y);

        Vector3 fin1 = new Vector3();
        Vector3 fin2 = new Vector3();

        Vector3 idiff1 = (h2 - h1).normalized;
        Vector3 icross = Vector3.Cross(idiff1, Vector3.up) * 3.7f * way.Lanes;

        Vector3 start1 = h1 + icross;
        Vector3 start2 = h1 - icross;

        //need a different method for if n==2 since 
        if (way.NodeIDs.Count == 2)
        {
            // Create points that represent the width of the road

            Vector3 v1 = h1 + icross;
            Vector3 v2 = h2 + icross;
            Vector3 v3 = h1 - icross;
            Vector3 v4 = h2 - icross;

            vectors.Add(v1);
            vectors.Add(v3);
            vectors.Add(v2);
            vectors.Add(v4);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);

            // first triangle v1, v3, v2
            indices.Add(0);
            indices.Add(2);
            indices.Add(1);
            indices.Add(0);
            indices.Add(1);
            indices.Add(2);

            // second   v3, v4, v2
            indices.Add(2);
            indices.Add(3);
            indices.Add(1);

            indices.Add(2);
            indices.Add(1);
            indices.Add(3);
            // set the starting position for the next bit of road that they connect at the same place.
            // Set the forward value of the mesh so that outside the loop I can accsess it, and draw the final mesh cube

        }
        else
        {
            //creating the internal nodes
            for (int i = 2; i < way.NodeIDs.Count; i++)
            {
                OsmNode p1 = map.nodes[way.NodeIDs[i - 2]];
                OsmNode p2 = map.nodes[way.NodeIDs[i - 1]];
                OsmNode p3 = map.nodes[way.NodeIDs[i]];

                Vector3 s1 = p1 - origin;
                Vector3 s2 = p2 - origin;
                Vector3 s3 = p3 - origin;

                Vector3 diff1 = (s2 - s1).normalized;
                Vector3 diff2 = (s3 - s2).normalized;

                // https://en.wikipedia.org/wiki/Lane
                // According to the article, it's 3.7m in Canada
                var cross1 = Vector3.Cross(diff1, Vector3.up) * 3.7f * way.Lanes;
                var cross2 = Vector3.Cross(diff2, Vector3.up) * 3.7f * way.Lanes;

                // Create points that represent the width of the road

                Vector3 v1 = s1 + cross1;
                Vector3 v2a = s2 + cross1;
                Vector3 v2b = s2 + cross2;
                Vector3 v3 = s3 + cross2;


                Vector3 v4 = s1 - cross1;
                Vector3 v5a = s2 - cross1;
                Vector3 v5b = s2 - cross2;
                Vector3 v6 = s3 - cross2;


                Vector3 v2 = Intersection(v1, v2a, v2b, v3);
                Vector3 v5 = Intersection(v4, v5a, v5b, v6);

                vectors.Add(start1);
                vectors.Add(start2);
                vectors.Add(v2);
                vectors.Add(v5);

                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));

                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);

                int idx1, idx2, idx3, idx4;
                idx4 = vectors.Count - 1;
                idx3 = vectors.Count - 2;
                idx2 = vectors.Count - 3;
                idx1 = vectors.Count - 4;

                // first triangle v1, v3, v2
                indices.Add(idx1);
                indices.Add(idx3);
                indices.Add(idx2);

                // second         v3, v4, v2
                indices.Add(idx3);
                indices.Add(idx4);
                indices.Add(idx2);

                // set the starting position for the next bit of road that they connect at the same place.
                // Set the forward value of the mesh so that outside the loop I can accsess it, and draw the final mesh cube.
                start1 = v2;
                start2 = v5;
                fin1 = v3;
                fin2 = v6;
            }

            //Final node
            vectors.Add(start1);
            vectors.Add(start2);
            vectors.Add(fin1);
            vectors.Add(fin2);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);

            int eidx1, eidx2, eidx3, eidx4;
            eidx4 = vectors.Count - 1;
            eidx3 = vectors.Count - 2;
            eidx2 = vectors.Count - 3;
            eidx1 = vectors.Count - 4;

            //first triangle 
            indices.Add(eidx1);
            indices.Add(eidx3);
            indices.Add(eidx2);

            //second triangle 
            indices.Add(eidx3);
            indices.Add(eidx4);
            indices.Add(eidx2);
        }
    
    }

    public Vector3 Intersection( Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        Vector3 sect = new Vector3();

        float Grad1 = (p2.z - p1.z) / (p2.x - p1.x);
        float Grad2 = (p4.z - p3.z) / (p4.x - p3.x);

        float c1 = p1.z - Grad1 * p1.x;
        float c2 = p3.z - Grad2 * p3.x;

        sect.x = (c2 - c1) / (Grad1 - Grad2);
        sect.y = 0;
        sect.z = Grad1 * sect.x + c1;
        return sect;

    }


}
