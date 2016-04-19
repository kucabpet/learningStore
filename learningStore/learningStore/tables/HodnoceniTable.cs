﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using learningStore.database.mssql;
using learningStore.database.proxy;

namespace learningStore.tables
{
    public class HodnoceniTable
    {
        #region SQL
        private static String SQL_SELECT = "SELECT * FROM hodnoceni;";
        private static String SQL_SELECT_BYSUB = "SELECT * FROM hodnoceni WHERE pID=@pId;";
        private static String SQL_INSERT =
            "INSERT INTO hodnoceni (hodnoceni, popis, datum, pID, uzID) VALUES (@hodnoceni, @popis, @datum, @pID, @uzID);";
        private static String SQL_UPDATE =
            "UPDATE hodnoceni SET hodnoceni=@hodnoceni, popis=@popis, datum=@datum WHERE pID=@pID AND uzID=@uzID;";
        private static string SQL_DELETE =
            "DELETE FROM hodnoceni WHERE pID=@pID AND uzID=@uzID;";
        #endregion

        #region CRUD
        public int Insert(Hodnoceni hodnoceni, DatabaseProxy pDb = null)
        {
            Database db;
            if (pDb == null)
            {
                db = new Database();
                db.Connect();
            }
            else
            {
                db = (Database)pDb;
            }

            SqlCommand command = db.CreateCommand(SQL_INSERT);
            PrepareCommand(command, hodnoceni);
            int row = db.ExecuteNonQuery(command);

            if (pDb == null)
            {
                pDb.Close();
            }

            return row;
        }

        public int Update(Hodnoceni hodnoceni, DatabaseProxy pDb = null)
        {
            Database db;
            if (pDb == null)
            {
                db = new Database();
                db.Connect();
            }
            else
            {
                db = (Database)pDb;
            }

            SqlCommand command = db.CreateCommand(SQL_UPDATE);
            PrepareCommand(command, hodnoceni);
            int row = db.ExecuteNonQuery(command);

            if (pDb == null)
            {
                pDb.Close();
            }

            return row;
        }

        public Collection<Hodnoceni> Select(DatabaseProxy pDb = null)
        {
            Database db;
            if (pDb == null)
            {
                db = new Database();
                db.Connect();
            }
            else
            {
                db = (Database)pDb;
            }

            SqlCommand command = db.CreateCommand(SQL_SELECT);
            SqlDataReader reader = db.Select(command);

            Collection<Hodnoceni> hodnoceni = Read(reader);
            reader.Close();

            if (pDb == null)
            {
                db.Close();
            }

            return hodnoceni;
        }

        public int Delete(Hodnoceni hodnoceni, DatabaseProxy pDb = null)
        {
            Database db;
            if (pDb == null)
            {
                db = new Database();
                db.Connect();
            }
            else
            {
                db = (Database)pDb;
            }

            SqlCommand command = db.CreateCommand(SQL_DELETE);

            command.Parameters.AddWithValue("@pID", hodnoceni.Predmet);
            command.Parameters.AddWithValue("@uzID", hodnoceni.Uzivatel);

            int row = db.ExecuteNonQuery(command);

            if (pDb == null)
            {
                db.Close();
            }

            return row;
        }
        #endregion

        public Collection<Hodnoceni> SelectBySubject(int pID, DatabaseProxy pDb = null)
        {
            Database db;
            if (pDb == null)
            {
                db = new Database();
                db.Connect();
            }
            else
            {
                db = (Database)pDb;
            }

            SqlCommand command = db.CreateCommand(SQL_SELECT_BYSUB);

            command.Parameters.AddWithValue("@pID", pID);
            SqlDataReader reader = db.Select(command);

            Collection<Hodnoceni> hodnoceni = Read(reader);
            reader.Close();

            if (pDb == null)
            {
                db.Close();
            }

            return hodnoceni;
        }

        private void PrepareCommand(SqlCommand command, Hodnoceni hodnoceni) 
        {
            command.Parameters.AddWithValue("@hodnoceni", hodnoceni.Ohodnoceni);
            command.Parameters.AddWithValue("@popis", hodnoceni.Popis);
            command.Parameters.AddWithValue("@datum", hodnoceni.Datum.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@pID", hodnoceni.Predmet.PId);
            command.Parameters.AddWithValue("@uzID", hodnoceni.Uzivatel.UzId);
        }

        private Collection<Hodnoceni> Read(SqlDataReader reader)
        {
            Collection<Hodnoceni> hodnoceni = new Collection<Hodnoceni>();

            while (reader.Read())
            {
                int i = -1;

                Hodnoceni h = new Hodnoceni();
                h.Ohodnoceni = reader.GetInt32(++i);
                h.Popis = reader.GetString(++i);
                h.Datum = reader.GetDateTime(++i);
                int pID = reader.GetInt32(++i);
                int uzID = reader.GetInt32(++i);

                h.Predmet = new PredmetTable().SelectByID(pID);
                h.Uzivatel = new UzivatelTable().SelectByID(uzID);
            }

            return hodnoceni;
        }
    }
}
