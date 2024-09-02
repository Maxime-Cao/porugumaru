using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Puroguramu.Infrastructures.Migrations.SQLServerMigrations
{
    public partial class InitSQLServer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    IdGroup = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.IdGroup);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.LessonId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Matricule = table.Column<string>(type: "nchar(7)", fixedLength: true, maxLength: 7, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LabGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Groups_LabGroupId",
                        column: x => x.LabGroupId,
                        principalTable: "Groups",
                        principalColumn: "IdGroup",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Difficulty = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Template = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stub = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Solution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.ExerciseId);
                    table.ForeignKey(
                        name: "FK_Exercises_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "LessonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseAttempts",
                columns: table => new
                {
                    AttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Proposal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExerciseStatus = table.Column<int>(type: "int", nullable: false),
                    AttemptTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseAttempts", x => x.AttemptId);
                    table.ForeignKey(
                        name: "FK_ExerciseAttempts_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseAttempts_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "ExerciseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "MSI25-06-24 23:48:00Student", "MSI25-06-24 23:48:00Student", "Student", "STUDENT" },
                    { "MSI25-06-24 23:48:00Teacher", "MSI25-06-24 23:48:00Teacher", "Teacher", "TEACHER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LockoutEnabled", "LockoutEnd", "Matricule", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PhotoPath", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "MSI25-06-24 23:48:00Dorian", 0, "35513f4d-8a97-4a7a-8f13-ea73e8971a2a", "IdentityTeacher", "d.lauwers@helmo.be", true, "Dorian", false, null, "p180039", "Lauwers", "D.LAUWERS@HELMO.BE", null, "AQAAAAEAACcQAAAAEDL7Zm+1AKGhBniPKQb4mCRICbsbngyd6484JDWd/BImiSU8awhv7ne2B2ew1GUsGw==", null, false, null, "b0657f90-9e49-4463-8a0e-33863c879f84", false, "d.lauwers@helmo.be" },
                    { "MSI25-06-24 23:48:00Nicolas", 0, "ffa29c2e-eec5-4431-a6c7-ccf67ff0a375", "IdentityTeacher", "n.hendrikx@helmo.be", true, "Nicolas", false, null, "p070039", "Hendrikx", "N.HENDRIKX@HELMO.BE", null, "AQAAAAEAACcQAAAAEB76QqrnWgokxUtxsOrSqIw05aM9IPNmQZsTgtvG7sMJIbM2gGnMrepaal1r3FQvpw==", null, false, null, "f7ff7b6d-b44d-4ea1-91a5-f79e21d5bbd1", false, "n.hendrikx@helmo.be" }
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "IdGroup", "GroupName" },
                values: new object[,]
                {
                    { new Guid("0092cae5-23fb-458a-bd67-21764203695f"), "2i3" },
                    { new Guid("57c96a00-e9f5-450e-b23f-5aaec0b737dc"), "2i4" },
                    { new Guid("b72ab3e2-d0d8-4521-8c3e-58074a5cceb9"), "2i2" },
                    { new Guid("dbae745e-0565-4228-8df8-23026243157f"), "2i1" }
                });

            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "LessonId", "Description", "IsPublished", "Position", "Title" },
                values: new object[,]
                {
                    { new Guid("9b5de29a-2216-4992-82c0-6b0b02cccc52"), "Ceci est la description de mon incroyable leçon v1", true, 1, "Mon incroyable leçon v1" },
                    { new Guid("beb3c393-9c58-4d7d-a571-6b9a6fa8e3f8"), "Ceci est la description de mon incroyable leçon v2", true, 2, "Mon incroyable leçon v2" },
                    { new Guid("c80e3592-40e8-446b-9530-2acfb60f7a38"), "Ceci est la description de mon incroyable leçon v4", true, 4, "Mon incroyable leçon v4" },
                    { new Guid("e1da2ee0-cc2f-4d4b-ad77-f5bbf612577e"), "Ceci est la description de mon incroyable leçon v3", true, 3, "Mon incroyable leçon v3" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "MSI25-06-24 23:48:00Teacher", "MSI25-06-24 23:48:00Dorian" },
                    { "MSI25-06-24 23:48:00Teacher", "MSI25-06-24 23:48:00Nicolas" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LabGroupId", "LockoutEnabled", "LockoutEnd", "Matricule", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PhotoPath", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "MSI25-06-24 23:48:00Maxime", 0, "90427301-3d98-40aa-aae8-96c8f43b65fe", "IdentityStudent", "m.cao@student.helmo.be", true, "Maxime", new Guid("dbae745e-0565-4228-8df8-23026243157f"), false, null, "d170051", "Cao", "M.CAO@STUDENT.HELMO.BE", null, "AQAAAAEAACcQAAAAEHHL/hETC8UxLJwUaegs3a9nsXmdx5FcmvBOfSipbtJ1JViIwg8u2UnN+B/WaGdzyA==", null, false, null, "153670d3-38ba-4570-a146-1d9bc48cbd64", false, "m.cao@student.helmo.be" },
                    { "MSI25-06-24 23:48:00Megan", 0, "2f2267f3-4bee-48c2-ae6e-da841a0556fd", "IdentityStudent", "m.levieux@student.helmo.be", true, "Megan", new Guid("b72ab3e2-d0d8-4521-8c3e-58074a5cceb9"), false, null, "d170000", "Levieux", "M.LEVIEUX@STUDENT.HELMO.BE", null, "AQAAAAEAACcQAAAAEOUWKtknS7cVUJkkbh1EPzRkhIqlNuOI1A07TSp44GLQgyszJWFG0m69dYYPyFCqFw==", null, false, null, "3865fb17-88d0-4f36-b383-2e24c9c97454", false, "m.levieux@student.helmo.be" }
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "ExerciseId", "Difficulty", "Instructions", "IsPublished", "LessonId", "Position", "Solution", "Stub", "Template", "Title" },
                values: new object[,]
                {
                    { new Guid("12ba9d1b-49aa-4d51-9319-f1f70fbfd7b5"), "Easy", "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.", true, new Guid("9b5de29a-2216-4992-82c0-6b0b02cccc52"), 1, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "public class Exercice\n{\n  // Tapez votre code ici\n}\n", "// code-insertion-point\n\npublic class Test\n{\n    public static TestResult Ensure(float b, int exponent, float expected)\n    {\n      TestStatus status = TestStatus.Passed;\n      float actual = float.NaN;\n      try\n      {\n         actual = Exercice.Power(b, exponent);\n         if(Math.Abs(actual - expected) > 0.00001f)\n         {\n             status = TestStatus.Failed;\n         }\n      }\n      catch(Exception ex)\n      {\n         status = TestStatus.Inconclusive;\n      }\n\n      return new TestResult(\n        string.Format(\"Power of {0} by {1} should be {2}\", b, exponent, expected),\n        status,\n        status == TestStatus.Passed ? string.Empty : string.Format(\"Expected {0}. Got {1}.\", expected, actual)\n      );\n    }\n}\n\nreturn new TestResult[] {\n  Test.Ensure(2, 4, 16.0f),\n  Test.Ensure(2, -4, 1.0f/16.0f)\n};\n", "Calcul de puissance en C# v1" },
                    { new Guid("36460812-6008-42f4-8032-add056ebf0a6"), "Hard", "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.", true, new Guid("c80e3592-40e8-446b-9530-2acfb60f7a38"), 4, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "public class Exercice\n{\n  // Tapez votre code ici\n}\n", "// code-insertion-point\n\npublic class Test\n{\n    public static TestResult Ensure(float b, int exponent, float expected)\n    {\n      TestStatus status = TestStatus.Passed;\n      float actual = float.NaN;\n      try\n      {\n         actual = Exercice.Power(b, exponent);\n         if(Math.Abs(actual - expected) > 0.00001f)\n         {\n             status = TestStatus.Failed;\n         }\n      }\n      catch(Exception ex)\n      {\n         status = TestStatus.Inconclusive;\n      }\n\n      return new TestResult(\n        string.Format(\"Power of {0} by {1} should be {2}\", b, exponent, expected),\n        status,\n        status == TestStatus.Passed ? string.Empty : string.Format(\"Expected {0}. Got {1}.\", expected, actual)\n      );\n    }\n}\n\nreturn new TestResult[] {\n  Test.Ensure(2, 4, 16.0f),\n  Test.Ensure(2, -4, 1.0f/16.0f)\n};\n", "Calcul de puissance en C# v10" },
                    { new Guid("4941040a-2ede-47a6-ac13-596676fdc33c"), "Medium", "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.", true, new Guid("e1da2ee0-cc2f-4d4b-ad77-f5bbf612577e"), 1, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "public class Exercice\n{\n  // Tapez votre code ici\n}\n", "// code-insertion-point\n\npublic class Test\n{\n    public static TestResult Ensure(float b, int exponent, float expected)\n    {\n      TestStatus status = TestStatus.Passed;\n      float actual = float.NaN;\n      try\n      {\n         actual = Exercice.Power(b, exponent);\n         if(Math.Abs(actual - expected) > 0.00001f)\n         {\n             status = TestStatus.Failed;\n         }\n      }\n      catch(Exception ex)\n      {\n         status = TestStatus.Inconclusive;\n      }\n\n      return new TestResult(\n        string.Format(\"Power of {0} by {1} should be {2}\", b, exponent, expected),\n        status,\n        status == TestStatus.Passed ? string.Empty : string.Format(\"Expected {0}. Got {1}.\", expected, actual)\n      );\n    }\n}\n\nreturn new TestResult[] {\n  Test.Ensure(2, 4, 16.0f),\n  Test.Ensure(2, -4, 1.0f/16.0f)\n};\n", "Calcul de puissance en C# v6" },
                    { new Guid("7eeb5b33-2776-48d2-ae51-fed8cac1a117"), "Medium", "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.", true, new Guid("c80e3592-40e8-446b-9530-2acfb60f7a38"), 2, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "public class Exercice\n{\n  // Tapez votre code ici\n}\n", "// code-insertion-point\n\npublic class Test\n{\n    public static TestResult Ensure(float b, int exponent, float expected)\n    {\n      TestStatus status = TestStatus.Passed;\n      float actual = float.NaN;\n      try\n      {\n         actual = Exercice.Power(b, exponent);\n         if(Math.Abs(actual - expected) > 0.00001f)\n         {\n             status = TestStatus.Failed;\n         }\n      }\n      catch(Exception ex)\n      {\n         status = TestStatus.Inconclusive;\n      }\n\n      return new TestResult(\n        string.Format(\"Power of {0} by {1} should be {2}\", b, exponent, expected),\n        status,\n        status == TestStatus.Passed ? string.Empty : string.Format(\"Expected {0}. Got {1}.\", expected, actual)\n      );\n    }\n}\n\nreturn new TestResult[] {\n  Test.Ensure(2, 4, 16.0f),\n  Test.Ensure(2, -4, 1.0f/16.0f)\n};\n", "Calcul de puissance en C# v8" },
                    { new Guid("8566b6d9-1460-4517-b378-5eb763b1a11c"), "Medium", "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.", true, new Guid("9b5de29a-2216-4992-82c0-6b0b02cccc52"), 2, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "public class Exercice\n{\n  // Tapez votre code ici\n}\n", "// code-insertion-point\n\npublic class Test\n{\n    public static TestResult Ensure(float b, int exponent, float expected)\n    {\n      TestStatus status = TestStatus.Passed;\n      float actual = float.NaN;\n      try\n      {\n         actual = Exercice.Power(b, exponent);\n         if(Math.Abs(actual - expected) > 0.00001f)\n         {\n             status = TestStatus.Failed;\n         }\n      }\n      catch(Exception ex)\n      {\n         status = TestStatus.Inconclusive;\n      }\n\n      return new TestResult(\n        string.Format(\"Power of {0} by {1} should be {2}\", b, exponent, expected),\n        status,\n        status == TestStatus.Passed ? string.Empty : string.Format(\"Expected {0}. Got {1}.\", expected, actual)\n      );\n    }\n}\n\nreturn new TestResult[] {\n  Test.Ensure(2, 4, 16.0f),\n  Test.Ensure(2, -4, 1.0f/16.0f)\n};\n", "Calcul de puissance en C# v2" },
                    { new Guid("d8b81be3-ef9d-4f56-b565-a2d7d7946081"), "Medium", "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.", true, new Guid("c80e3592-40e8-446b-9530-2acfb60f7a38"), 3, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "public class Exercice\n{\n  // Tapez votre code ici\n}\n", "// code-insertion-point\n\npublic class Test\n{\n    public static TestResult Ensure(float b, int exponent, float expected)\n    {\n      TestStatus status = TestStatus.Passed;\n      float actual = float.NaN;\n      try\n      {\n         actual = Exercice.Power(b, exponent);\n         if(Math.Abs(actual - expected) > 0.00001f)\n         {\n             status = TestStatus.Failed;\n         }\n      }\n      catch(Exception ex)\n      {\n         status = TestStatus.Inconclusive;\n      }\n\n      return new TestResult(\n        string.Format(\"Power of {0} by {1} should be {2}\", b, exponent, expected),\n        status,\n        status == TestStatus.Passed ? string.Empty : string.Format(\"Expected {0}. Got {1}.\", expected, actual)\n      );\n    }\n}\n\nreturn new TestResult[] {\n  Test.Ensure(2, 4, 16.0f),\n  Test.Ensure(2, -4, 1.0f/16.0f)\n};\n", "Calcul de puissance en C# v9" },
                    { new Guid("e019163d-076f-465a-bc4e-59af11cdd2b9"), "Easy", "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.", true, new Guid("c80e3592-40e8-446b-9530-2acfb60f7a38"), 1, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "public class Exercice\n{\n  // Tapez votre code ici\n}\n", "// code-insertion-point\n\npublic class Test\n{\n    public static TestResult Ensure(float b, int exponent, float expected)\n    {\n      TestStatus status = TestStatus.Passed;\n      float actual = float.NaN;\n      try\n      {\n         actual = Exercice.Power(b, exponent);\n         if(Math.Abs(actual - expected) > 0.00001f)\n         {\n             status = TestStatus.Failed;\n         }\n      }\n      catch(Exception ex)\n      {\n         status = TestStatus.Inconclusive;\n      }\n\n      return new TestResult(\n        string.Format(\"Power of {0} by {1} should be {2}\", b, exponent, expected),\n        status,\n        status == TestStatus.Passed ? string.Empty : string.Format(\"Expected {0}. Got {1}.\", expected, actual)\n      );\n    }\n}\n\nreturn new TestResult[] {\n  Test.Ensure(2, 4, 16.0f),\n  Test.Ensure(2, -4, 1.0f/16.0f)\n};\n", "Calcul de puissance en C# v7" },
                    { new Guid("ec4e79f3-f907-4dcc-adbe-a82455b39889"), "Hard", "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.", true, new Guid("beb3c393-9c58-4d7d-a571-6b9a6fa8e3f8"), 3, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "public class Exercice\n{\n  // Tapez votre code ici\n}\n", "// code-insertion-point\n\npublic class Test\n{\n    public static TestResult Ensure(float b, int exponent, float expected)\n    {\n      TestStatus status = TestStatus.Passed;\n      float actual = float.NaN;\n      try\n      {\n         actual = Exercice.Power(b, exponent);\n         if(Math.Abs(actual - expected) > 0.00001f)\n         {\n             status = TestStatus.Failed;\n         }\n      }\n      catch(Exception ex)\n      {\n         status = TestStatus.Inconclusive;\n      }\n\n      return new TestResult(\n        string.Format(\"Power of {0} by {1} should be {2}\", b, exponent, expected),\n        status,\n        status == TestStatus.Passed ? string.Empty : string.Format(\"Expected {0}. Got {1}.\", expected, actual)\n      );\n    }\n}\n\nreturn new TestResult[] {\n  Test.Ensure(2, 4, 16.0f),\n  Test.Ensure(2, -4, 1.0f/16.0f)\n};\n", "Calcul de puissance en C# v5" },
                    { new Guid("edc48bbf-ef66-498e-9163-133aecde56f7"), "Easy", "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.", true, new Guid("beb3c393-9c58-4d7d-a571-6b9a6fa8e3f8"), 1, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "public class Exercice\n{\n  // Tapez votre code ici\n}\n", "// code-insertion-point\n\npublic class Test\n{\n    public static TestResult Ensure(float b, int exponent, float expected)\n    {\n      TestStatus status = TestStatus.Passed;\n      float actual = float.NaN;\n      try\n      {\n         actual = Exercice.Power(b, exponent);\n         if(Math.Abs(actual - expected) > 0.00001f)\n         {\n             status = TestStatus.Failed;\n         }\n      }\n      catch(Exception ex)\n      {\n         status = TestStatus.Inconclusive;\n      }\n\n      return new TestResult(\n        string.Format(\"Power of {0} by {1} should be {2}\", b, exponent, expected),\n        status,\n        status == TestStatus.Passed ? string.Empty : string.Format(\"Expected {0}. Got {1}.\", expected, actual)\n      );\n    }\n}\n\nreturn new TestResult[] {\n  Test.Ensure(2, 4, 16.0f),\n  Test.Ensure(2, -4, 1.0f/16.0f)\n};\n", "Calcul de puissance en C# v3" },
                    { new Guid("fcc5b251-4492-4f50-8842-51d13d2eb108"), "Medium", "Créez une fonction Power C# prenant en paramètre une base b de type float et un exposant e de type int. Power(b, e) retourne le float b exposant e.", true, new Guid("beb3c393-9c58-4d7d-a571-6b9a6fa8e3f8"), 2, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "public class Exercice\n{\n  // Tapez votre code ici\n}\n", "// code-insertion-point\n\npublic class Test\n{\n    public static TestResult Ensure(float b, int exponent, float expected)\n    {\n      TestStatus status = TestStatus.Passed;\n      float actual = float.NaN;\n      try\n      {\n         actual = Exercice.Power(b, exponent);\n         if(Math.Abs(actual - expected) > 0.00001f)\n         {\n             status = TestStatus.Failed;\n         }\n      }\n      catch(Exception ex)\n      {\n         status = TestStatus.Inconclusive;\n      }\n\n      return new TestResult(\n        string.Format(\"Power of {0} by {1} should be {2}\", b, exponent, expected),\n        status,\n        status == TestStatus.Passed ? string.Empty : string.Format(\"Expected {0}. Got {1}.\", expected, actual)\n      );\n    }\n}\n\nreturn new TestResult[] {\n  Test.Ensure(2, 4, 16.0f),\n  Test.Ensure(2, -4, 1.0f/16.0f)\n};\n", "Calcul de puissance en C# v4" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "MSI25-06-24 23:48:00Student", "MSI25-06-24 23:48:00Maxime" },
                    { "MSI25-06-24 23:48:00Student", "MSI25-06-24 23:48:00Megan" }
                });

            migrationBuilder.InsertData(
                table: "ExerciseAttempts",
                columns: new[] { "AttemptId", "AttemptTime", "ExerciseId", "ExerciseStatus", "Proposal", "StudentId" },
                values: new object[,]
                {
                    { new Guid("0385cc18-cc16-4cea-9567-356bf22f9194"), new DateTime(2024, 8, 7, 12, 30, 36, 759, DateTimeKind.Local).AddTicks(2857), new Guid("edc48bbf-ef66-498e-9163-133aecde56f7"), 3, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "MSI25-06-24 23:48:00Maxime" },
                    { new Guid("16b92fef-19ed-44ac-a602-0f7ff3d8be99"), new DateTime(2024, 8, 7, 12, 30, 36, 759, DateTimeKind.Local).AddTicks(2863), new Guid("fcc5b251-4492-4f50-8842-51d13d2eb108"), 3, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "MSI25-06-24 23:48:00Maxime" },
                    { new Guid("2ccfd519-9925-442d-a988-1650f99b1076"), new DateTime(2024, 8, 7, 12, 30, 36, 759, DateTimeKind.Local).AddTicks(2576), new Guid("8566b6d9-1460-4517-b378-5eb763b1a11c"), 3, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "MSI25-06-24 23:48:00Maxime" },
                    { new Guid("47d1fbfe-2614-403a-b8a3-3f8fbb8125b8"), new DateTime(2024, 8, 7, 12, 30, 36, 759, DateTimeKind.Local).AddTicks(2895), new Guid("7eeb5b33-2776-48d2-ae51-fed8cac1a117"), 3, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "MSI25-06-24 23:48:00Megan" },
                    { new Guid("531be782-a0d1-4c6c-9a22-633f533a2b9a"), new DateTime(2024, 8, 7, 12, 30, 36, 759, DateTimeKind.Local).AddTicks(2882), new Guid("7eeb5b33-2776-48d2-ae51-fed8cac1a117"), 2, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return 1.0f;\n    }\n}\n", "MSI25-06-24 23:48:00Maxime" },
                    { new Guid("7af6d338-c4be-4cc2-8b6b-ddd74bb11553"), new DateTime(2024, 8, 7, 12, 30, 36, 759, DateTimeKind.Local).AddTicks(2886), new Guid("8566b6d9-1460-4517-b378-5eb763b1a11c"), 2, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return 1.0f;\n    }\n}\n", "MSI25-06-24 23:48:00Megan" },
                    { new Guid("7c207469-e108-4e84-a13b-c6fa59a7cad6"), new DateTime(2024, 8, 7, 12, 30, 36, 759, DateTimeKind.Local).AddTicks(2890), new Guid("edc48bbf-ef66-498e-9163-133aecde56f7"), 3, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "MSI25-06-24 23:48:00Megan" },
                    { new Guid("90fd61c7-79f3-4b34-9a46-bbc763164134"), new DateTime(2024, 8, 7, 12, 30, 36, 759, DateTimeKind.Local).AddTicks(2899), new Guid("d8b81be3-ef9d-4f56-b565-a2d7d7946081"), 3, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "MSI25-06-24 23:48:00Megan" },
                    { new Guid("94ecc30b-f54f-4939-86d8-b5d308d25702"), new DateTime(2024, 8, 7, 12, 30, 36, 759, DateTimeKind.Local).AddTicks(2873), new Guid("ec4e79f3-f907-4dcc-adbe-a82455b39889"), 3, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "MSI25-06-24 23:48:00Maxime" },
                    { new Guid("a72bf326-dd03-4f2f-9e0e-585eb69c836b"), new DateTime(2024, 8, 7, 12, 30, 36, 759, DateTimeKind.Local).AddTicks(2878), new Guid("e019163d-076f-465a-bc4e-59af11cdd2b9"), 3, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return (float)Math.Pow(b, e);\n    }\n}\n", "MSI25-06-24 23:48:00Maxime" },
                    { new Guid("d072ac27-7d77-4d28-8b56-b6453a01922b"), new DateTime(2024, 8, 7, 12, 30, 36, 759, DateTimeKind.Local).AddTicks(2903), new Guid("36460812-6008-42f4-8032-add056ebf0a6"), 2, "public class Exercice\n{\n    public static float Power(float b, int e)\n    {\n        return 1.0f;\n    }\n}\n", "MSI25-06-24 23:48:00Megan" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LabGroupId",
                table: "AspNetUsers",
                column: "LabGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Matricule",
                table: "AspNetUsers",
                column: "Matricule",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseAttempts_ExerciseId",
                table: "ExerciseAttempts",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseAttempts_StudentId",
                table: "ExerciseAttempts",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_LessonId_Position",
                table: "Exercises",
                columns: new[] { "LessonId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_LessonId_Title",
                table: "Exercises",
                columns: new[] { "LessonId", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_GroupName",
                table: "Groups",
                column: "GroupName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_Position",
                table: "Lessons",
                column: "Position",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_Title",
                table: "Lessons",
                column: "Title",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ExerciseAttempts");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Lessons");
        }
    }
}
