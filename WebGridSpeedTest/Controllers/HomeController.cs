using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Dynamic;

namespace WebGridSpeedTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Util function to generate data
        /// </summary>
        /// <param name="totalColumns">The total columns in data to generate</param>
        /// <param name="totalRows">The total rows in the data to generate</param>
        /// <returns></returns>
        private IEnumerable<ExpandoObject> generateData(int totalColumns, int totalRows)
        {
            var data = new List<ExpandoObject>();

            var rand = new Random();
            var cols = new List<String>();
            for (int i = 0; i < totalColumns; i++)
                cols.Add("Col_" + i.ToString());

            for (int i = 0; i < totalRows; i++)
            {
                var row = new ExpandoObject();

                foreach (var col in cols)
                    ((IDictionary<string, object>)row).Add(col, rand.Next());

                data.Add(row);
            }

            return data;
        }

        /// <summary>
        /// Small Util function, that adds columns and plain formatting for 
        /// the column values resulting in a huge performance gain
        /// </summary>
        /// <param name="data">The web grid data</param>
        /// <returns></returns>
        public static IHtmlString GridHtmlFromExpandoList(IEnumerable<ExpandoObject> data)
        {
            WebGrid grid = new WebGrid(
                    source: data
                );

            var cols = new List<WebGridColumn>();

            if (data.Count() > 0)
            {
                IEnumerable<KeyValuePair<string, Object>> example = data.First();

                foreach (var pair in example)
                    cols.Add(grid.Column(
                        columnName: pair.Key,
                        //Seems that specifying a formatter saves the web grid ridiculous amounts of time
                        format: (item => ((IDictionary<String, Object>)item.Value)[pair.Key])
                    ));
            }

            return grid.GetHtml(
                tableStyle: "table",
                columns: cols
            );
        }

        /// <summary>
        /// Partial view used for lazy loading of web grid with no columns/formatting
        /// </summary>
        /// <param name="totalColumns">The columns for the grid</param>
        /// <param name="totalRows">The rows for the grid</param>
        /// <returns></returns>
        public PartialViewResult NoColumnsGrid(int totalColumns, int totalRows)
        {
            var data = generateData(totalColumns, totalRows);

            var noColumnsGrid = new WebGrid(
                source: data
            );

            return PartialView("BaseGrid", noColumnsGrid.GetHtml("table"));
        }

        /// <summary>
        /// Partial view used for lazy loading of web grid with columns and basic formatting
        /// </summary>
        /// <param name="totalColumns">Columns for the grid</param>
        /// <param name="totalRows">Rows for the grid</param>
        /// <returns></returns>
        public PartialViewResult ColumnsGrid(int totalColumns, int totalRows)
        {
            var data = generateData(totalColumns, totalRows);

            return PartialView("BaseGrid", GridHtmlFromExpandoList(data));
        }
    }
}
