using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Resources;
using System.Collections;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Data;
using System.Text;

namespace Wages
{
    public partial class _Default : System.Web.UI.Page
    {
        List<string> Provinces;
        List<string> Years;
        Hashtable WagesList;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                _readDetailsfromFile();
        }

        private void _readDetailsfromFile()
        {
            Provinces = new List<string>();
            Years = new List<string>();
            WagesList = new Hashtable();
            AverageWagesDetails avg;
            string resName = "20140926-01-historicalminimumwageratesincanada.csv";
            // read relative path
            var file = System.Web.HttpContext.Current.Server.MapPath(resName);
            using (var reader = new StreamReader(file))
            {
                var line = reader.ReadLine();
                avg = new AverageWagesDetails();
                avg.Count = 0;
                avg.Wages = 0;
                string key = null;
                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(',');

                    if (!string.IsNullOrEmpty(values[0]))
                    {

                        if (!Provinces.Contains(values[0]))
                            Provinces.Add(values[0]);
                    }
                    if (!string.IsNullOrEmpty(values[1]))
                    {

                        var date = values[1].Split('/');
                        string yr = date[date.Length - 1];
                        if (!Years.Contains(yr))
                            Years.Add(yr);
                        key = values[0] + "_" + yr;
                    }
                    if (!string.IsNullOrEmpty(values[2]))
                    {
                        var wage = values[2].Split('$');
                        avg = new AverageWagesDetails();
                        if (!WagesList.ContainsKey(key))
                        {
                            avg.Count = 1;
                            avg.Wages = Convert.ToDouble(wage[1]);
                            WagesList.Add(key, avg);

                        }
                        else
                        {
                            AverageWagesDetails totalWages = (AverageWagesDetails)WagesList[key];
                            totalWages.Wages += Convert.ToDouble(wage[1]);
                            totalWages.Count++;
                            WagesList[key] = totalWages;

                        }
                    }

                }
                //Iterator to loop through and modify Wages list
                List<string> NewKeys = new List<string>();
                foreach (System.Collections.DictionaryEntry de in WagesList)
                    NewKeys.Add(de.Key.ToString());

                foreach (string itrKey in NewKeys)
                {
                    AverageWagesDetails obj = (AverageWagesDetails)WagesList[itrKey];
                    obj.Wages = Math.Round((obj.Wages / obj.Count), 3);
                    if (obj.Wages > 15)
                        obj.Wages = Math.Round(obj.Wages / 40);
                    WagesList[itrKey] = obj;

                }
                HttpContext.Current.Session["WagesDetails"] = WagesList;
                HttpContext.Current.Session["Province"] = Provinces;
                Year1.DataSource = Year2.DataSource = Years;
                Year1.DataBind();
                Year2.DataBind();
                Province1.DataSource = Province2.DataSource = Provinces;
                Province1.DataBind();
                Province2.DataBind();
                _drawChart();
            }
        }

        protected void Compare_Click(object sender, EventArgs e)
        {
            string y1 = Year1.SelectedItem.Value;
            string y2 = Year2.SelectedItem.Value;
            string p1 = Province1.SelectedItem.Value;
            string p2 = Province2.SelectedItem.Value;
            string key1 = p1 + "_" + y1;
            string key2 = p2 + "_" + y2;
            double w1=0, w2=0;
            var WagesDetails = HttpContext.Current.Session["WagesDetails"] as Hashtable;
            if (WagesDetails.ContainsKey(key1))
            {
                AverageWagesDetails obj = (AverageWagesDetails)WagesDetails[key1];
                w1 = obj.Wages;
                Result1.Text = "Avegare wages for <b>" + p1 + " </b>for the year of <b> " + y1 + "</b> is $ <b>" + obj.Wages + "</b>";
            }
            else
            {
                Result1.Text = "No such record found for <b> " + p1 + " </b>and <b>" + y1 + "</b>. Please try again!";
            }
            if (WagesDetails.ContainsKey(key2))
                {
                    AverageWagesDetails obj = (AverageWagesDetails)WagesDetails[key2];
                    w2 = obj.Wages;
                    Result2.Text = "Avegare wages for <b>" + p2 + " </b>for the year of <b>" + y2 + "</b> is $ <b>" + obj.Wages + "</b>";
                }
                else
                {
                    Result2.Text = "No such record found for <b>" + p2 + "</b> and <b>" + y2 + "</b>. Please try again!";
                }
                if (w1 > 0 && w2 > 0)
                {

                    double percentage = ((w1 - w2) / w1) * 100;
                    Percentage.Text = " Province <b>" + p1 + "</b> is  <b>" + Math.Round(percentage, 2) + "% </b> ahead of Province <b>" + p2 + "</b> in the year of <b>" + y1 +"</b>";
                }

                
            

        }

        private void _drawChart()
        {
            var WagesDetails = HttpContext.Current.Session["WagesDetails"] as Hashtable;
            Hashtable YearWise = new Hashtable();
            List<AverageWagesDetails> YeartDetails = null;
            List<AverageWagesDetails> ProvinceDetails = null;
            Hashtable ProvinceWise = new Hashtable();
            List<string> NewKeys = new List<string>();
            foreach (System.Collections.DictionaryEntry de in WagesDetails)
                NewKeys.Add(de.Key.ToString());

            foreach (string itrKey in NewKeys)
            {
                var keyValue = itrKey.Split('_');
                string year = keyValue[1];
                string province = keyValue[0];
                YeartDetails = new List<AverageWagesDetails>();
                ProvinceDetails = new List<AverageWagesDetails>();
                foreach (string itr in NewKeys)
                {
                    //year wise
                    if (itr.Contains(year))
                    {
                        var itrVal = itr.Split('_');
                        AverageWagesDetails obj = (AverageWagesDetails)WagesDetails[itr];
                        obj.Year = year;
                        obj.Province = itrVal[0];

                        if (!YeartDetails.Contains(obj))
                            YeartDetails.Add(obj);
                    }
                    //province wise
                    if (itr.Contains(province))
                    {
                        var itrVal = itr.Split('_');
                        AverageWagesDetails obj = (AverageWagesDetails)WagesDetails[itr];
                        obj.Year = itrVal[1];
                        obj.Province = province;
                        if (!ProvinceDetails.Contains(obj))
                            ProvinceDetails.Add(obj);

                    }

                }
                if(!YearWise.ContainsKey(year))
                    YearWise.Add(year,YeartDetails);
                if(!ProvinceWise.ContainsKey(province))
                    ProvinceWise.Add(province,ProvinceDetails);
            }

            _prepareJSONObjectfromList(YearWise, ProvinceWise);
           
        }

        private void _prepareJSONObjectfromList(Hashtable YearWise, Hashtable ProvinceWise)
        {
            StringBuilder sb = new StringBuilder();
            List<string> provincesList = (List<string>)HttpContext.Current.Session["Province"];

            #region Province List to Dictionary conversion

            //creating province json
            sb.Append("[");
            sb.Append("\"" + "Year" + "\"");

            Dictionary<string, double> yearwiseWagesPerYear = new Dictionary<string, double>();
            //Get unique list of Provinces in a dictionary
            for (int i = 0; i < provincesList.Count; i++)
            {
                yearwiseWagesPerYear.Add(provincesList[i], 0);

                sb.Append(",");
                sb.Append("\"" + provincesList[i] + "\"");
            }

            sb.Append("]");
            #endregion

            Dictionary<string, List<double>> wagesByyear = new Dictionary<string, List<double>>();
            SortedList<string, List<AverageWagesDetails>> sortedYears = new SortedList<string, List<AverageWagesDetails>>();
            foreach (DictionaryEntry year in YearWise)
            {
                sortedYears.Add(year.Key.ToString(), (List<AverageWagesDetails>)year.Value);
            }
            foreach (var year in sortedYears)
            {
                //creating a new duictionary for each year
                Dictionary<string, double> yearwiseWagesNew = new Dictionary<string, double>(yearwiseWagesPerYear);

                for (int i = 0; i < yearwiseWagesNew.Count; i++)
                {
                    yearwiseWagesNew[provincesList[i]] = this._getProvinceWages((List<AverageWagesDetails>)year.Value, provincesList[i]);

                }

                sb.Append(",[");
                sb.Append("\"" + year.Key + "\"");

                //Adding each year wise value
                foreach (var wage in yearwiseWagesNew)
                {
                    sb.Append(",");
                    sb.Append(wage.Value);
                }

                sb.Append("]");
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            string YearWiseJson = sb.ToString();
            string provinceWiseJson = Newtonsoft.Json.JsonConvert.SerializeObject(ProvinceWise);
            YearJson.Value = YearWiseJson;
            ProvinceJson.Value = provinceWiseJson;
        }

        

        private double _getProvinceWages(List<AverageWagesDetails> yearVar, string Province)
        {
            for (int i = 0; i < yearVar.Count; i++)
            {
                if (yearVar[i].Province == Province)
                {
                    return yearVar[i].Wages;
                }
            }
            return 0;
        }
    }
}
