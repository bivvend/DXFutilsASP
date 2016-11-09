using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace DXFUtilsASP
{
    public partial class TNxTiler : System.Web.UI.Page
    {
        List<Entity> entity_list = new List<Entity>();
        string upload_location = @"C:\DXFutilswebsite\Uploads\";
        string script_location = @"C:\DXFutilswebsite\Tiling_Scripts";
        string python_location = @"C:\Python\Python35-64bit\python.exe";
        string python_path = @"C:\Python\Python35-64bit";
        string entity_file_storage = @"C:\DXFutilswebsite\Script_Storage\";
        string tile_set_storage_location = @"C:\DXFutilswebsite\Tile_Set_Storage\";
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
                if (FileUploadDXF_TO_BMP.FileName.Contains(".dxf"))
                {
                    try
                    {
                        FileUploadDXF_TO_BMP.SaveAs(upload_location +
                             FileUploadDXF_TO_BMP.FileName);
                        Session["uploaded_dxf_location"] = upload_location +
                             FileUploadDXF_TO_BMP.FileName;
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

        private void Save_Data(string layer_name)
        {
            try
            {
                Session["output_file_name"] = tile_set_storage_location + TextBoxOutputFilename.Text + @"\" + TextBoxOutputFilename.Text + ".zip";
                Session["root_of_filename"] = TextBoxOutputFilename.Text;
            }
            catch
            {
                return;
            }
        }

        protected void ButtonRender_Click(object sender, EventArgs e)
        {
            Session["selected_script"] = script_location + @"\" + TextBoxSelectedScript.Text;
            string script = Session["selected_script"].ToString();
            

            if (script == null)
                return;

            //input_file = str(sys.argv[1])
            string input_file = Session["uploaded_dxf_location"].ToString();
            //layer_string = str(sys.argv[2])
            string layer_name = TextBoxSelectedLayer.Text;
            //root_output_filename = str(sys.argv[3])
            string root_output_filename = TextBoxOutputFilename.Text;
            //dxf_format = str(sys.argv[4])
            string DXF_format = DropDownListDXFVesrion.SelectedValue;
            //invert_x = bool(sys.argv[5] == "True")
            bool invert_x = CheckBoxInvertX.Checked;
            string invert_x_str = "False";
            if (invert_x)
                invert_x_str = "True";
            //invert_y = bool(sys.argv[6] == "True")
            bool invert_y = CheckBoxInvertY.Checked;
            string invert_y_str = "False";
            if (invert_y)
                invert_y_str = "True";
            //output_dir = str(sys.argv[7])
            string output_dir = tile_set_storage_location + root_output_filename;
            //m_scan_dxf_dir = str(sys.argv[8])   #location where MScan looks for files
            string mscan_dxf_dir = TextBoxMScanDir.Text;
            //lines_only = bool(sys.argv[9] == "True")
            bool convert_lines = CheckBoxConvertToLines.Checked;
            string convert_lines_str = "False";
            if (convert_lines)
                convert_lines_str = "True";


            if (Directory.Exists(output_dir))
            {
                LabelRenderWarning.Text = "Recipe directory already exists - Please rename recipe.";
                return;
            }

            if (File.Exists(script))
            {
                Save_Data("All");
                string args = script;  //sys.argv[0]
                //input_file = str(sys.argv[1])
                args += " " + input_file;
                //layer_string = str(sys.argv[2])
                args += " " + layer_name;
                //root_output_filename = str(sys.argv[3])
                args += " " + root_output_filename;
                //dxf_format = str(sys.argv[4])
                args += " " + DXF_format;
                //invert_x = bool(sys.argv[5] == "True")
                args += " " + invert_x_str;
                //invert_y = bool(sys.argv[6] == "True")
                args += " " + invert_y_str;
                //output_dir = str(sys.argv[7])
                args += " " + output_dir;
                //m_scan_dxf_dir = str(sys.argv[8])   #location where MScan looks for files
                args += " " + mscan_dxf_dir;
                //lines_only = bool(sys.argv[9] == "True")
                args += " " + convert_lines_str;

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
                response.AddHeader("Content-Disposition", "attachment; filename=" + Session["root_of_filename"].ToString() + ".zip" + ";");
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

        protected void ButtonAddLayer_Click(object sender, EventArgs e)
        {
            string layer_to_add = ListBoxLayers.SelectedValue;
            if (layer_to_add == null || layer_to_add == "")
                return;
            else
            {
                if (TextBoxSelectedLayer.Text.Trim() == "" || TextBoxSelectedLayer.Text.Contains("All"))
                {
                    TextBoxSelectedLayer.Text = layer_to_add;
                }
                else
                {
                    if(!TextBoxSelectedLayer.Text.Contains(layer_to_add))
                        TextBoxSelectedLayer.Text += "," + layer_to_add.Trim();
                }
            }
        }
    }
}