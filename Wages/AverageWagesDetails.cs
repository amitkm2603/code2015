using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wages
{
    public class AverageWagesDetails
    {
        private double wages;

        public double Wages
        {
            get { return wages; }
            set { wages = value; }
        }

       
        private int count;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }
       private string province;

        public string Province
        {
            get { return province; }
            set { province = value; }
        }
       private string year;

        public string Year
        {
            get { return year; }
            set { year = value; }
        }
    }
}