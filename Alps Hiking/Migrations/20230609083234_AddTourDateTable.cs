using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alps_Hiking.Migrations
{
    public partial class AddTourDateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxPassengerCount",
                table: "TourDates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxPassengerCount",
                table: "TourDates");
        }
    }
}
