using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankSysteam.Api.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Contas",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Saldo",
                table: "Contas",
                newName: "Balance");

            migrationBuilder.RenameColumn(
                name: "DataCriacao",
                table: "Contas",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Numero",
                table: "Contas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<Guid>(
                name: "ClienteId",
                table: "Contas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contas_ClienteId",
                table: "Contas",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contas_Clients_ClienteId",
                table: "Contas",
                column: "ClienteId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contas_Clients_ClienteId",
                table: "Contas");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Contas_ClienteId",
                table: "Contas");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Contas");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Contas",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Contas",
                newName: "DataCriacao");

            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "Contas",
                newName: "Saldo");

            migrationBuilder.AlterColumn<int>(
                name: "Numero",
                table: "Contas",
                type: "int",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
