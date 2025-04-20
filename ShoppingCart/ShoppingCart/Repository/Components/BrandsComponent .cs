using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Repository.Components
{
    public class BrandsViewComponent : ViewComponent
    {
        private readonly DataContext _dbContext;

        public BrandsViewComponent(DataContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _dbContext.Brands.ToListAsync();
            return View(items);
        }
    }
}
