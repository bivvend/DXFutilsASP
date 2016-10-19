using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DXFUtilsASP
{
    public partial class DXFTiler : System.Web.UI.Page
    {
        List<Entity> entity_list = new List<Entity>();
        string upload_location = @"C:\DXFutilswebsite\Uploads\";
        string script_location = @"C:\DXFutilswebsite\Scripts";
        string python_location = @"C:\Python\Python34-64bit\WinPython-64bit-3.4.2.4\python-3.4.2.amd64\python.exe";
        string entity_file_storage = @"C:\DXFutilswebsite\Script_Storage\";
        List<string> current_layer_list = new List<string>();
        List<string> script_list = new List<string>();
        int line_count = 0;
        int arc_count = 0;
        int circle_count = 0;
        int point_count = 0;

        protected void Page_Load(object sender, EventArgs e)
        {

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
    }
}