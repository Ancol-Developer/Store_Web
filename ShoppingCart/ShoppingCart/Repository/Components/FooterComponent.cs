using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Repository.Components
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly DataContext _db;
        public FooterViewComponent(DataContext db)
        {
            _db = db;   
        }

        public async Task<IViewComponentResult> InvokeAsync() => View(await _db.Contacts.FirstOrDefaultAsync());
    }
}
