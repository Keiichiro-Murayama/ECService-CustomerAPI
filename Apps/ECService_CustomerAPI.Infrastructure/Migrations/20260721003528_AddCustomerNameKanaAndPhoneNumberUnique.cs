using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECService_CustomerAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerNameKanaAndPhoneNumberUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name_kana",
                table: "customer",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_customer_phone_number",
                table: "customer",
                column: "phone_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_customer_phone_number",
                table: "customer");

            migrationBuilder.DropColumn(
                name: "name_kana",
                table: "customer");
        }
    }
}
