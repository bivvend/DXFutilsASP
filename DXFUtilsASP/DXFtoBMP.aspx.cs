using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DXFUtilsASP
{
    public partial class DXFtoBMP : System.Web.UI.Page
    {
        List<Entity> entity_list = new List<Entity>();
        string upload_location = @"C:\DXFutilswebsite\Uploads\";

        protected void Page_Load(object sender, EventArgs e)
        {
            LabelWarn.Visible = false;
            BulletedListDXFInfo.Items.Add(new ListItem("Number of LAYERS:"));
            BulletedListDXFInfo.Items.Add(new ListItem("Number of LINES:"));
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
    }
}