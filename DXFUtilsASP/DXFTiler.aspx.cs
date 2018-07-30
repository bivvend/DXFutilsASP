using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Data.SqlClient;

namespace DXFUtilsASP
{
    public partial class DXFTiler : System.Web.UI.Page
    {
        List<Entity> entity_list = new List<Entity>();
        string upload_location = @"D:\DXFutilswebsite\Uploads\";
        string script_location = @"D:\DXFutilswebsite\Tiling_Scripts";
        string python_location = @"C:\WinPython-64bit-3.4.2.4\python-3.4.2.amd64\python.exe";
        string python_path = @"C:\WinPython-64bit-3.4.2.4\python-3.4.2.amd64";
        string entity_file_storage = @"D:\DXFutilswebsite\Script_Storage\";
        string tile_set_storage_location = @"D:\DXFutilswebsite\Tile_Set_Storage\";
        List<string> current_layer_list = new List<string>();
        List<string> script_list = new List<string>();
        List<Tile> tile_list = new List<Tile>();
        int line_count = 0;
        int arc_count = 0;
        int circle_count = 0;
        int point_count = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            LabelWarn.Visible = false;
            string a_name = "";
            //this.ListBoxScripts.Items.Clear();
            //find available scripts
            bool new_item = true;
            if (!Page.IsPostBack)
            {
                DropDownListDXFVesrion.Items.Clear();
                DropDownListDXFVesrion.Items.Add("R12");
                DropDownListDXFVesrion.Items.Add("R2000");
                DropDownListDXFVesrion.Items.Add("R2004");
                DropDownListDXFVesrion.Items.Add("R2007");
                DropDownListDXFVesrion.Items.Add("R2010");
                DropDownListDXFVesrion.Items.Add("R2013");
                DropDownListDXFVesrion.Items.Add("Unstructured");
                //# AC1009	R12
                //# AC1015	R2000
                //# AC1018	R2004
                //# AC1021	R2007
                //# AC1024	R2010
                //# AC1027	R2013 
            }
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
                        foreach (ListItem item in ListBoxScripts.Items)
                        {
                            if (item.Value == a_name)
                                new_item = false;
                        }
                        if (new_item)
                            this.ListBoxScripts.Items.Add(a_name);
                    }

                }
            }
        }

        List<string> Get_Layer_List()
        {
            line_count = 0;
            arc_count = 0;
            circle_count = 0;
            point_count = 0;

            ListBoxLayers.Items.Clear();

            List<string> layer_list = new List<string>();
            foreach (Entity e in this.entity_list)
            {
                if (!layer_list.Contains(e.layer))
                {
                    layer_list.Add(e.layer);
                }
                if (e.type == "LINE")
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
            if (FileUploadDXF_TO_BMP.HasFile)
            {
                if (FileUploadDXF_TO_BMP.FileName.ToLower().Contains(".dxf"))
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
                        entity_list = DXFlibCS.Extract_Vectors(upload_location + FileUploadDXF_TO_BMP.FileName, "All");

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

        protected void ButtonPreview_Click(object sender, EventArgs e)
        {
            entity_list = (List<Entity>)Session["entity_list"];

            if (entity_list == null)
                return;

            //generate Tiles
             
            tile_list.Clear(); //just in case
            try
            {
                int number_in_x = Convert.ToInt32(TextBoxNumberX.Text);
                int number_in_y = Convert.ToInt32(TextBoxNumberY.Text);
                double pitch_x = Convert.ToDouble(TextBoxPitchX.Text);
                double pitch_y = Convert.ToDouble(TextBoxPitchY.Text);
                double center_x = Convert.ToDouble(TextBoxCenterX.Text);
                double center_y = Convert.ToDouble(TextBoxCenterY.Text);

                double tile_x = 0.0d;
                double tile_y = 0.0d;

                for (int x = 0; x < number_in_x; x++)
                {
                    for (int y = 0; y < number_in_y; y++)
                    {
                        //calculate positions of tiles
                        tile_x = (pitch_x * ((double)x - ((double)number_in_x / 2) + 0.5d)) + center_x;
                        tile_y = (pitch_y * ((double)y - ((double)number_in_y / 2) + 0.5d)) + center_y;
                        tile_list.Add(new Tile(tile_x, tile_y, pitch_x, pitch_y));
                    }
                }

                Session["tile_list"] = tile_list;
            }
            catch
            {

            }

            string selected_layer = "All";

            try
            {
                Session["selected_layer"] = ListBoxLayers.SelectedValue;
                LabelSelectedLayer.Text = @"Selected Layer = " + ListBoxLayers.SelectedValue;
                //TextBoxSelectedLayer.Text = ListBoxLayers.SelectedValue;
                selected_layer = ListBoxLayers.SelectedValue;

            }
            catch
            {
                Session["selected_layer"] = "All";
                LabelSelectedLayer.Text = @"Selected Layer = All";
            }

            if (this.entity_list.Count > 0)
            {
                DXF_Display_Control.Show_Bitmap(true);
            }
        }

        protected void ButtonRender_Click(object sender, EventArgs e)
        {
            Session["selected_script"] = script_location + @"\" + TextBoxSelectedScript.Text;
            string script = Session["selected_script"].ToString();
            string layer_name = TextBoxSelectedLayer.Text;

            if (script == null)
                return;
            string DXF_format = DropDownListDXFVesrion.SelectedValue;

            bool invert_x = CheckBoxInvertX.Checked;
            string invert_x_str = "False";
            if (invert_x)
                invert_x_str = "True";

            bool invert_y = CheckBoxInvertY.Checked;
            string invert_y_str = "False";
            if (invert_y)
                invert_y_str = "True";

            bool add_knots = CheckBoxKnots.Checked;
            string add_knots_str = "False";
            if (add_knots)
                add_knots_str = "True";

            bool convert_curves = CheckBoxConvertToChords.Checked;
            string convert_curves_str = "False";
            if (convert_curves)
                convert_curves_str = "True";

            string number_in_x = TextBoxNumberX.Text;
            string number_in_y = TextBoxNumberY.Text;

            string pitch_x = TextBoxPitchX.Text;
            string pitch_y = TextBoxPitchY.Text;

            string center_x = TextBoxCenterX.Text;
            string center_y= TextBoxCenterY.Text;

            string knot_type = TextBoxKnotType.Text;
            if (!add_knots)
                knot_type = "None";

            string knot_size = TextBoxKnotSize.Text;

            string root_output_filename = TextBoxOutputFilename.Text;

            string extend_length = TextBoxExtensionsMM.Text;

            string mscan_dxf_dir = TextBoxMScanDir.Text;

            if(Directory.Exists(tile_set_storage_location + root_output_filename))
            {
                LabelRenderWarning.Text = "Recipe directory already exists - Please rename recipe.";
                return;
            }

            if (File.Exists(script))
            {
                Save_Data("All");
                string args = script;  //sys.argv[0]
                //input_file = str(sys.argv[1])
                args += " " + "\"" + Session["data_file"].ToString() +"\"";
                // layer_name = str(sys.argv[2])
                args += " " + "\"" + layer_name + "\"";
                // number_in_x = int(sys.argv[3])
                args += " " + number_in_x;
                // number_in_y = int(sys.argv[4])
                args += " " + number_in_y;
                // tile_pitch_x = float(sys.argv[5])
                args += " " + pitch_x;
                // tile_pitch_y = float(sys.argv[6])
                args += " " + pitch_y;
                // center_x = float(sys.argv[7])
                args += " " + center_x;
                // center_y = float(sys.argv[8])
                args += " " + center_y;
                // root_output_filename = str(sys.argv[9])
                args += " " + "\"" + root_output_filename + "\"";
                // dxf_format = str(sys.argv[10])
                args += " " + DXF_format;
                // extend_length = float(sys.argv[11])
                args += " " + extend_length;
                // knot_style = str(sys.argv[12])
                args += " " + knot_type;
                // knot_size = float(sys.argv[13])
                args += " " + knot_size;
                // convert_to_chords = bool(sys.argv[14]=="True")
                args += " " + convert_curves_str;
                // invert_x = bool(sys.argv[15]=="True")
                args += " " + invert_x_str;
                // invert_y= bool(sys.argv[16]=="True")
                args += " " + invert_y_str;
                // TNx_type = bool(sys.argv[17])
                args += " " + "False";
                // output_dir = str(sys.argv[18])
                args += " " + "\"" + tile_set_storage_location + root_output_filename + "\"";
                //m_scan_dxf_dir = str(sys.argv[19])
                args += " " +  "\"" + mscan_dxf_dir + "\"";

                run_script(python_location, args);
            }
            else
            {
                LabelRenderWarning.Text = "Script not found";
                return;
            }
            try
            {
                if (TextBoxScriptOutput.Text.Contains("SCRIPT_SUCCESS"))
                {
                    LabelRenderWarning.Text = "SCRIPT_SUCCESS -  Render complete";
                    ButtonDownload.Visible = true;

                    using (SqlConnection con = new SqlConnection())
                    {
                        try
                        {

                            // Set Connection String property of Connection object..................
                            con.ConnectionString = TilerRecipeDB.ConnectionString;

                            // Open Connection..................
                            con.Open();

                            //Create object of Command Class................
                            SqlCommand cmd = new SqlCommand("INSERT INTO Tiler_Recipes(Name, Filepath, Created_By, Description, Date) VALUES(@name, @path, @created, @desc, @date)", con);
                            string Name = root_output_filename;
                            string Filepath = Session["output_file_name"].ToString();
                            string Created_By = User.Identity.Name.ToString();
                            string Description = "DXF Conversion    ";
                            string Date = DateTime.Today.ToString();

                            //Assign values as `parameter`. It avoids `SQL Injection`
                            cmd.Parameters.AddWithValue("@name", Name);
                            cmd.Parameters.AddWithValue("@path", Filepath);
                            cmd.Parameters.AddWithValue("@created", Created_By);
                            cmd.Parameters.AddWithValue("@desc", Description);
                            cmd.Parameters.AddWithValue("@date", Date);

                            cmd.ExecuteNonQuery();

                        }
                        catch (Exception ex)
                        {
                            LabelRenderWarning.Text = "SQL Write Error " + ex.ToString();

                        }

                    }

                }
                else
                {
                    LabelRenderWarning.Text = "SCRIPT_FAIL -  Render failed";
                    ButtonDownload.Visible = false;
                }
            }
            catch (Exception ex)
            {
                LabelRenderWarning.Text = ex.ToString();
            }
        }

        protected void ButtonDownload_Click(object sender, EventArgs e)
        {
            try
            {
                //LabelRenderWarning.Text = "SCRIPT_SUCCESS -  Render complete";
                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                response.ClearContent();
                response.Clear();
                response.ContentType = "application/octet-stream";
                string filepath = Session["output_file_name"].ToString();
                response.AddHeader("Content-Disposition", "attachment; filename=" + Session["root_of_filename"].ToString()+".zip" + ";");
                response.TransmitFile(filepath);
                response.Flush();
                response.End();
            }
            catch (Exception ex)
            {
                LabelRenderWarning.Text = "Download exception " + ex.ToString();
            }
        }

        protected void ButtonSelectScript_Click(object sender, EventArgs e)
        {
            try
            {
                Session["selected_script"] = script_location + @"\" + ListBoxScripts.SelectedValue;
                TextBoxSelectedScript.Text = ListBoxScripts.SelectedValue;
            }
            catch
            {

            }
        }

        private void run_script(string python_exe, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = python_exe;//full path to python.exe
            start.Arguments = args;//args is path to .py file and any cmd line args
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.WorkingDirectory = python_path;
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


        private void Save_Data(string layer_name)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string rand_name = new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            try
            {
                entity_list = (List<Entity>)Session["entity_list"];
                string filename = entity_file_storage + rand_name + ".csv";
                Session["data_file"] = filename;
                Session["output_file_name"] = tile_set_storage_location + TextBoxOutputFilename.Text + @"\" + TextBoxOutputFilename.Text + ".zip";
                Session["root_of_filename"] = TextBoxOutputFilename.Text;

                string to_write = "";

                FileStream fs = File.Open(filename, FileMode.Create);
                StreamWriter writer = new StreamWriter(fs);

                //get properties from Entity object
                PropertyInfo[] properties = null;


                foreach (Entity e in entity_list)
                {
                    if (e.layer == layer_name || layer_name == "All")
                    {
                        properties = e.GetType().GetProperties();
                        foreach (PropertyInfo pi in properties)
                        {
                            to_write += pi.GetValue(e, null).ToString() + ",";
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
    }
}