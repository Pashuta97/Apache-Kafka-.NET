using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ST_KafkaConsumer
{
    public partial class KafkaContext : DbContext
    {
        public KafkaContext()
        {
        }

        public KafkaContext(DbContextOptions<KafkaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Temperature> Temperatures { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=PASHUTA-PC\\KAFKASERVER;Initial Catalog=Kafka;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Temperature>(entity =>
            {
                entity.ToTable("Temperature");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
