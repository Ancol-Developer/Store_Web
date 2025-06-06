﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Repository.Components
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly DataContext _dbContext;

        public CategoriesViewComponent(DataContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _dbContext.Categories.Where(x => x.Status == 1).ToListAsync();
            return View(items);
        }
    }
}
