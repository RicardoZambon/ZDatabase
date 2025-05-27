using System.Text.Json;
using ZDatabase.SpecFlowTests.Fakes;
using ZDatabase.SpecFlowTests.Fakes.Entities;

namespace ZDatabase.SpecFlowTests.Support
{
    public class DatabaseSeedDriver
    {
        private readonly DbContextFake _dbContext;

        public DatabaseSeedDriver(DbContextFake dbContext)
        {
            _dbContext = dbContext;
        }

        public void SeedAll()
        {
            SeedClients();
            SeedRoles();
            SeedUsers();
            SeedProducts();
            SeedPurchases();
            SeedPurchasesItems();
            SeedRiskAssessments();
            _dbContext.SaveChanges();
        }

        public void SeedUsers()
        {
            var json = File.ReadAllText("Fakes/SeedData/SeedData.UsersFake.json");
            var users = JsonSerializer.Deserialize<List<UsersFake>>(json)!;
            _dbContext.Set<UsersFake>().AddRange(users);
        }

        public void SeedRoles()
        {
            var json = File.ReadAllText("Fakes/SeedData/SeedData.RolesFake.json");
            var roles = JsonSerializer.Deserialize<List<RolesFake>>(json)!;
            _dbContext.Set<RolesFake>().AddRange(roles);
        }

        public void SeedClients()
        {
            var json = File.ReadAllText("Fakes/SeedData/SeedData.ClientsFake.json");
            var clients = JsonSerializer.Deserialize<List<ClientsFake>>(json)!;
            _dbContext.Set<ClientsFake>().AddRange(clients);
        }

        public void SeedPurchases()
        {
            var json = File.ReadAllText("Fakes/SeedData/SeedData.PurchasesFake.json");
            var purchases = JsonSerializer.Deserialize<List<PurchasesFake>>(json)!;
            _dbContext.Set<PurchasesFake>().AddRange(purchases);
        }

        public void SeedPurchasesItems()
        {
            var json = File.ReadAllText("Fakes/SeedData/SeedData.PurchasesItemsFake.json");
            var items = JsonSerializer.Deserialize<List<PurchasesItemsFake>>(json)!;
            _dbContext.Set<PurchasesItemsFake>().AddRange(items);
        }

        public void SeedProducts()
        {
            var json = File.ReadAllText("Fakes/SeedData/SeedData.ProductsFake.json");
            var products = JsonSerializer.Deserialize<List<ProductsFake>>(json)!;
            _dbContext.Set<ProductsFake>().AddRange(products);
        }

        public void SeedRiskAssessments()
        {
            var json = File.ReadAllText("Fakes/SeedData/SeedData.RiskAssessmentsFake.json");
            var risks = JsonSerializer.Deserialize<List<RiskAssessmentsFake>>(json)!;
            _dbContext.Set<RiskAssessmentsFake>().AddRange(risks);
        }
    }
}
