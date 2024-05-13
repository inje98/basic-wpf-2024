using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls; //스킨
using MahApps.Metro.Controls.Dialogs; //스킨
using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using ex12_Daegu_restaurant.Models;

namespace ex12_Daegu_restaurant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
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
                CboFoodcategory.ItemsSource = saveDates;
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



            try
            {
                req = WebRequest.Create(openApiUri);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                // await this.ShowMessageAsync("결과", result);
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"OpneAPI 조회 오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(result);
            var status = Convert.ToString(jsonResult["status"]);

            if (status == "DONE")
            {
                var data = jsonResult["data"];
                var jsonArray = data as JArray; // json자체에서 []안에 들어간 배열데이터만 JArray 변환가능
                var MatJibs = new List<MatJib>();
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


                //string selectedCategory = CboFoodcategory.SelectedItem as string;

                //// 선택된 카테고리에 맞는 데이터만 필터링
                //var filteredMatJibs = MatJibs.Where(item => item.FD_CS == selectedCategory).ToList();

                //// 필터링된 데이터를 그리드에 바인딩
                //GrdResult.ItemsSource = filteredMatJibs;

                //// 결과 개수 표시
                //StsResult.Content = $"카테고리 '{selectedCategory}'의 데이터 {filteredMatJibs.Count}건 조회완료!";
            }
        }
        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await this.ShowMessageAsync("저장오류", "실시간 조회후 저장하십시오.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var insRes = 0;
                    foreach (MatJib item in GrdResult.Items)
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
                        await this.ShowMessageAsync("저장", "DB저장성공!");
                        StsResult.Content = $"DB저장 {insRes}건 성공!";
                    }
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("저장오류", $"저장오류 {ex.Message}");

            }
            InitComboDateFromDB();

        }

        private void CboFoodcategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

    }
}