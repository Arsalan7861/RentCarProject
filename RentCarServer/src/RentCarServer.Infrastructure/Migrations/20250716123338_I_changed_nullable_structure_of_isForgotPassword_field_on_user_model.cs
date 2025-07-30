using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentCarServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class I_changed_nullable_structure_of_isForgotPassword_field_on_user_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsForgotPasswordCreated_Value",
                table: "Users",
                newName: "IsForgotPasswordCompleted_Value");

            migrationBuilder.AlterColumn<bool>(
                name: "IsForgotPasswordCompleted_Value",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsForgotPasswordCompleted_Value",
                table: "Users",
                newName: "IsForgotPasswordCreated_Value");

            migrationBuilder.AlterColumn<bool>(
                name: "IsForgotPasswordCreated_Value",
                table: "Users",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
