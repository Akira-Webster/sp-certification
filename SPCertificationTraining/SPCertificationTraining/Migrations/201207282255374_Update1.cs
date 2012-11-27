namespace SPCertificationTraining.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Update1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Problems", "Course_ID", "Courses");
            DropForeignKey("Choices", "Problem_ID", "Problems");
            DropForeignKey("Questions", "Test_ID", "Tests");
            DropForeignKey("Answers", "Question_ID", "Questions");
            DropIndex("Problems", new[] { "Course_ID" });
            DropIndex("Choices", new[] { "Problem_ID" });
            DropIndex("Questions", new[] { "Test_ID" });
            DropIndex("Answers", new[] { "Question_ID" });
            RenameColumn(table: "Problems", name: "Course_ID", newName: "CourseID");
            RenameColumn(table: "Choices", name: "Problem_ID", newName: "ProblemID");
            RenameColumn(table: "Questions", name: "Test_ID", newName: "TestID");
            RenameColumn(table: "Answers", name: "Question_ID", newName: "QuestionID");
            AddForeignKey("Problems", "CourseID", "Courses", "ID", cascadeDelete: true);
            AddForeignKey("Choices", "ProblemID", "Problems", "ID", cascadeDelete: true);
            AddForeignKey("Questions", "TestID", "Tests", "ID", cascadeDelete: true);
            AddForeignKey("Answers", "QuestionID", "Questions", "ID", cascadeDelete: true);
            CreateIndex("Problems", "CourseID");
            CreateIndex("Choices", "ProblemID");
            CreateIndex("Questions", "TestID");
            CreateIndex("Answers", "QuestionID");
        }
        
        public override void Down()
        {
            DropIndex("Answers", new[] { "QuestionID" });
            DropIndex("Questions", new[] { "TestID" });
            DropIndex("Choices", new[] { "ProblemID" });
            DropIndex("Problems", new[] { "CourseID" });
            DropForeignKey("Answers", "QuestionID", "Questions");
            DropForeignKey("Questions", "TestID", "Tests");
            DropForeignKey("Choices", "ProblemID", "Problems");
            DropForeignKey("Problems", "CourseID", "Courses");
            RenameColumn(table: "Answers", name: "QuestionID", newName: "Question_ID");
            RenameColumn(table: "Questions", name: "TestID", newName: "Test_ID");
            RenameColumn(table: "Choices", name: "ProblemID", newName: "Problem_ID");
            RenameColumn(table: "Problems", name: "CourseID", newName: "Course_ID");
            CreateIndex("Answers", "Question_ID");
            CreateIndex("Questions", "Test_ID");
            CreateIndex("Choices", "Problem_ID");
            CreateIndex("Problems", "Course_ID");
            AddForeignKey("Answers", "Question_ID", "Questions", "ID");
            AddForeignKey("Questions", "Test_ID", "Tests", "ID");
            AddForeignKey("Choices", "Problem_ID", "Problems", "ID");
            AddForeignKey("Problems", "Course_ID", "Courses", "ID");
        }
    }
}
