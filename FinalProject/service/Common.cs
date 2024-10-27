using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalProject.model;

namespace FinalProject.service
{
    class Common
    {
        /**
         * 取得玩家撲克牌列表
         */
        public List<Poker> GetPokerRecordsFromDatabase(string database)
        {
            List<Poker> PokerRecords = new List<Poker>();

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|DB.mdf;Integrated Security=True";
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                string query = "SELECT * FROM " + database;
                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    // 執行查詢並讀取結果
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // 將查詢結果中的每條記錄轉換為 Poker 對象
                            Poker playerRecords = new Poker
                            {
                                id = reader.GetInt32(0),         // 取得 Id 欄位的值
                                points = reader.GetString(1),     // 取得 Points 欄位的值
                                suits = reader.GetString(2),     // 取得 suits 欄位的值
                                joker = reader.IsDBNull(3) ? null : reader.GetString(3)     // 取得 joker 欄位的值
                            };

                            // 將 Poker 對象添加到列表中
                            PokerRecords.Add(playerRecords);
                        }
                    }
                }
            }

            return PokerRecords;
        }

        /**
         * 移除成對牌
         */
        public void RemoveDuplicateCards(string database)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;"
                + @"AttachDbFilename=|DataDirectory|DB.mdf;"
                + "Integrated Security=True";
                cn.Open();

                // playerRecords來自資料庫的數據，可以通過查詢填充
                List<Poker> playerRecords = GetPokerRecordsFromDatabase(database);

                // 使用 LINQ 進行分組和篩選
                var recordsToDelete = playerRecords
                    .Where(p => p.points != "g" && p.points != "gg")      // 篩選 points 不為 "g" 或 "gg"
                    .GroupBy(p => p.points)                               // 按 points 分組
                    .SelectMany(g =>
                        g.Count() > 1
                            ? g.Count() % 2 == 0
                                ? g                                          // 偶數個：刪除全部
                                : g.OrderByDescending(p => p.id).Skip(1)     // 奇數個：保留最新的一筆
                            : Enumerable.Empty<Poker>()
                    ).ToList();

                var idsToDelete = recordsToDelete.Select(p => p.id).ToList();
                if (idsToDelete.Any())
                {
                    string cmdstr = $"DELETE FROM {database} WHERE Id IN ({string.Join(",", idsToDelete)})";
                    using (SqlCommand cmd = new SqlCommand(cmdstr, cn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
