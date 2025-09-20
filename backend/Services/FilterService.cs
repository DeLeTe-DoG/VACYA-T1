using System;
using System.Collections.Generic;
using System.Linq;
using backend.Models;

namespace backend.Services
{
    public class FilterService
    {
        public List<WebSiteDTO> DateFilter(UserDTO user, DateTime dateFrom, DateTime dateTo)
        {
            var dateFromStart = dateFrom.Date;
            var dateToEnd = dateTo.Date.AddDays(1).AddTicks(-1);

            var filteredSites = user.Sites
                .Select(site =>
                {
                    // Фильтруем данные только по дате
                    var filteredData = site.WebSiteData
                        .Where(d => d.LastChecked >= dateFromStart &&
                                    d.LastChecked <= dateToEnd)
                        .ToList();

                    return new WebSiteDTO
                    {
                        Id = site.Id,
                        Name = site.Name,
                        URL = site.URL,
                        WebSiteData = filteredData,
                        TotalErrors = filteredData.Count
                    };
                })
                // Оставляем только сайты, у которых есть данные за период
                .Where(site => site.WebSiteData.Any())
                .ToList();

            return filteredSites;
        }
    }
}
