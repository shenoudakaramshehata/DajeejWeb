using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Dajeej.Models;

namespace Dajeej.Data
{
    public class DajeejContext : DbContext
    {
        public DajeejContext()
        {
        }

        public DajeejContext(DbContextOptions<DajeejContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ContactUs> ContactUs { get; set; }
        public virtual DbSet<PageContent> PageContents { get; set; }
        public virtual DbSet<FAQ> FAQs { get; set; }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<PaymentMehod> PaymentMehods { get; set; }
        public virtual DbSet<Plan> Plans { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<ItemImage> ItemImages { get; set; }
        public virtual DbSet<CouponType> CouponTypes { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public virtual DbSet<AddressType> AddressTypes { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<Favourite> Favourites { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<Point> Points { get; set; }
        public virtual DbSet<SoicialMidiaLink> SoicialMidiaLinks { get; set; }
        public virtual DbSet<EntityType> EntityTypes { get; set; }
        public virtual DbSet<ShopImage> ShopImages { get; set; }
        public virtual DbSet<Slider> Sliders { get; set; }
        public virtual DbSet<PublicDevice> PublicDevices { get; set; }
        public virtual DbSet<PublicNotification> PublicNotifications { get; set; }
        public virtual DbSet<PublicNotificationDevice> PublicNotificationDevices { get; set; }
        public virtual DbSet<EntityTypeNotify> EntityTypeNotifies { get; set; }
        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<Collection> Collections { get; set; }
        public virtual DbSet<CollectionItem> CollectionItems { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EntityType>().HasData(new EntityType { EntityTypeId = 1, TitleAr = "متجر", TitleEn = "Shop" });
            modelBuilder.Entity<EntityType>().HasData(new EntityType { EntityTypeId = 2, TitleAr = "شركة", TitleEn = "Company" });
            modelBuilder.Entity<EntityType>().HasData(new EntityType { EntityTypeId = 3, TitleAr = "مشاريع صغيره", TitleEn = "Small Projects" });
            modelBuilder.Entity<PageContent>().HasData(new PageContent { PageContentId = 1, PageTitleAr = "من نحن", PageTitleEn = "About",ContentAr="من نحن",ContentEn="About Page" });
            modelBuilder.Entity<PageContent>().HasData(new PageContent { PageContentId = 2, PageTitleAr = "الشروط والاحكام", PageTitleEn = "Condition and Terms",ContentAr="الشروط والاحكام",ContentEn= "Condition and Terms Page" });
            modelBuilder.Entity<PageContent>().HasData(new PageContent { PageContentId = 3, PageTitleAr = "سياسة الخصوصية", PageTitleEn = "Privacy Policy",ContentAr="سياسة الخصوصية",ContentEn= "Privacy Policy Page" });
            modelBuilder.Entity<EntityTypeNotify>().HasData(new EntityTypeNotify { EntityTypeNotifyId = 1, TitleAr = "عناصر",TitleEn = "Items"});
            modelBuilder.Entity<EntityTypeNotify>().HasData(new EntityTypeNotify { EntityTypeNotifyId = 2, TitleAr = "متاجر",TitleEn = "Shops"});


        }

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}
