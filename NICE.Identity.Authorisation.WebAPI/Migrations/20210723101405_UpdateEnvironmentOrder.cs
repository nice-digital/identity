using Microsoft.EntityFrameworkCore.Migrations;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    public partial class UpdateEnvironmentOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        const string environmentTableName = "Environments";
	        const string nameColumnKey = "Name";
	        const string orderColumnKey = "Order";


            migrationBuilder.UpdateData(
		        table: environmentTableName,
		        keyColumn: nameColumnKey,
		        keyValue: "Live",
		        column: orderColumnKey,
		        value: 1);

            migrationBuilder.UpdateData(
	            table: environmentTableName,
	            keyColumn: nameColumnKey,
	            keyValue: "Beta",
	            column: orderColumnKey,
	            value: 2);

            migrationBuilder.UpdateData(
	            table: environmentTableName,
	            keyColumn: nameColumnKey,
	            keyValue: "Alpha",
	            column: orderColumnKey,
	            value: 3);

            migrationBuilder.UpdateData(
	            table: environmentTableName,
	            keyColumn: nameColumnKey,
	            keyValue: "Test",
	            column: orderColumnKey,
	            value: 4);

            migrationBuilder.UpdateData(
	            table: environmentTableName,
	            keyColumn: nameColumnKey,
	            keyValue: "Dev",
	            column: orderColumnKey,
	            value: 5);

            migrationBuilder.UpdateData(
	            table: environmentTableName,
	            keyColumn: nameColumnKey,
	            keyValue: "Local",
	            column: orderColumnKey,
	            value: 6);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
