using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Byd.AuthManagement.Migrations
{
    /// <inheritdoc />
    public partial class KPB125_AddFeatureManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeatureGroups",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feature_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    parent_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    default_value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    is_visible_to_clients = table.Column<bool>(type: "boolean", nullable: false),
                    is_available_to_host = table.Column<bool>(type: "boolean", nullable: false),
                    allowed_providers = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    value_type = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_features", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "FeatureValues",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    value = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    provider_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    provider_key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feature_values", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_feature_groups_name",
                schema: "auth_management",
                table: "FeatureGroups",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_features_group_name",
                schema: "auth_management",
                table: "Features",
                column: "group_name");

            migrationBuilder.CreateIndex(
                name: "ix_features_name",
                schema: "auth_management",
                table: "Features",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_feature_values_name_provider_name_provider_key",
                schema: "auth_management",
                table: "FeatureValues",
                columns: new[] { "name", "provider_name", "provider_key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeatureGroups",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "Features",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "FeatureValues",
                schema: "auth_management");
        }
    }
}
