using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Document_Extractor.Migrations
{
    public partial class AddedPatientTemp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUploadComfirmed",
                table: "Patients");

            migrationBuilder.RenameColumn(
                name: "FileName2",
                table: "Patients",
                newName: "TxtFileName");

            migrationBuilder.RenameColumn(
                name: "FileName1",
                table: "Patients",
                newName: "FileName");

            migrationBuilder.CreateTable(
                name: "PatientTemps",
                columns: table => new
                {
                    PatientTempId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperatorFirstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatorMiddle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatorSurname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientNHS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientFirstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientMiddle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientSurname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientDOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PatientSex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientSexCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientHousename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientAddress1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientAddress2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientAddress3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientAddress4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientPostcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientPhoneno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientReligion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientEthnicity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientPractice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientPracticeAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientPracticeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientGPTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientGPFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientGPSurname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientGPCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TxtFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientTemps", x => x.PatientTempId);
                    table.ForeignKey(
                        name: "FK_PatientTemps_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientTemps_TeamId",
                table: "PatientTemps",
                column: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientTemps");

            migrationBuilder.RenameColumn(
                name: "TxtFileName",
                table: "Patients",
                newName: "FileName2");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Patients",
                newName: "FileName1");

            migrationBuilder.AddColumn<bool>(
                name: "IsUploadComfirmed",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
