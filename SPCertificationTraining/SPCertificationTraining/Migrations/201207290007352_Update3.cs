namespace SPCertificationTraining.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Update3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Tests", "Date", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("Tests", "Date", c => c.DateTime(precision: 0, storeType: "datetime2"));
        }
    }
}
