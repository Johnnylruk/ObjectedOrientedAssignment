using EVotingSystem_SBMM.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using EVotingSystem_SBMM.Repository;

namespace EVotingSystem_SBMM.Data
{
    public class EVotingSystemDB : DbContext
    {
        public EVotingSystemDB(DbContextOptions<EVotingSystemDB> options) : base(options)
        {
        }

        public DbSet<VoterModel> Voters { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<CandidateModel> Candidates { get; set; }
        public DbSet<VoteModel> Votes { get; set; }
        
        public DbSet<EventModel> Events { get; set; }
        public DbSet<VotePreferenceModel> VotePreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VoterModel>()
                .HasKey(v => v.Id);
            modelBuilder.Entity<EventModel>()
                .HasKey(v => v.EventId);

            modelBuilder.Entity<VoteModel>()
                .HasOne(v => v.Voter)
                .WithMany(v => v.Votes)
                .HasForeignKey(v => v.VoterId);

            modelBuilder.Entity<VoteModel>()
                .HasOne(v => v.Candidates)
                .WithMany(c => c.Votes)
                .HasForeignKey(v => v.CandidateId);

            modelBuilder.Entity<VoteModel>()
                .HasOne(v => v.Event)
                .WithMany(e => e.Votes)
                .HasForeignKey(v => v.EventId);

            modelBuilder.Entity<VotePreferenceModel>()
                .HasKey(vp => vp.Id);

            modelBuilder.Entity<VotePreferenceModel>()
                .HasOne(vp => vp.Votes)
                .WithMany(v => v.Preferences)
                .HasForeignKey(vp => vp.VoteId);

            modelBuilder.Entity<VotePreferenceModel>()
                .HasOne(vp => vp.Candidates)
                .WithMany()
                .HasForeignKey(vp => vp.CandidateId);
            
            modelBuilder.Entity<VotePreferenceModel>()
                .HasOne(vp => vp.Event)
                .WithMany()
                .HasForeignKey(vp => vp.EventId);
            
            modelBuilder.Entity<VotePreferenceModel>()
                .HasOne(vp => vp.Voters)
                .WithMany(vp => vp.PreferenceVotes)
                .HasForeignKey(vp => vp.VoterId);
        }
    }
}
    

