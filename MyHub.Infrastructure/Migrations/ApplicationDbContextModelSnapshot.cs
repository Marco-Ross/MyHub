﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHub.Infrastructure.Repository.EntityFramework;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MyHub.Domain.Emails.Email", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FromName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TemplateName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("To")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ToName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Emails");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("MyHub.Domain.Users.AccessingUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EmailVerificationDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsEmailVerified")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastResetPasswordDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegisterToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RegisterTokenExpireDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RegisterTokenSalt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResetPasswordToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ResetPasswordTokenExpireDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ResetPasswordTokenSalt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AccessingUsers");
                });

            modelBuilder.Entity("MyHub.Domain.Users.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MyHub.Domain.Emails.AccountRegisterEmail", b =>
                {
                    b.HasBaseType("MyHub.Domain.Emails.Email");

                    b.ToTable("AccountRegisterEmails");
                });

            modelBuilder.Entity("MyHub.Domain.Emails.PasswordRecoveryEmail", b =>
                {
                    b.HasBaseType("MyHub.Domain.Emails.Email");

                    b.ToTable("PasswordRecoveryEmails");
                });

            modelBuilder.Entity("MyHub.Domain.Emails.Email", b =>
                {
                    b.HasOne("MyHub.Domain.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MyHub.Domain.Users.AccessingUser", b =>
                {
                    b.HasOne("MyHub.Domain.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MyHub.Domain.Emails.AccountRegisterEmail", b =>
                {
                    b.HasOne("MyHub.Domain.Emails.Email", null)
                        .WithOne()
                        .HasForeignKey("MyHub.Domain.Emails.AccountRegisterEmail", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MyHub.Domain.Emails.PasswordRecoveryEmail", b =>
                {
                    b.HasOne("MyHub.Domain.Emails.Email", null)
                        .WithOne()
                        .HasForeignKey("MyHub.Domain.Emails.PasswordRecoveryEmail", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
