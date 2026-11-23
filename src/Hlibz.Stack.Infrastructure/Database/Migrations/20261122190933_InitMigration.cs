using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hlibz.Stack.Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class _20261122190933_InitMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                id = table.Column<Guid>(type: "TEXT", nullable: false),
                email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                password_hash = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                role = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                created_at = table.Column<long>(type: "INTEGER", nullable: false),
                updated_at = table.Column<long>(type: "INTEGER", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "todo_lists",
            columns: table => new
            {
                id = table.Column<Guid>(type: "TEXT", nullable: false),
                owner_id = table.Column<Guid>(type: "TEXT", nullable: false),
                title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                created_at = table.Column<long>(type: "INTEGER", nullable: false),
                updated_at = table.Column<long>(type: "INTEGER", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_todo_lists", x => x.id);
                table.ForeignKey(
                    name: "fk_todo_lists_users_owner_id",
                    column: x => x.owner_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "todo_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "TEXT", nullable: false),
                todo_list_id = table.Column<Guid>(type: "TEXT", nullable: false),
                title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                position = table.Column<int>(type: "INTEGER", nullable: false),
                done_at = table.Column<long>(type: "INTEGER", nullable: true),
                created_at = table.Column<long>(type: "INTEGER", nullable: false),
                updated_at = table.Column<long>(type: "INTEGER", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_todo_items", x => x.id);
                table.ForeignKey(
                    name: "fk_todo_items_todo_lists_todo_list_id",
                    column: x => x.todo_list_id,
                    principalTable: "todo_lists",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_todo_items_todo_list_id_position",
            table: "todo_items",
            columns: new[] { "todo_list_id", "position" });

        migrationBuilder.CreateIndex(
            name: "ix_todo_lists_owner_id",
            table: "todo_lists",
            column: "owner_id");

        migrationBuilder.CreateIndex(
            name: "ix_users_email",
            table: "users",
            column: "email",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "todo_items");

        migrationBuilder.DropTable(
            name: "todo_lists");

        migrationBuilder.DropTable(
            name: "users");
    }
}
