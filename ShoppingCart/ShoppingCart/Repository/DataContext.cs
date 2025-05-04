using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;

namespace ShoppingCart.Repository
{
	public class DataContext : IdentityDbContext<AppUserModel, AppRoleModel, string>
	{
		public DataContext(DbContextOptions<DataContext> options): base(options) 
		{ 
			
		}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ContactModel>().HasData(
				new ContactModel
				{
                    Name = "Ancol Shop",
                    Map = @"<iframe src=""https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d29771.98541937672!2d105.63061636236145!3d21.1325637843889!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3134f88fb945c557%3A0xf1df123f17e6339c!2zVGjhu40gWHXDom4sIMSQYW4gUGjGsOG7o25nLCBIw6AgTuG7mWksIFZp4buHdCBOYW0!5e0!3m2!1svi!2s!4v1746372131514!5m2!1svi!2s"" width=""600"" height=""450"" style=""border:0;"" allowfullscreen="""" loading=""lazy"" referrerpolicy=""no-referrer-when-downgrade""></iframe>",
                    Description = "Hãy tạo ngay 1 tài khoản để sử dụng đầy đủ các tính năng, tích lũy ưu đãi khi thanh toán các sản phẩm và tham gia vào nhiều chương trình hấp dẫn.",
                    Phone = "0987654321",
                    LogoImage = "LogoImage",
					Email = "abc@gmail.com",
                });
        }

        public DbSet<BrandModel> Brands { get; set; }
		public DbSet<CategoryModel> Categories { get; set; }
		public DbSet<ProductModel> Products { get; set; }
		public DbSet<OrderModel> Orders { get; set; }
		public DbSet<OrderDetails> OrderDetails { get; set; }
		public DbSet<RatingModel> Ratings { get; set; }
		public DbSet<SliderModel> Sliders { get; set; }
		public DbSet<ContactModel> Contacts { get; set; }
	}
}
