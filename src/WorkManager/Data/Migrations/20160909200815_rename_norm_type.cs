using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using WorkManager.Models;

namespace WorkManager.Data.Migrations
{
    public partial class rename_norm_type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskTime",
                table: "Norms");

            migrationBuilder.DropColumn(
                name: "TaskType",
                table: "Norms");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Goal",
                table: "Norms",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Norms",
                nullable: false,
                defaultValue: NormType.Day);

            migrationBuilder.CreateIndex(
                name: "IX_Timers_Started",
                table: "Timers",
                column: "Started");

            migrationBuilder.CreateIndex(
                name: "IX_Timers_Stopped",
                table: "Timers",
                column: "Stopped");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Timers_Started",
                table: "Timers");

            migrationBuilder.DropIndex(
                name: "IX_Timers_Stopped",
                table: "Timers");

            migrationBuilder.DropColumn(
                name: "Goal",
                table: "Norms");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Norms");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TaskTime",
                table: "Norms",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "TaskType",
                table: "Norms",
                nullable: false,
                defaultValue: 0);
        }
    }
}
