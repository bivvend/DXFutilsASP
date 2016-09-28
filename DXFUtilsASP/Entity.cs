/*
 * Created by SharpDevelop.
 * User: Simon Henley
 * Date: 29/06/2015
 * Time: 23:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace DXFutilsCS
{
    /// <summary>
    /// Description of MarkVector.
    /// </summary>
    public class Entity
    {
        public float x_start { get; set; }
        public float y_start { get; set; }
        public float x_end { get; set; }
        public float y_end { get; set; }

        public string type { get; set; }    //Can be LINE, CIRCLE, ARC
        public float radius { get; set; }
        public float start_angle { get; set; }
        public float end_angle { get; set; }
        public float x_center { get; set; }
        public float y_center { get; set; }

        public string layer { get; set; }

        public Entity()
        {
            this.x_start = 0.0f;
            this.y_start = 0.0f;
            this.x_end = 0.0f;
            this.y_end = 0.0f;

            this.type = "LINE";
            this.radius = 1.0f;
            this.start_angle = 0.0f;
            this.end_angle = 0.0f;
            this.x_center = 0.0f;
            this.y_center = 0.0f;

            this.layer = "0";

        }

        //LINE CONSTRUCTOR
        public Entity(float fx1, float fy1, float fx2, float fy2, string stype)
        {
            this.x_start = fx1;
            this.y_start = fy1;
            this.x_end = fx2;
            this.y_end = fy2;
            this.type = stype;
            this.layer = "0";
        }

        //CIRCLE CONSTRUCTOR
        public Entity(float fx, float fy, float fradius, string stype)
        {
            this.x_center = fx;
            this.y_center = fy;
            //this.x_end =fx2;
            //this.y_end =fy2;
            this.radius = fradius;
            this.type = stype;  //SHOULD BE "CIRCLE"
            this.layer = "0";
        }

        //ARC CONSTRUCTOR
        public Entity(float fx, float fy, float fradius, float fstart_angle, float fend_angle, string stype)
        {
            this.x_center = fx;
            this.y_center = fy;
            //this.x_end =fx2;
            //this.y_end =fy2;
            this.radius = fradius;
            this.start_angle = fstart_angle;
            this.end_angle = fend_angle;
            this.type = stype;  //SHOULD BE "ARC"
            this.layer = "0";
        }
    }
}
