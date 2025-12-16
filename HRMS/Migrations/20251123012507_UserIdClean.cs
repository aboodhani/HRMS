using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Migrations
{
    /// <inheritdoc />
    public partial class UserIdClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Employees",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TypeId",
                table: "Departments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_TypeId",
                table: "Departments",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Lookups_TypeId",
                table: "Departments",
                column: "TypeId",
                principalTable: "Lookups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_users_UserId",
                table: "Employees",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Lookups_TypeId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_users_UserId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_UserId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Departments_TypeId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Departments");
        }
    }
}
