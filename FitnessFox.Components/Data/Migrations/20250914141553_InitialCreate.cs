using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessFox.Components.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    HeightInches = table.Column<float>(type: "REAL", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => new { x.Id, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "Foods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BrandRestaurant = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ServingSize = table.Column<float>(type: "REAL", nullable: false),
                    ServingUnit = table.Column<string>(type: "TEXT", nullable: false),
                    TotalServings = table.Column<float>(type: "REAL", nullable: false),
                    Calories = table.Column<float>(type: "REAL", nullable: false),
                    TotalFat = table.Column<float>(type: "REAL", nullable: false),
                    SaturatedFat = table.Column<float>(type: "REAL", nullable: false),
                    PolysaturatedFat = table.Column<float>(type: "REAL", nullable: false),
                    MonosaturatedFat = table.Column<float>(type: "REAL", nullable: false),
                    TransFat = table.Column<float>(type: "REAL", nullable: false),
                    Cholesterol = table.Column<float>(type: "REAL", nullable: false),
                    Sodium = table.Column<float>(type: "REAL", nullable: false),
                    Potassium = table.Column<float>(type: "REAL", nullable: false),
                    Carbs = table.Column<float>(type: "REAL", nullable: false),
                    Fiber = table.Column<float>(type: "REAL", nullable: false),
                    Sugar = table.Column<float>(type: "REAL", nullable: false),
                    Protein = table.Column<float>(type: "REAL", nullable: false),
                    VitaminA = table.Column<float>(type: "REAL", nullable: false),
                    VitaminC = table.Column<float>(type: "REAL", nullable: false),
                    Calcium = table.Column<float>(type: "REAL", nullable: false),
                    Iron = table.Column<float>(type: "REAL", nullable: false),
                    VitaminK = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Foods_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    NumberOfPeople = table.Column<float>(type: "REAL", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Calories = table.Column<float>(type: "REAL", nullable: false),
                    TotalFat = table.Column<float>(type: "REAL", nullable: false),
                    SaturatedFat = table.Column<float>(type: "REAL", nullable: false),
                    PolysaturatedFat = table.Column<float>(type: "REAL", nullable: false),
                    MonosaturatedFat = table.Column<float>(type: "REAL", nullable: false),
                    TransFat = table.Column<float>(type: "REAL", nullable: false),
                    Cholesterol = table.Column<float>(type: "REAL", nullable: false),
                    Sodium = table.Column<float>(type: "REAL", nullable: false),
                    Potassium = table.Column<float>(type: "REAL", nullable: false),
                    Carbs = table.Column<float>(type: "REAL", nullable: false),
                    Fiber = table.Column<float>(type: "REAL", nullable: false),
                    Sugar = table.Column<float>(type: "REAL", nullable: false),
                    Protein = table.Column<float>(type: "REAL", nullable: false),
                    VitaminA = table.Column<float>(type: "REAL", nullable: false),
                    VitaminC = table.Column<float>(type: "REAL", nullable: false),
                    Calcium = table.Column<float>(type: "REAL", nullable: false),
                    Iron = table.Column<float>(type: "REAL", nullable: false),
                    VitaminK = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<float>(type: "REAL", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGoals_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserVitals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVitals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserVitals_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeFoods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RecipeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FoodId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<float>(type: "REAL", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeFoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeFoods_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeFoods_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserMeals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Servings = table.Column<float>(type: "REAL", nullable: false),
                    FoodId = table.Column<int>(type: "INTEGER", nullable: true),
                    RecipeId = table.Column<int>(type: "INTEGER", nullable: true),
                    FoodId1 = table.Column<Guid>(type: "TEXT", nullable: true),
                    RecipeId1 = table.Column<Guid>(type: "TEXT", nullable: true),
                    Calories = table.Column<float>(type: "REAL", nullable: false),
                    TotalFat = table.Column<float>(type: "REAL", nullable: false),
                    SaturatedFat = table.Column<float>(type: "REAL", nullable: false),
                    PolysaturatedFat = table.Column<float>(type: "REAL", nullable: false),
                    MonosaturatedFat = table.Column<float>(type: "REAL", nullable: false),
                    TransFat = table.Column<float>(type: "REAL", nullable: false),
                    Cholesterol = table.Column<float>(type: "REAL", nullable: false),
                    Sodium = table.Column<float>(type: "REAL", nullable: false),
                    Potassium = table.Column<float>(type: "REAL", nullable: false),
                    Carbs = table.Column<float>(type: "REAL", nullable: false),
                    Fiber = table.Column<float>(type: "REAL", nullable: false),
                    Sugar = table.Column<float>(type: "REAL", nullable: false),
                    Protein = table.Column<float>(type: "REAL", nullable: false),
                    VitaminA = table.Column<float>(type: "REAL", nullable: false),
                    VitaminC = table.Column<float>(type: "REAL", nullable: false),
                    Calcium = table.Column<float>(type: "REAL", nullable: false),
                    Iron = table.Column<float>(type: "REAL", nullable: false),
                    VitaminK = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMeals_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMeals_Foods_FoodId1",
                        column: x => x.FoodId1,
                        principalTable: "Foods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserMeals_Recipes_RecipeId1",
                        column: x => x.RecipeId1,
                        principalTable: "Recipes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Foods_UserId",
                table: "Foods",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeFoods_FoodId",
                table: "RecipeFoods",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeFoods_RecipeId",
                table: "RecipeFoods",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_UserId",
                table: "Recipes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGoals_UserId",
                table: "UserGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMeals_FoodId1",
                table: "UserMeals",
                column: "FoodId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserMeals_RecipeId1",
                table: "UserMeals",
                column: "RecipeId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserMeals_UserId",
                table: "UserMeals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVitals_UserId",
                table: "UserVitals",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeFoods");

            migrationBuilder.DropTable(
                name: "UserGoals");

            migrationBuilder.DropTable(
                name: "UserMeals");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "UserVitals");

            migrationBuilder.DropTable(
                name: "Foods");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
