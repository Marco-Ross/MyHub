﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHub.Infrastructure.Repository.EntityFramework;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230703164650_AddGalleryImage")]
    partial class AddGalleryImage
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MyHub.Domain.Authentication.RefreshToken", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

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

            modelBuilder.Entity("MyHub.Domain.Gallery.UsersGallery", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateUploaded")
                        .HasColumnType("datetime2");

                    b.Property<int>("Likes")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UsersGallery");
                });

            modelBuilder.Entity("MyHub.Domain.Users.AccessingUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ChangeEmailToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ChangeEmailTokenExpireDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ChangeEmailTokenSalt")
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

                    b.Property<string>("TemporaryNewEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AccessingUsers");
                });

            modelBuilder.Entity("MyHub.Domain.Users.ThirdPartyDetails", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ThirdPartyIssuerId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ThirdPartyDetails");
                });

            modelBuilder.Entity("MyHub.Domain.Users.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Theme")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

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

            modelBuilder.Entity("MyHub.Domain.Authentication.RefreshToken", b =>
                {
                    b.HasOne("MyHub.Domain.Users.AccessingUser", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
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

            modelBuilder.Entity("MyHub.Domain.Gallery.UsersGallery", b =>
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

            modelBuilder.Entity("MyHub.Domain.Users.ThirdPartyDetails", b =>
                {
                    b.HasOne("MyHub.Domain.Users.AccessingUser", "AccessingUser")
                        .WithOne("ThirdPartyDetails")
                        .HasForeignKey("MyHub.Domain.Users.ThirdPartyDetails", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccessingUser");
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

            modelBuilder.Entity("MyHub.Domain.Users.AccessingUser", b =>
                {
                    b.Navigation("RefreshTokens");

                    b.Navigation("ThirdPartyDetails")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
