using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DXFUtilsASP
{
    public partial class DXFtoBMP : System.Web.UI.Page
    {
        List<Entity> entity_list = new List<Entity>();
        string upload_location = @"C:\DXFutilswebsite\Uploads\";
        List<string> current_layer_list = new List<string>();
        int line_count = 0;
        int arc_count = 0;
        int circle_count = 0;


        double dxf_max_x = 10.0d;
        double dxf_max_y = 10.0d;
        double dxf_min_x = -10.0d;
        double dxf_min_y = -10.0d;
        int angle_dir = 0;


        int preview_width = 1600; //overridden later
        int preview_height = 1600; 

            

        protected void Page_Load(object sender, EventArgs e)
        {
            LabelWarn.Visible = false;
      
        }

        List<string> Get_Layer_List()
        {
            line_count = 0;
            arc_count = 0;
            circle_count = 0;

            ListBoxLayers.Items.Clear();

            List<string> layer_list = new List<string>();
            foreach(Entity e in this.entity_list)
            {
                if(!layer_list.Contains(e.layer))
                {
                    layer_list.Add(e.layer);
                }
                if(e.type=="LINE")
                {
                    line_count++;
                }
                if (e.type == "ARC")
                {
                    arc_count++;
                }
                if (e.type == "CIRCLE")
                {
                    circle_count++;
                }
            }
            return layer_list;
        }
        protected void ButtonUploadToServer_Click(object sender, EventArgs e)
        {
            LabelWarn.Visible = false;
            if(FileUploadDXF_TO_BMP.HasFile)
            {
                if (FileUploadDXF_TO_BMP.FileName.Contains(".dxf"))
                {
                    try
                    {
                        FileUploadDXF_TO_BMP.SaveAs(upload_location +
                             FileUploadDXF_TO_BMP.FileName);
                        LabelWarn.Text = "File name: " +
                            FileUploadDXF_TO_BMP.PostedFile.FileName + "<br>" +
                             FileUploadDXF_TO_BMP.PostedFile.ContentLength + " kb<br>" +
                             "Content type: " +
                            FileUploadDXF_TO_BMP.PostedFile.ContentType;
                        LabelWarn.Visible = true;
                        //Convert DXF to entity list
                        entity_list = DXFlibCS.Extract_Vectors(upload_location + FileUploadDXF_TO_BMP.FileName,"All");
                        
                        //Store in session
                        Session["entity_list"] = entity_list;

                        LabelNoEntities.Text = @"Number of Entities: " + entity_list.Count;

                        line_count = 0;
                        arc_count = 0;
                        circle_count = 0;

                        //extract data from current entity set
                        current_layer_list = Get_Layer_List();

                        foreach(string layer_name in current_layer_list)
                        {
                            ListBoxLayers.Items.Add(new ListItem(layer_name));
                        }
                        //add layer "All"

                        ListBoxLayers.Items.Add(new ListItem("All"));

                        BulletedListDXFInfo.Items.Clear();
                        BulletedListDXFInfo.Items.Add(new ListItem(@"Number of LAYERS: " + current_layer_list.Count.ToString()));
                        BulletedListDXFInfo.Items.Add(new ListItem(@"Number of LINES: " + line_count.ToString()));
                        BulletedListDXFInfo.Items.Add(new ListItem(@"Number of ARCS: " + arc_count.ToString()));
                        BulletedListDXFInfo.Items.Add(new ListItem(@"Number of CIRCLES: " + circle_count.ToString()));
                    }
                    catch (Exception ex)
                    {
                        LabelWarn.Text = "ERROR: " + ex.Message.ToString();
                        LabelWarn.Visible = true;
                    }
                }
                else
                {
                    LabelWarn.Text = "Not a DXF file.";
                    LabelWarn.Visible = true;
                }
            }
            else
            {
                LabelWarn.Visible = true;
                LabelWarn.Text = "No DXF file selected";
            }


        }

        public void Calculate_Scale()
        {
            float SearchXMin = 1000000000.0f;
            float SearchYMin = 1000000000.0f;
            float SearchXMax = -1000000000.0f;
            float SearchYMax = -1000000000.0f;

            float x1 = 0.0f;
            float y1 = 0.0f;
            float start_angle = 0.0f;
            float end_angle = 0.0f;
            float sweep = 0.0f;
            float radius = 0.0f;
            float angle_step = 1.0f;
            float start_x = 0.0f;
            float start_y = 0.0f;
            float added_angle = 0.0f;
            float new_x = 0.0f;
            float new_y = 0.0f;

            foreach (Entity e in entity_list)
            {
                //LINES
                if (e.type == "LINE")
                {

                    if (e.x_start > SearchXMax)
                        SearchXMax = e.x_start;
                    if (e.x_end > SearchXMax)
                        SearchXMax = e.x_end;
                    if (e.y_start > SearchYMax)
                        SearchYMax = e.y_start;
                    if (e.y_end > SearchYMax)
                        SearchYMax = e.y_end;
                    if (e.x_start < SearchXMin)
                        SearchXMin = e.x_start;
                    if (e.x_end < SearchXMin)
                        SearchXMin = e.x_end;
                    if (e.y_start < SearchYMin)
                        SearchYMin = e.y_start;
                    if (e.y_end < SearchYMin)
                        SearchYMin = e.y_end;
                }

                //    # CIRCLES

                if (e.type == "CIRCLE")
                {
                    if (e.x_center + e.radius > SearchXMax)
                        SearchXMax = e.x_center + e.radius;
                    if (e.y_center + e.radius > SearchYMax)
                        SearchYMax = e.y_center + e.radius;
                    if (e.x_center - e.radius < SearchXMin)
                        SearchXMin = e.x_center - e.radius;
                    if (e.y_center - e.radius < SearchYMin)
                        SearchYMin = e.y_center - e.radius;
                }
                //    # ARCS   -  more complex than circles        
                if (e.type == "ARC")
                {

                    sweep = 0.0f;
                    x1 = e.x_center;
                    y1 = e.y_center;
                    start_angle = e.start_angle;
                    end_angle = e.end_angle;


                    if (angle_dir == 0)
                    {
                        sweep = end_angle - start_angle;
                        if (sweep < 0)
                            sweep = 360.0f + sweep;
                    }


                    if (angle_dir == 1)
                    {
                        start_angle = start_angle - 360.0f;
                        end_angle = end_angle - 360.0f;
                        sweep = 360.0f + (end_angle - start_angle);
                        if (sweep > 360.0)
                            sweep = sweep - 360.0f;
                    }

                    radius = e.radius;
                    //#start point
                    start_x = x1 + radius * (float)Math.Cos(Math.PI * start_angle / 180.0f);
                    start_y = y1 + radius * (float)Math.Sin(Math.PI * start_angle / 180.0f);


                    if (start_x > SearchXMax)
                        SearchXMax = start_x;
                    if (start_y > SearchYMax)
                        SearchYMax = start_y;
                    if (start_x < SearchXMin)
                        SearchXMin = start_x;
                    if (start_y < SearchYMin)
                        SearchYMin = start_y;


                    added_angle = 0.0f;


                    //# move around arc checking..
                    while (added_angle < sweep)
                    {
                        added_angle += angle_step;
                        new_x = x1 + radius * (float)Math.Cos(Math.PI * (start_angle + added_angle) / 180.0f);
                        new_y = y1 + radius * (float)Math.Sin(Math.PI * (start_angle + added_angle) / 180.0f);
                        if (added_angle > sweep)
                        {
                            //  #ensure accurate end-point
                            new_x = x1 + radius * (float)Math.Cos(Math.PI * (start_angle + sweep) / 180.0f);
                            new_y = y1 + radius * (float)Math.Sin(Math.PI * (start_angle + sweep) / 180.0f);
                        }

                        if (new_x > SearchXMax)
                            SearchXMax = new_x;
                        if (new_y > SearchYMax)
                            SearchYMax = new_y;
                        if (new_x < SearchXMin)
                            SearchXMin = new_x;
                        if (new_y < SearchYMin)
                            SearchYMin = new_y;
                    }
                }
            }

            dxf_max_x = (double)SearchXMax;
            dxf_max_y = (double)SearchYMax;
            dxf_min_x = (double)SearchXMin;
            dxf_min_y = (double)SearchYMin;

            Session["dxf_max_x"] = dxf_max_x;
            Session["dxf_max_y"] = dxf_max_y;
            Session["dxf_min_x"] = dxf_min_x;
            Session["dxf_min_y"] = dxf_min_y;

        }

        protected void ButtonPreview_Click(object sender, EventArgs e)
        {
            entity_list= (List<Entity>)Session["entity_list"];            

            if (this.entity_list.Count>0)
            {
                Calculate_Scale();

                Bitmap bitMap = Make_Preview_BMP();

                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();

                    Convert.ToBase64String(byteImage);
                    ImagePreview.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }

            }
        }

        public Bitmap Make_Preview_BMP()
        {
            //read back session data
            dxf_max_x = (double)Session["dxf_max_x"];
            dxf_max_y = (double)Session["dxf_max_y"];
            dxf_min_x = (double)Session["dxf_min_x"];
            dxf_min_y = (double)Session["dxf_min_y"];

            entity_list  = (List<Entity>)Session["entity_list"];

            Bitmap a_bmp = new Bitmap(preview_width, preview_height);
            double scale_x = 1.0f;
            double scale_y = 1.0f;
            double x1 = 0.0d;
            double x2 = 0.0d;
            double y1 = 0.0d;
            double y2 = 0.0d;

            double center_x = (dxf_max_x + dxf_min_x) / 2.0d;
            double center_y = (dxf_max_y + dxf_min_y) / 2.0d;

            double radius = 0.0d;
            double start_angle = 0.0d;
            double end_angle = 0.0d;
            double sweep = 0.0d;

            double initial_start_angle = 0.0d;
            double initial_end_angle = 0.0d;



            if (Math.Abs(dxf_max_x - dxf_min_x) > 0)
            {
                scale_x = ((double)preview_width * 0.9d) / (dxf_max_x - dxf_min_x);
            }
            if (Math.Abs(dxf_max_y - dxf_min_y) > 0)
            {
                scale_y = ((double)preview_height * 0.9d) / (dxf_max_y - dxf_min_y);
            }

            Pen whitePen = new Pen(Color.White, 0.1f);

            // Draw line to screen.
            using (var graphics = Graphics.FromImage(a_bmp))
            {
                foreach (Entity e in entity_list)
                {
                    if (e.type == "LINE")
                    {
                        x1 = (e.x_start - center_x) * scale_x + preview_width / 2;
                        y1 = preview_height - ((e.y_start - center_y) * scale_y) - preview_height / 2;
                        x2 = ((e.x_end - center_x) * scale_x) + preview_width / 2;
                        y2 = preview_height - ((e.y_end - center_y) * scale_y) - preview_height / 2;
                        graphics.DrawLine(whitePen, (float)x1, (float)y1, (float)x2, (float)y2);


                        if (y1 < 0 || y2 < 0)
                            angle_dir = 0;

                    }

                    if(e.type =="CIRCLE")
                    {
                        x1 = (e.x_center- center_x) * scale_x + preview_width / 2;
                        y1 = preview_height - ((e.y_center - center_y) * scale_y) - preview_height / 2;
                        radius = e.radius;
                        graphics.DrawEllipse(whitePen, (float)(x1 - radius * scale_x), (float)(y1 - radius * scale_y), 2.0f * (float)(radius * scale_x), 2.0f * (float)(radius * scale_y));

                    }

                    if(e.type=="ARC")
                    {
                        string layer_current = e.layer;
                        x1 = (e.x_center - center_x) * scale_x + preview_width / 2;
                        y1 = preview_height - ((e.y_center - center_y) * scale_y) - preview_height / 2;
                        radius = e.radius;

                        //Draw arc uses CW  and degrees
                        initial_start_angle = e.start_angle;
                        initial_end_angle = e.end_angle;

                        start_angle = e.start_angle;
                        end_angle = e.end_angle;

                        if (angle_dir == 0)
                        {
                            //Angle dir ==0 = CCW definition of angles so need to convert to CW
                            start_angle = 360.0d - start_angle;
                            end_angle = 360.0d - end_angle;

                            //drawing is upside down, so need to reflect arc in x axis
                            start_angle = 360.0d - start_angle;
                            end_angle = 360.0d - end_angle;

                            sweep = end_angle - start_angle;

                        }
                        else
                        {
                            //Angle dir ==1 = CW definition of angles 
                            sweep = end_angle - start_angle;
                        }

                        graphics.DrawArc(whitePen, (float)(x1 - radius * scale_x), (float)(y1 - radius * scale_y), 2.0f * (float)(radius * scale_x), 2.0f * (float)(radius*scale_y), (float)start_angle,(float)sweep );
                    }
                }

            }
            whitePen.Dispose();
            return a_bmp;

        }
    }
}