using E_Library.Helpers;
using E_Library.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace E_Library.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly string _connectionString;
        public AppDbContext(DbContextOptions options, ICurrentUserService currentUserService, IConfiguration configuration)
        : base(options)
        {
            _currentUserService = currentUserService;
            _connectionString = configuration.GetConnectionString("Default");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //ProcessSave();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ProcessSave()
        {
            var currentTime = DateTime.Now;
            foreach (var item in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added && e.Entity is AuditTrail))
            {
                var entity = item.Entity as AuditTrail;
                entity.CreatedOn = currentTime;
                entity.CreatedBy = _currentUserService.GetCurrentUser();
                entity.ModifiedOn = currentTime;
                entity.ModifiedBy = _currentUserService.GetCurrentUser();
            }

            foreach (var item in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified && e.Entity is AuditTrail))
            {
                var entity = item.Entity as AuditTrail;
                entity.ModifiedOn = currentTime;
                entity.ModifiedBy = _currentUserService.GetCurrentUser();
                item.Property(nameof(entity.CreatedBy)).IsModified = false;
                item.Property(nameof(entity.CreatedOn)).IsModified = false;
            }
        }

        public async Task<int> UpdateUserRoles(int groupID, string userID)
        {
            int resultMessage = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("sp_UpdateUserRoles", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@GroupID", groupID));
                    cmd.Parameters.Add(new SqlParameter("@UserID", userID));

                    var resultMessageParam = new SqlParameter("@ResultMessage", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(resultMessageParam);

                    await connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    resultMessage = (int)resultMessageParam.Value;
                }
            }
            return resultMessage;
        }
        public async Task<List<IdentityRole>> GetUserRolesById(params SqlParameter[] parameters)
        {
            var results = await Roles.FromSqlRaw($"EXEC sp_GetUserRolesByUserID", parameters).ToListAsync();
            return results;
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookAccessLog> BookAccessLogs { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupRole> GroupRoles { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
    }
}
