namespace SPCertificationTraining.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Update2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Tests", "Date", c => c.DateTime(precision: 0, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("Tests", "Date");
        }
    }
}
