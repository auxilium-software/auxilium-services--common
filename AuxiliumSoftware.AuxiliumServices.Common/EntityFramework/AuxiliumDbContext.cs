using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Converters;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;

public class AuxiliumDbContext : DbContext
{
    public AuxiliumDbContext(DbContextOptions<AuxiliumDbContext> options)
        : base(options)
    {
    }





    public DbSet<CaseEntityModel> Cases { get; set; }
    public DbSet<CaseAdditionalPropertyEntityModel> CaseAdditionalProperties { get; set; }
    public DbSet<CaseClientEntityModel> CaseClients { get; set; }
    public DbSet<CaseFileEntityModel> CaseFiles { get; set; }
    public DbSet<CaseMessageEntityModel> CaseMessages { get; set; }
    public DbSet<CaseTodoEntityModel> CaseTodos { get; set; }
    public DbSet<CaseWorkerEntityModel> CaseWorkers { get; set; }
    public DbSet<DataEnumeratorEntityModel> DataEnumerator_Enumerators { get; set; }
    public DbSet<DataEnumeratorTranslationEntityModel> DataEnumerator_EnumeratorTranslations { get; set; }
    public DbSet<DataEnumeratorValueEntityModel> DataEnumerator_EnumeratorValues { get; set; }
    public DbSet<DataEnumeratorValueTranslationEntityModel> DataEnumerator_EnumeratorValueTranslations { get; set; }
    public DbSet<LogCaseMessageReadByEventEntityModel> Log_CaseMessageReadBys { get; set; }
    public DbSet<LogCaseModificationEventEntityModel> Log_CaseModificationEvents { get; set; }
    public DbSet<LogLoginAttemptEventEntityModel> Log_LoginAttempts { get; set; }
    public DbSet<LogSystemBulletinEntryDismissalEventEntityModel> Log_SystemBulletinEntryDismissals { get; set; }
    public DbSet<LogSystemBulletinEntryViewEventEntityModel> Log_SystemBulletinEntryViews { get; set; }
    public DbSet<LogSystemMessageQueueSentEmailEntityModel> Log_SystemMessageQueueSentEmail { get; set; }
    public DbSet<LogSystemMessageQueueFailedActionEntityModel> Log_SystemMessageQueueFailedActions { get; set; }
    public DbSet<LogUserModificationEventEntityModel> Log_UserModificationEvents { get; set; }
    public DbSet<SystemBulletinEntryEntityModel> System_Bulletins { get; set; }
    public DbSet<SystemMetricEntityModel> System_Metrics { get; set; }
    public DbSet<SystemSettingEntityModel> System_Settings { get; set; }
    public DbSet<SystemWafIpBlacklistEntryEntityModel> System_Waf_IpBlacklist { get; set; }
    public DbSet<SystemWafIpWhitelistEntryEntityModel> System_Waf_IpWhitelist { get; set; }
    public DbSet<SystemWafUserBlacklistEntryEntityModel> System_Waf_UserBlacklist { get; set; }
    public DbSet<SystemWafUserWhitelistEntryEntityModel> System_Waf_UserWhitelist { get; set; }
    public DbSet<UserEntityModel> Users { get; set; }
    public DbSet<UserAdditionalPropertyEntityModel> UserAdditionalProperties { get; set; }
    public DbSet<UserFileEntityModel> UserFiles { get; set; }
    public DbSet<PasswordSetTokenEntityModel> UserPasswordSetTokens { get; set; }
    public DbSet<RefreshTokenEntityModel> UserRefreshTokens { get; set; }
    public DbSet<TotpRecoveryCodeEntityModel> UserTotpRecoveryCodes { get; set; }
    public DbSet<WemwbsAssessmentEntityModel> UserWemwbsAssessments { get; set; }




    
    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<DateTime>()
            .HaveConversion<UtcDateTimeConverter>();
    }




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);



        // case__cases
        modelBuilder.Entity<CaseEntityModel>(entity =>
        {
            entity.ToTable("case__cases");
            entity.HasKey(e => e.Id);


            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            entity.Property(e => e.LastUpdatedAtUtc)                .HasColumnName("last_updated_at_utc")                       .HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy)                   .HasColumnName("last_updated_by")                           .HasColumnType("char(36)");

            entity.Property(e => e.Title)                           .HasColumnName("title")                                     .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Description)                     .HasColumnName("description")                               .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Status)                          .HasColumnName("status")                                    .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<CaseStatusEnum>()).HasDefaultValue(CaseStatusEnum.Open).IsRequired();
            entity.Property(e => e.Sensitivity)                     .HasColumnName("sensitivity")                               .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<CaseSensitivityEnum>()).HasDefaultValue(CaseSensitivityEnum.Confidential).IsRequired();


            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.LastUpdatedByUser)                 .WithMany()                                                 .HasForeignKey(e => e.LastUpdatedBy)    .OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(e => e.Workers)                          .WithOne(w => w.Case)                                       .HasForeignKey(w => w.CaseId)           .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Clients)                          .WithOne(c => c.Case)                                       .HasForeignKey(c => c.CaseId)           .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.AdditionalProperties)             .WithOne(p => p.Case)                                       .HasForeignKey(p => p.CaseId)           .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Messages)                         .WithOne(m => m.Case)                                       .HasForeignKey(m => m.CaseId)           .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Files)                            .WithOne(f => f.Case)                                       .HasForeignKey(f => f.CaseId)           .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Todos)                            .WithOne(t => t.Case)                                       .HasForeignKey(t => t.CaseId)           .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.EventLog)                         .WithOne(t => t.Case)                                       .HasForeignKey(t => t.CaseId)           .OnDelete(DeleteBehavior.Cascade);
        });

        // case__additional_properties
        modelBuilder.Entity<CaseAdditionalPropertyEntityModel>(entity =>
        {
            entity.ToTable("case__additional_properties");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            entity.Property(e => e.LastUpdatedAtUtc)                .HasColumnName("last_updated_at_utc")                       .HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy)                   .HasColumnName("last_updated_by")                           .HasColumnType("char(36)");

            entity.Property(e => e.CaseId)                          .HasColumnName("case_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.OriginalName)                    .HasColumnName("original_name")                             .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.UrlSlug)                         .HasColumnName("url_slug")                                  .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Content)                         .HasColumnName("content")                                   .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.ContentType)                     .HasColumnName("content_type")                              .HasColumnType("text")                                                                                                              .IsRequired();
            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.LastUpdatedByUser)                 .WithMany()                                                 .HasForeignKey(e => e.LastUpdatedBy)    .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Case)                              .WithMany(c => c.AdditionalProperties)                      .HasForeignKey(e => e.CaseId);

            // entity.HasIndex(e => new { e.CaseId, e.UrlSlug }).IsUnique();
        });

        // case__clients
        modelBuilder.Entity<CaseClientEntityModel>(entity =>
        {
            entity.ToTable("case__clients");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");

            entity.Property(e => e.CaseId)                          .HasColumnName("case_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.UserId)                          .HasColumnName("user_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();
            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Case)                              .WithMany(c => c.Clients)                                   .HasForeignKey(e => e.CaseId)           .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User)                              .WithMany(u => u.ClientOnCases)                             .HasForeignKey(e => e.UserId)           .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.CaseId, e.UserId })        .IsUnique();
        });

        // case__files
        modelBuilder.Entity<CaseFileEntityModel>(entity =>
        {
            entity.ToTable("case__files");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            entity.Property(e => e.LastUpdatedAtUtc)                .HasColumnName("last_updated_at_utc")                       .HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy)                   .HasColumnName("last_updated_by")                           .HasColumnType("char(36)");

            entity.Property(e => e.CaseId)                          .HasColumnName("case_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.Filename)                        .HasColumnName("filename")                                  .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.ContentType)                     .HasColumnName("content_type")                              .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Size)                            .HasColumnName("size")                                      .HasColumnType("bigint")                                                                                                            .IsRequired();
            entity.Property(e => e.Hash)                            .HasColumnName("hash")                                      .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.LfsPath)                         .HasColumnName("lfs_path")                                  .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Description)                     .HasColumnName("description")                               .HasColumnType("text")                                                                                                              .IsRequired();
            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.LastUpdatedByUser)                 .WithMany()                                                 .HasForeignKey(e => e.LastUpdatedBy)    .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Case)                              .WithMany(c => c.Files)                                     .HasForeignKey(e => e.CaseId)           .OnDelete(DeleteBehavior.Cascade);
        });

        // case__messages
        modelBuilder.Entity<CaseMessageEntityModel>(entity =>
        {
            entity.ToTable("case__messages");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            entity.Property(e => e.LastUpdatedAtUtc)                .HasColumnName("last_updated_at_utc")                       .HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy)                   .HasColumnName("last_updated_by")                           .HasColumnType("char(36)");

            entity.Property(e => e.CaseId)                          .HasColumnName("case_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.SenderId)                        .HasColumnName("sender_id")                                 .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.Subject)                         .HasColumnName("subject")                                   .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Content)                         .HasColumnName("content")                                   .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.IsUrgent)                        .HasColumnName("is_urgent")                                 .HasColumnType("tinyint(1)")                                                    .HasDefaultValue(false)                             .IsRequired();
            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.LastUpdatedByUser)                 .WithMany()                                                 .HasForeignKey(e => e.LastUpdatedBy)    .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Case)                              .WithMany(c => c.Messages)                                  .HasForeignKey(e => e.CaseId);
            entity.HasOne(e => e.Sender)                            .WithMany(u => u.SentMessages)                              .HasForeignKey(e => e.SenderId)         .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.ReadBy)                           .WithOne(r => r.Message)                                    .HasForeignKey(r => r.MessageId)        .OnDelete(DeleteBehavior.Cascade);
        });

        // case__todos
        modelBuilder.Entity<CaseTodoEntityModel>(entity =>
        {
            entity.ToTable("case__todos");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            entity.Property(e => e.LastUpdatedAtUtc)                .HasColumnName("last_updated_at_utc")                       .HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy)                   .HasColumnName("last_updated_by")                           .HasColumnType("char(36)");

            entity.Property(e => e.CaseId)                          .HasColumnName("case_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.Summary)                         .HasColumnName("summary")                                   .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Description)                     .HasColumnName("description")                               .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Status)                          .HasColumnName("status")                                    .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<TodoStatusEnum>())                         .IsRequired();
            entity.Property(e => e.Priority)                        .HasColumnName("priority")                                  .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<TodoPriorityEnum>())                       .IsRequired();
            entity.Property(e => e.DueDate)                         .HasColumnName("due_date_utc")                              .HasColumnType("datetime");
            entity.Property(e => e.AssignedTo)                      .HasColumnName("assigned_to")                               .HasColumnType("char(36)");
            entity.Property(e => e.ReminderUtc)                     .HasColumnName("reminder_utc")                              .HasColumnType("datetime");
            entity.Property(e => e.CompletedAtUtc)                  .HasColumnName("completed_at_utc")                          .HasColumnType("datetime");
            entity.Property(e => e.CompletedBy)                     .HasColumnName("completed_by")                              .HasColumnType("char(36)");
            entity.Property(e => e.CompletionNote)                  .HasColumnName("completion_note")                           .HasColumnType("text");
            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.LastUpdatedByUser)                 .WithMany()                                                 .HasForeignKey(e => e.LastUpdatedBy)    .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Case)                              .WithMany(c => c.Todos)                                     .HasForeignKey(e => e.CaseId)           .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.AssignedToUser)                    .WithMany(u => u.AssignedTodos)                             .HasForeignKey(e => e.AssignedTo)       .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.CompletedByUser)                   .WithMany(u => u.CompletedTodos)                            .HasForeignKey(e => e.CompletedBy)      .OnDelete(DeleteBehavior.SetNull);
        });

        // case__workers
        modelBuilder.Entity<CaseWorkerEntityModel>(entity =>
        {
            entity.ToTable("case__workers");
            entity.HasKey(e => e.Id);


            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");

            entity.Property(e => e.CaseId)                          .HasColumnName("case_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.UserId)                          .HasColumnName("user_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();


            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Case)                              .WithMany(c => c.Workers)                                   .HasForeignKey(e => e.CaseId)           .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User)                              .WithMany(u => u.WorkerOnCases)                             .HasForeignKey(e => e.UserId)           .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.CaseId, e.UserId })        .IsUnique();
        });

        // enumerator__enumerators
        modelBuilder.Entity<DataEnumeratorEntityModel>(entity =>
        {
            entity.ToTable("enumerator__enumerators");
            entity.HasKey(e => e.Id);



            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            entity.Property(e => e.LastUpdatedAtUtc)                .HasColumnName("last_updated_at_utc")                       .HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy)                   .HasColumnName("last_updated_by")                           .HasColumnType("char(36)");

            entity.Property(e => e.CanonicalName)                   .HasColumnName("canonical_name")                            .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Description)                     .HasColumnName("description")                               .HasColumnType("text");
            entity.Property(e => e.IsActive)                        .HasColumnName("is_active")                                 .HasColumnType("tinyint(1)")                                                                                                        .IsRequired();

            entity.HasOne(e => e.CreatedByUser)                     .WithMany().HasForeignKey(e => e.CreatedBy)                                                         .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.LastUpdatedByUser)                 .WithMany().HasForeignKey(e => e.LastUpdatedBy)                                                     .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.CanonicalName).IsUnique();
        });

        // enumerator__enumerator_value
        modelBuilder.Entity<DataEnumeratorTranslationEntityModel>(entity =>
        {
            entity.ToTable("enumerator__enumerator_translations");
            entity.HasKey(e => e.Id);



            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            entity.Property(e => e.LastUpdatedAtUtc)                .HasColumnName("last_updated_at_utc")                       .HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy)                   .HasColumnName("last_updated_by")                           .HasColumnType("char(36)");

            entity.Property(e => e.DataEnumeratorId)                .HasColumnName("data_enumerator_id")                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.LanguageCode)                    .HasColumnName("language_code")                             .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Translation)                     .HasColumnName("translation")                               .HasColumnType("text")                                                                                                              .IsRequired();


            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.LastUpdatedByUser)                 .WithMany()                                                 .HasForeignKey(e => e.LastUpdatedBy)    .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Enum)                              .WithMany(v => v.Translations)                              .HasForeignKey(e => e.DataEnumeratorId).OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.DataEnumeratorId, e.LanguageCode }).IsUnique();
        });

        // enumerator__enumerator_values
        modelBuilder.Entity<DataEnumeratorValueEntityModel>(entity =>
        {
            entity.ToTable("enumerator__enumerator_values");
            entity.HasKey(e => e.Id);



            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            entity.Property(e => e.LastUpdatedAtUtc)                .HasColumnName("last_updated_at_utc")                       .HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy)                   .HasColumnName("last_updated_by")                           .HasColumnType("char(36)");

            entity.Property(e => e.EnumTypeId)                      .HasColumnName("enum_type_id")                              .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CanonicalName )                  .HasColumnName("canonical_name")                            .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.IsActive)                        .HasColumnName("is_active")                                 .HasColumnType("tinyint(1)")                                                                                                        .IsRequired();
            entity.Property(e => e.SortOrder)                       .HasColumnName("sort_order")                                .HasColumnType("int")                                                                                                               .IsRequired();


            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.LastUpdatedByUser)                 .WithMany()                                                 .HasForeignKey(e => e.LastUpdatedBy)    .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.EnumType)                          .WithMany(t => t.EnumeratorValues)                          .HasForeignKey(e => e.EnumTypeId)       .OnDelete(DeleteBehavior.Cascade);

            // entity.HasIndex(e => new { e.EnumTypeId, e.ValueHash }).IsUnique();
        });

        // enumerator__enumerator_value_translations
        modelBuilder.Entity<DataEnumeratorValueTranslationEntityModel>(entity =>
        {
            entity.ToTable("enumerator__enumerator_value_translations");
            entity.HasKey(e => e.Id);



            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            entity.Property(e => e.LastUpdatedAtUtc)                .HasColumnName("last_updated_at_utc")                       .HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy)                   .HasColumnName("last_updated_by")                           .HasColumnType("char(36)");

            entity.Property(e => e.DataEnumeratorValueId)           .HasColumnName("data_enumerator_value_id")                  .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.LanguageCode)                    .HasColumnName("language_code")                             .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Translation)                     .HasColumnName("translation")                               .HasColumnType("text")                                                                                                              .IsRequired();


            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.LastUpdatedByUser)                 .WithMany()                                                 .HasForeignKey(e => e.LastUpdatedBy)    .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.EnumValue)                         .WithMany(v => v.Translations)                              .HasForeignKey(e => e.DataEnumeratorValueId).OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.DataEnumeratorValueId, e.LanguageCode }).IsUnique();
        });

        // log__case_messages_read_bys
        modelBuilder.Entity<LogCaseMessageReadByEventEntityModel>(entity =>
        {
            entity.ToTable("log__case_messages_read_bys");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)")                                                                                                          .IsRequired();

            entity.Property(e => e.MessageId)                       .HasColumnName("message_id")                                .HasColumnType("char(36)")                                                                                                          .IsRequired();
            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany(u => u.MessageReadReceipts)                       .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Message)                           .WithMany(m => m.ReadBy)                                    .HasForeignKey(e => e.MessageId)        .OnDelete(DeleteBehavior.Cascade);
        });

        // log__case_modification_events
        modelBuilder.Entity<LogCaseModificationEventEntityModel>(entity =>
        {
            entity.ToTable("log__case_modification_events");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)")                                                                                                          .IsRequired();
            
            entity.Property(e => e.CaseId)                          .HasColumnName("case_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();
            
            entity.Property(e => e.EntityType)                      .HasColumnName("entity_type")                               .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<CaseEntityTypeEnum>())                     .IsRequired();
            entity.Property(e => e.EntityId)                        .HasColumnName("entity_id")                                 .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.Action)                          .HasColumnName("action")                                    .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<AuditLogActionTypeEnum>())                 .IsRequired();
            entity.Property(e => e.PropertyName)                    .HasColumnName("property_name")                             .HasColumnType("text");
            entity.Property(e => e.PreviousValue)                   .HasColumnName("previous_value")                            .HasColumnType("text");
            entity.Property(e => e.NewValue)                        .HasColumnName("new_value")                                 .HasColumnType("text");
            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Case)                              .WithMany(c => c.EventLog)                                  .HasForeignKey(e => e.CaseId)           .OnDelete(DeleteBehavior.Cascade);
        });

        // log__login_attempts
        modelBuilder.Entity<LogLoginAttemptEventEntityModel>(entity =>
        {
            entity.ToTable("log__login_attempts");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();

            entity.Property(e => e.AttemptedEmailAddress)           .HasColumnName("attempted_email_address")                   .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.TargetUserId)                    .HasColumnName("target_user_id")                            .HasColumnType("char(36)");
            entity.Property(e => e.ClientIpAddress)                 .HasColumnName("client_ip_address")                         .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.WasLoginSuccessful)              .HasColumnName("was_login_successful")                      .HasColumnType("tinyint(1)")                                                                                                        .IsRequired();
            entity.Property(e => e.WasBlockedByWaf)                 .HasColumnName("was_blocked_by_waf")                        .HasColumnType("tinyint(1)")                                                                                                        .IsRequired();
            entity.Property(e => e.FailureReason)                   .HasColumnName("failure_reason")                            .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<LoginAttemptFailureReasonEnum>());
            
            entity.HasOne(e => e.TargetUser)                        .WithMany()                                                 .HasForeignKey(e => e.TargetUserId)     .OnDelete(DeleteBehavior.SetNull);
        });

        // log__system_bulletin_dismissals
        modelBuilder.Entity<LogSystemBulletinEntryDismissalEventEntityModel>(entity =>
        {
            entity.ToTable("log__system_bulletin_dismissals");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)")                                                                                                          .IsRequired();

            entity.Property(e => e.SystemBulletinId)                .HasColumnName("system_bulletin_id")                        .HasColumnType("char(36)")                                                                                                          .IsRequired();

            entity.HasOne(e => e.CreatedByUser)                     .WithMany(u => u.BulletinDismissals)                        .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.SystemBulletin)                    .WithMany(b => b.Dismissals)                                .HasForeignKey(e => e.SystemBulletinId) .OnDelete(DeleteBehavior.Cascade);
        });

        // log__system_bulletin_views
        modelBuilder.Entity<LogSystemBulletinEntryViewEventEntityModel>(entity =>
        {
            entity.ToTable("log__system_bulletin_views");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)")                                                                                                          .IsRequired();

            entity.Property(e => e.SystemBulletinId)                .HasColumnName("system_bulletin_id")                        .HasColumnType("char(36)")                                                                                                          .IsRequired();

            entity.HasOne(e => e.CreatedByUser)                     .WithMany(u => u.BulletinViews)                             .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.SystemBulletin)                    .WithMany(b => b.Views)                                     .HasForeignKey(e => e.SystemBulletinId) .OnDelete(DeleteBehavior.Cascade);
        });

        // log__system_message_queue_sent_emails
        modelBuilder.Entity<LogSystemMessageQueueSentEmailEntityModel>(entity =>
        {
            entity.ToTable("log__system_message_queue_sent_emails");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            
            entity.Property(e => e.MessageId)                       .HasColumnName("message_id")                                .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.MessageCreatedAt)                .HasColumnName("message_created_at_utc")                    .HasColumnType("datetime")                                                                                                          .IsRequired();
            entity.Property(e => e.MessageCorrelationId)            .HasColumnName("message_correlation_id")                    .HasColumnType("text");
            entity.Property(e => e.MessageRoutingKey)               .HasColumnName("message_routing_key")                       .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.MessageJson)                     .HasColumnName("message_json")                              .HasColumnType("longtext")                                                                                                          .IsRequired();

            entity.Property(e => e.EmailRecipientAddress)           .HasColumnName("email_recipient_address")                   .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.EmailRecipientName)              .HasColumnName("email_recipient_name")                      .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.EmailLanguage)                   .HasColumnName("email_language")                            .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.EmailTemplate)                   .HasColumnName("email_template")                            .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.EmailSubject)                    .HasColumnName("email_subject")                             .HasColumnType("longtext")                                                                                                          .IsRequired();
            entity.Property(e => e.EmailBodyHtml)                   .HasColumnName("email_body_html")                           .HasColumnType("longtext")                                                                                                          .IsRequired();
            entity.Property(e => e.EmailBodyTxt)                    .HasColumnName("email_body_txt")                            .HasColumnType("longtext")                                                                                                          .IsRequired();
        });

        // log__system_message_queue_failed_actions
        modelBuilder.Entity<LogSystemMessageQueueFailedActionEntityModel>(entity =>
        {
            entity.ToTable("log__system_message_queue_failed_actions");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            
            entity.Property(e => e.MessageId)                       .HasColumnName("message_id")                                .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.MessageCreatedAtUtc)             .HasColumnName("message_created_at_utc")                    .HasColumnType("datetime")                                                                                                          .IsRequired();
            entity.Property(e => e.MessageCorrelationId)            .HasColumnName("message_correlation_id")                    .HasColumnType("text");
            entity.Property(e => e.MessageRoutingKey)               .HasColumnName("message_routing_key")                       .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.MessageJson)                     .HasColumnName("message_json")                              .HasColumnType("longtext")                                                                                                          .IsRequired();

            entity.Property(e => e.ExceptionType)                   .HasColumnName("exception_type")                            .HasColumnType("longtext")                                                                                                          .IsRequired();
            entity.Property(e => e.ExceptionMessage)                .HasColumnName("exception_message")                         .HasColumnType("longtext")                                                                                                          .IsRequired();
            entity.Property(e => e.ExceptionStackTrace)             .HasColumnName("exception_stack_trace")                     .HasColumnType("longtext")                                                                                                          .IsRequired();
        });

        // log__user_modification_events
        modelBuilder.Entity<LogUserModificationEventEntityModel>(entity =>
        {
            entity.ToTable("log__user_modification_events");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)")                                                                                                          .IsRequired();
            
            entity.Property(e => e.UserId)                          .HasColumnName("user_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();
            
            entity.Property(e => e.EntityType)                      .HasColumnName("entity_type")                               .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<UserEntityTypeEnum>())                     .IsRequired();
            entity.Property(e => e.EntityId)                        .HasColumnName("entity_id")                                 .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.Action)                          .HasColumnName("action")                                    .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<AuditLogActionTypeEnum>())                 .IsRequired();
            entity.Property(e => e.PropertyName)                    .HasColumnName("property_name")                             .HasColumnType("text");
            entity.Property(e => e.PreviousValue)                   .HasColumnName("previous_value")                            .HasColumnType("text");
            entity.Property(e => e.NewValue)                        .HasColumnName("new_value")                                 .HasColumnType("text");
            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User)                              .WithMany(u => u.EventLog)                                  .HasForeignKey(e => e.UserId)           .OnDelete(DeleteBehavior.Cascade);
        });

        // system_bulletin
        modelBuilder.Entity<SystemBulletinEntryEntityModel>(entity =>
        {
            entity.ToTable("system__bulletin");
            entity.HasKey(e => e.Id);



            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            
            entity.Property(e => e.Severity)                        .HasColumnName("severity")                                  .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<SystemBulletinMessageSeverityEnum>())      .IsRequired();
            entity.Property(e => e.Title)                           .HasColumnName("title")                                     .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Content)                         .HasColumnName("content")                                   .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.IsActive)                        .HasColumnName("is_active")                                 .HasColumnType("tinyint(1)")                                                    .HasDefaultValue(false)                             .IsRequired();
            entity.Property(e => e.IsDismissible)                   .HasColumnName("is_dismissible")                            .HasColumnType("tinyint(1)")                                                                                                        .IsRequired();
            entity.Property(e => e.StartsAtUtc)                     .HasColumnName("starts_at_utc")                             .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.EndsAtUtc)                       .HasColumnName("ends_at_utc")                               .HasColumnType("datetime");
            entity.Property(e => e.TargetAudience)                  .HasColumnName("target_audience")                           .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<SystemBulletinMessageTargetAudienceEnum>()).IsRequired();
            entity.Property(e => e.SpecificUserId)                  .HasColumnName("specific_user_id")                          .HasColumnType("char(36)");


            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.SpecificUser)                      .WithMany(u => u.TargetedBulletins)                         .HasForeignKey(e => e.SpecificUserId)   .OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(e => e.Dismissals)                       .WithOne(d => d.SystemBulletin)                             .HasForeignKey(d => d.SystemBulletinId) .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Views)                            .WithOne(v => v.SystemBulletin)                             .HasForeignKey(v => v.SystemBulletinId) .OnDelete(DeleteBehavior.Cascade);
        });

        // system__metrics
        modelBuilder.Entity<SystemMetricEntityModel>(entity =>
        {
            entity.ToTable("system__metrics");
            entity.HasKey(e => e.Id);



            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            
            entity.Property(e => e.MetricKey)                       .HasColumnName("metric_key")                                .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<SystemMetricKeyEnum>())                    .IsRequired();
            entity.Property(e => e.MetricValue)                     .HasColumnName("metric_value")                              .HasColumnType("float")                                                                                                             .IsRequired();
        });

        // system__settings
        modelBuilder.Entity<SystemSettingEntityModel>(entity =>
        {
            entity.ToTable("system__settings");
            entity.HasKey(e => e.Id);



            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            
            entity.Property(e => e.ConfigKey)                       .HasColumnName("config_key")                                .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<SystemSettingKeyEnum>())                   .IsRequired();
            entity.Property(e => e.ValueType)                       .HasColumnName("value_type")                                .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<SystemSettingValueTypeEnum>())             .IsRequired();
            entity.Property(e => e.ConfigValue)                     .HasColumnName("config_value")                              .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.ReasonForModification)           .HasColumnName("reason_for_modification")                   .HasColumnType("text")                                                                                                              .IsRequired();


            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
        });

        // system__waf__ip_blacklist
        modelBuilder.Entity<SystemWafIpBlacklistEntryEntityModel>(entity =>
        {
            entity.ToTable("system__waf__ip_blacklist");
            entity.HasKey(e => e.Id);



            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            
            entity.Property(e => e.IpAddress)                       .HasColumnName("ip_address")                                .HasColumnType("text")                  .HasConversion(new IpAddressConverter())                                                    .IsRequired();
            entity.Property(e => e.JustificationForBlacklist)       .HasColumnName("justification_for_blacklist")               .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.IsPermanent)                     .HasColumnName("is_permanent")                              .HasColumnType("tinyint(1)")                                                                                                        .IsRequired();
            
            entity.Property(e => e.ExpiresAtUtc)                    .HasColumnName("expires_at_utc")                            .HasColumnType("datetime");
            entity.Property(e => e.UnblacklistedAtUtc)              .HasColumnName("unblacklisted_at_utc")                      .HasColumnType("datetime");
            entity.Property(e => e.UnblacklistedBy)                 .HasColumnName("unblacklisted_by")                          .HasColumnType("char(36)");
            entity.Property(e => e.JustificationForUnblacklist)     .HasColumnName("justification_for_unblacklist")             .HasColumnType("text");


            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.UnblockedByUser)                   .WithMany()                                                 .HasForeignKey(e => e.UnblacklistedBy)  .OnDelete(DeleteBehavior.SetNull);
        });

        // system__waf__ip_whitelist
        modelBuilder.Entity<SystemWafIpWhitelistEntryEntityModel>(entity =>
        {
            entity.ToTable("system__waf__ip_whitelist");
            entity.HasKey(e => e.Id);



            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            
            entity.Property(e => e.IpAddress)                       .HasColumnName("ip_address")                                .HasColumnType("text")                  .HasConversion(new IpAddressConverter())                                                    .IsRequired();
            entity.Property(e => e.JustificationForWhitelist)       .HasColumnName("justification_for_whitelist")               .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.IsPermanent)                     .HasColumnName("is_permanent")                              .HasColumnType("tinyint(1)")                                                                                                        .IsRequired();
            
            entity.Property(e => e.ExpiresAtUtc)                    .HasColumnName("expires_at_utc")                            .HasColumnType("datetime");
            entity.Property(e => e.UnwhitelistedAtUtc)              .HasColumnName("unwhitelisted_at_utc")                      .HasColumnType("datetime");
            entity.Property(e => e.UnwhitelistedBy)                 .HasColumnName("unwhitelisted_by")                          .HasColumnType("char(36)");
            entity.Property(e => e.JustificationForUnwhitelist)     .HasColumnName("justification_for_unwhitelist")             .HasColumnType("text");


            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
        });

        // system__waf__user_blacklist
        modelBuilder.Entity<SystemWafUserBlacklistEntryEntityModel>(entity =>
        {
            entity.ToTable("system__waf__user_blacklist");
            entity.HasKey(e => e.Id);



            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            
            entity.Property(e => e.UserId)                          .HasColumnName("user_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.JustificationForBlacklist)       .HasColumnName("justification_for_blacklist")               .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.IsPermanent)                     .HasColumnName("is_permanent")                              .HasColumnType("tinyint(1)")                                                                                                        .IsRequired();
            
            entity.Property(e => e.ExpiresAtUtc)                    .HasColumnName("expires_at_utc")                            .HasColumnType("datetime");
            entity.Property(e => e.UnblacklistedAtUtc)              .HasColumnName("unblacklisted_at_utc")                      .HasColumnType("datetime");
            entity.Property(e => e.UnblacklistedBy)                 .HasColumnName("unblacklisted_by")                          .HasColumnType("char(36)");
            entity.Property(e => e.JustificationForUnblacklist)     .HasColumnName("justification_for_unblacklist")             .HasColumnType("text");


            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.BlockedUser)                       .WithMany()                                                 .HasForeignKey(e => e.UserId)           .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.UnblockedByUser)                   .WithMany()                                                 .HasForeignKey(e => e.UnblacklistedBy)  .OnDelete(DeleteBehavior.SetNull);
        });

        // system__waf__user_whitelist
        modelBuilder.Entity<SystemWafUserWhitelistEntryEntityModel>(entity =>
        {
            entity.ToTable("system__waf__user_whitelist");
            entity.HasKey(e => e.Id);



            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            
            entity.Property(e => e.UserId)                          .HasColumnName("user_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.JustificationForWhitelist)       .HasColumnName("justification_for_whitelist")               .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.IsPermanent)                     .HasColumnName("is_permanent")                              .HasColumnType("tinyint(1)")                                                                                                        .IsRequired();
            
            entity.Property(e => e.ExpiresAtUtc)                    .HasColumnName("expires_at_utc")                            .HasColumnType("datetime");
            entity.Property(e => e.UnwhitelistedAtUtc)              .HasColumnName("unwhitelisted_at_utc")                      .HasColumnType("datetime");
            entity.Property(e => e.UnwhitelistedBy)                 .HasColumnName("unwhitelisted_by")                          .HasColumnType("char(36)");
            entity.Property(e => e.JustificationForUnwhitelist)     .HasColumnName("justification_for_unwhitelist")             .HasColumnType("text");


            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
        });

        // user__users
        modelBuilder.Entity<UserEntityModel>(entity =>
        {
            entity.ToTable("user__users");
            entity.HasKey(e => e.Id);



            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            entity.Property(e => e.LastUpdatedAtUtc)                .HasColumnName("last_updated_at_utc")                       .HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy)                   .HasColumnName("last_updated_by")                           .HasColumnType("char(36)");

            entity.Property(e => e.EmailAddress)                    .HasColumnName("email_address")                             .HasColumnType("text");
            entity.Property(e => e.PasswordHash)                    .HasColumnName("password_hash")                             .HasColumnType("text");
            entity.Property(e => e.FullName)                        .HasColumnName("full_name")                                 .HasColumnType("text")                                                                                                              .IsRequired();

            entity.Property(e => e.FullAddress)                     .HasColumnName("full_address")                              .HasColumnType("text");
            entity.Property(e => e.TelephoneNumber)                 .HasColumnName("telephone_number")                          .HasColumnType("text");
            entity.Property(e => e.Gender)                          .HasColumnName("gender")                                    .HasColumnType("text");
            entity.Property(e => e.DateOfBirth)                     .HasColumnName("date_of_birth")                             .HasColumnType("date");
            entity.Property(e => e.HowDidYouFindOutAboutOurService) .HasColumnName("how_did_you_find_out_about_our_service")    .HasColumnType("text");
            entity.Property(e => e.LanguagePreference)              .HasColumnName("language_preference")                       .HasColumnType("text")                                                          .HasDefaultValue("en-GB")                           .IsRequired();
            
            entity.Property(e => e.TotpSecret)                      .HasColumnName("totp_secret")                               .HasColumnType("text");
            entity.Property(e => e.TotpEnabled)                     .HasColumnName("totp_enabled")                              .HasColumnType("tinyint(1)")                                                    .HasDefaultValue(false)                             .IsRequired();
            entity.Property(e => e.TotpEnabledAtUtc)                .HasColumnName("totp_enabled_at_utc")                       .HasColumnType("datetime");
            
            entity.Property(e => e.HasEmailAddressBeenVerified)     .HasColumnName("has_email_address_been_verified")           .HasColumnType("tinyint(1)")                                                    .HasDefaultValue(false)                             .IsRequired();
            entity.Property(e => e.AllowLogin)                      .HasColumnName("allow_login")                               .HasColumnType("tinyint(1)")                                                    .HasDefaultValue(false)                             .IsRequired();
            entity.Property(e => e.MustChangePassword)              .HasColumnName("must_change_password")                      .HasColumnType("tinyint(1)")                                                    .HasDefaultValue(false)                             .IsRequired();

            entity.Property(e => e.IsAdministrator)                 .HasColumnName("is_administrator")                          .HasColumnType("tinyint(1)")                                                    .HasDefaultValue(false)                             .IsRequired();
            entity.Property(e => e.IsCaseWorker)                    .HasColumnName("is_case_worker")                            .HasColumnType("tinyint(1)")                                                    .HasDefaultValue(false)                             .IsRequired();
            entity.Property(e => e.IsCaseWorkerManager)             .HasColumnName("is_case_worker_manager")                    .HasColumnType("tinyint(1)")                                                    .HasDefaultValue(false)                             .IsRequired();
            
            entity.Property(e => e.DeletionRequested)               .HasColumnName("deletion_requested")                        .HasColumnType("tinyint(1)")                                                    .HasDefaultValue(false)                             .IsRequired();
            entity.Property(e => e.DeletionRequestReason)           .HasColumnName("deletion_request_reason")                   .HasColumnType("text");



            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.LastUpdatedByUser)                 .WithMany()                                                 .HasForeignKey(e => e.LastUpdatedBy)    .OnDelete(DeleteBehavior.SetNull);

            // case-related collections
            entity.HasMany(e => e.WorkerOnCases)                    .WithOne(w => w.User)                                       .HasForeignKey(w => w.UserId);
            entity.HasMany(e => e.ClientOnCases)                    .WithOne(c => c.User)                                       .HasForeignKey(c => c.UserId);

            // user's own data collections
            entity.HasMany(e => e.Files)                            .WithOne(f => f.User)                                       .HasForeignKey(f => f.UserId)           .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.AdditionalProperties)             .WithOne(p => p.User)                                       .HasForeignKey(p => p.UserId)           .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.RefreshTokens)                    .WithOne(r => r.CreatedByUser)                              .HasForeignKey(r => r.CreatedBy)        .OnDelete(DeleteBehavior.Cascade);

            // message-related collections
            entity.HasMany(e => e.SentMessages)                     .WithOne(m => m.Sender)                                     .HasForeignKey(m => m.SenderId)         .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.MessageReadReceipts)              .WithOne(r => r.CreatedByUser)                              .HasForeignKey(r => r.CreatedBy)        .OnDelete(DeleteBehavior.Cascade);

            // todo-related collections
            entity.HasMany(e => e.AssignedTodos)                    .WithOne(t => t.AssignedToUser)                             .HasForeignKey(t => t.AssignedTo)       .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.CompletedTodos)                   .WithOne(t => t.CompletedByUser)                            .HasForeignKey(t => t.CompletedBy)      .OnDelete(DeleteBehavior.Restrict);

            // assessment collections
            entity.HasMany(e => e.WEMWBSAssessments)                .WithOne(w => w.User)                                       .HasForeignKey(w => w.UserId)           .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.CreatedWEMWBSAssessments)         .WithOne(w => w.CreatedByUser)                              .HasForeignKey(w => w.CreatedBy)        .OnDelete(DeleteBehavior.Cascade);

            // system bulletin collections
            entity.HasMany(e => e.TargetedBulletins)                .WithOne(b => b.SpecificUser)                               .HasForeignKey(b => b.SpecificUserId)   .OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(e => e.BulletinDismissals)               .WithOne(d => d.CreatedByUser)                              .HasForeignKey(d => d.CreatedBy)        .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.BulletinViews)                    .WithOne(v => v.CreatedByUser)                              .HasForeignKey(v => v.CreatedBy)        .OnDelete(DeleteBehavior.Cascade);

            // misc collections
            entity.HasMany(e => e.EventLog)                         .WithOne(t => t.User)                                       .HasForeignKey(t => t.UserId)           .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.EmailAddress).IsUnique();
        });

        // user_additional_properties
        modelBuilder.Entity<UserAdditionalPropertyEntityModel>(entity =>
        {
            entity.ToTable("user__additional_properties");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            entity.Property(e => e.LastUpdatedAtUtc)                .HasColumnName("last_updated_at_utc")                       .HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy)                   .HasColumnName("last_updated_by")                           .HasColumnType("char(36)");

            entity.Property(e => e.UserId)                          .HasColumnName("user_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.OriginalName)                    .HasColumnName("original_name")                             .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.UrlSlug)                         .HasColumnName("url_slug")                                  .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Content)                         .HasColumnName("content")                                   .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.ContentType)                     .HasColumnName("content_type")                              .HasColumnType("text")                                                                                                              .IsRequired();
            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.LastUpdatedByUser)                 .WithMany()                                                 .HasForeignKey(e => e.LastUpdatedBy)    .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.User)                              .WithMany(u => u.AdditionalProperties)                      .HasForeignKey(e => e.UserId);

            // entity.HasIndex(e => new { e.UserId, e.UrlSlug }).IsUnique();
        });

        // user_files
        modelBuilder.Entity<UserFileEntityModel>(entity =>
        {
            entity.ToTable("user__files");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            entity.Property(e => e.LastUpdatedAtUtc)                .HasColumnName("last_updated_at_utc")                       .HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy)                   .HasColumnName("last_updated_by")                           .HasColumnType("char(36)");

            entity.Property(e => e.UserId)                          .HasColumnName("user_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.Filename)                        .HasColumnName("filename")                                  .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.ContentType)                     .HasColumnName("content_type")                              .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Size)                            .HasColumnName("size")                                      .HasColumnType("bigint")                                                                                                            .IsRequired();
            entity.Property(e => e.Hash)                            .HasColumnName("hash")                                      .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.LfsPath)                         .HasColumnName("lfs_path")                                  .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.Description)                     .HasColumnName("description")                               .HasColumnType("text")                                                                                                              .IsRequired();
            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany()                                                 .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.LastUpdatedByUser)                 .WithMany()                                                 .HasForeignKey(e => e.LastUpdatedBy)    .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.User)                              .WithMany(u => u.Files)                                     .HasForeignKey(e => e.UserId)           .OnDelete(DeleteBehavior.Cascade);
        });

        // user_password_set_tokens
        modelBuilder.Entity<PasswordSetTokenEntityModel>(entity =>
        {
            entity.ToTable("user__password_set_tokens");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");
            
            entity.Property(e => e.UserId)                          .HasColumnName("user_id")                                   .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.TokenHash)                       .HasColumnName("token_hash")                                .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.ExpiresAtUtc)                    .HasColumnName("expires_at_utc")                            .HasColumnType("datetime")                                                                                                          .IsRequired();
            entity.Property(e => e.UsedAtUtc)                       .HasColumnName("used_at_utc")                               .HasColumnType("datetime");
            entity.Property(e => e.Reason)                          .HasColumnName("reason")                                    .HasColumnType("text")                  .HasConversion(new JsonPropertyNameEnumConverter<PasswordSetTokenReasonEnum>())             .IsRequired();
            
            entity.HasOne(e => e.CreatedByUser)                     .WithMany(u => u.CreatedPasswordSetTokens)                  .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.User)                              .WithMany(u => u.PasswordSetTokens)                         .HasForeignKey(e => e.UserId)           .OnDelete(DeleteBehavior.Cascade);
        });

        // user__refresh_tokens
        modelBuilder.Entity<RefreshTokenEntityModel>(entity =>
        {
            entity.ToTable("user__refresh_tokens");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)");

            entity.Property(e => e.TokenHash)                       .HasColumnName("token_hash")                                .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.ExpiresAtUtc)                    .HasColumnName("expires_at_utc")                            .HasColumnType("datetime")                                                                                                          .IsRequired();

            entity.HasOne(e => e.CreatedByUser)                     .WithMany(u => u.RefreshTokens)                             .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.Cascade);
        });

        // user__totp_recovery_codes
        modelBuilder.Entity<TotpRecoveryCodeEntityModel>(entity =>
        {
            entity.ToTable("user__totp_recovery_codes");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)")                                                                                                          .IsRequired();
            
            entity.Property(e => e.CodeHash)                        .HasColumnName("code_hash")                                 .HasColumnType("text")                                                                                                              .IsRequired();
            entity.Property(e => e.IsUsed)                          .HasColumnName("is_used")                                   .HasColumnType("tinyint(1)")                                                    .HasDefaultValue(false)                             .IsRequired();
            entity.Property(e => e.UsedAtUtc)                       .HasColumnName("used_at_utc")                               .HasColumnType("datetime");

            entity.HasOne(e => e.CreatedByUser)                     .WithMany(u => u.TotpRecoveryCodes)                         .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.Cascade);
        });

        // user__wemwbs_assessments
        modelBuilder.Entity<WemwbsAssessmentEntityModel>(entity =>
        {
            entity.ToTable("user__wemwbs_assessments");
            entity.HasKey(e => e.Id);



            entity.Property(e => e.Id)                              .HasColumnName("id")                                        .HasColumnType("char(36)")                                                                                                          .IsRequired();
            entity.Property(e => e.CreatedAtUtc)                    .HasColumnName("created_at_utc")                            .HasColumnType("datetime")                                                      .HasDefaultValueSql("UTC_TIMESTAMP()")              .IsRequired();
            entity.Property(e => e.CreatedBy)                       .HasColumnName("created_by")                                .HasColumnType("char(36)")                                                                                                          .IsRequired();
            
            entity.Property(e => e.OptimismScore)                   .HasColumnName("optimism_score")                            .HasColumnType("int")                                                                                                               .IsRequired();
            entity.Property(e => e.UsefulnessScore)                 .HasColumnName("usefulness_score")                          .HasColumnType("int")                                                                                                               .IsRequired();
            entity.Property(e => e.RelaxedScore)                    .HasColumnName("relaxed_score")                             .HasColumnType("int")                                                                                                               .IsRequired();
            entity.Property(e => e.InterestedInPeopleScore)         .HasColumnName("interested_in_people_score")                .HasColumnType("int")                                                                                                               .IsRequired();
            entity.Property(e => e.SpareEnergyScore)                .HasColumnName("spare_energy_score")                        .HasColumnType("int")                                                                                                               .IsRequired();
            entity.Property(e => e.ProblemHandlingScore)            .HasColumnName("problem_handling_score")                    .HasColumnType("int")                                                                                                               .IsRequired();
            entity.Property(e => e.ClearThoughtScore)               .HasColumnName("clear_thought_score")                       .HasColumnType("int")                                                                                                               .IsRequired();
            entity.Property(e => e.FeelingGoodSelfScore)            .HasColumnName("feeling_good_self_score")                   .HasColumnType("int")                                                                                                               .IsRequired();
            entity.Property(e => e.FeelingCloseToPeopleScore)       .HasColumnName("feeling_close_to_people_score")             .HasColumnType("int")                                                                                                               .IsRequired();
            entity.Property(e => e.ConfidenceScore)                 .HasColumnName("confidence_score")                          .HasColumnType("int")                                                                                                               .IsRequired();
            entity.Property(e => e.MakingUpOwnMindScore)            .HasColumnName("making_up_own_mind_score")                  .HasColumnType("int")                                                                                                               .IsRequired();
            entity.Property(e => e.FeelingLovedScore)               .HasColumnName("feeling_loved_score")                       .HasColumnType("int")                                                                                                               .IsRequired();
            entity.Property(e => e.InterestedInNewThingsScore)      .HasColumnName("interested_in_new_things_score")            .HasColumnType("int")                                                                                                               .IsRequired();
            entity.Property(e => e.FeelingCheerfulScore)            .HasColumnName("feeling_cheerful_score")                    .HasColumnType("int")                                                                                                               .IsRequired();



            entity.HasOne(e => e.CreatedByUser)                     .WithMany(u => u.CreatedWEMWBSAssessments)                  .HasForeignKey(e => e.CreatedBy)        .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User)                              .WithMany(u => u.WEMWBSAssessments)                         .HasForeignKey(e => e.UserId)           .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
