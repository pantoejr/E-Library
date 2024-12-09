using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Library.Migrations
{
    /// <inheritdoc />
    public partial class spGetUserRoleByUserID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROC sp_GetUserRolesByUserID
            @UserID NVARCHAR(255)
            AS
            BEGIN
	            SELECT r.Id, r.Name From AspNetRoles r
	            JOIN AspNetUserRoles ar ON r.Id = ar.RoleId
	            WHERE ar.UserId = @UserID;
            END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetUserRolesByUserID");
        }
    }
}
