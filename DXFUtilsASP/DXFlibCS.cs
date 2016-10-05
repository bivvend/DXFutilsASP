/*
 * Created by SharpDevelop.
 * User: 101
 * Date: 21/09/2015
 * Time: 13:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using netDxf;
using System.Linq;

namespace DXFUtilsASP
{
    /// <summary>
    /// Description of DXFlibCS.
    /// </summary>
    public class DXFlibCS
    {
        #region DECLARTIONS
        public static string error_string = "";
        //logic variables
        static bool ReadingLine = false;
        static bool ReadingPoint = false;
        static bool ReadingPolyline = false;
        static bool ReadingLWPolyline = false;
        static bool ReadingArc = false;
        static bool ReadingCircle = false;
        static bool ReadingSpline = false;
        static bool FoundNewLayer = false;
        static bool polyline_closed = false;
        static bool polyline_delay = false;
        static bool spline_closed = false;
        static bool spline_Periodic = false;

        //storage variables        
        static string layer_string = "";
        static string layer_temp = "";
        static int? LwPointCount = null;
        static int? SplinePointCount = null;
        static double? X1 = 0.0d;  //nullable   to allow for "not yet set" logic.
        static double? Y1 = 0.0d; //nullable 
        static double? X2 = 0.0d;//nullable 
        static double? Y2 = 0.0d; //nullable 
        static double? bulge = 0.0d; //nullable 
        static double? radius = 0.0d;//nullable 
        static double? start_angle = 0.0d;//nullable 
        static double? end_angle = 0.0d;//nullable 
        static int? spline_degree = null;
        public static int entity_count = 0;
        public static int SplineInterpolationNumber = 10;
        static netDxf.Entities.LwPolylineVertex LastPolyVertex = null;
        static netDxf.Entities.SplineVertex LastSplineVertex = null;
        static List<netDxf.Entities.LwPolylineVertex> LWpoints = new List<netDxf.Entities.LwPolylineVertex>();
        static List<netDxf.Entities.SplineVertex> SplinePoints = new List<netDxf.Entities.SplineVertex>();
        static List<double> SplineKnots = new List<double>();
        static List<netDxf.Entities.LwPolylineVertex> points = new List<netDxf.Entities.LwPolylineVertex>();
        public static List<Entity> vector_list = new List<Entity>();
        //public static List<netDx

        static int number = 0;
        #endregion

        #region CONSTRUCTOR
        static DXFlibCS()
        {

        }
        #endregion

        #region METHODS
        static void reset_logic()
        {
            //called to reset the current state of the line-by-line DXF loader after new enity created.
            ReadingLine = false;
            ReadingPoint = false;
            ReadingPolyline = false;
            ReadingLWPolyline = false;
            ReadingSpline = false;
            ReadingArc = false;
            ReadingCircle = false;
            FoundNewLayer = false;
            polyline_closed = false;
            spline_closed = false;
            spline_Periodic = false;
            layer_string = "LAYER";
            LwPointCount = null;
            X1 = null;
            Y1 = null;
            X2 = null;
            Y2 = null;
            bulge = 0.0;
            radius = null;
            start_angle = null;
            end_angle = null;
            spline_degree = null;
            LWpoints = new List<netDxf.Entities.LwPolylineVertex>();
            points = new List<netDxf.Entities.LwPolylineVertex>();
            SplinePoints = new List<netDxf.Entities.SplineVertex>();
            SplineKnots = new List<double>();



        }

        //Method to parse DXF line by line when netDXFdll LoadDXF fails
        public static DxfDocument LineByLineLoader(string filename)
        {
            vector_list.Clear();

            DxfDocument dxfdata = new DxfDocument();
            List<string> list_of_strings = new List<string>();
            //reset logic function
            reset_logic();

            entity_count = 0;
            double intermediate = 0.0d;  //used for nullable exchange
            int int_intermediate = 0;

            bool in_block = false;
            bool in_entities = false;
            bool final_entity = false;
            int LineCount = 0;
            string coder_string = "";
            string a_string = "";
            string layer_string = "LAYER";

            Vector2 start_point = new Vector2(0, 0);
            Vector2 end_point = new Vector2(0, 0);

            try
            {
                string line = null;
                bool found_file_end = false;
                System.IO.TextReader readFile = new StreamReader(filename);
                while (found_file_end == false)
                {
                    line = readFile.ReadLine();
                    if (line != null)
                    {
                        list_of_strings.Add(line);
                    }
                    else
                    {
                        found_file_end = true;
                    }
                }
                readFile.Close();
                readFile = null;
            }
            catch (Exception e)
            {
                error_string = e.ToString();
                return dxfdata;
            }


            for (int i = 0; i < list_of_strings.Count; i += 2)
            {
                LineCount = LineCount + 1;
                coder_string = list_of_strings[i].Trim();
                try
                {
                    a_string = list_of_strings[i + 1].Trim();
                }
                catch
                {
                    a_string = "";
                }

                //check location in structure - only read from entities section
                if (coder_string == "0" && a_string == "BLOCK")
                {
                    in_block = true;
                }
                if (coder_string == "2" && a_string == "ENTITIES")
                {
                    in_entities = true;
                }
                if (coder_string == "0" && a_string == "ENDBLK")
                {
                    in_block = false;
                }
                if (coder_string == "0" && a_string == "ENDSEC")
                {
                    in_entities = false;
                    final_entity = true;
                }

                //read in layer info
                if (coder_string == "8" && in_block == false && in_entities == true && (ReadingSpline==true|| ReadingLine == true || ReadingLWPolyline==true || ReadingPolyline == true || ReadingArc == true || ReadingCircle == true))
                {
                    layer_string = a_string.Trim();
                    bool bad_layer = false;
                    if(layer_string.Contains("\\"))
                    {
                        bad_layer = true;
                    }
                    if(layer_string.Contains("<"))
                    {
                        bad_layer = true;
                    }
                    if (layer_string.Contains(">"))
                    {
                        bad_layer = true;
                    }
                    if (layer_string.Contains("/"))
                    {
                        bad_layer = true;
                    }
                    if (layer_string.Contains(":"))
                    {
                        bad_layer = true;
                    }
                    if (layer_string.Contains(";"))
                    {
                        bad_layer = true;
                    }
                    if (layer_string.Contains("*"))
                    {
                        bad_layer = true;
                    }
                    if (layer_string.Contains("*"))
                    {
                        bad_layer = true;
                    }
                    if (layer_string.Contains("|"))
                    {
                        bad_layer = true;
                    }
                    if (layer_string.Contains(","))
                    {
                        bad_layer = true;
                    }
                    if (layer_string.Contains("="))
                    {
                        bad_layer = true;
                    }
                    if (layer_string.Contains("?"))
                    {
                        bad_layer = true;
                    }
                    if (layer_string.Contains("'"))
                    {
                        bad_layer = true;
                    }
                    if (layer_string.Contains("\""))
                    {
                        bad_layer = true;
                    }
                    if (layer_string.Contains("`"))
                    {
                        bad_layer = true;
                    }
                   ///ERROR: The following characters \<>/? ":;*|,=` are not supported for table object names. Parameter name: name
                    if (bad_layer)
                    {
                        layer_string = "BAD_LAYER_NAME_FOUND";
                    }
                    FoundNewLayer = true;
                }
                //read data
                if (coder_string == "10")
                {
                    double.TryParse(a_string, out intermediate);
                    X1 = (double?)intermediate;
                }

                if (coder_string == "11")
                {
                    double.TryParse(a_string, out intermediate);
                    X2 = (double?)intermediate;
                }

                if (coder_string == "20")
                {
                    double.TryParse(a_string, out intermediate);
                    Y1 = (double?)intermediate;
                }

                if (coder_string == "21")
                {
                    double.TryParse(a_string, out intermediate);
                    Y2 = (double?)intermediate;
                }

                if (coder_string == "40")
                {
                    double.TryParse(a_string, out intermediate);
                    radius = (double?)intermediate;

                    if (ReadingSpline && radius != null)
                    {
                        SplineKnots.Add((double)radius);
                    }
                }

                if (coder_string == "50")
                {
                    double.TryParse(a_string, out intermediate);
                    start_angle = (double?)intermediate;
                }

                if (coder_string == "51")
                {
                    double.TryParse(a_string, out intermediate);
                    end_angle = (double?)intermediate;
                }

                if (coder_string == "42")
                {

                    double.TryParse(a_string, out intermediate);
                    bulge = (double?)intermediate;

                    if (ReadingLWPolyline)
                    {
                        LastPolyVertex.Bulge = (double)bulge;
                        bulge = 0.0d;
                    }

                }

                if (coder_string == "70")
                {
                    if (ReadingLWPolyline == true || ReadingPolyline == true)
                    {
                        if (a_string == "1")
                        {
                            polyline_closed = true;
                        }
                        else
                        {
                            polyline_closed = false;
                        }
                    }

                    if (ReadingSpline == true)
                    {
                        if (a_string == "1")
                        {
                            spline_closed = true;
                        }
                        else
                        {
                            spline_closed = false;
                        }

                        if (a_string == "2")
                        {
                            spline_Periodic = true;
                        }
                        else
                        {
                            spline_Periodic = false;
                        }
                    }
                }
                if (coder_string == "71")
                {
                    if (ReadingSpline == true)
                    {
                        int.TryParse(a_string, out int_intermediate);
                        spline_degree = (int?)int_intermediate;
                    }

                }


                if (coder_string == "90")
                {
                    if (ReadingLWPolyline == true)
                    {
                        int.TryParse(a_string, out int_intermediate);
                        LwPointCount = (int?)int_intermediate;
                    }

                }

                //this code registers the start of a new entity, executes the adding of last entity if now complete
                if (coder_string == "0" || final_entity)
                {
                    if (final_entity)
                        final_entity = false;

                    //add line if data complete
                    if (ReadingLine == true && X1 != null && Y1 != null && X2 != null && Y2 != null)
                    {
                        start_point = new Vector2((double)X1, (double)Y1);
                        end_point = new Vector2((double)X2, (double)Y2);

                        netDxf.Entities.Line aLine = new netDxf.Entities.Line(start_point, end_point) { Layer = new netDxf.Tables.Layer(layer_string) };
                        //NEED TO ADD LAYER
                        dxfdata.AddEntity(aLine);
                        reset_logic();

                        entity_count++;
                    }

                    //add point to polyline
                    if (ReadingPolyline == true && X1 != null && Y1 != null)
                    {
                        netDxf.Entities.LwPolylineVertex aVertex = new netDxf.Entities.LwPolylineVertex((double)X1, (double)Y1, (double)bulge);
                        points.Add(aVertex);
                        X1 = null;
                        Y1 = null;
                        bulge = 0.0d;
                    }


                    //add LWpolyline if have enough points

                    if (ReadingLWPolyline == true && LWpoints.Count == LwPointCount && LwPointCount != null)
                    {
                        netDxf.Entities.LwPolyline aPolyline = new netDxf.Entities.LwPolyline(LWpoints, polyline_closed) { Layer = new netDxf.Tables.Layer(layer_string) };
                        //aPolyline.Layer.Name=layer_string;
                        dxfdata.AddEntity(aPolyline);
                        reset_logic();
                        entity_count++;
                    }

                    //add arc if data complete
                    if (ReadingArc == true && X1 != null && Y1 != null && radius != null && start_angle != null && end_angle != null)
                    {
                        Vector2 center = new Vector2((double)X1, (double)Y1);
                        netDxf.Entities.Arc aArc = new netDxf.Entities.Arc(center, (double)radius, (double)start_angle, (double)end_angle) { Layer = new netDxf.Tables.Layer(layer_string) };
                        //aArc.Layer.Name=layer_string;
                        dxfdata.AddEntity(aArc);
                        reset_logic();
                        entity_count++;
                    }

                    //add circle if data complete
                    if (ReadingCircle == true && X1 != null && Y1 != null && radius != null)
                    {
                        Vector2 center = new Vector2((double)X1, (double)Y1);
                        netDxf.Entities.Circle aCircle = new netDxf.Entities.Circle(center, (double)radius) { Layer = new netDxf.Tables.Layer(layer_string) };
                        //aCircle.Layer.Name=layer_string;
                        dxfdata.AddEntity(aCircle);
                        reset_logic();
                        entity_count++;
                    }


                    if (ReadingSpline == true)
                    {
                        netDxf.Entities.Spline aSpline;
                        if (SplineKnots != null && SplineKnots.Count > 0 && spline_degree != null)
                        {
                            aSpline = new netDxf.Entities.Spline(SplinePoints, SplineKnots, (short)spline_degree) { Layer = new netDxf.Tables.Layer(layer_string) };
                        }
                        else if (spline_degree != null)
                        {
                            aSpline = new netDxf.Entities.Spline(SplinePoints, (short)spline_degree, spline_Periodic) { Layer = new netDxf.Tables.Layer(layer_string) };
                        }
                        else
                        {
                            aSpline = new netDxf.Entities.Spline(SplinePoints, spline_Periodic) { Layer = new netDxf.Tables.Layer(layer_string) };
                        }


                        dxfdata.AddEntity(aSpline);
                        reset_logic();
                        entity_count++;
                    }

                }

                //add point to LWpolyline
                if (ReadingLWPolyline == true && X1 != null && Y1 != null)
                {
                    netDxf.Entities.LwPolylineVertex aVertex = new netDxf.Entities.LwPolylineVertex(new Vector2((double)X1, (double)Y1), (double)bulge);
                    LastPolyVertex = aVertex;
                    LWpoints.Add(aVertex);
                    X1 = null;
                    Y1 = null;
                    bulge = 0.0d;
                }

                //add point to spline
                if (ReadingSpline == true && X1 != null && Y1 != null)
                {
                    Vector2 V = new Vector2((double)X1, (double)Y1);
                    netDxf.Entities.SplineVertex sVertex = new netDxf.Entities.SplineVertex(V);
                    LastSplineVertex = sVertex;
                    SplinePoints.Add(sVertex);
                    X1 = null;
                    Y1 = null;
                    bulge = 0.0d;
                }


                //flag start of a line		       
                if (coder_string == "0" && a_string == "LINE" && in_block == false && in_entities == true)
                {
                    if (ReadingLine == false)
                    {
                        reset_logic();
                        ReadingLine = true;
                    }
                    else
                    {
                        reset_logic();
                    }
                }


                //flag start of a POLYLINE  
                if (coder_string == "0" && a_string == "POLYLINE" && in_block == false && in_entities == true)
                {
                    if (ReadingPolyline == false)
                    {
                        reset_logic();
                        ReadingPolyline = true;
                    }
                    else
                    {
                        reset_logic();
                    }
                }





                //add polyline once have reached SEQEND

                if (ReadingPolyline == true && coder_string == "0" && a_string == "SEQEND")
                {
                    points.Remove(points[0]);
                    netDxf.Entities.LwPolyline aPolyline = new netDxf.Entities.LwPolyline(points, polyline_closed) { Layer = new netDxf.Tables.Layer(layer_string) };
                    //aPolyline.Layer.Name=layer_string;
                    dxfdata.AddEntity(aPolyline);
                    reset_logic();
                    entity_count++;
                }



                //flag start of a LWPOLYLINE
                if (coder_string == "0" && a_string == "LWPOLYLINE" && in_block == false && in_entities == true)
                {
                    if (ReadingLWPolyline == false)
                    {
                        reset_logic();
                        ReadingLWPolyline = true;
                    }
                    else
                    {
                        reset_logic();
                    }
                }

                //flag start of a SPLINE
                if (coder_string == "0" && a_string == "SPLINE" && in_block == false && in_entities == true)
                {
                    if (ReadingSpline == false)
                    {
                        reset_logic();
                        ReadingSpline = true;
                    }
                    else
                    {
                        reset_logic();
                    }
                }





                //flag start of a ARC
                if (coder_string == "0" && a_string == "ARC" && in_block == false)
                {
                    if (ReadingArc == false)
                    {
                        reset_logic();
                        ReadingArc = true;
                    }
                    else
                    {
                        reset_logic();
                    }
                }


                //flag star of a CIRCLE
                if (coder_string == "0" && a_string == "CIRCLE" && in_block == false)
                {
                    if (ReadingArc == false)
                    {
                        reset_logic();
                        ReadingCircle = true;
                    }
                    else
                    {
                        reset_logic();
                    }
                }


            }

            return dxfdata;

        }

        public static DxfDocument LoadDXF(string filename)
        {
            //Only works with proper AutoCAD dxfs
            DxfDocument dxf_document = new DxfDocument();
            try
            {

                vector_list.Clear();
                entity_count = 0;
                dxf_document = DxfDocument.Load(filename);
                entity_count = dxf_document.LwPolylines.Count + dxf_document.Lines.Count
                    + dxf_document.Arcs.Count + dxf_document.Polylines.Count
                    + dxf_document.Circles.Count;  //must be a nice way to do this!

            }
            catch
            {
                dxf_document = null;
            }
            return dxf_document;
        }

        //extract mark vectors from DXF 

        public static List<Entity> Extract_Vectors(string filename, string layer_name)
        {
            List<Entity> list_of_vectors = new List<Entity>();
            DxfDocument dxf = LoadDXF(filename);
            DxfDocument new_dxf = new DxfDocument(); //storage for new objects after conversion
            List<netDxf.Entities.EntityObject> temp_entity_storage = new List<netDxf.Entities.EntityObject>();
            netDxf.Entities.Polyline temp_polyline= new netDxf.Entities.Polyline();

            number = 0;
            if (dxf == null)
            {
                //MessageBox.Show("netDXF extraction from DXF failed, trying line by line.");
                dxf = LineByLineLoader(filename);

            }
            if (dxf == null)
            {
                //System.Windows.Forms.MessageBox.Show("Line by line processing failed");
                list_of_vectors = null;
                //return list_of_vectors;
            }


            netDxf.Vector2 previous_point = new netDxf.Vector2(0.0d, 0.0d);
            netDxf.Entities.LwPolylineVertex avertex = new netDxf.Entities.LwPolylineVertex();
            netDxf.Entities.PolylineVertex bvertex = new netDxf.Entities.PolylineVertex();
            

            //convert Splines to polylines 
            foreach (netDxf.Entities.Spline spline in dxf.Splines)
            {
                if (spline.Layer.Name == layer_name || layer_name == "All")
                {
                    temp_polyline = spline.ToPolyline(spline.ControlPoints.Count * SplineInterpolationNumber);
                    temp_polyline.Layer.Name = spline.Layer.Name;
                    dxf.AddEntity(temp_polyline);
                }
            }


            //convert LWPolylines to Line and Arcs using EXPLODE 
            foreach (netDxf.Entities.LwPolyline apolyline in dxf.LwPolylines)
            {
                if (apolyline.Layer.Name == layer_name || layer_name == "All")
                {
                    temp_entity_storage = apolyline.Explode();
                    foreach (netDxf.Entities.EntityObject e in temp_entity_storage)
                    {
                        netDxf.Entities.EntityObject e_new = (netDxf.Entities.EntityObject)e.Clone();
                        e_new.Layer.Name = apolyline.Layer.Name;
                        dxf.AddEntity(e_new);
                    }
                }
            }

            //convert polylines to Lines 
            foreach (netDxf.Entities.Polyline apolyline in dxf.Polylines)
            {
                if (apolyline.Layer.Name == layer_name || layer_name == "All")
                {
                    temp_entity_storage = apolyline.Explode();
                    foreach (netDxf.Entities.EntityObject e in temp_entity_storage)
                    {
                        netDxf.Entities.EntityObject e_new = (netDxf.Entities.EntityObject)e.Clone();
                        e_new.Layer.Name = apolyline.Layer.Name;
                        dxf.AddEntity(e_new);
                    }
                }
            }

            //add all lines, arcs and circles to new_dxf	
            foreach (netDxf.Entities.Line aline in dxf.Lines)
            {
                if (aline.Layer.Name == layer_name || layer_name == "All")
                {
                    new_dxf.AddEntity((netDxf.Entities.Line)aline.Clone());
                }
            }

            foreach (netDxf.Entities.Circle acircle in dxf.Circles)
            {
                if (acircle.Layer.Name == layer_name || layer_name == "All")
                {
                    new_dxf.AddEntity((netDxf.Entities.Circle)acircle.Clone());
                }
            }

            foreach (netDxf.Entities.Arc aarc in dxf.Arcs)
            {
                if (aarc.Layer.Name == layer_name || layer_name == "All")
                {
                    new_dxf.AddEntity((netDxf.Entities.Arc)aarc.Clone());
                }
            }

            List<DxfObject> entities = new List<DxfObject>();

            //Convert to Entity

            if (layer_name == "All")
            {
                foreach (var layer in dxf.Layers)
                {
                    entities = new_dxf.Layers.GetReferences(layer.Name);
                    list_of_vectors.AddRange(ProcessEnitities(new_dxf, entities));
                }
            }
            else
            {
                entities = new_dxf.Layers.GetReferences(layer_name);
                list_of_vectors = ProcessEnitities(new_dxf, entities);

            }

            //MessageBox.Show(number.ToString());
            //			
            return list_of_vectors;
            //vector_list = list_of_vectors;

        }

        public static List<Entity> ProcessEnitities(DxfDocument new_dxf, List<DxfObject> entities)
        {


            List<Entity> list_of_vectors = new List<Entity>();

            if (entities == null)
                return list_of_vectors;

            foreach (DxfObject o in entities)
            {
                if (o.GetType() == typeof(netDxf.Entities.Line))
                {
                    netDxf.Entities.Line new_entity = o as netDxf.Entities.Line;
                    Entity aEntity = new Entity((float)new_entity.StartPoint.X, (float)new_entity.StartPoint.Y, (float)new_entity.EndPoint.X, (float)new_entity.EndPoint.Y,"LINE");
                    aEntity.layer = new_entity.Layer.Name;
                    list_of_vectors.Add(aEntity);
                    number++;
                }
                if (o.GetType() == typeof(netDxf.Entities.Circle))
                {
                    netDxf.Entities.Circle new_entity = o as netDxf.Entities.Circle;
                    number++;
                    Entity aEntity = new Entity((float)new_entity.Center.X, (float)new_entity.Center.Y, (float)new_entity.Radius,"CIRCLE");
                    aEntity.layer = new_entity.Layer.Name;
                    list_of_vectors.Add(aEntity);
                }
                if (o.GetType() == typeof(netDxf.Entities.Arc))
                {
                    netDxf.Entities.Arc new_entity = o as netDxf.Entities.Arc;
                    //new_dxf.AddEntity((netDxf.Entities.Arc)new_entity.Clone());

                    //switch start and end angles
                    double moddedStartangle = 360 - (new_entity.EndAngle);
                    double moddedEndangle = 360 - (new_entity.StartAngle);

                    //rescale if end angle is less than start
                    if (moddedEndangle < moddedStartangle)
                        moddedEndangle = moddedEndangle + 360f;

                    //makes sure no negative angles

                    while (moddedStartangle < 0)
                        moddedStartangle = 360f + moddedStartangle;

                    while (moddedEndangle < 0)
                        moddedEndangle = 360f + moddedEndangle;

                    Entity aEntity = new Entity((float)new_entity.Center.X, (float)new_entity.Center.Y, (float)new_entity.Radius, (float)moddedStartangle, (float)moddedEndangle,"ARC");
                    aEntity.layer = new_entity.Layer.Name;
                    list_of_vectors.Add(aEntity);
                }
            }

            return list_of_vectors;
        }
        #endregion

    }
}

