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
    [Migration("20230823121141_AddedUserFeedback")]
    partial class AddedUserFeedback
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GalleryImageUser", b =>
                {
                    b.Property<string>("LikedGalleryImagesId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LikedGalleryUsersId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LikedGalleryImagesId", "LikedGalleryUsersId");

                    b.HasIndex("LikedGalleryUsersId");

                    b.ToTable("GalleryImageUser");
                });

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

            modelBuilder.Entity("MyHub.Domain.Feedback.UserFeedback", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserCreatedId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserCreatedId");

                    b.ToTable("Feedback");
                });

            modelBuilder.Entity("MyHub.Domain.Gallery.GalleryImage", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Caption")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateUploaded")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserCreatedId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("UserCreatedId");

                    b.ToTable("GalleryImages");
                });

            modelBuilder.Entity("MyHub.Domain.Gallery.GalleryImageComment", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CommentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ImageId");

                    b.HasIndex("UserId");

                    b.ToTable("GalleryImageComment");
                });

            modelBuilder.Entity("MyHub.Domain.Titbits.Titbit", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CategoryId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUploaded")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserCreatedId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserUpdatedId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserCreatedId");

                    b.HasIndex("UserUpdatedId");

                    b.ToTable("Titbits");
                });

            modelBuilder.Entity("MyHub.Domain.Titbits.TitbitCategory", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateUploaded")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TitbitCategories");
                });

            modelBuilder.Entity("MyHub.Domain.Titbits.TitbitLink", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TitbitId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TitbitId");

                    b.ToTable("TitbitLinks");
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

            modelBuilder.Entity("TitbitUser", b =>
                {
                    b.Property<string>("LikedTitbitUsersId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LikedTitbitsId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LikedTitbitUsersId", "LikedTitbitsId");

                    b.HasIndex("LikedTitbitsId");

                    b.ToTable("TitbitUser");
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

            modelBuilder.Entity("GalleryImageUser", b =>
                {
                    b.HasOne("MyHub.Domain.Gallery.GalleryImage", null)
                        .WithMany()
                        .HasForeignKey("LikedGalleryImagesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyHub.Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("LikedGalleryUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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

            modelBuilder.Entity("MyHub.Domain.Feedback.UserFeedback", b =>
                {
                    b.HasOne("MyHub.Domain.Users.User", "UserCreated")
                        .WithMany()
                        .HasForeignKey("UserCreatedId");

                    b.Navigation("UserCreated");
                });

            modelBuilder.Entity("MyHub.Domain.Gallery.GalleryImage", b =>
                {
                    b.HasOne("MyHub.Domain.Users.User", "UserCreated")
                        .WithMany("GalleryImages")
                        .HasForeignKey("UserCreatedId");

                    b.Navigation("UserCreated");
                });

            modelBuilder.Entity("MyHub.Domain.Gallery.GalleryImageComment", b =>
                {
                    b.HasOne("MyHub.Domain.Gallery.GalleryImage", "GalleryImage")
                        .WithMany("GalleryImageComments")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyHub.Domain.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GalleryImage");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MyHub.Domain.Titbits.Titbit", b =>
                {
                    b.HasOne("MyHub.Domain.Titbits.TitbitCategory", "TitbitCategory")
                        .WithMany("Titbits")
                        .HasForeignKey("CategoryId");

                    b.HasOne("MyHub.Domain.Users.User", "UserCreated")
                        .WithMany("Titbits")
                        .HasForeignKey("UserCreatedId");

                    b.HasOne("MyHub.Domain.Users.User", "UserUpdated")
                        .WithMany("TitbitsUpdated")
                        .HasForeignKey("UserUpdatedId");

                    b.Navigation("TitbitCategory");

                    b.Navigation("UserCreated");

                    b.Navigation("UserUpdated");
                });

            modelBuilder.Entity("MyHub.Domain.Titbits.TitbitLink", b =>
                {
                    b.HasOne("MyHub.Domain.Titbits.Titbit", "Titbit")
                        .WithMany("TitbitLinks")
                        .HasForeignKey("TitbitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Titbit");
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

            modelBuilder.Entity("TitbitUser", b =>
                {
                    b.HasOne("MyHub.Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("LikedTitbitUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyHub.Domain.Titbits.Titbit", null)
                        .WithMany()
                        .HasForeignKey("LikedTitbitsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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

            modelBuilder.Entity("MyHub.Domain.Gallery.GalleryImage", b =>
                {
                    b.Navigation("GalleryImageComments");
                });

            modelBuilder.Entity("MyHub.Domain.Titbits.Titbit", b =>
                {
                    b.Navigation("TitbitLinks");
                });

            modelBuilder.Entity("MyHub.Domain.Titbits.TitbitCategory", b =>
                {
                    b.Navigation("Titbits");
                });

            modelBuilder.Entity("MyHub.Domain.Users.AccessingUser", b =>
                {
                    b.Navigation("RefreshTokens");

                    b.Navigation("ThirdPartyDetails")
                        .IsRequired();
                });

            modelBuilder.Entity("MyHub.Domain.Users.User", b =>
                {
                    b.Navigation("GalleryImages");

                    b.Navigation("Titbits");

                    b.Navigation("TitbitsUpdated");
                });
#pragma warning restore 612, 618
        }
    }
}
