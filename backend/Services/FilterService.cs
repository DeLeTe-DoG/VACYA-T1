using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Entities;
using backend.Models;
using backend.Data; // DbContext
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class FilterService
    {
        private readonly AppDbContext _context;

        public FilterService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<WebSiteDTO>> DateFilterAsync(int userId, DateTime dateFrom, DateTime dateTo)
        {
            var dateFromStart = dateFrom.Date;
            var dateToEnd = dateTo.Date.AddDays(1).AddTicks(-1);

            // Получаем сайты пользователя вместе с данными за период
            var sites = await _context.WebSites
                .Where(s => s.UserId == userId)
                .Include(s => s.WebSiteData
                    .Where(d => d.LastChecked >= dateFromStart && d.LastChecked <= dateToEnd))
                .ToListAsync();

            var result = sites
                .Where(s => s.WebSiteData.Any())
                .Select(s => new WebSiteDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    URL = s.URL,
                    TotalErrors = s.WebSiteData.Count(d => !string.IsNullOrEmpty(d.ErrorMessage)),
                    WebSiteData = s.WebSiteData.Select(d => new WebSiteDataDTO
                    {
                        Id = d.Id,
                        StatusCode = d.StatusCode,
                        ErrorMessage = d.ErrorMessage,
                        LastChecked = d.LastChecked
                    }).ToList()
                })
                .ToList();

            return result;
        }
    }
}
