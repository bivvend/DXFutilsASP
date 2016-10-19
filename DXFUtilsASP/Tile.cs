using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXFUtilsASP
{

    public class Tile
    {
        double center_x { get; set; }
        double center_y { get; set; }
        double width { get; set; }
        double height { get; set; }

        public Tile()
        {
            center_x = 0.0d;
            center_y = 0.0d;
            width = 1.0d;
            height = 1.0d;

        }

        public Tile(double centerx, double centery, double size_x, double size_y)
        {
            this.center_x = centerx;
            this.center_y = centery;
            this.width = size_x;
            this.height = size_y;

        }
    }
}