using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex12_Daegu_restaurant.Models
{

    //cnt": "1",
    //        "OPENDATA_ID": "1816",
    //        "GNG_CS": "대구광역시 중구 삼덕동2가 149-6",  주소
    //        "FD_CS": "한식", 음식카테고리
    //        "BZ_NM": "장모님국밥", 음식점명
    //        "TLNO": "053-425-9347", 연락처
    //        "MBZ_HR": "09:00 ~ 21:00", 영업시간
    //        "SEAT_CNT": "40석(룸1)", 좌석수
    //        "PKPL": "없음", 주차장
    //        "HP": "없음", 웹사이트
    //        //"PSB_FRN": "가능한 외국어가 없습니다.", 가능언어
    //        "BKN_YN": "가능", 예약가능여부
    //        //"INFN_FCL": "불가능", 놀이시설여부
    //        //"BRFT_YN": "불가능", 조식여부
    //        //"DSSRT_YN": "가능", 후식여부
    //        "MNU": "[저염메뉴] 순대국밥 9,000원 <br />돼지국밥 9,000원<br /> 메뉴 
    //        "SMPL_DESC": "장모님국밥은 대구에서 유일한 특별한 돼지국밥을 제공하는 전문점입니다.", 소개
    //        //"SBW": "지하철 2호선 경대병원역 1번 출구에서 도보로 약 123m 거리.", 위치간략
    //        //"BUS": "버스 정류장은 경북대학교병원앞 정류장이 가장 가깝습니다." 버스
    public class MatJib
    {
        public int Id { get; set; } // 추가 생성
        public int OPENDATA_ID { get; set; }
        public string GNG_CS {  get; set; } // 주소
        public string FD_CS { get; set; } // 음식 카테고리
        public string BZ_NM { get; set; } // 음식점 명
        public string TLNO { get; set; } // 전화번호
        public string MBZ_HR { get; set; } // 영업시간
        public string SEAT_CNT { get; set; } // 좌석 수
        public string PKPL { get; set; } // 주차장여부
        public string HP { get; set; } // 웹주소
        public string BKN_YN { get; set; } // 예약가능 여부
        public string MNU { get; set; } // 메뉴 리스트
        public string SMPL_DESC { get; set; } // 식당 소개

        public static readonly string INSERT_QUERY = @"INSERT INTO [dbo].[Restaurant]
                                                               ([OPENDATA_ID]
                                                               ,[GNG_CS]
                                                               ,[FD_CS]
                                                               ,[BZ_NM]
                                                               ,[TLNO]
                                                               ,[MBZ_HR]
                                                               ,[SEAT_CNT]
                                                               ,[PKPL]
                                                               ,[HP]
                                                               ,[BKN_YN]
                                                               ,[MNU]
                                                               ,[SMPL_DESC])
                                                         VALUES
                                                               (@OPENDATA_ID
                                                               ,@GNG_CS
                                                               ,@FD_CS
                                                               ,@BZ_NM
                                                               ,@TLNO
                                                               ,@MBZ_HR
                                                               ,@SEAT_CNT
                                                               ,@PKPL
                                                               ,@HP
                                                               ,@BKN_YN
                                                               ,@MNU
                                                               ,@SMPL_DESC)";


        public static readonly string SELECT_QUERY = @"SELECT [Id]
                                                              ,[OPENDATA_ID]
                                                              ,[GNG_CS]
                                                              ,[FD_CS]
                                                              ,[BZ_NM]
                                                              ,[TLNO]
                                                              ,[MBZ_HR]
                                                              ,[SEAT_CNT]
                                                              ,[PKPL]
                                                              ,[HP]
                                                              ,[BKN_YN]
                                                              ,[MNU]
                                                              ,[SMPL_DESC]
                                                          FROM [dbo].[Restaurant]";

        public static readonly string GETCATGORI_QUERY = @"SELECT DISTINCT FD_CS AS Save_Date
                                                            FROM [EMS].[dbo].[Restaurant]";
    }
}
