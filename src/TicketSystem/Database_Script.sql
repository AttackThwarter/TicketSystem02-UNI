CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "Users" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
    "FullName" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "Role" TEXT NOT NULL
);

CREATE TABLE "Tickets" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tickets" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UserId" INTEGER NOT NULL,
    CONSTRAINT "FK_Tickets_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "TicketReplies" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_TicketReplies" PRIMARY KEY AUTOINCREMENT,
    "Message" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "IsAdminReply" INTEGER NOT NULL,
    "TicketId" INTEGER NOT NULL,
    CONSTRAINT "FK_TicketReplies_Tickets_TicketId" FOREIGN KEY ("TicketId") REFERENCES "Tickets" ("Id") ON DELETE CASCADE
);

INSERT INTO "Users" ("Id", "Email", "FullName", "PasswordHash", "Role")
VALUES (1, 'admin@ticket.com', 'System Admin', 'admin123', 'Admin');
SELECT changes();


CREATE INDEX "IX_TicketReplies_TicketId" ON "TicketReplies" ("TicketId");

CREATE INDEX "IX_Tickets_UserId" ON "Tickets" ("UserId");

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260206154256_InitialCreate', '10.0.2');

COMMIT;

