using Microsoft.EntityFrameworkCore;
using RiskManagement.Models;

public class RiskManagementDbContext : DbContext
{
    public RiskManagementDbContext(DbContextOptions<RiskManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Organization> Organizations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Risk> Risks { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<UserProfileChangeRequest> UserProfileChangeRequests { get; set; }
    public DbSet<Incident> Incidents { get; set; }
    public DbSet<ActionPlan> ActionPlans { get; set; }
    public DbSet<RiskAssessment> RiskAssessments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .ToTable("user")
            .Property(user => user.Id)
            .HasColumnName("id");

        modelBuilder.Entity<User>()
            .Property(user => user.FirstName)
            .HasColumnName("first_name");

        modelBuilder.Entity<User>()
            .Property(user => user.LastName)
            .HasColumnName("last_name");

        modelBuilder.Entity<User>()
            .Property(user => user.Email)
            .HasColumnName("email");

        modelBuilder.Entity<User>()
            .Property(user => user.PasswordHash)
            .HasColumnName("password");

        modelBuilder.Entity<User>()
            .Property(user => user.Role)
            .HasColumnName("user_role");

        modelBuilder.Entity<User>()
            .Property(user => user.Status)
            .HasColumnName("user_status");

        modelBuilder.Entity<User>()
            .Property(user => user.CreatedAt)
            .HasColumnName("created_at");

        modelBuilder.Entity<User>()
            .Property(user => user.UpdatedAt)
            .HasColumnName("updated_at");

        modelBuilder.Entity<User>()
            .Property(user => user.OrganizationId)
            .HasColumnName("organization_id");

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<Organization>()
            .ToTable("organization")
            .Property(organization => organization.Id)
            .HasColumnName("id");

        modelBuilder.Entity<Organization>()
            .Property(organization => organization.Name)
            .HasColumnName("organization_name");

        modelBuilder.Entity<Organization>()
            .Property(organization => organization.IsoScope)
            .HasColumnName("iso_scope");

        modelBuilder.Entity<Organization>()
            .Property(organization => organization.Status)
            .HasColumnName("organization_status");

        modelBuilder.Entity<Organization>()
            .Property(organization => organization.CreatedAt)
            .HasColumnName("created_at");

        modelBuilder.Entity<Organization>()
            .Property(organization => organization.UpdatedAt)
            .HasColumnName("updated_at");

        modelBuilder.Entity<Asset>()
            .ToTable("asset")
            .Property(asset => asset.Id)
            .HasColumnName("id");

        modelBuilder.Entity<Asset>()
            .Property(asset => asset.OrganizationId)
            .HasColumnName("organization_id");

        modelBuilder.Entity<Asset>()
            .Property(asset => asset.Name)
            .HasColumnName("asset_name");

        modelBuilder.Entity<Asset>()
            .Property(asset => asset.Definition)
            .HasColumnName("asset_definition");

        modelBuilder.Entity<Asset>()
            .Property(asset => asset.AssetType)
            .HasColumnName("asset_type");

        modelBuilder.Entity<Asset>()
            .Property(asset => asset.Classification)
            .HasColumnName("classification");

        modelBuilder.Entity<Asset>()
            .Property(asset => asset.Status)
            .HasColumnName("asset_status");

        modelBuilder.Entity<Asset>()
            .Property(asset => asset.CreatedAt)
            .HasColumnName("created_at");

        modelBuilder.Entity<Asset>()
            .Property(asset => asset.UpdatedAt)
            .HasColumnName("updated_at");

        modelBuilder.Entity<Risk>()
            .ToTable("risk")
            .Property(risk => risk.Id)
            .HasColumnName("id");

        modelBuilder.Entity<Risk>()
            .Property(risk => risk.OrganizationId)
            .HasColumnName("organization_id");

        modelBuilder.Entity<Risk>()
            .Property(risk => risk.Title)
            .HasColumnName("title");

        modelBuilder.Entity<Risk>()
            .Property(risk => risk.Description)
            .HasColumnName("risk_definition");

        modelBuilder.Entity<Risk>()
            .Property(risk => risk.OwnerUserId)
            .HasColumnName("risk_owner_user_id");

        modelBuilder.Entity<Risk>()
            .Property(risk => risk.AssetId)
            .HasColumnName("asset_id");

        modelBuilder.Entity<Risk>()
            .Property(risk => risk.Status)
            .HasColumnName("risk_status");

        modelBuilder.Entity<Risk>()
            .Property(risk => risk.CreatedAt)
            .HasColumnName("created_at");

        modelBuilder.Entity<Risk>()
            .Property(risk => risk.UpdatedAt)
            .HasColumnName("updated_at");

        modelBuilder.Entity<Risk>()
            .HasOne(risk => risk.OwnerUser)
            .WithMany(user => user.OwnedRisks)
            .HasForeignKey(risk => risk.OwnerUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Risk>()
            .HasOne(risk => risk.Asset)
            .WithMany(asset => asset.Risks)
            .HasForeignKey(risk => risk.AssetId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<RefreshToken>()
            .ToTable("refresh_token")
            .Property(refreshToken => refreshToken.Id)
            .HasColumnName("id");

        modelBuilder.Entity<RefreshToken>()
            .Property(refreshToken => refreshToken.UserId)
            .HasColumnName("user_id");

        modelBuilder.Entity<RefreshToken>()
            .Property(refreshToken => refreshToken.TokenHash)
            .HasColumnName("token_hash");

        modelBuilder.Entity<RefreshToken>()
            .Property(refreshToken => refreshToken.ExpiresAt)
            .HasColumnName("expires_at");

        modelBuilder.Entity<RefreshToken>()
            .Property(refreshToken => refreshToken.CreatedAt)
            .HasColumnName("created_at");

        modelBuilder.Entity<RefreshToken>()
            .Property(refreshToken => refreshToken.RevokedAt)
            .HasColumnName("revoked_at");

        modelBuilder.Entity<RefreshToken>()
            .Property(refreshToken => refreshToken.ReplacedByTokenHash)
            .HasColumnName("replaced_by_token_hash");

        modelBuilder.Entity<RefreshToken>()
            .Property(refreshToken => refreshToken.CreatedByIp)
            .HasColumnName("created_by_ip");

        modelBuilder.Entity<RefreshToken>()
            .Property(refreshToken => refreshToken.RevokedByIp)
            .HasColumnName("revoked_by_ip");

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(refreshToken => refreshToken.TokenHash)
            .IsUnique();

        modelBuilder.Entity<RefreshToken>()
            .HasOne(refreshToken => refreshToken.User)
            .WithMany(user => user.RefreshTokens)
            .HasForeignKey(refreshToken => refreshToken.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserProfileChangeRequest>()
            .ToTable("user_profile_change_request")
            .Property(changeRequest => changeRequest.Id)
            .HasColumnName("id");

        modelBuilder.Entity<UserProfileChangeRequest>()
            .Property(changeRequest => changeRequest.UserId)
            .HasColumnName("user_id");

        modelBuilder.Entity<UserProfileChangeRequest>()
            .Property(changeRequest => changeRequest.OrganizationId)
            .HasColumnName("organization_id");

        modelBuilder.Entity<UserProfileChangeRequest>()
            .Property(changeRequest => changeRequest.RequestedFirstName)
            .HasColumnName("requested_first_name");

        modelBuilder.Entity<UserProfileChangeRequest>()
            .Property(changeRequest => changeRequest.RequestedLastName)
            .HasColumnName("requested_last_name");

        modelBuilder.Entity<UserProfileChangeRequest>()
            .Property(changeRequest => changeRequest.RequestedEmail)
            .HasColumnName("requested_email");

        modelBuilder.Entity<UserProfileChangeRequest>()
            .Property(changeRequest => changeRequest.Status)
            .HasColumnName("request_status");

        modelBuilder.Entity<UserProfileChangeRequest>()
            .Property(changeRequest => changeRequest.ReviewedByUserId)
            .HasColumnName("reviewed_by_user_id");

        modelBuilder.Entity<UserProfileChangeRequest>()
            .Property(changeRequest => changeRequest.CreatedAt)
            .HasColumnName("created_at");

        modelBuilder.Entity<UserProfileChangeRequest>()
            .Property(changeRequest => changeRequest.ReviewedAt)
            .HasColumnName("reviewed_at");

        modelBuilder.Entity<UserProfileChangeRequest>()
            .HasOne(changeRequest => changeRequest.User)
            .WithMany(user => user.ProfileChangeRequests)
            .HasForeignKey(changeRequest => changeRequest.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<UserProfileChangeRequest>()
            .HasOne(changeRequest => changeRequest.ReviewedByUser)
            .WithMany(user => user.ReviewedProfileChangeRequests)
            .HasForeignKey(changeRequest => changeRequest.ReviewedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Incident>()
            .ToTable("incident")
            .Property(i => i.Id)
            .HasColumnName("id");

        modelBuilder.Entity<Incident>()
            .Property(i => i.OrganizationId)
            .HasColumnName("organization_id");

        modelBuilder.Entity<Incident>()
            .Property(i => i.ReportedByUserId)
            .HasColumnName("reported_by_user_id");

        modelBuilder.Entity<Incident>()
            .Property(i => i.Title)
            .HasColumnName("title");

        modelBuilder.Entity<Incident>()
            .Property(i => i.Severity)
            .HasColumnName("severity");

        modelBuilder.Entity<Incident>()
            .Property(i => i.OccuredOn)
            .HasColumnName("occured_on");

        modelBuilder.Entity<Incident>()
            .Property(i => i.IncidentStatus)
            .HasColumnName("incident_status");

        modelBuilder.Entity<Incident>()
            .Property(i => i.CreatedAt)
            .HasColumnName("created_at");

        modelBuilder.Entity<Incident>()
            .Property(i => i.UpdatedAt)
            .HasColumnName("updated_at");

        modelBuilder.Entity<Incident>()
            .HasOne(i => i.Organization)
            .WithMany(o => o.Incidents)
            .HasForeignKey(i => i.OrganizationId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Incident>()
            .HasOne(i => i.ReportedByUser)
            .WithMany()
            .HasForeignKey(i => i.ReportedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ActionPlan>()
            .ToTable("action_plan")
            .Property(ap => ap.Id)
            .HasColumnName("id");

        modelBuilder.Entity<ActionPlan>()
            .Property(ap => ap.RiskId)
            .HasColumnName("risk_id");

        modelBuilder.Entity<ActionPlan>()
            .Property(ap => ap.IncidentId)
            .HasColumnName("incident_id");

        modelBuilder.Entity<ActionPlan>()
            .Property(ap => ap.OwnerUserId)
            .HasColumnName("owner_user_id");

        modelBuilder.Entity<ActionPlan>()
    .Property(ap => ap.OrganizationId)
    .HasColumnName("organization_id");

        modelBuilder.Entity<ActionPlan>()
            .Property(ap => ap.SuggestedAction)
            .HasColumnName("suggested_action");

        modelBuilder.Entity<ActionPlan>()
            .Property(ap => ap.PlannedCompletionDate)
            .HasColumnName("planned_completion_date");

        modelBuilder.Entity<ActionPlan>()
            .Property(ap => ap.ActionPlanStatus)
            .HasColumnName("action_plan_status");

        modelBuilder.Entity<ActionPlan>()
            .Property(ap => ap.FollowUp)
            .HasColumnName("follow_up");

        modelBuilder.Entity<ActionPlan>()
            .Property(ap => ap.Notes)
            .HasColumnName("notes");

        modelBuilder.Entity<ActionPlan>()
            .Property(ap => ap.CreatedAt)
            .HasColumnName("created_at");

        modelBuilder.Entity<ActionPlan>()
            .Property(ap => ap.UpdatedAt)
            .HasColumnName("updated_at");

        modelBuilder.Entity<ActionPlan>()
            .HasOne(ap => ap.Risk)
            .WithMany(risk => risk.ActionPlans)
            .HasForeignKey(ap => ap.RiskId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ActionPlan>()
            .HasOne(ap => ap.Incident)
            .WithMany(incident => incident.ActionPlans)
            .HasForeignKey(ap => ap.IncidentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ActionPlan>()
            .HasOne(ap => ap.OwnerUser)
            .WithMany(user => user.ActionPlans)
            .HasForeignKey(ap => ap.OwnerUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ActionPlan>()
     .HasOne(ap => ap.Organization)
     .WithMany(o => o.ActionPlans)
     .HasForeignKey(ap => ap.OrganizationId)
     .OnDelete(DeleteBehavior.Cascade)
     .IsRequired();

        modelBuilder.Entity<RiskAssessment>()
            .ToTable("risk_assessment")
            .Property(ra => ra.Id)
            .HasColumnName("id");

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.RiskId)
            .HasColumnName("risk_id")
            .IsRequired();

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.AssessedByUserId)
            .HasColumnName("assessed_by_user_id")
            .IsRequired();

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.OrganizationId)
            .HasColumnName("organization_id");

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.Notes)
            .HasColumnName("notes");

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.RiskPhase)
            .HasColumnName("risk_phase")
            .HasColumnType("varchar(100)");

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.Likelihood)
            .HasColumnName("likelihood_1_5");

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.Impact)
            .HasColumnName("impact_1_5");

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.RiskScore)
            .HasColumnName("risk_score");

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.EconomicalLoss)
            .HasColumnName("economical_loss")
            .HasColumnType("decimal(12,2)");

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.RiskMitigation)
            .HasColumnName("risk_mitigation")
            .HasColumnType("varchar(50)");

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.RiskTransfer)
            .HasColumnName("risk_transfer")
            .HasColumnType("varchar(50)");

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.RiskAvoidance)
            .HasColumnName("risk_avoidance")
            .HasColumnType("varchar(50)");

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.RiskAcceptance)
            .HasColumnName("risk_acceptance")
            .HasColumnType("varchar(50)");

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.CreatedAt)
            .HasColumnName("created_at");

        modelBuilder.Entity<RiskAssessment>()
            .Property(ra => ra.UpdatedAt)
            .HasColumnName("updated_at");

        modelBuilder.Entity<RiskAssessment>()
            .HasOne(ra => ra.Organization)
            .WithMany()
            .HasForeignKey(ra => ra.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<RiskAssessment>()
            .HasOne(ra => ra.Risk)
            .WithMany( risk => risk.RiskAssessments)
            .HasForeignKey(ra => ra.RiskId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<RiskAssessment>()
            .HasOne(ra => ra.AssessedByUser)
            .WithMany()
            .HasForeignKey(ra => ra.AssessedByUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}