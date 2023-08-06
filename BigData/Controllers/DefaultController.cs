using BigData.Dal;
using BigData.Dal.Dtos;
using BigData.Dal.Entities;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BigData.Controllers
{
    public class DefaultController : Controller
    {
        public readonly string context = "Server = YAGMUR\\SQLEXPRESS; initial catalog = CARPLATES; integrated security = true";


        public async Task<IActionResult> Index()
        {
            await using var connect = new SqlConnection(context);

            //PLAKALAR
            var plateMax = (await connect.QueryAsync<PlaterDto>("SELECT TOP 1 SUBSTRING(PLATE, 1, 2) AS plate, COUNT(*) AS count FROM PLATES GROUP BY SUBSTRING(PLATE, 1, 2) ORDER BY count DESC")).FirstOrDefault();
            var plateMin = (await connect.QueryAsync<PlaterDto>("SELECT TOP 1 SUBSTRING(PLATE, 1, 2) AS plate, COUNT(*) AS count FROM PLATES GROUP BY SUBSTRING(PLATE, 1, 2) ORDER BY count ASC")).FirstOrDefault();
            ViewData["plateMax"] = plateMax.PLATE;
            ViewData["countMax"] = plateMax.Count;
            ViewData["plateMin"] = plateMin.PLATE;
            ViewData["countMin"] = plateMin.Count;

            // YAKIT TÜRÜ --EN AZ KULLANILAN YAKIT TÜRÜ
          
            var fuelMin = (await connect.QueryAsync<FuelDto>("SELECT TOP 1 FUEL, COUNT(*) AS count FROM PLATES GROUP BY FUEL ORDER BY count DESC")).FirstOrDefault();
            ViewData["fuelMin"] = fuelMin.FUEL;
            ViewData["fuelcountMin"] = fuelMin.Count;

            // vites türü- enaz kullanılan
            var shiftMin = (await connect.QueryAsync<ShiftTypeDto>("SELECT TOP 1 SHIFTTYPE, COUNT(*) AS count FROM PLATES GROUP BY SHIFTTYPE ORDER BY count ASC")).FirstOrDefault();
            ViewData["shiftMin"] = shiftMin.SHIFTTYPE;
            ViewData["shiftCountMin"] = shiftMin.Count;


            // En Fazla BUlunan Araç Rengi
            var colorMax = (await connect.QueryAsync<ColorDto>("SELECT TOP 1 COLOR, COUNT(*) AS count FROM PLATES GROUP BY COLOR ORDER BY count DESC")).FirstOrDefault();
            ViewData["colorMax"] = colorMax.COLOR;
            ViewData["colorCountMax"] = colorMax.Count;

            // Araba Markaları
            var brandMax = (await connect.QueryAsync<BrandDto>("SELECT TOP 1 BRAND, COUNT(*) AS count FROM PLATES GROUP BY BRAND ORDER BY count DESC")).FirstOrDefault();
            var brandMin = (await connect.QueryAsync<BrandDto>("SELECT TOP 1 BRAND, COUNT(*) AS count FROM PLATES GROUP BY BRAND ORDER BY count ASC")).FirstOrDefault();

            ViewData["brandMax"] = brandMax.BRAND;
            ViewData["countBrandMax"] = brandMax.Count;

            ViewData["brandMin"] = brandMin.BRAND;
            ViewData["countBrandMin"] = brandMin.Count;

            return View();
        }
        public async Task<IActionResult> Search(string keyword)
        {
            string query = @"
            SELECT TOP 10000 BRAND, SUBSTRING(PLATE, 1, 2) AS CITYNR, SHIFTTYPE, FUEL, COLOR, TITLE FROM PLATES
            WHERE BRAND LIKE '%' + @Keyword + '%'
               OR CITYNR LIKE '%' + @Keyword + '%'
               OR SHIFTTYPE LIKE '%' + @Keyword + '%'
               OR FUEL LIKE '%' + @Keyword + '%'
               OR COLOR LIKE '%' + @Keyword + '%'
               OR TITLE LIKE '%' + @Keyword + '%'
        ";
            await using var connect = new SqlConnection(context);
            connect.Open();

            // Sorguyu çalıştırın ve sonuçları alın
            var searchResults = await connect.QueryAsync<SearchDtos>(query, new { Keyword = keyword });

            // Sonuçları JSON formatında döndürün
            return Json(searchResults);

        }
    }
}
