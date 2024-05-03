using Caliburn.Micro;
using ex07_EmployeeMngApp.Models;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace ex07_EmployeeMngApp.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        // 멤버변수
        private int id;
        private string empName;
        private decimal salary;
        private string deptName;
        private string addr;

        // List는 정적인 컬렉션, BindableCollection은 동적인 컬렉션
        // MVVM처럼 List를 사용못함
        private BindableCollection<Employees> listEmployees;

        private Employees selectedEmployee;

        public Employees SelectedEmployee
        {
            get => selectedEmployee;
            set
            {
                selectedEmployee = value;
                // 데이터를 TextBox들에 전달.
                if (selectedEmployee != null) 
                {
                    Id = value.Id;
                    EmpName = value.EmpName;
                    Salary = value.Salary;
                    DeptName = value.DeptName;
                    Addr = value.Addr;

                    NotifyOfPropertyChange(() => SelectedEmployee);
                    NotifyOfPropertyChange(() => Id);
                    NotifyOfPropertyChange(() => EmpName);
                    NotifyOfPropertyChange(() => Salary);
                    NotifyOfPropertyChange(() => DeptName);
                    NotifyOfPropertyChange(() => Addr);

                }
            }
        }

        public int Id
        {
            get => id; 
            set
            {
                id = value;
                NotifyOfPropertyChange(() => Id);
                NotifyOfPropertyChange(() => CanDelEmployee); // 삭제 여부 속성도 변경했다 공지
            }
        }
        public string EmpName
        {
            get => empName; 
            set
            {
                empName = value;
                NotifyOfPropertyChange(() => EmpName);
                NotifyOfPropertyChange(() => CanSaveEmployee);

            }
        }
        public decimal Salary
        {
            get => salary; 
            set
            {
                salary = value;
                NotifyOfPropertyChange(() => Salary);
                NotifyOfPropertyChange(() => CanSaveEmployee);

            }
        }
        public string DeptName
        {
            get => deptName; 
            set
            {
                deptName = value;
                NotifyOfPropertyChange(() => DeptName);
                NotifyOfPropertyChange(() => CanSaveEmployee);

            }
        }
        public string Addr
        {
            get => addr; 
            set
            {
                addr = value;
                NotifyOfPropertyChange(() => Addr);

            }
        }

        public BindableCollection<Employees> ListEmployees
        {
            get => listEmployees;
            set
            {
                listEmployees = value;
                NotifyOfPropertyChange(() => listEmployees);
            }
        }

        // 속성
        public MainViewModel()
        {
            DisplayName = "직원관리 시스템";
            // 조회 실행
            GetEmployees();
        }

        // 저장버튼 활성화 여부
        public bool CanSaveEmployee
        {
            get
            {
                if (string.IsNullOrEmpty(EmpName) || Salary == 0 || string.IsNullOrEmpty(DeptName))
                    return false;
                else
                    return true;

            }
        }

        /// <summary>
        /// Caliburn.Micro가 Xaml의 버튼 x:Name과 동일한 이름의 메서드로 매핑
        /// </summary>
        public void SaveEmployee() // save버튼 // 이게 클릭이벤트처럼 동작하네..?
        {
            // MessageBox.Show("저장버튼 동작!");
            using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (Id == 0)
                    cmd.CommandText = Models.Employees.INSERT_QUERY;
                else
                    cmd.CommandText = Models.Employees.UPDATE_QUERY;


                SqlParameter prmEmpName = new SqlParameter("@EmpName", EmpName);
                cmd.Parameters.Add(prmEmpName);
                SqlParameter prmSalary = new SqlParameter("@Salary", Salary);
                cmd.Parameters.Add(prmSalary);
                SqlParameter prmDeptName = new SqlParameter("@DeptName", DeptName);
                cmd.Parameters.Add(prmDeptName);
                SqlParameter prmAddr = new SqlParameter("@Addr", Addr ?? (object)DBNull.Value); // 수소가 null이면 이렇게
                cmd.Parameters.Add(prmAddr);

                if (Id != 0)
                {
                    SqlParameter prmId = new SqlParameter("@Id", Id);
                    cmd.Parameters.Add(prmId);
                }

                var result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("저장성공!");
                }
                else
                {
                    MessageBox.Show("저장실패!");
                }
                GetEmployees();
                NewEmployee(); // 입력란 초기화해야지
            }
            
        }

        public void GetEmployees() // 조회
        {
            using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING)) 
            {
                conn.Open ();
                SqlCommand cmd = new SqlCommand(Models.Employees.SELECT_QUERY, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                ListEmployees = new BindableCollection<Employees>();

                while (reader.Read()) 
                {
                    ListEmployees.Add(new Employees()
                    {
                        Id = (int)reader["Id"],
                        EmpName = reader["EmpName"].ToString(),
                        Salary = (decimal)reader["Salary"],
                        DeptName = reader["DeptName"].ToString(),
                        Addr = reader["Addr"].ToString()
                    });
                }
            }
        }

        // 삭제 버튼을 누를 수 있는지 여부확인
        public bool CanDelEmployee
        {
            get { return Id != 0; } // TextBox Id속성의 값이 0이면 false, 0이 아니면 true
        }


        public void DelEmployee() // 삭제
        {
            if (Id == 0)
            {
                MessageBox.Show("삭제불가!");
                return;
            }
            if (MessageBox.Show("삭제하시겠습니까?", "삭제여부", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
            using (SqlConnection conn = new SqlConnection (Helpers.Common.CONNSTRING)) 
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(Models.Employees.DELETE_QUERY, conn);
                SqlParameter prId = new SqlParameter("@Id", Id);
                cmd.Parameters.Add(prId);

                var res = cmd.ExecuteNonQuery();
                if (res >= 0)
                {
                    MessageBox.Show("삭제성공!");
                }
                else
                {
                    MessageBox.Show("삭제실패!");
                }
                GetEmployees();
                NewEmployee(); // 입력값 초기화
            }
        }

        public void NewEmployee() // 신규
        {
            Id = 0;
            Salary = 0;
            EmpName = DeptName = Addr = "";
        }
    }
}
