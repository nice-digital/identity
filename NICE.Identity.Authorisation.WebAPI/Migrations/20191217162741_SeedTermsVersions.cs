using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    public partial class SeedTermsVersions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CreatedByUserID",
                table: "TermsVersions",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.InsertData(
                table: "TermsVersions",
                columns: new[]{ "TermsVersionID", "VersionDate"},
                values: new object[,]
                {
                    {30, new DateTime(2019, 9, 2, 13, 25, 0) }
                }
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TermsVersions",
                keyColumn: "TermsVersionID",
                keyValue: 30
            );

            migrationBuilder.AlterColumn<int>(
                name: "CreatedByUserID",
                table: "TermsVersions",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
