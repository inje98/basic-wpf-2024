using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using ex12_Daegu_restaurant.Models;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ex12_Daegu_restaurant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private List<MatJib> MatJibs;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitComboDateFromDB();
        }

        private void InitComboDateFromDB()
        {
            using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(Models.MatJib.GETCATGORI_QUERY, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet dSet = new DataSet();
                adapter.Fill(dSet);
                List<string> saveDates = new List<string>();

                foreach (DataRow row in dSet.Tables[0].Rows)
                {
                    saveDates.Add(Convert.ToString(row["Save_Date"]));
                }
                // CboFoodcategory.ItemsSource = saveDates;
               

                
            }
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string openApiUri = "https://www.daegufood.go.kr/kor/api/tasty.html?mode=json&addr=%EC%A4%91%EA%B5%AC";
            string result = string.Empty;

            // WebRequest, WebResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            // 콤보 상자에서 항목을 선택했는지 확인
            try
            {
                req = WebRequest.Create(openApiUri);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                // JSON 결과를 파싱하여 MatJibs 리스트에 저장
                var jsonResult = JObject.Parse(result);
                var status = Convert.ToString(jsonResult["status"]);

                if (status == "DONE")
                {
                    var data = jsonResult["data"];
                    var jsonArray = data as JArray; // json자체에서 []안에 들어간 배열데이터만 JArray 변환가능
                    MatJibs = new List<MatJib>(); // MatJibs 리스트 초기화

                    foreach (var item in jsonArray)
                    {
                        MatJibs.Add(new MatJib()
                        {
                            Id = 0,
                            OPENDATA_ID = Convert.ToInt32(item["OPENDATA_ID"]),
                            GNG_CS = Convert.ToString(item["GNG_CS"]),
                            FD_CS = Convert.ToString(item["FD_CS"]),
                            BZ_NM = Convert.ToString(item["BZ_NM"]),
                            TLNO = Convert.ToString(item["TLNO"]),
                            MBZ_HR = Convert.ToString(item["MBZ_HR"]),
                            SEAT_CNT = Convert.ToString(item["SEAT_CNT"]),
                            PKPL = Convert.ToString(item["PKPL"]),
                            HP = Convert.ToString(item["HP"]),
                            BKN_YN = Convert.ToString(item["BKN_YN"]),
                            MNU = Convert.ToString(item["MNU"]),
                            SMPL_DESC = Convert.ToString(item["SMPL_DESC"]),
                        });
                    }
                    this.DataContext = MatJibs;
                    StsResult.Content = $"OpenAPI {MatJibs.Count}건 조회완료!";
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"OpenAPI 조회 오류 {ex.Message}");
            }
        }

        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await this.ShowMessageAsync("저장오류", "조회 후 저장하십시오.");
                return;
            }
            var addMatJib = new List<MatJib>();
            foreach (MatJib item in GrdResult.SelectedItems)
            {
                addMatJib.Add(item);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var insRes = 0;
                    foreach (MatJib item in addMatJib)
                    {
                        SqlCommand cmd = new SqlCommand(Models.MatJib.INSERT_QUERY, conn);
                        cmd.Parameters.AddWithValue("@OPENDATA_ID", item.OPENDATA_ID);
                        cmd.Parameters.AddWithValue("@GNG_CS", item.GNG_CS);
                        cmd.Parameters.AddWithValue("@FD_CS", item.FD_CS);
                        cmd.Parameters.AddWithValue("@BZ_NM", item.BZ_NM);
                        cmd.Parameters.AddWithValue("@TLNO", item.TLNO);
                        cmd.Parameters.AddWithValue("@MBZ_HR", item.MBZ_HR);
                        cmd.Parameters.AddWithValue("@SEAT_CNT", item.SEAT_CNT);
                        cmd.Parameters.AddWithValue("@PKPL", item.PKPL);
                        cmd.Parameters.AddWithValue("@HP", item.HP);
                        cmd.Parameters.AddWithValue("@BKN_YN", item.BKN_YN);
                        cmd.Parameters.AddWithValue("@MNU", item.MNU);
                        cmd.Parameters.AddWithValue("@SMPL_DESC", item.SMPL_DESC);

                        insRes += cmd.ExecuteNonQuery();
                    }
                    if (insRes > 0)
                    {
                        await this.ShowMessageAsync("저장", "DB 저장 성공!");
                        StsResult.Content = $"DB 저장 {insRes}건 성공!";
                    }
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("저장오류", $"저장오류: {ex.Message}");
            }
            InitComboDateFromDB();
        }

        private void CboFoodcategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 선택된 항목을 가져옵니다.
            string selectedCategory = CboFoodcategory.SelectedItem as string;

            // 선택된 항목이 있는 경우에만 필터링합니다.
            if (!string.IsNullOrEmpty(selectedCategory) && MatJibs != null)
            {
                // 선택된 카테고리에 맞는 데이터만 필터링합니다.
                var filteredMatJibs = MatJibs.Where(item => item.FD_CS == selectedCategory).ToList();

                // 필터링된 데이터를 그리드에 바인딩합니다.
                GrdResult.ItemsSource = filteredMatJibs;

                // 결과 개수를 표시합니다.
                StsResult.Content = $"카테고리 '{selectedCategory}'의 데이터 {filteredMatJibs.Count}건 조회완료!";
            }

        }

        private async void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var curItem = GrdResult.SelectedItem as MatJib;
            await this.ShowMessageAsync($"{curItem.SMPL_DESC})", Name);
        }

        private async void BtnViewData_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            // 5.17
            List<MatJib> matJibs = new List<MatJib>();
            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var cmd = new SqlCommand(Models.MatJib.SELECT_QUERY, conn);
                    var adapter = new SqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "MatJib");

                    foreach (DataRow row in dSet.Tables["MatJib"].Rows)
                    {
                        var matjib = new MatJib()
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            OPENDATA_ID = Convert.ToInt32(row["OPENDATA_ID"]),
                            GNG_CS = Convert.ToString(row["GNG_CS"]),
                            FD_CS = Convert.ToString(row["FD_CS"]),
                            BZ_NM = Convert.ToString(row["BZ_NM"]),
                            TLNO = Convert.ToString(row["TLNO"]),
                            MBZ_HR = Convert.ToString(row["MBZ_HR"]),
                            SEAT_CNT = Convert.ToString(row["SEAT_CNT"]),
                            PKPL = Convert.ToString(row["PKPL"]),
                            HP = Convert.ToString(row["HP"]),
                            BKN_YN = Convert.ToString(row["BKN_YN"]),
                            MNU = Convert.ToString(row["MNU"]),
                            SMPL_DESC = Convert.ToString(row["SMPL_DESC"])
                        };
                        matJibs.Add(matjib);
                        }
                    this.DataContext = matJibs;
                    StsResult.Content = $"즐겨찾기 {matJibs.Count}건 조회완료";

                    }
                }
            
            catch (Exception ex)
            {
                await this.ShowMessageAsync("즐겨찾기", "즐겨찾기 조회오류.");
            }

        }

        private async void BtnDelData_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await this.ShowMessageAsync("삭제", "삭제할 영화를 선택하세요.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var delRes = 0;

                    foreach (MatJib item in GrdResult.SelectedItems)
                    {
                        SqlCommand cmd = new SqlCommand(Models.MatJib.DELETE_QUERY, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);

                        delRes += cmd.ExecuteNonQuery();
                    }

                    if (delRes == GrdResult.SelectedItems.Count)
                    {
                        await this.ShowMessageAsync("삭제", $"즐겨찾기 {delRes}건 삭제");
                    }
                    else
                    {
                        await this.ShowMessageAsync("삭제", $"즐겨찾기 {GrdResult.SelectedItems.Count}건 중 {delRes} 삭제");
                    }
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 오류 {ex.Message}");
            }
            BtnViewData_Click(sender, e);

        }
    }


}
