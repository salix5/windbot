using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System;
using System.IO;
using System.Linq;

namespace YGOSharp.OCGWrapper
{
    public static class NamedCardsManager
    {
        private static IDictionary<int, NamedCard> _cards;

        public static void Init(string databaseFullPath)
        {
            try
            {
                if (!File.Exists(databaseFullPath))
                {
                    throw new Exception("Could not find the cards database.");
                }

                _cards = new Dictionary<int, NamedCard>();

                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + databaseFullPath))
                {
                    connection.Open();

                    using (IDbCommand command = new SQLiteCommand(
                        "SELECT datas.id, ot, alias, setcode, type, level, race, attribute, atk, def, texts.name, texts.desc"
                        + " FROM datas INNER JOIN texts ON datas.id = texts.id",
                        connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LoadCard(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not initialize the cards database. Check the inner exception for more details.", ex);
            }
        }

        internal static NamedCard GetCard(int id)
        {
            if (_cards.ContainsKey(id))
                return _cards[id];
            return null;
        }

        public static IList<NamedCard> GetAllCards()
        {
            return _cards.Values.ToList();
        }

        private static void LoadCard(IDataRecord reader)
        {
            NamedCard card = new NamedCard(reader);
            _cards.Add(card.Id, card);
        }
    }
}