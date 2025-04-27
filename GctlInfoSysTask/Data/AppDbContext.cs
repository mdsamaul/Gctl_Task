using GctlInfoSysTask.ModelDto;
using GctlInfoSysTask.ModelDto.ViewModals;
using GctlInfoSysTask.Models;
using Microsoft.EntityFrameworkCore;

namespace GctlInfoSysTask.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer>? Customers { get; set; }
        public DbSet<CustomerType>? CustomerTypes { get; set; }
        public DbSet<DeliveryAddress>? DeliveryAddresses { get; set; }
        public DbSet<HRM_ATD_RosterScheduleEntry>? HRM_ATD_RosterScheduleEntry { get; set; }
        public DbSet<HRM_ATD_Shift>? HRM_ATD_Shift { get; set; }
        public DbSet<HRM_Def_Designation>? HRM_Def_Designation { get; set; }
        public DbSet<HRM_Employee>? HRM_Employee { get; set; }
        public DbSet<RosterView>? rosterViews{ get; set; }

        public DbSet<HrmEmployeeViewModel> hrmEmployeeViewModels{ get; set; }

        //protected override void onModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<HrmEmployeeViewModel>().HasNoKey().ToView("vw_HRM_EmployeeInfo");
        //}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // DeliveryAddress -> Customer (One to Many)
            modelBuilder.Entity<DeliveryAddress>()
                .HasOne(d => d.Customer)
                .WithMany(c => c.DeliveryAddresses)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional: Configure Employee's Designation as ForeignKey if needed
            modelBuilder.Entity<HRM_Employee>()
                .HasOne<HRM_Def_Designation>()
                .WithMany()
                .HasForeignKey(e => e.DesignationCode)
                .HasPrincipalKey(d => d.DesignationCode)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<HrmEmployeeViewModel>().HasNoKey().ToView("vw_EmployeeList");
            modelBuilder.Entity<RosterScheduleViewModel>().HasNoKey();
            modelBuilder.Entity<HrmAtdRosterScheduleEntry>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_HRM_ATD_RosterScheduleEntry");
            });
            
        }
    }
}
