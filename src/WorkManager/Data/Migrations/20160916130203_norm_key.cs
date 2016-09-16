using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WorkManager.Data.Migrations
{
    public partial class norm_key : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Norms",
                table: "Norms");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Norms");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Norms",
                table: "Norms",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Norms",
                table: "Norms");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Norms",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Norms",
                table: "Norms",
                column: "Id");
        }
    }
}
