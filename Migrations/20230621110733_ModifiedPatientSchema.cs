using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Document_Extractor.Migrations
{
    public partial class ModifiedPatientSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName2",
                table: "Patients",
                newName: "TxtFileName");

            migrationBuilder.RenameColumn(
                name: "FileName1",
                table: "Patients",
                newName: "FileName");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Patients");

            migrationBuilder.RenameColumn(
                name: "TxtFileName",
                table: "Patients",
                newName: "FileName2");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Patients",
                newName: "FileName1");
        }
    }
}
