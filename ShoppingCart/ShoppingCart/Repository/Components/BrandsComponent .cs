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
            var items = await _dbContext.Brands.Where(x => x.Status == 1).ToListAsync();
            return View(items);
        }
    }
}
