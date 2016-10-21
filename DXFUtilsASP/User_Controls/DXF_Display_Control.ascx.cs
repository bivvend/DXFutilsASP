using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.IO;

namespace DXFUtilsASP.User_Controls
{
    public partial class DXF_Display_Control : System.Web.UI.UserControl
    {
        int preview_width = 600;
        int preview_height = 600;
        double dxf_max_x = 0.0d;
        double dxf_max_y = 0.0d;
        double dxf_min_x = 0.0d;
        double dxf_min_y = 0.0d;
        List<Entity> entity_list = new List<Entity>();
        List<Tile> tile_list = new List<Tile>();        

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void Show_Bitmap(bool show_grid)
        {

            Bitmap bitMap = new Bitmap(preview_width,preview_height);
            Calculate_Scale();
            try
            {
                
                bitMap = Make_Preview_BMP(Session["selected_layer"].ToString(),show_grid);
            }
            catch
            {
                bitMap = Make_Preview_BMP("All", show_grid);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = ms.ToArray();

                Convert.ToBase64String(byteImage);
                ImagePreview.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
            }
        }

        private void Calculate_Scale()
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

            entity_list = (List<Entity>)Session["entity_list"];

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
                //POINTS
                if (e.type == "POINT")
                {
                    if (e.x_center > SearchXMax)
                        SearchXMax = e.x_center;
                    if (e.y_center > SearchYMax)
                        SearchYMax = e.y_center;
                    if (e.x_center < SearchXMin)
                        SearchXMin = e.x_center;
                    if (e.y_center < SearchYMin)
                        SearchYMin = e.y_center;
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

                    sweep = end_angle - start_angle;

                    while (sweep < 0.0d)
                    {
                        end_angle += 360.0f;
                        sweep = end_angle - start_angle;
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
        private Bitmap Make_Preview_BMP(string selected_layer, bool show_grid)
        {
            //read back session data
            dxf_max_x = (double)Session["dxf_max_x"];
            dxf_max_y = (double)Session["dxf_max_y"];
            dxf_min_x = (double)Session["dxf_min_x"];
            dxf_min_y = (double)Session["dxf_min_y"];

            if(show_grid)
            {
                tile_list = (List<Tile>)Session["tile_list"];
            }
            else
            {
                tile_list = new List<Tile>();
            }

            entity_list = (List<Entity>)Session["entity_list"];

            Bitmap a_bmp = new Bitmap(preview_width, preview_height);
            double scale_x = 1.0f;
            double scale_y = 1.0f;
            double x1 = 0.0d;
            double x2 = 0.0d;
            double y1 = 0.0d;
            double y2 = 0.0d;
            double width = 0.0d;
            double scaled_width = 0.0d;
            double scaled_height = 0.0d;
            double height = 0.0d;

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
            Pen orangePen = new Pen(Color.DarkOrange, 0.1f);

            // Draw line to screen.
            using (var graphics = Graphics.FromImage(a_bmp))
            {
                foreach (Entity e in entity_list)
                {
                    if (e.layer == selected_layer || selected_layer == "All")
                    {
                        if (e.type == "LINE")
                        {
                            x1 = (e.x_start - center_x) * scale_x + preview_width / 2;
                            y1 = preview_height - ((e.y_start - center_y) * scale_y) - preview_height / 2;
                            x2 = ((e.x_end - center_x) * scale_x) + preview_width / 2;
                            y2 = preview_height - ((e.y_end - center_y) * scale_y) - preview_height / 2;
                            graphics.DrawLine(whitePen, (float)x1, (float)y1, (float)x2, (float)y2);

                        }

                        if (e.type == "POINT")
                        {
                            x1 = (e.x_center - center_x) * scale_x + preview_width / 2;
                            y1 = preview_height - ((e.y_center - center_y) * scale_y) - preview_height / 2;
                            graphics.FillRectangle(Brushes.White, new Rectangle((int)x1, (int)y1, 1, 1));


                        }

                        if (e.type == "CIRCLE")
                        {
                            x1 = (e.x_center - center_x) * scale_x + preview_width / 2;
                            y1 = preview_height - ((e.y_center - center_y) * scale_y) - preview_height / 2;
                            radius = e.radius;
                            graphics.DrawEllipse(whitePen, (float)(x1 - radius * scale_x), (float)(y1 - radius * scale_y), 2.0f * (float)(radius * scale_x), 2.0f * (float)(radius * scale_y));

                        }

                        if (e.type == "ARC")
                        {
                            string layer_current = e.layer;
                            x1 = (e.x_center - center_x) * scale_x + preview_width / 2;
                            y1 = preview_height - ((e.y_center - center_y) * scale_y) - preview_height / 2;
                            radius = e.radius;

                            //Draw arc uses CW  and degrees  -  so 360 -  then swap end and starts  - Process entities in DXFUtilsCs should already have handled ang_dir 
                            initial_start_angle = e.start_angle;
                            initial_end_angle = e.end_angle;

                            start_angle = 360.0d - initial_end_angle;  //Note swap....
                            end_angle = 360.0d - initial_start_angle;
                            if (end_angle < start_angle)
                                sweep = (end_angle + 360.0) - start_angle;
                            else
                                sweep = end_angle - start_angle;

                            graphics.DrawArc(whitePen, (float)(x1 - radius * scale_x), (float)(y1 - radius * scale_y), 2.0f * (float)(radius * scale_x), 2.0f * (float)(radius * scale_y), (float)start_angle, (float)sweep);
                        }
                    }
                }

                //Draw grid
                foreach(Tile a_tile in tile_list)
                {
                    width = a_tile.width;
                    scaled_width = width * scale_x;                    
                    height = a_tile.height;
                    scaled_height = height * scale_y;
                    x1 = (a_tile.center_x - center_x -( width / 2.0d)) * scale_x + preview_width / 2;
                    y1 = preview_height - ((a_tile.center_y - center_y + (height/2.0d)) * scale_y) - preview_height / 2;

                    graphics.DrawRectangle(orangePen, (float)x1, (float)y1, (float)scaled_width, (float)scaled_height);

                }

            }
            whitePen.Dispose();
            orangePen.Dispose();
            return a_bmp;

        }



    }

}