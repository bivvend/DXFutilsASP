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
using System.Data.SqlClient;

namespace DXFUtilsASP
{
    public partial class DXFtoBMP : System.Web.UI.Page
    {
        List<Entity> entity_list = new List<Entity>();
        string upload_location = @"C:\DXFutilswebsite\Uploads\";
        string script_location = @"C:\DXFutilswebsite\Scripts";
        string python_location = @"C:\Python\Python35-64bit\python.exe";
        string entity_file_storage  = @"C:\DXFutilswebsite\Script_Storage\";
        List<string> current_layer_list = new List<string>();
        List<string> script_list = new List<string>();
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


        protected void ButtonPreview_Click(object sender, EventArgs e)
        {
            entity_list = (List<Entity>)Session["entity_list"];

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

            if (this.entity_list.Count > 0)
            {
                DXF_Display_Control.Show_Bitmap(false); //no grid show
            }
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
                if (TextBoxOutputFilename.Text.Contains(".bmp") || TextBoxOutputFilename.Text.Contains(".tiff"))
                {
                    Session["output_file_name"] = TextBoxOutputFilename.Text;
                }
                else
                {
                    Session["output_file_name"] = "Output.tiff";
                }
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
                if (TextBoxScriptOutput.Text.Contains("SCRIPT_SUCCESS"))
                {
                    LabelRenderWarning.Text = "SCRIPT_SUCCESS -  Render complete";
                    ButtonDownload.Visible = true;
                    //Add recipe to database

                    //create  object  of Connection Class..................
                    SqlConnection con = new SqlConnection();

                    try
                    {     

                        // Set Connection String property of Connection object..................
                        con.ConnectionString = RecipeDB.ConnectionString;

                        // Open Connection..................
                        con.Open();

                        //Create object of Command Class................
                        SqlCommand cmd = new SqlCommand();

                        //set Connection Property  of  Command object.............
                        cmd.Connection = con;

                        //cmd.CommandType = System.Data.CommandType.Text;


                        //Set Command text Property of command object.........

                        cmd.CommandText = "Insert into Bitmap_Recipes (Name, Filepath, Created_By, Description, Date) values ("
                                + Session["output_file_name"].ToString() + ","
                                + entity_file_storage + Session["output_file_name"].ToString() + ","
                                + User.ToString() + ","
                                + "DXF Conversion" + ","
                                + DateTime.Today.Date.ToString() + ")";

                        //Assign values as `parameter`. It avoids `SQL Injection`
                        //cmd.Parameters.AddWithValue("@name", Session["output_file_name"].ToString());
                        //cmd.Parameters.AddWithValue("@path", entity_file_storage + @"\" + Session["output_file_name"]);
                        //cmd.Parameters.AddWithValue("@created", User.ToString());
                        //cmd.Parameters.AddWithValue("@desc", "DXF conversion" );                     

                        //cmd.Parameters.AddWithValue("@date", DateTime.Today.Date.ToString());

                        cmd.ExecuteNonQuery();
                        con.Close();

                    }
                    catch (Exception ex)
                    {
                        LabelRenderWarning.Text = "SQL Write Error " + ex.ToString();
                        con.Close();
                    }



            }
                else
                {
                    LabelRenderWarning.Text = "SCRIPT_FAIL -  Render failed";
                    ButtonDownload.Visible = false;
                }
            }
            catch(Exception ex)
            {
                LabelRenderWarning.Text = ex.ToString();
            }

        }

        protected void ButtonDownload_Click(object sender, EventArgs e)
        {
            try
            {
                LabelRenderWarning.Text = "SCRIPT_SUCCESS -  Render complete";
                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                response.ClearContent();
                response.Clear();
                response.ContentType = "image/bmp";
                string filepath = entity_file_storage + @"\" + Session["output_file_name"];
                response.AddHeader("Content-Disposition", "attachment; filename=" + Session["output_file_name"] + ";");
                response.TransmitFile(filepath);
                response.Flush();
                response.End();
            }
            catch(Exception ex)
            {
                LabelRenderWarning.Text = "Download exception " + ex.ToString();
            }
        }
    }
}