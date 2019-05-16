using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AspNetCore.EFBasics;
using AspNetCore.EFBasics.Models;

namespace AspNetCore.EFBasics.Migrations
{
    [DbContext(typeof(BookDbContext))]
    partial class BookDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AspNetCore.EFBasics.Models.Author", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("Age");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("AspNetCore.EFBasics.Models.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Isbn")
                        .IsRequired();

                    b.Property<decimal>("Price");

                    b.Property<DateTime?>("ReleaseDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<string>("TopSecret");

                    b.HasKey("Id");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("AspNetCore.EFBasics.Models.BookAuthorRel", b =>
                {
                    b.Property<int>("BookId");

                    b.Property<int>("AuthorId");

                    b.HasKey("BookId", "AuthorId");

                    b.HasIndex("AuthorId");

                    b.ToTable("BookAuthorRel");
                });

            modelBuilder.Entity("AspNetCore.EFBasics.Models.ContactInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AuthorId");

                    b.Property<int>("Type");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("ContactInfo");
                });

            modelBuilder.Entity("AspNetCore.EFBasics.Models.BookAuthorRel", b =>
                {
                    b.HasOne("AspNetCore.EFBasics.Models.Author", "Author")
                        .WithMany("BookRelations")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AspNetCore.EFBasics.Models.Book", "Book")
                        .WithMany("AuthorRelations")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AspNetCore.EFBasics.Models.ContactInfo", b =>
                {
                    b.HasOne("AspNetCore.EFBasics.Models.Author", "Author")
                        .WithMany("ContactInfos")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
