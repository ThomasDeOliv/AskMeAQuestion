using AskMeAQuestion.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace AskMeAQuestion.Data
{
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="option"></param>
        public AppDbContext(DbContextOptions option) : base(option)
        {

        }

        // Commandes à Executer pour les migrations et la construction de la BDD avec dotnet ef
        // dotnet ef migrations add initial -p .\AskMeAQuestion.Data\AskMeAQuestion.Data.csproj -s .\AskMeAQuestion\AskMeAQuestion.csproj
        // dotnet ef database update -p .\AskMeAQuestion.Data\AskMeAQuestion.Data.csproj -s .\AskMeAQuestion\AskMeAQuestion.csproj

        /// <summary>
        /// Table participants
        /// </summary>
        public DbSet<Participant> Participants { get; set; }

        /// <summary>
        /// Table sondage
        /// </summary>
        public DbSet<Poll> Polls { get; set; }

        /// <summary>
        /// Table Reponses
        /// </summary>
        public DbSet<Response> Responses { get; set; }

        /// <summary>
        /// Table utilisateurs
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Table votes
        /// </summary>
        public DbSet<Vote> Votes { get; set; }

        /// <summary>
        /// Data seeding
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Utilisateur de base
            var user = new User { UserId = 1, UserName = "ADMIN", UserLogin = "Admin", UserMail = "tom.deoliv@outlook.com", UserPassword = "irsY9YV0r94dmQpyGSSSGmbftcQYHnW+P6IAPu6EGv8=" };

            modelBuilder.Entity<User>().HasData(user);

            base.OnModelCreating(modelBuilder);
        }
    }
}
