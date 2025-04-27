using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GctlInfoSysTask.Migrations
{
    /// <inheritdoc />
    public partial class gctlChangeDbName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Designations_DesignationCode",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RosterScheduleEntries",
                table: "RosterScheduleEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Designations_DesignationCode",
                table: "Designations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Designations",
                table: "Designations");

            migrationBuilder.RenameTable(
                name: "Shifts",
                newName: "HRM_ATD_Shift");

            migrationBuilder.RenameTable(
                name: "RosterScheduleEntries",
                newName: "HRM_ATD_RosterScheduleEntry");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "HRM_Employee");

            migrationBuilder.RenameTable(
                name: "Designations",
                newName: "HRM_Def_Designation");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_DesignationCode",
                table: "HRM_Employee",
                newName: "IX_HRM_Employee_DesignationCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HRM_ATD_Shift",
                table: "HRM_ATD_Shift",
                column: "ShiftCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HRM_ATD_RosterScheduleEntry",
                table: "HRM_ATD_RosterScheduleEntry",
                column: "AI_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HRM_Employee",
                table: "HRM_Employee",
                column: "AI_ID");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_HRM_Def_Designation_DesignationCode",
                table: "HRM_Def_Designation",
                column: "DesignationCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HRM_Def_Designation",
                table: "HRM_Def_Designation",
                column: "AI_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_HRM_Employee_HRM_Def_Designation_DesignationCode",
                table: "HRM_Employee",
                column: "DesignationCode",
                principalTable: "HRM_Def_Designation",
                principalColumn: "DesignationCode",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HRM_Employee_HRM_Def_Designation_DesignationCode",
                table: "HRM_Employee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HRM_Employee",
                table: "HRM_Employee");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_HRM_Def_Designation_DesignationCode",
                table: "HRM_Def_Designation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HRM_Def_Designation",
                table: "HRM_Def_Designation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HRM_ATD_Shift",
                table: "HRM_ATD_Shift");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HRM_ATD_RosterScheduleEntry",
                table: "HRM_ATD_RosterScheduleEntry");

            migrationBuilder.RenameTable(
                name: "HRM_Employee",
                newName: "Employees");

            migrationBuilder.RenameTable(
                name: "HRM_Def_Designation",
                newName: "Designations");

            migrationBuilder.RenameTable(
                name: "HRM_ATD_Shift",
                newName: "Shifts");

            migrationBuilder.RenameTable(
                name: "HRM_ATD_RosterScheduleEntry",
                newName: "RosterScheduleEntries");

            migrationBuilder.RenameIndex(
                name: "IX_HRM_Employee_DesignationCode",
                table: "Employees",
                newName: "IX_Employees_DesignationCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "AI_ID");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Designations_DesignationCode",
                table: "Designations",
                column: "DesignationCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Designations",
                table: "Designations",
                column: "AI_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts",
                column: "ShiftCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RosterScheduleEntries",
                table: "RosterScheduleEntries",
                column: "AI_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Designations_DesignationCode",
                table: "Employees",
                column: "DesignationCode",
                principalTable: "Designations",
                principalColumn: "DesignationCode",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
