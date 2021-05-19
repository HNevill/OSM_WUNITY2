using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/*
    Copyright (c) 2017 Sloan Kelly

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

/// <summary>
/// An OSM object that describes an arrangement of OsmNodes into a shape or road.
/// </summary>
class OsmWay : BaseOsm
{
    /// <summary>
    /// Way ID.
    /// </summary>
    public ulong ID { get; private set; }


    /// <summary>
    /// List of node IDs.
    /// </summary>
    public List<ulong> NodeIDs { get; private set; }



    /// <summary>
    /// Height of the structure.
    /// </summary>
    public float Height { get; private set; }

    /// <summary>
    /// The name of the object.
    /// </summary>
    public string Name { get; private set; }


    /// <summary>
    /// The number of lanes on the road. Default is 1 for contra-flow
    /// </summary>
    public int Lanes { get; private set; }
    public bool IsWalk { get; private set; }

    public enum OSMStructureType
    {
        node,
        Way,
        Boundary,
        Building
    }
    public enum OSMType
    {
        unknown,
        Landuse,
        Water,
        Building,
        Railway,
        Road,
        Residential

    }

    MapReader map;

    //public Material _material;

    public Material _material { get; private set; }

    public bool Path { get; private set; }



    public OSMType type { get; private set; }
    public OSMStructureType structureType { get; private set; }

    /// Constructor.
    /// </summary>
    /// <param name="node"></param>
    public OsmWay(XmlNode node)
    {


        NodeIDs = new List<ulong>();
        Height = 5.0f; // Default height (approx. 5m)
        Lanes = 1;      // Number of lanes either side of the divide 
        Name = "";
        IsWalk = false;
        Path = false;


        // Get the data from the attributes
        ID = GetAttribute<ulong>("id", node.Attributes);
        //Visible = GetAttribute<bool>("visible", node.Attributes);

        // Get the nodes
        XmlNodeList nds = node.SelectNodes("nd");
        foreach (XmlNode n in nds)
        {
            ulong refNo = GetAttribute<ulong>("ref", n.Attributes);
            //if (map.nodes[refNo].X < map.minx || map.nodes[refNo].X > map.maxx || map.nodes[refNo].Y > map.maxy || map.nodes[refNo].Y < map.miny)
            //{ }

            NodeIDs.Add(refNo);

        }

        //if it is a boundary
        if (NodeIDs.Count > 1)
        {
            if (NodeIDs[0] == NodeIDs[NodeIDs.Count - 1])
            structureType = OSMStructureType.Boundary;
            else
            structureType = OSMStructureType.Way;
            //IsWay = NodeIDs[0] != NodeIDs[NodeIDs.Count - 1];
        }



        // Read the tags
        XmlNodeList tags = node.SelectNodes("tag");
        foreach (XmlNode t in tags)
        {
            string key = GetAttribute<string>("k", t.Attributes);
            string value = GetAttribute<string>("v", t.Attributes);


            if (key == "building")
            {
                structureType = OSMStructureType.Building;
                type = OSMType.Building;
                IsWalk = false;
                _material = Resources.Load("Building", typeof(Material)) as Material;

            }
            if (key == "lanes")
            {
                Lanes = GetAttribute<int>("v", t.Attributes);
            }
            if (key == "name")
            {
                Name = GetAttribute<string>("v", t.Attributes);

            }

           // Asseigning

            //if (key == "amenity" && value == "school") 
            //{
            //    IsWalk = true;
            //    _material = Resources.Load("Amenity", typeof(Material)) as Material;
            //}

            if (key == "highway")
            {
                IsWalk = true;
                type = OSMType.Road;

                if (value == "footway" | value == "bridleway" | value == "path" | value == "track" | value == "pedestrian" | value == "corridor" | value == "sidewalk" | value == "cycleway" | value == "secondary_link" | value == "tertiary_link" | value == "living_street")
                {

                    _material = Resources.Load("Path", typeof(Material)) as Material;

                }

                else if (key == "highway" && (value == "motorway" | value == "trunk" | value == "primary" | value == "secondary" | value == "residential" | value == "motorway_link" | value == "motorway_link" | value == "trunk_link" | value == "primary_link" | value == "road" | value == "teritiary" | value == "unclassified"))
                {
                    _material = Resources.Load("Basic Road", typeof(Material)) as Material;

                }
                else
                {
                    _material = Resources.Load("Path", typeof(Material)) as Material;

                }
            }

            if (key == "landuse")
            {

                if (value == "grass" | value == "farmland" | value == "meadow" | value == "cemetary" | value == "village_green" | value == "allotments" | value == "farmyard" | value == "flowerbed" | value == "orchard")
                {
                    type = OSMType.Landuse;

                    _material = Resources.Load("Grass", typeof(Material)) as Material;
                }
                if (value == "residential")
                {
                    type = OSMType.Residential;
                    _material = Resources.Load("Residential", typeof(Material)) as Material;

                }


                if (value == " military" | value == "protected_area" | value == "forest" | value == "vineyard" | value == "allotments")
                {
                    _material = Resources.Load("Grass", typeof(Material)) as Material;
                    type = OSMType.Landuse;
                }


            }
            if (key == "railway")
            {
                type = OSMType.Railway;
     
                Name = "Railway";
                IsWalk = false;

                _material = Resources.Load("Railway", typeof(Material)) as Material;
            }

            if (key == "water")
            {
                type = OSMType.Water;
                Name = "Water";
                //Name = value;
                _material = Resources.Load("Water", typeof(Material)) as Material;
                IsWalk = false;

            }
            if (key == "waterway" && (value == "canal" | value == "drain" | value == "ditch" | value == "river" | value == "drain"))
            {
                type = OSMType.Water;
                Name = "Water";
                //Name = value;
                _material = Resources.Load("Water", typeof(Material)) as Material;
                IsWalk = false;
    
            }


            if (key == "leisure" && value == "park")
            {
                type = OSMType.Landuse;
                Name = "Park";
                _material = Resources.Load("Grass", typeof(Material)) as Material;
                IsWalk = false;
            }
            if (key == "natural" && value == "wood")
            {

                type = OSMType.Landuse;
                Name = "Wood";
                _material = Resources.Load("Forest", typeof(Material)) as Material;
                IsWalk = false;
            }

            //if ( type == OSMType.unknown && IsWay != true)
            //{
            //    Path = true;
            //}
        }
    }
}
