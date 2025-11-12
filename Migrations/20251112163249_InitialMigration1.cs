using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_Question_QuestionId",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Attempt_Students_StudentId",
                table: "Attempt");

            migrationBuilder.DropForeignKey(
                name: "FK_Attempt_Test_TestId",
                table: "Attempt");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_Test_TestId",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_User_UserId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_test_courses_Test_TestsId",
                table: "test_courses");

            migrationBuilder.DropForeignKey(
                name: "FK_test_directions_Test_TestsId",
                table: "test_directions");

            migrationBuilder.DropForeignKey(
                name: "FK_test_groups_Test_TestsId",
                table: "test_groups");

            migrationBuilder.DropForeignKey(
                name: "FK_test_projects_Test_TestsId",
                table: "test_projects");

            migrationBuilder.DropForeignKey(
                name: "FK_test_students_Test_TestsId",
                table: "test_students");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResult_Attempt_AttemptId",
                table: "TestResult");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResult_Students_StudentId",
                table: "TestResult");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResult_Test_TestId",
                table: "TestResult");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAttemptAnswer_Attempt_AttemptId",
                table: "UserAttemptAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAttemptAnswer_Question_QuestionId",
                table: "UserAttemptAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSelectedOption_Answer_AnswerId",
                table: "UserSelectedOption");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSelectedOption_UserAttemptAnswer_UserAttemptAnswerId",
                table: "UserSelectedOption");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSelectedOption_UserTextAnswer_UserTextAnswerId",
                table: "UserSelectedOption");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTextAnswer_UserAttemptAnswer_UserAttemptAnswerId",
                table: "UserTextAnswer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTextAnswer",
                table: "UserTextAnswer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSelectedOption",
                table: "UserSelectedOption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAttemptAnswer",
                table: "UserAttemptAnswer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestResult",
                table: "TestResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Test",
                table: "Test");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attempt",
                table: "Attempt");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answer",
                table: "Answer");

            migrationBuilder.RenameTable(
                name: "UserTextAnswer",
                newName: "UserTextAnswers");

            migrationBuilder.RenameTable(
                name: "UserSelectedOption",
                newName: "UserSelectedOptions");

            migrationBuilder.RenameTable(
                name: "UserAttemptAnswer",
                newName: "UserAttemptAnswers");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "TestResult",
                newName: "TestResults");

            migrationBuilder.RenameTable(
                name: "Test",
                newName: "Tests");

            migrationBuilder.RenameTable(
                name: "Attempt",
                newName: "Attempts");

            migrationBuilder.RenameTable(
                name: "Answer",
                newName: "Answers");

            migrationBuilder.RenameIndex(
                name: "IX_UserTextAnswer_UserAttemptAnswerId",
                table: "UserTextAnswers",
                newName: "IX_UserTextAnswers_UserAttemptAnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSelectedOption_UserTextAnswerId",
                table: "UserSelectedOptions",
                newName: "IX_UserSelectedOptions_UserTextAnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSelectedOption_UserAttemptAnswerId",
                table: "UserSelectedOptions",
                newName: "IX_UserSelectedOptions_UserAttemptAnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSelectedOption_AnswerId",
                table: "UserSelectedOptions",
                newName: "IX_UserSelectedOptions_AnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAttemptAnswer_QuestionId",
                table: "UserAttemptAnswers",
                newName: "IX_UserAttemptAnswers_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAttemptAnswer_AttemptId",
                table: "UserAttemptAnswers",
                newName: "IX_UserAttemptAnswers_AttemptId");

            migrationBuilder.RenameIndex(
                name: "IX_User_Login",
                table: "Users",
                newName: "IX_Users_Login");

            migrationBuilder.RenameIndex(
                name: "IX_User_Email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameIndex(
                name: "IX_TestResult_TestId",
                table: "TestResults",
                newName: "IX_TestResults_TestId");

            migrationBuilder.RenameIndex(
                name: "IX_TestResult_StudentId",
                table: "TestResults",
                newName: "IX_TestResults_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_TestResult_AttemptId",
                table: "TestResults",
                newName: "IX_TestResults_AttemptId");

            migrationBuilder.RenameIndex(
                name: "IX_Attempt_TestId",
                table: "Attempts",
                newName: "IX_Attempts_TestId");

            migrationBuilder.RenameIndex(
                name: "IX_Attempt_StudentId",
                table: "Attempts",
                newName: "IX_Attempts_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Answer_QuestionId",
                table: "Answers",
                newName: "IX_Answers_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTextAnswers",
                table: "UserTextAnswers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSelectedOptions",
                table: "UserSelectedOptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAttemptAnswers",
                table: "UserAttemptAnswers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestResults",
                table: "TestResults",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tests",
                table: "Tests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attempts",
                table: "Attempts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answers",
                table: "Answers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Question_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attempts_Students_StudentId",
                table: "Attempts",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attempts_Tests_TestId",
                table: "Attempts",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Tests_TestId",
                table: "Question",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Users_UserId",
                table: "Students",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_test_courses_Tests_TestsId",
                table: "test_courses",
                column: "TestsId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_test_directions_Tests_TestsId",
                table: "test_directions",
                column: "TestsId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_test_groups_Tests_TestsId",
                table: "test_groups",
                column: "TestsId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_test_projects_Tests_TestsId",
                table: "test_projects",
                column: "TestsId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_test_students_Tests_TestsId",
                table: "test_students",
                column: "TestsId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Attempts_AttemptId",
                table: "TestResults",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Students_StudentId",
                table: "TestResults",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Tests_TestId",
                table: "TestResults",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAttemptAnswers_Attempts_AttemptId",
                table: "UserAttemptAnswers",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAttemptAnswers_Question_QuestionId",
                table: "UserAttemptAnswers",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSelectedOptions_Answers_AnswerId",
                table: "UserSelectedOptions",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSelectedOptions_UserAttemptAnswers_UserAttemptAnswerId",
                table: "UserSelectedOptions",
                column: "UserAttemptAnswerId",
                principalTable: "UserAttemptAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSelectedOptions_UserTextAnswers_UserTextAnswerId",
                table: "UserSelectedOptions",
                column: "UserTextAnswerId",
                principalTable: "UserTextAnswers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTextAnswers_UserAttemptAnswers_UserAttemptAnswerId",
                table: "UserTextAnswers",
                column: "UserAttemptAnswerId",
                principalTable: "UserAttemptAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Question_QuestionId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Attempts_Students_StudentId",
                table: "Attempts");

            migrationBuilder.DropForeignKey(
                name: "FK_Attempts_Tests_TestId",
                table: "Attempts");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_Tests_TestId",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Users_UserId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_test_courses_Tests_TestsId",
                table: "test_courses");

            migrationBuilder.DropForeignKey(
                name: "FK_test_directions_Tests_TestsId",
                table: "test_directions");

            migrationBuilder.DropForeignKey(
                name: "FK_test_groups_Tests_TestsId",
                table: "test_groups");

            migrationBuilder.DropForeignKey(
                name: "FK_test_projects_Tests_TestsId",
                table: "test_projects");

            migrationBuilder.DropForeignKey(
                name: "FK_test_students_Tests_TestsId",
                table: "test_students");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Attempts_AttemptId",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Students_StudentId",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_TestId",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAttemptAnswers_Attempts_AttemptId",
                table: "UserAttemptAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAttemptAnswers_Question_QuestionId",
                table: "UserAttemptAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSelectedOptions_Answers_AnswerId",
                table: "UserSelectedOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSelectedOptions_UserAttemptAnswers_UserAttemptAnswerId",
                table: "UserSelectedOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSelectedOptions_UserTextAnswers_UserTextAnswerId",
                table: "UserSelectedOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTextAnswers_UserAttemptAnswers_UserAttemptAnswerId",
                table: "UserTextAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTextAnswers",
                table: "UserTextAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSelectedOptions",
                table: "UserSelectedOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAttemptAnswers",
                table: "UserAttemptAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tests",
                table: "Tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestResults",
                table: "TestResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attempts",
                table: "Attempts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answers",
                table: "Answers");

            migrationBuilder.RenameTable(
                name: "UserTextAnswers",
                newName: "UserTextAnswer");

            migrationBuilder.RenameTable(
                name: "UserSelectedOptions",
                newName: "UserSelectedOption");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "UserAttemptAnswers",
                newName: "UserAttemptAnswer");

            migrationBuilder.RenameTable(
                name: "Tests",
                newName: "Test");

            migrationBuilder.RenameTable(
                name: "TestResults",
                newName: "TestResult");

            migrationBuilder.RenameTable(
                name: "Attempts",
                newName: "Attempt");

            migrationBuilder.RenameTable(
                name: "Answers",
                newName: "Answer");

            migrationBuilder.RenameIndex(
                name: "IX_UserTextAnswers_UserAttemptAnswerId",
                table: "UserTextAnswer",
                newName: "IX_UserTextAnswer_UserAttemptAnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSelectedOptions_UserTextAnswerId",
                table: "UserSelectedOption",
                newName: "IX_UserSelectedOption_UserTextAnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSelectedOptions_UserAttemptAnswerId",
                table: "UserSelectedOption",
                newName: "IX_UserSelectedOption_UserAttemptAnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSelectedOptions_AnswerId",
                table: "UserSelectedOption",
                newName: "IX_UserSelectedOption_AnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Login",
                table: "User",
                newName: "IX_User_Login");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "User",
                newName: "IX_User_Email");

            migrationBuilder.RenameIndex(
                name: "IX_UserAttemptAnswers_QuestionId",
                table: "UserAttemptAnswer",
                newName: "IX_UserAttemptAnswer_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAttemptAnswers_AttemptId",
                table: "UserAttemptAnswer",
                newName: "IX_UserAttemptAnswer_AttemptId");

            migrationBuilder.RenameIndex(
                name: "IX_TestResults_TestId",
                table: "TestResult",
                newName: "IX_TestResult_TestId");

            migrationBuilder.RenameIndex(
                name: "IX_TestResults_StudentId",
                table: "TestResult",
                newName: "IX_TestResult_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_TestResults_AttemptId",
                table: "TestResult",
                newName: "IX_TestResult_AttemptId");

            migrationBuilder.RenameIndex(
                name: "IX_Attempts_TestId",
                table: "Attempt",
                newName: "IX_Attempt_TestId");

            migrationBuilder.RenameIndex(
                name: "IX_Attempts_StudentId",
                table: "Attempt",
                newName: "IX_Attempt_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_QuestionId",
                table: "Answer",
                newName: "IX_Answer_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTextAnswer",
                table: "UserTextAnswer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSelectedOption",
                table: "UserSelectedOption",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAttemptAnswer",
                table: "UserAttemptAnswer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Test",
                table: "Test",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestResult",
                table: "TestResult",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attempt",
                table: "Attempt",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answer",
                table: "Answer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_Question_QuestionId",
                table: "Answer",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attempt_Students_StudentId",
                table: "Attempt",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attempt_Test_TestId",
                table: "Attempt",
                column: "TestId",
                principalTable: "Test",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Test_TestId",
                table: "Question",
                column: "TestId",
                principalTable: "Test",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_User_UserId",
                table: "Students",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_test_courses_Test_TestsId",
                table: "test_courses",
                column: "TestsId",
                principalTable: "Test",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_test_directions_Test_TestsId",
                table: "test_directions",
                column: "TestsId",
                principalTable: "Test",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_test_groups_Test_TestsId",
                table: "test_groups",
                column: "TestsId",
                principalTable: "Test",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_test_projects_Test_TestsId",
                table: "test_projects",
                column: "TestsId",
                principalTable: "Test",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_test_students_Test_TestsId",
                table: "test_students",
                column: "TestsId",
                principalTable: "Test",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResult_Attempt_AttemptId",
                table: "TestResult",
                column: "AttemptId",
                principalTable: "Attempt",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResult_Students_StudentId",
                table: "TestResult",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResult_Test_TestId",
                table: "TestResult",
                column: "TestId",
                principalTable: "Test",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAttemptAnswer_Attempt_AttemptId",
                table: "UserAttemptAnswer",
                column: "AttemptId",
                principalTable: "Attempt",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAttemptAnswer_Question_QuestionId",
                table: "UserAttemptAnswer",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSelectedOption_Answer_AnswerId",
                table: "UserSelectedOption",
                column: "AnswerId",
                principalTable: "Answer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSelectedOption_UserAttemptAnswer_UserAttemptAnswerId",
                table: "UserSelectedOption",
                column: "UserAttemptAnswerId",
                principalTable: "UserAttemptAnswer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSelectedOption_UserTextAnswer_UserTextAnswerId",
                table: "UserSelectedOption",
                column: "UserTextAnswerId",
                principalTable: "UserTextAnswer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTextAnswer_UserAttemptAnswer_UserAttemptAnswerId",
                table: "UserTextAnswer",
                column: "UserAttemptAnswerId",
                principalTable: "UserAttemptAnswer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
