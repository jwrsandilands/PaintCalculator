using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintCalculator
{
    public class Paint
    {
        public Paint(string paintName, float paintAmount, float paintCost)
        {
            this.PaintName = paintName;
            this.PaintAmount = paintAmount;
            this.PaintCost = paintCost; 
        }
        public string PaintName { set; get; }

        public float PaintAmount { set; get; }

        public float PaintCost { set; get; }
    }
}
