using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace DXFUtilsASP
{
    public partial class DXFtoBMP : System.Web.UI.Page
    {
        List<Entity> entity_list = new List<Entity>();
        string upload_location = @"C:\DXFutilswebsite\Uploads\";
        string script_location = @"C:\DXFutilswebsite\Scripts";
        string python_location = @"C:\Python\Python34-64bit\WinPython-64bit-3.4.2.4\python-3.4.2.amd64\python.exe";
        string entity_file_storage  = @"C:\DXFutilswebsite\Script_Storage\";
        List<string> current_layer_list = new List<string>();
        List<string> script_list = new List<string>();
        int line_count = 0;
        int arc_count = 0;
        int circle_count = 0;
        int point_count = 0;


        double dxf_max_x = 10.0d;
        double dxf_max_y = 10.0d;
        double dxf_min_x = -10.0d;
        double dxf_min_y = -10.0d;
        int angle_dir = 0;

        int preview_width = 600; //overridden later
        int preview_height = 600;



        protected void Page_Load(object sender, EventArgs e)
        {
            LabelWarn.Visible = false;
            string a_name = "";
            //this.ListBoxScripts.Items.Clear();
            //find available scripts
            bool new_item = true;
            if (Directory.Exists(script_location))
            {
                script_list = Directory.GetFiles(script_location).ToList();
                foreach (string a_path in script_list)
                {
                    new_item = true;
                    FileInfo info = new FileInfo(a_path);
                    if (info.Name.Contains(".py"))
                    {
                        a_name = info.Name;
                        foreach(ListItem item in ListBoxScripts.Items)
                        {
                            if (item.Value == a_name)
                                new_item = false;
                        }
                        if(new_item)
                            this.ListBoxScripts.Items.Add(a_name);
                    }

                }
            }

            //try
            //{
            //    TextBoxSelectedScript.Text = Session["selected_script"].ToString();
            //}
            //catch
            //{

            //}
        }

        List<string> Get_Layer_List()
        {
            line_count = 0;
            arc_count = 0;
            circle_count = 0;
            point_count = 0;

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
                if (e.type == "POINT")
                {
                    point_count++;
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
                        point_count = 0;

                        //extract data from current entity set
                        current_layer_list = Get_Layer_List();

                        //add layer "All"

                        ListBoxLayers.Items.Add(new ListItem("All"));

                        foreach (string layer_name in current_layer_list)
                        {
                            ListBoxLayers.Items.Add(new ListItem(layer_name));
                        }
                        
                        ListBoxLayers.SelectedIndex = 0;

                        BulletedListDXFInfo.Items.Clear();
                        BulletedListDXFInfo.Items.Add(new ListItem(@"Number of LAYERS: " + current_layer_list.Count.ToString()));
                        BulletedListDXFInfo.Items.Add(new ListItem(@"Number of LINES: " + line_count.ToString()));
                        BulletedListDXFInfo.Items.Add(new ListItem(@"Number of ARCS: " + arc_count.ToString()));
                        BulletedListDXFInfo.Items.Add(new ListItem(@"Number of CIRCLES: " + circle_count.ToString()));
                        BulletedListDXFInfo.Items.Add(new ListItem(@"Number of POINTS: " + point_count.ToString()));
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
                //POINTS
                if (e.type == "POINT")
                {
                    if (e.x_center > SearchXMax)
                        SearchXMax = e.x_center;
                    if (e.y_center> SearchYMax)
                        SearchYMax = e.y_center;
                    if (e.x_center< SearchXMin)
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

            if (entity_list == null)
                return;

            string selected_layer = "All";

            try
            {
                Session["selected_layer"] = ListBoxLayers.SelectedValue;
                LabelSelectedLayer.Text = @"Selected Layer = " + ListBoxLayers.SelectedValue;
                TextBoxSelectedLayer.Text = ListBoxLayers.SelectedValue;
                selected_layer = ListBoxLayers.SelectedValue;

            }
            catch
            {
                Session["selected_layer"] = "All";
                LabelSelectedLayer.Text = @"Selected Layer = All";
            }

            if (this.entity_list.Count>0)
            {
                Calculate_Scale();

                Bitmap bitMap = Make_Preview_BMP(selected_layer);

                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();

                    Convert.ToBase64String(byteImage);
                    ImagePreview.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }

            }
        }

        public Bitmap Make_Preview_BMP(string selected_layer)
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
                    if (e.layer == selected_layer || selected_layer == "All")
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
                                //drawing is upside down, so need to reflect arc in x axis
                                start_angle = 360.0d - start_angle;
                                end_angle = 360.0d - end_angle;

                                sweep = end_angle - start_angle;
                            }

                            graphics.DrawArc(whitePen, (float)(x1 - radius * scale_x), (float)(y1 - radius * scale_y), 2.0f * (float)(radius * scale_x), 2.0f * (float)(radius * scale_y), (float)start_angle, (float)sweep);
                        }
                    }
                }

            }
            whitePen.Dispose();
            return a_bmp;

        }

        protected void ButtonSelectScript_Click(object sender, EventArgs e)
        {
            try
            {
                Session["selected_script"] = script_location +@"\" + ListBoxScripts.SelectedValue;
                TextBoxSelectedScript.Text = ListBoxScripts.SelectedValue;
            }
            catch
            {
                
            }
        }

        private void Save_Data(string layer_name)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string rand_name = new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            bool scale_to_all = CheckBoxScaleToAll.Checked;
            try
            {
                entity_list = (List<Entity>)Session["entity_list"];
                string filename = entity_file_storage + rand_name + ".csv";
                Session["data_file"] = filename;
                Session["output_file_name"] = rand_name + ".tiff";
                string to_write = "";

                FileStream fs = File.Open(filename, FileMode.Create);
                StreamWriter writer = new StreamWriter(fs);

                //get properties from Entity object
                PropertyInfo[] properties = null;
                    
                
                foreach (Entity e in entity_list)
                {
                    if (e.layer == layer_name || layer_name == "All" || scale_to_all)
                    {
                        properties = e.GetType().GetProperties();
                        foreach (PropertyInfo pi in properties)
                        {
                            to_write += pi.GetValue(e, null).ToString() + "," ;
                        }

                        writer.WriteLine(to_write);
                        to_write = "";
                    }                    
                }

                writer.Flush();
                writer.Close();
                fs.Close();
            }

            catch
            {
                return;
            }
        }


        private void run_script(string python_exe, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = python_exe;//full path to python.exe
            start.Arguments = args;//args is path to .py file and any cmd line args
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            TextBoxScriptOutput.Text = "";
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    TextBoxScriptOutput.Text = result;
                }
            }
        }

    
        protected void ButtonRender_Click(object sender, EventArgs e)
        {
            Session["selected_script"] = script_location + @"\" + TextBoxSelectedScript.Text;
            string script = Session["selected_script"].ToString();
            double dpi_x = Convert.ToDouble(TextBoxDPIX.Text);
            double dpi_y = Convert.ToDouble(TextBoxDPIY.Text);
            string layer_name = TextBoxSelectedLayer.Text;
            double border = Convert.ToDouble(TextBoxBorder.Text);

            if (script == null)
                return;
            if (dpi_x <= 0.0d || dpi_y < 0.0d || dpi_x > 20000.0d || dpi_y > 20000.0d || border <= 0.0d )
            {
                LabelWarn.Text = "Data incorrectly set - Out of bounds.";
                return;
            }

            bool invert_x = CheckBoxInvertX.Checked;
            bool invert_y = CheckBoxInvertY.Checked;
            bool invert_colour = CheckBoxInvertColor.Checked;

            if (File.Exists(script))
            {
                Save_Data(layer_name);
                string args = script;  //sys.argv[0]
                //input_file = str(sys.argv[1])
                args += " " + Session["data_file"].ToString();
                //layer_name = str(sys.argv[2])
                args += " " + layer_name;
                //DPI_x = float(sys.argv[3])
                args += " " + dpi_x.ToString();
                //DPI_y = float(sys.argv[4])
                args += " " + dpi_y.ToString();
                //output_filename = str(sys.argv[5])
                args += " " + entity_file_storage + @"\" + Session["output_file_name"];
                //invert_x = bool(sys.argv[6] == "True")
                if (CheckBoxInvertX.Checked)
                    args += " " + "True";
                else
                    args += " " + "False";
                //invert_y = bool(sys.argv[7] == "True")
                if (CheckBoxInvertY.Checked)
                    args += " " + "True";
                else
                    args += " " + "False";
                //invert_colour = bool(sys.argv[8] == "True")
                if (CheckBoxInvertColor.Checked)
                    args += " " + "True";
                else
                    args += " " + "False";
                //border = float(sys.argv[9])
                args += " " + (Convert.ToDouble(TextBoxBorder.Text)).ToString();
                //scale_to_all = bool(sys.argv[10] == "True")
                if (CheckBoxScaleToAll.Checked)
                    args += " " + "True";
                else
                    args += " " + "False";

                run_script(python_location, args);
            }
            else
            {
                LabelRenderWarning.Text = "Script not found";
                return;
            }
            try
            {
                LabelRenderWarning.Text = "Render complete";
            }
            catch(Exception ex)
            {
                LabelRenderWarning.Text = ex.ToString();
            }

        }
    }
}