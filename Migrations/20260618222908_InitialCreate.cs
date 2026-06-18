using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileSyncBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blobs",
                columns: table => new
                {
                    Hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blobs", x => x.Hash);
                });

            migrationBuilder.CreateTable(
                name: "Commits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commits_Commits_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Commits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Conflicts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Path = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    BaseBlobHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    LocalBlobHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ServerBlobHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ResolvedBlobHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ResolvedBy = table.Column<string>(type: "text", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conflicts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommitFiles",
                columns: table => new
                {
                    CommitId = table.Column<Guid>(type: "uuid", nullable: false),
                    Path = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    BlobHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitFiles", x => new { x.CommitId, x.Path });
                    table.ForeignKey(
                        name: "FK_CommitFiles_Blobs_BlobHash",
                        column: x => x.BlobHash,
                        principalTable: "Blobs",
                        principalColumn: "Hash",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommitFiles_Commits_CommitId",
                        column: x => x.CommitId,
                        principalTable: "Commits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SyncSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SnapshotData = table.Column<string>(type: "jsonb", nullable: false),
                    ServerCommitId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyncSnapshots_Commits_ServerCommitId",
                        column: x => x.ServerCommitId,
                        principalTable: "Commits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommitFiles_BlobHash",
                table: "CommitFiles",
                column: "BlobHash");

            migrationBuilder.CreateIndex(
                name: "IX_Commits_Author",
                table: "Commits",
                column: "Author");

            migrationBuilder.CreateIndex(
                name: "IX_Commits_ParentId",
                table: "Commits",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Commits_Timestamp",
                table: "Commits",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Conflicts_Path_CreatedAt",
                table: "Conflicts",
                columns: new[] { "Path", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SyncSnapshots_ServerCommitId",
                table: "SyncSnapshots",
                column: "ServerCommitId");

            migrationBuilder.CreateIndex(
                name: "IX_SyncSnapshots_UserId",
                table: "SyncSnapshots",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommitFiles");

            migrationBuilder.DropTable(
                name: "Conflicts");

            migrationBuilder.DropTable(
                name: "SyncSnapshots");

            migrationBuilder.DropTable(
                name: "Blobs");

            migrationBuilder.DropTable(
                name: "Commits");
        }
    }
}
