using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace At.luki0606.ClassPulse.Data
{
    /// <summary>
    /// Brings an existing SQLite database in line with the current EF model.
    /// <see cref="Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade.EnsureCreated"/> only creates a database that does not exist yet,
    /// so columns added to entities later would be missing in existing databases.
    /// This adds every missing column (additive changes only - no renames or removals).
    /// </summary>
    public static class DatabaseSchemaUpdater
    {
        /// <summary>
        /// Creates the database if it does not exist and then adds any missing columns.
        /// </summary>
        /// <param name="dbContext">The context of the database to update.</param>
        public static void EnsureCreatedAndUpToDate(AppDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();
            AddMissingColumns(dbContext);
        }

        /// <summary>
        /// Adds every column of the EF model that is missing from an existing table. Missing tables are skipped.
        /// </summary>
        /// <param name="dbContext">The context of the database to update.</param>
        public static void AddMissingColumns(AppDbContext dbContext)
        {
            DbConnection connection = dbContext.Database.GetDbConnection();
            bool openedHere = connection.State != ConnectionState.Open;
            if (openedHere)
            {
                connection.Open();
            }

            try
            {
                foreach (IEntityType entityType in dbContext.Model.GetEntityTypes())
                {
                    string? tableName = entityType.GetTableName();
                    if (tableName is null)
                    {
                        continue;
                    }

                    HashSet<string> existingColumns = GetExistingColumns(connection, tableName);
                    if (existingColumns.Count == 0)
                    {
                        // Table does not exist at all - not handled here.
                        continue;
                    }

                    StoreObjectIdentifier table = StoreObjectIdentifier.Table(tableName, entityType.GetSchema());

                    foreach (IProperty property in entityType.GetProperties())
                    {
                        string? columnName = property.GetColumnName(table);
                        if (columnName is null || existingColumns.Contains(columnName))
                        {
                            continue;
                        }

                        string sql = $"ALTER TABLE \"{tableName}\" ADD COLUMN {BuildColumnDefinition(property, columnName)};";
                        dbContext.Database.ExecuteSqlRaw(sql);
                        existingColumns.Add(columnName);
                    }
                }
            }
            finally
            {
                if (openedHere)
                {
                    connection.Close();
                }
            }
        }

        private static HashSet<string> GetExistingColumns(DbConnection connection, string tableName)
        {
            HashSet<string> columns = new(StringComparer.OrdinalIgnoreCase);

            using DbCommand command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info(\"{tableName.Replace("\"", "\"\"")}\");";
            using DbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                columns.Add(reader.GetString(reader.GetOrdinal("name")));
            }

            return columns;
        }

        private static string BuildColumnDefinition(IProperty property, string columnName)
        {
            RelationalTypeMapping typeMapping = property.GetRelationalTypeMapping();
            string columnType = property.GetColumnType() ?? typeMapping.StoreType;
            string definition = $"\"{columnName}\" {columnType}";

            string? defaultSql = property.GetDefaultValueSql();
            object? defaultValue = property.GetDefaultValue();

            if (defaultSql is not null)
            {
                definition += $" DEFAULT ({defaultSql})";
            }
            else if (defaultValue is not null)
            {
                definition += $" DEFAULT {typeMapping.GenerateSqlLiteral(defaultValue)}";
            }
            else if (!property.IsNullable)
            {
                // SQLite requires a default for NOT NULL columns added to existing tables.
                object fallback = GetClrDefault(property.ClrType);
                definition += $" DEFAULT {typeMapping.GenerateSqlLiteral(fallback)}";
            }

            if (!property.IsNullable)
            {
                definition += " NOT NULL";
            }

            return definition;
        }

        private static object GetClrDefault(Type clrType)
        {
            Type type = Nullable.GetUnderlyingType(clrType) ?? clrType;
            if (type == typeof(string))
            {
                return string.Empty;
            }

            return Activator.CreateInstance(type)
                ?? throw new InvalidOperationException($"Cannot determine default value for {clrType}.");
        }
    }
}
