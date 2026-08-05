using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAS.Assets.Persistence.Migrations {

    /// <inheritdoc />
    public partial class V_1_0_0 : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.EnsureSchema(
                name: "Asset");

            migrationBuilder.CreateSequence(
                name: "FundNavSeq",
                schema: "Asset",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "FundSeq",
                schema: "Asset",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "Asset",
                columns: table => new {
                    Id = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    EnglishName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Funds",
                schema: "Asset",
                columns: table => new {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CurrencyId = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Isin = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Funds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Funds_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "Asset",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundNavs",
                schema: "Asset",
                columns: table => new {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    FundId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_FundNavs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FundNavs_Funds_FundId",
                        column: x => x.FundId,
                        principalSchema: "Asset",
                        principalTable: "Funds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FundNavs_Date",
                schema: "Asset",
                table: "FundNavs",
                column: "Date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FundNavs_FundId",
                schema: "Asset",
                table: "FundNavs",
                column: "FundId");

            migrationBuilder.CreateIndex(
                name: "IX_Funds_CurrencyId",
                schema: "Asset",
                table: "Funds",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Funds_Name",
                schema: "Asset",
                table: "Funds",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Funds_Status",
                schema: "Asset",
                table: "Funds",
                column: "Status");

            // Add default currencies
            // Not working with HasData because Symbol is defined as a ComplexProperty,
            // so we use raw SQL to insert the default currencies
            // (see EF issue: https://github.com/dotnet/efcore/issues/31254)
            migrationBuilder.Sql(@"
                INSERT INTO [Asset].[Currencies] (Id, EnglishName, Symbol) VALUES 
                ('EUR', 'Euro', '€'),
                ('USD', 'US Dollar', '$'),
                ('GBP', 'British Pound', '£'),
                ('JPY', 'Japanese Yen', '¥'),
                ('CHF', 'Swiss Franc', 'CHF');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "FundNavs",
                schema: "Asset");

            migrationBuilder.DropTable(
                name: "Funds",
                schema: "Asset");

            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "Asset");

            migrationBuilder.DropSequence(
                name: "FundNavSeq",
                schema: "Asset");

            migrationBuilder.DropSequence(
                name: "FundSeq",
                schema: "Asset");
        }
    }
}
